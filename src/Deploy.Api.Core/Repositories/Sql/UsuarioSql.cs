namespace Deploy.Api.Core.Repositories.Sql
{
    public static class UsuarioSql
    {
        public static readonly string ObterUsuarioPorEmail = "SELECT * FROM Usuarios WHERE Email = @Email;";
        public static readonly string ListarTodosUsuarios = "SELECT * FROM Usuarios;";
        public static readonly string InserirUsuario = "INSERT INTO Usuarios (Nome, Email, Senha, Criado_Em) VALUES (@Nome, @Email, @Senha, @Criado_Em);";

        
    }
}