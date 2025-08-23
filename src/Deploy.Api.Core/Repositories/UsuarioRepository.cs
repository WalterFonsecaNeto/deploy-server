using Deploy.Api.Core.Domain.Entidades;
using Deploy.Api.Core.Repositories.Sql;
using Deploy.Api.Core.Interfaces.Repositories.Base.IDbCon;
using Deploy.Api.Core.Repositories.Base;
using Deploy.Api.Core.Interfaces.Repositories;

namespace Deploy.Api.Core.Repositories
{
    public class UsuarioRepository : BaseRepository, IUsuarioRepository
    {
        public UsuarioRepository(IDatabaseConnection dbConnection) : base(dbConnection) { }

        public async Task<int> InserirUsuarioAsync(Usuario usuario)
        {
            var parametros = new { usuario.Nome, usuario.Email, usuario.Senha, usuario.Criado_Em};
            var dbCon = GerarConexaoMySql();
            return await ExecuteAsync(dbCon, UsuarioSql.InserirUsuario, parametros, 60);
        }

        public async Task<List<Usuario>> ListarTodosUsuariosAsync()
        {
            var dbCon = GerarConexaoMySql();
            var response = await QueryAsync<Usuario>(dbCon, UsuarioSql.ListarTodosUsuarios, 60);
            return response.ToList();
        }

        public async Task<Usuario> ObterUsuarioPorEmailAsync(string email)
        {
            var parametros = new { Email = email };
            var dbCon = GerarConexaoMySql();
            var response = await QueryAsync<Usuario>(dbCon, UsuarioSql.ObterUsuarioPorEmail, parametros, 60);
            return response.FirstOrDefault();
        }
       

    }
}
