
using Deploy.Api.Core.Domain.Entidades;
using Deploy.Api.Core.Domain.Response.Base;

namespace Deploy.Api.Core.Interfaces.Services
{
    public interface IProjetoService
    {
        Task<ResponseViewModel<List<Projeto>>> ListarTodosProjetosDeUmUsuariosAsync(int usuarioId);
        Task<ResponseViewModel<Projeto>> CadastrarProjetoAsync(Projeto projeto);
        
    }
}
