using Deploy.Api.Core.Domain.Response.Base;
using Microsoft.AspNetCore.Http;

namespace Deploy.Api.Core.Interfaces.Services
{
    public interface IDeployService
    {
        Task<ResponseViewModel<object>> ProcessarDeployAsync(IFormFile projetoFile, string subdominio);

    }
}
