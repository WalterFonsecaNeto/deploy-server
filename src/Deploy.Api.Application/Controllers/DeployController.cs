using Deploy.Api.Application.Controllers.Base;
using Deploy.Api.Core.Domain.Request;
using Deploy.Api.Core.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Deploy.Api.Application.Controllers
{
    [ApiController]
    [Route("api/")]
    public class DeployController : BaseController
    {
        private readonly IDeployService _deployService;

        public DeployController(IDeployService deployService)
        {
            _deployService = deployService;
        }

        [AllowAnonymous]
        [HttpPost("deploy")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> InserirFinancaParceladaAsync([FromForm] DeployRequest request)
        {
            var response = await _deployService.ProcessarDeployAsync(request.ProjetoFile, request.Subdominio);
            return ApiResponse(response);
        }


    }
}
