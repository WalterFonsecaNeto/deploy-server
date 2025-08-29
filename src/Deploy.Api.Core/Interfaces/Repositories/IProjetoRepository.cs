using Deploy.Api.Core.Domain.Entidades;

namespace Deploy.Api.Core.Interfaces.Repositories
{
      public interface IProjetoRepository
      {
            Task<int> CadastrarProjetoAsync(Projeto projeto);
            Task<List<Projeto>> ListarTodosProjetosDeUmUsuariosAsync(int usuarioId);
            Task<Projeto> ObterProjetoPorIdAsync(int projetoId);       
    }
}