namespace Deploy.Api.Core.Repositories.Sql
{
    public static class DominioSql
    {
        public static readonly string ValidarExistenciaDoDominioPeloSubdominio = @"SELECT EXISTS(SELECT 1 FROM Dominios WHERE subdominio = @Subdominio);";
    }
}