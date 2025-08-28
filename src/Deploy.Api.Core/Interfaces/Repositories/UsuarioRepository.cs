using Deploy.Api.Core.Domain.Entidades;

namespace Deploy.Api.Core.Interfaces.Repositories
{
    public interface IUsuarioRepository
    {
        Task<Usuario> ObterUsuarioPorEmailAsync(string email);
        Task<Usuario> ObterUsuarioPorIdAsync(int usuarioId);
        Task<List<Usuario>> ListarTodosUsuariosAsync();
        Task<int> InserirUsuarioAsync(Usuario usuario);
    }
}