using Deploy.Api.Application.Controllers.Base;
using Deploy.Api.Core.Domain.Entidades;
using Deploy.Api.Core.Domain.Request;
using Deploy.Api.Core.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Deploy.Api.Application.Controllers
{
      [ApiController]
      [Route("api/[controller]")]
      public class ProjetoController : BaseController
      {
            private readonly IProjetoService _projetoService;

            public ProjetoController(IProjetoService projetoService)
            {
                  _projetoService = projetoService;
            }

            [AllowAnonymous]
            [HttpGet("Listar")]
            public async Task<IActionResult> ListarTodosUsuariosAsync([FromQuery] int usuarioId)
            {
                  var response = await _projetoService.ListarTodosProjetosDeUmUsuariosAsync(usuarioId);
                  return ApiResponse(response);
            }

            [AllowAnonymous]
            [HttpPost("Cadastrar")]
            public async Task<IActionResult> CadastrarProjetoAsync([FromBody] ProjetoCriarRequest projetoCriarRequest)
            {
                  var projetoDominio = new Projeto
                  {
                        Usuario_Id = projetoCriarRequest.Usuario_Id,
                        Nome = projetoCriarRequest.Nome,
                        Descricao = projetoCriarRequest.Descricao
                  };
                  var response = await _projetoService.CadastrarProjetoAsync(projetoDominio);
                  return ApiResponse(response);
            }

      }
}