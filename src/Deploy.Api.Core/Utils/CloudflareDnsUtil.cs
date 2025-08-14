using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace Deploy.Api.Core.Utils
{
    public static class CloudflareDnsUtil
    {
        private static readonly string ID_ZONA = "db9392be796742f4c0800a8d3595987c";
        private static readonly string TOKEN = "JZT_sOz94j1O0YGBNghOpyBCeUyCBknbsymzpEyr";

        public static async Task CriarEntradaAsync(string subdominio)
        {
            Console.WriteLine("[CloudflareDns] INICIADO COM SUCESSO");

            using var http = new HttpClient();
            http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TOKEN);
            http.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var corpo = new
            {
                type = "CNAME",
                name = subdominio,
                content = "fb8dd814-8b2a-4c46-ab2d-e928b5db0909.cfargotunnel.com",
                ttl = 1,
                proxied = true
            };

            var json = JsonSerializer.Serialize(corpo);
            var conteudo = new StringContent(json, Encoding.UTF8, "application/json");

            var url = $"https://api.cloudflare.com/client/v4/zones/{ID_ZONA}/dns_records";
            var resposta = await http.PostAsync(url, conteudo);

            resposta.EnsureSuccessStatusCode();

            Console.WriteLine("[CloudflareDns] FINALIZADO COM SUCESSO");
        }
    }
}
