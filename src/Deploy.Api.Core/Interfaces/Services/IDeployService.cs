using Deploy.Api.Core.Domain.Request;
using Deploy.Api.Core.Domain.Response.Base;

namespace Deploy.Api.Core.Interfaces.Services
{
    public interface IDeployService
    {
        Task<ResponseViewModel<object>> ProcessarDeployAsync(DeployRequest dadosDeploy);

    }
}
