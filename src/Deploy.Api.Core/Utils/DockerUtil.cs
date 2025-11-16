using Docker.DotNet;
using Docker.DotNet.Models;
using System.Net.Sockets;

namespace Deploy.Api.Core.Utils
{
    public static class DockerUtil
    {
        private static DockerClient CriarCliente() =>
            new DockerClientConfiguration(new Uri("unix:///var/run/docker.sock")).CreateClient();

        private static async Task<bool> PortaDisponivelAsync(int porta)
        {
            try
            {
                var listener = new TcpListener(System.Net.IPAddress.Loopback, porta);
                listener.Start();
                listener.Stop();
                return true;
            }
            catch { return false; }
        }

        public static async Task<int> GerarPortaDisponivelAsync()
        {
            var rnd = new Random();
            for (int i = 0; i < 40; i++)
            {
                int porta = rnd.Next(2000, 6999);

                if (await PortaDisponivelAsync(porta))
                    return porta;
            }

            throw new Exception("Nenhuma porta disponível encontrada.");
        }

        public static async Task<(bool existe, int? porta)> InspecionarContainerAsync(string nome)
        {
            var client = CriarCliente();

            var containers = await client.Containers.ListContainersAsync(
                new ContainersListParameters { All = true });

            var container = containers
                .FirstOrDefault(c => c.Names.Contains("/" + nome));

            if (container == null)
                return (false, null);

            var port = container.Ports.FirstOrDefault()?.PublicPort;
            return (true, port);
        }

        public static async Task<(int porta, bool reutilizado)> ConstruirERodarAsync(string nome, string caminhoProjeto)
        {
            var client = CriarCliente();

            var (existe, _) = await InspecionarContainerAsync(nome);
            var reutilizado = existe;

            if (existe)
            {
                Console.WriteLine($"🗑 Removendo container antigo {nome}");
                await client.Containers.RemoveContainerAsync(nome,
                    new ContainerRemoveParameters { Force = true });
            }

            Console.WriteLine("📦 Gerando TAR do projeto...");
            using var tarStream = TarUtil.CreateTarFromDirectory(caminhoProjeto);

            Console.WriteLine("🐳 Buildando imagem Docker...");
            await client.Images.BuildImageFromDockerfileAsync(
                tarStream,
                new ImageBuildParameters
                {
                    Dockerfile = "Dockerfile",
                    Tags = new List<string> { nome },
                }
            );

            var porta = await GerarPortaDisponivelAsync();

            Console.WriteLine($"🚀 Criando container {nome} na porta interna {porta}…");
            await client.Containers.CreateContainerAsync(new CreateContainerParameters
            {
                Image = nome,
                Name = nome,
                Env = new List<string>
                {
                    $"PORT={porta}"
                },
                HostConfig = new HostConfig
                {
                    RestartPolicy = new RestartPolicy
                    {
                        Name = RestartPolicyKind.UnlessStopped
                    },
                    NetworkMode = "host",
                }
            });


            Console.WriteLine("▶ Iniciando container...");
            await client.Containers.StartContainerAsync(nome, null);

            Console.WriteLine($"✅ Container {nome} rodando na porta: {porta}");

            return (porta, reutilizado);
        }

        public static async Task ReiniciarTunnelAsync()
        {
            var client = CriarCliente();

            Console.WriteLine("▶ Localizando container servidor-pessoal-tunnel...");

            var containers = await client.Containers.ListContainersAsync(
                new ContainersListParameters { All = true }
            );

            var container = containers.FirstOrDefault(c =>
                c.Names.Contains("/servidor-pessoal-tunnel")
            );

            if (container == null)
            {
                Console.WriteLine("❌ Container servidor-pessoal-tunnel não encontrado.");
                return;
            }

            Console.WriteLine("⛔ Parando container servidor-pessoal-tunnel...");
            await client.Containers.StopContainerAsync(container.ID, new ContainerStopParameters());

            // ⏳ Cloudflare precisa de tempo para descartar caches internos
            await Task.Delay(3000);

            Console.WriteLine("▶ Iniciando container servidor-pessoal-tunnel...");
            await client.Containers.StartContainerAsync(container.ID, new ContainerStartParameters());

            Console.WriteLine("✅ Tunnel reiniciado com sucesso (stop ➝ delay ➝ start).");
        }




    }
}
