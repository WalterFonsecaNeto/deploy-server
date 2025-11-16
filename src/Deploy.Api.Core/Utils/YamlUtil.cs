using System.IO;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Deploy.Api.Core.Utils
{
    public static class YamlUtil
    {
        private static readonly string CAMINHO_YAML = "/root/.cloudflared/servidor-pessoal-config.yml";

        public static void AtualizarYaml(string subdominio, int porta)
        {
            Console.WriteLine("[ConfigYaml] INICIADO");

            if (!File.Exists(CAMINHO_YAML))
                throw new FileNotFoundException($"Arquivo YAML não encontrado: {CAMINHO_YAML}");

            var yamlTexto = File.ReadAllText(CAMINHO_YAML);

            var deserializer = new DeserializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .Build();

            var root = deserializer.Deserialize<Dictionary<string, object>>(yamlTexto);

            var ingress = root["ingress"] as List<object>;

            string dominio = $"{subdominio}.walterfonsecaneto.com.br";

            bool existe = ingress.Any(i =>
            {
                var entry = i as Dictionary<object, object>;
                return entry != null &&
                       entry.ContainsKey("hostname") &&
                       entry["hostname"].ToString() == dominio;
            });

            if (!existe)
            {
                // insere antes do bloco final do 404
                ingress.Insert(ingress.Count - 1, new Dictionary<object, object>
                {
                    { "hostname", dominio },
                    { "service", $"http://localhost:{porta}" }
                });
            }

            var serializer = new SerializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .ConfigureDefaultValuesHandling(DefaultValuesHandling.OmitDefaults)
                .Build();

            // serializa sem formatação
            var yamlFormatado = serializer.Serialize(root);

            // agora, formatamos manualmente para ficar IGUAL seu exemplo

            // 1) linha em branco após ingress:
            yamlFormatado = yamlFormatado.Replace("ingress:\n", "ingress:\n\n");

            // 2) uma linha em branco entre cada item
            yamlFormatado = yamlFormatado.Replace("\n  -", "\n\n  -");

            // remove linha dupla antes do fallback final
            yamlFormatado = yamlFormatado.Replace("\n\n  - service: http_status:404", "\n  - service: http_status:404");

            File.WriteAllText(CAMINHO_YAML, yamlFormatado);

            Console.WriteLine("[ConfigYaml] FINALIZADO");
        }
    }
}
