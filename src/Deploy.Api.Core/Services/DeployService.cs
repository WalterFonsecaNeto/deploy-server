using Deploy.Api.Core.Domain.Request;
using Deploy.Api.Core.Domain.Response.Base;
using Deploy.Api.Core.Interfaces.Repositories;
using Deploy.Api.Core.Interfaces.Services;
using Deploy.Api.Core.Utils;
using Microsoft.AspNetCore.Http;
using System.Diagnostics;

namespace Deploy.Api.Core.Services
{
    public class DeployService : IDeployService
    {
        private readonly string _pastaProjetos = Path.Combine(Directory.GetCurrentDirectory(), "Projetos");

        private readonly IProjetoRepository _projetoRepository;
        private readonly IDominioRepository _dominioRepository;
        private string _caminhoTemp = string.Empty;

        public DeployService(IProjetoRepository projetoRepository, IDominioRepository dominioRepository)
        {
            _projetoRepository = projetoRepository;
            _dominioRepository = dominioRepository;
        }

        public async Task<ResponseViewModel<object>> ProcessarDeployAsync(DeployRequest dadosDeploy)
        {
            try
            {
                //* Valida dados de entrada
                var existeErro = ValidarDadosDeEntrada(dadosDeploy);
                if (existeErro != null)
                    return existeErro;

                //* Valida se projeto existe
                var projetoExiste = await _projetoRepository.ObterProjetoPorIdAsync(dadosDeploy.ProjetoId);
                if (projetoExiste == null)
                    return new ResponseViewModel<object>(404, false, new List<string> { "ProjetoId não encontrado." });

                dadosDeploy.Subdominio = dadosDeploy.Subdominio.ToLowerInvariant();

                //* Valida se dominio existe
                var dominioExiste = await _dominioRepository.ValidarExistenciaDoDominioPeloSubdominioAsync(dadosDeploy.Subdominio);
                if (dominioExiste == 1)
                    return new ResponseViewModel<object>(409, false, new List<string> { "Subdomínio já em uso. Escolha outro." });

                //* Cria caminho temporário
                _caminhoTemp = Path.Combine(Path.GetTempPath(), dadosDeploy.ProjetoFile.FileName);

                //* Salvar arquivo temporário
                using (var stream = new FileStream(_caminhoTemp, FileMode.Create))
                {
                    await dadosDeploy.ProjetoFile.CopyToAsync(stream);
                }

                //* Extrair projeto
                var caminhoProjeto = await UnzipUtil.ExtrairZipAsync(_caminhoTemp, Path.Combine(_pastaProjetos, dadosDeploy.Subdominio));
                if (caminhoProjeto == null)
                    return new ResponseViewModel<object>(400, false, new List<string> { "Erro ao extrair o arquivo ZIP." });




                //! Inserir dados no banco (tabela Aplicacao) - Futuro



                // 2️⃣ Verificar estado atual do container
                var (existe, porta) = await DockerUtil.InspecionarContainerAsync(dadosDeploy.Subdominio);
                var novoContainer = !existe;

                // 3️⃣ Build e run do container
                var (portaFinal, reutilizado) = await DockerUtil.ConstruirERodarAsync(dadosDeploy.Subdominio, caminhoProjeto);

                // 4️⃣ Configura DNS e YAML apenas para novos containers
                if (novoContainer)
                {
                    YamlUtil.AtualizarYaml(dadosDeploy.Subdominio, portaFinal);
                    await CloudflareDnsUtil.CriarEntradaAsync(dadosDeploy.Subdominio);
                    Console.WriteLine($"🌐 DNS e YAML configurados para {dadosDeploy.Subdominio}");

                    // 5️⃣ Reiniciar container do tunnel
                    await DockerUtil.ReiniciarTunnelAsync();
                }

                // 6️⃣ Retornar resultado
                return new ResponseViewModel<object>(201, true, new
                {
                    Porta = portaFinal,
                    NovoContainer = novoContainer,
                    PortaReutilizada = !novoContainer && reutilizado,
                    Caminho = caminhoProjeto,
                    Url = $"{dadosDeploy.Subdominio}.walterfonsecaneto.com.br"
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
                    if (File.Exists(_caminhoTemp))
                    {
                        File.Delete(_caminhoTemp);
                        Console.WriteLine("🗑️ Arquivo temporário removido");
                    }
                }
                catch (Exception erroLimpeza)
                {
                    Console.Error.WriteLine("⚠️ Erro ao limpar arquivo temporário: " + erroLimpeza);
                }
            }
        }

        

        private ResponseViewModel<object> ValidarDadosDeEntrada(DeployRequest dadosDeploy)
        {
            if (dadosDeploy.ProjetoFile == null || dadosDeploy.ProjetoFile.Length == 0)
                return new ResponseViewModel<object>(400, false, new List<string> { "Nenhum arquivo enviado." });

            var fileName = Path.GetFileName(dadosDeploy.ProjetoFile.FileName);
            var extensao = Path.GetExtension(fileName)?.ToLower();

            if (extensao != ".zip")
                return new ResponseViewModel<object>(400, false, new List<string> { "Apenas arquivos ZIP são permitidos." });

            if (string.IsNullOrWhiteSpace(dadosDeploy.Subdominio) || dadosDeploy.Subdominio.Length < 3 || dadosDeploy.Subdominio.Length > 63)
                return new ResponseViewModel<object>(400, false, new List<string> { "Subdomínio inválido. Deve ter entre 3 e 63 caracteres." });

            if (dadosDeploy.ProjetoId <= 0)
                return new ResponseViewModel<object>(400, false, new List<string> { "ProjetoId inválido. Deve ser um número positivo." });

            return null;
        }
    }
}
