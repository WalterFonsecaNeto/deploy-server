using Microsoft.AspNetCore.Mvc;
using FluentValidation.Results;
using  Deploy.Api.Core.Domain.Response.Base;
 
namespace Deploy.Api.Application.Controllers.Base
{
    public abstract class BaseController : ControllerBase
    {
        protected IActionResult ApiResponse<T>(ResponseViewModel<T>? result = null) where T : class
        {
            if (result is null) return NoContent();
 
            if (result.Notificacoes != null)
                return StatusCode(result.StatusCode, new { result.Sucesso, Notificacoes = result.Notificacoes });
 
            if (result.Dados == null)
                return StatusCode(result.StatusCode);
 
            return StatusCode(result.StatusCode, new { result.Sucesso, result.Dados });
        }
 
        protected IActionResult ValidationResponse(ValidationResult validationResult)
        {
            var notificacoes = validationResult.Errors.Select(erro => erro.ErrorMessage).ToList();
 
            return ApiResponse(new ResponseViewModel<string>(StatusCodes.Status400BadRequest, false, notificacoes));
        }
    }
}