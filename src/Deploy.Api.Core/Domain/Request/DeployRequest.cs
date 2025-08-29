using Microsoft.AspNetCore.Http;

namespace Deploy.Api.Core.Domain.Request
{
    public class DeployRequest
    {
        public IFormFile ProjetoFile { get; set; }
        public string Subdominio { get; set; }
        public int ProjetoId { get; set; }
    }
}