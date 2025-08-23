using Deploy.Api.Core.Domain.Entidades;
using Deploy.Api.Core.Domain.Response.Base;

namespace Deploy.Api.Core.Interfaces.Services
{
    public interface IUsuarioService
    {
        Task<ResponseViewModel<Usuario>> ValidarLoginUsuarioAsync(Usuario usuarioLogin);
        Task<ResponseViewModel<List<Usuario>>> ListarTodosUsuariosAsync();
        Task<ResponseViewModel<Usuario>> CadastrarUsuarioAsync(Usuario usuario);
        
    }
}