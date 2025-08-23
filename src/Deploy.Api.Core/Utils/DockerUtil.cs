using System.Diagnostics;
using System.Net.Sockets;

namespace Deploy.Api.Core.Utils
{
    public static class DockerUtil
    {
        private static async Task<string> ExecutarComandoAsync(string comando, string argumentos)
        {
            var tcs = new TaskCompletionSource<string>();

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

            string saida = "";
            processo.OutputDataReceived += (s, e) => { if (e.Data != null) saida += e.Data + "\n"; };
            processo.ErrorDataReceived += (s, e) => { if (e.Data != null) saida += e.Data + "\n"; };

            processo.EnableRaisingEvents = true;
            processo.Exited += (s, e) =>
            {
                if (processo.ExitCode == 0)
                    tcs.SetResult(saida.Trim());
                else
                    tcs.SetException(new Exception($"Comando falhou: {comando} {argumentos}\n{saida}"));
                processo.Dispose();
            };

            processo.Start();
            processo.BeginOutputReadLine();
            processo.BeginErrorReadLine();

            return await tcs.Task;
        }

        private static async Task<bool> PortaDisponivelAsync(int porta)
        {
            try
            {
                var listener = new TcpListener(System.Net.IPAddress.Loopback, porta);
                listener.Start();
                listener.Stop();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static async Task<int> GerarPortaDisponivelAsync()
        {
            var portaMin = 1000;
            var portaMax = 3999;
            var aleatorio = new Random();

            for (int i = 0; i < 50; i++)
            {
                var porta = aleatorio.Next(portaMin, portaMax + 1);
                if (await PortaDisponivelAsync(porta))
                    return porta;
            }

            throw new Exception("Não foi possível encontrar uma porta disponível");
        }

        public static async Task<(bool existe, int? porta, bool rodando)> InspecionarContainerAsync(string nome)
        {
            try
            {
                var portaStr = await ExecutarComandoAsync("docker", $"inspect --format \"{{{{(index (index .NetworkSettings.Ports \\\"80/tcp\\\") 0).HostPort}}}}\" {nome}");
                var rodando = !string.IsNullOrWhiteSpace(await ExecutarComandoAsync("docker", $"ps -q -f name={nome}"));
                return (true, int.Parse(portaStr), rodando);
            }
            catch
            {
                return (false, null, false);
            }
        }

        public static async Task<(int porta, bool reutilizado)> ConstruirERodarAsync(string nome, string caminhoProjeto)
        {
            Console.WriteLine($"[DockerUtil] INICIADO COM SUCESSO para o container: {nome}");

            var (existe, porta, _) = await InspecionarContainerAsync(nome);
            var reutilizado = false;

            if (existe)
            {
                Console.WriteLine($"[DockerUtil] Container existente encontrado na porta {porta}");
                reutilizado = true;
                await ExecutarComandoAsync("docker", $"rm -f {nome}");
            }

            if (!porta.HasValue)
            {
                porta = await GerarPortaDisponivelAsync();
                Console.WriteLine($"[DockerUtil] Nova porta alocada: {porta}");
            }

            Console.WriteLine("[DockerUtil]  Construindo imagem Docker...");
            await ExecutarComandoAsync("docker", $"build -t {nome} \"{caminhoProjeto}\"");

            Console.WriteLine($"[DockerUtil] Iniciando container na porta {porta}");

            //HOST DOCKER
            // await ExecutarComandoAsync("docker", $"run -d --restart unless-stopped --network host --name {nome} -e PORT=80 -e ASPNETCORE_URLS=http://0.0.0.0:80 {nome}");

            //BRIGE PADRÃO DOCKER
            // await ExecutarComandoAsync("docker", $"run -d --restart unless-stopped -p {porta}:80 --name {nome} -e PORT=80 -e ASPNETCORE_URLS=http://0.0.0.0:80 {nome}");

            //BRIGE CUSTOMIZADO DOCKER (Criado por mim)
            await ExecutarComandoAsync("docker", $"run -d --name {nome} --restart unless-stopped -e PORT={porta} -e ASPNETCORE_URLS=http://0.0.0.0:{porta} {nome}");

            Console.WriteLine("[DockerUtil] FINALIZADO COM SUCESSO");

            return (porta.Value, reutilizado);
        }
    }
}
