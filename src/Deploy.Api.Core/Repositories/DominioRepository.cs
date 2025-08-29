using Deploy.Api.Core.Domain.Entidades;
using Deploy.Api.Core.Repositories.Sql;
using Deploy.Api.Core.Interfaces.Repositories.Base.IDbCon;
using Deploy.Api.Core.Repositories.Base;
using Deploy.Api.Core.Interfaces.Repositories;

namespace Deploy.Api.Core.Repositories
{
      public class DominioRepository : BaseRepository, IDominioRepository
      {
            public DominioRepository(IDatabaseConnection dbConnection) : base(dbConnection) { }

            public async Task<int> ValidarExistenciaDoDominioPeloSubdominioAsync(string subdominio)
            {
                  var parametros = new { Subdominio = subdominio };
                  var dbCon = GerarConexaoMySql();
                  return await ExecuteScalarAsync<int>(dbCon, DominioSql.ValidarExistenciaDoDominioPeloSubdominio, parametros, 60);
            }
      }
}
