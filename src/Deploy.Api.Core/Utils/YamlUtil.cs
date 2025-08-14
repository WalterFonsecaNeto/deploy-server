namespace Deploy.Api.Core.Utils
{
    public static class YamlUtil
    {
        //Local do arquivo YAML do Cloudflare Tunnel
        //private static readonly string CAMINHO_YAML = "/home/walter/.cloudflared/config.yml";

        //Docker do arquivo YAML do Cloudflare Tunnel
        private static readonly string CAMINHO_YAML = "/root/.cloudflared/config.yml";

        public static void AtualizarYaml(string subdominio, int porta)
        {
            Console.WriteLine("[ConfigYaml] INICIADO COM SUCESSO");

            var dominio = $"{subdominio}.walterfonsecaneto.com.br";
            var entrada = $"  - hostname: {dominio}\n    service: http://localhost:{porta}\n";

            var yaml = File.ReadAllText(CAMINHO_YAML);
            if (!yaml.Contains(dominio))
            {
                yaml = yaml.Replace("  - service: http_status:404", entrada + "  - service: http_status:404");
                File.WriteAllText(CAMINHO_YAML, yaml);
            }

            Console.WriteLine("[ConfigYaml] FINALIZADO COM SUCESSO");
        }
    }
}
