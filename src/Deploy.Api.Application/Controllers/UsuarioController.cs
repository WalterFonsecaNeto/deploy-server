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
    public class UsuarioController : BaseController
    {
        private readonly IUsuarioService _usuarioService;

        public UsuarioController(IUsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
        }

        [AllowAnonymous]
        [HttpGet("Listar")]
        public async Task<IActionResult> ListarTodosUsuariosAsync()
        {
            var response = await _usuarioService.ListarTodosUsuariosAsync();
            return ApiResponse(response);
        }

        [AllowAnonymous]
        [HttpPost("Login")]
        public async Task<IActionResult> ValidarLoginUsuarioAsync([FromBody] UsuariologinRequest usuarioLoginRequest)
        {
            var usuarioDominio = new Usuario
            {
                Email = usuarioLoginRequest.Email,
                Senha = usuarioLoginRequest.Senha,
            };
            var response = await _usuarioService.ValidarLoginUsuarioAsync(usuarioDominio);
            return ApiResponse(response);
        }

        [AllowAnonymous]
        [HttpPost("Cadastrar")]
        public async Task<IActionResult> CadastrarUsuarioAsync([FromBody] UsuarioCriarRequest usuarioCriarRequest)
        {
            var usuarioDominio = new Usuario
            {
                Nome = usuarioCriarRequest.Nome,
                Email = usuarioCriarRequest.Email,
                Senha = usuarioCriarRequest.Senha,
            };
            var response = await _usuarioService.CadastrarUsuarioAsync(usuarioDominio);
            return ApiResponse(response);
        }

    }
}