using Deploy.Api.Core.Domain.Response.Base;
using Deploy.Api.Core.Interfaces.Services;
using Deploy.Api.Core.Utils;
using Microsoft.AspNetCore.Http;
using System.Diagnostics;

namespace Deploy.Api.Core.Services
{
    public class DeployService : IDeployService
    {
        private readonly string _pastaProjetos = Path.Combine(Directory.GetCurrentDirectory(), "Projetos");

        public DeployService() { }

        public async Task<ResponseViewModel<object>> ProcessarDeployAsync(IFormFile projetoFile, string subdominio)
        {

            if (projetoFile == null || projetoFile.Length == 0)
                return new ResponseViewModel<object>(400, false, new List<string> { "Nenhum arquivo enviado." });

            var fileName = Path.GetFileName(projetoFile.FileName);
            var extensao = Path.GetExtension(fileName)?.ToLower();
            
            if (extensao != ".zip")
                return new ResponseViewModel<object>(400, false, new List<string> { "Apenas arquivos ZIP são permitidos." });
            
            if (string.IsNullOrWhiteSpace(subdominio) || subdominio.Length < 3 || subdominio.Length > 63)
                return new ResponseViewModel<object>(400, false, new List<string> { "Subdomínio inválido. Deve ter entre 3 e 63 caracteres." });

            subdominio = subdominio.ToLowerInvariant();

            var caminhoTemp = Path.Combine(Path.GetTempPath(), projetoFile.FileName);

            try
            {
                // Salvar arquivo temporário
                using (var stream = new FileStream(caminhoTemp, FileMode.Create))
                {
                    await projetoFile.CopyToAsync(stream);
                }

                // 1️⃣ Extrair projeto
                var caminhoProjeto = await UnzipUtil.ExtrairZipAsync(caminhoTemp, Path.Combine(_pastaProjetos, subdominio));
                Console.WriteLine($"📦 Projeto extraído para: {caminhoProjeto}");

                // 2️⃣ Verificar estado atual do container
                var (existe, porta, rodando) = await DockerUtil.InspecionarContainerAsync(subdominio);
                var novoContainer = !existe;

                // 3️⃣ Build e run do container
                var (portaFinal, reutilizado) = await DockerUtil.ConstruirERodarAsync(subdominio, caminhoProjeto);

                // 4️⃣ Configura DNS e YAML apenas para novos containers
                if (novoContainer)
                {
                    YamlUtil.AtualizarYaml(subdominio, portaFinal);
                    await CloudflareDnsUtil.CriarEntradaAsync(subdominio);
                    Console.WriteLine($"🌐 DNS e YAML configurados para {subdominio}");

                    // 5️⃣ Reiniciar container do tunnel
                    await ReiniciarTunnelAsync();
                }

                // 6️⃣ Retornar resultado
                return new ResponseViewModel<object>(201, true, new
                {
                    Porta = portaFinal,
                    NovoContainer = novoContainer,
                    PortaReutilizada = !novoContainer && reutilizado,
                    Caminho = caminhoProjeto,
                    Url = $"{subdominio}.walterfonsecaneto.com.br"
                });
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("🚨 Erro no deploy: " + ex);
                return new ResponseViewModel<object>(500, false, new List<string> { ex.Message });
            }
            finally
            {
                // Limpar arquivo temporário
                try
                {
                    if (File.Exists(caminhoTemp))
                    {
                        File.Delete(caminhoTemp);
                        Console.WriteLine("🗑️ Arquivo temporário removido");
                    }
                }
                catch (Exception erroLimpeza)
                {
                    Console.Error.WriteLine("⚠️ Erro ao limpar arquivo temporário: " + erroLimpeza);
                }
            }
        }

        private async Task ReiniciarTunnelAsync()
        {
            var comando = "docker";
            var argumentos = "compose -f /home/walter/.cloudflared/docker-compose.yml restart tunnel";

            var processo = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = comando,
                    Arguments = argumentos,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };

            processo.Start();
            string saida = await processo.StandardOutput.ReadToEndAsync();
            string erro = await processo.StandardError.ReadToEndAsync();
            processo.WaitForExit();

            Console.WriteLine($"Tunnel reiniciado:\n{saida}");
            if (!string.IsNullOrWhiteSpace(erro))
                Console.Error.WriteLine($"Stderr: {erro}");
        }
    }
}
