namespace Deploy.Api.Core.Repositories.Sql
{
    public static class ProjetoSql
    {
        public static readonly string ObterTodosProjetosPorUsuarioId = "SELECT * FROM Projetos WHERE Usuario_Id = @Usuario_Id;";
        public static readonly string ObterProjetoPorId = "SELECT * FROM Projetos WHERE Id = @ProjetoId;";
        public static readonly string InserirProjeto = "INSERT INTO Projetos (Usuario_Id, Nome, Descricao, Criado_Em) VALUES (@Usuario_Id, @Nome, @Descricao, @Criado_Em);";
    }
}