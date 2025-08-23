using System.Data;

namespace Deploy.Api.Core.Interfaces.Repositories.Base.IDbCon
{
    public interface IDatabaseConnection
    {
        IDbConnection GetConexaoMySql();
    }
}