using Deploy.Api.Core.Domain.Entidades;
using Deploy.Api.Core.Repositories.Sql;
using Deploy.Api.Core.Interfaces.Repositories.Base.IDbCon;
using Deploy.Api.Core.Repositories.Base;
using Deploy.Api.Core.Interfaces.Repositories;

namespace Deploy.Api.Core.Repositories
{
      public class ProjetoRepository : BaseRepository, IProjetoRepository
      {
            public ProjetoRepository(IDatabaseConnection dbConnection) : base(dbConnection) { }

            public async Task<int> CadastrarProjetoAsync(Projeto projeto)
            {
                  var parametros = new { projeto.Usuario_Id, projeto.Nome, projeto.Descricao, projeto.Criado_Em };
                  var dbCon = GerarConexaoMySql();
                  return await ExecuteAsync(dbCon, ProjetoSql.InserirProjeto, parametros, 60);
            }

            public async Task<List<Projeto>> ListarTodosProjetosDeUmUsuariosAsync(int usuarioId)
            {
                  var parametros = new { Usuario_Id = usuarioId };
                  var dbCon = GerarConexaoMySql();
                  var response = await QueryAsync<Projeto>(dbCon, ProjetoSql.ObterTodosProjetosPorUsuarioId, parametros, 60);
                  return response.ToList();
            }
      }
}
