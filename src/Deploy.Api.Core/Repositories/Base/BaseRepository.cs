using System.Data;
using Dapper;
using Deploy.Api.Core.Interfaces.Repositories.Base.IDbCon;
using Polly;

namespace Deploy.Api.Core.Repositories.Base
{
    public abstract class BaseRepository
    {
        private readonly IDatabaseConnection _dbConnection;

        private static readonly AsyncPolicy RetryPolicy = Policy
            .Handle<Exception>()
            .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));

        protected BaseRepository(IDatabaseConnection dbConnection) => _dbConnection = dbConnection;

        protected IDbConnection GerarConexaoMySql() => _dbConnection.GetConexaoMySql();

        public async Task<IEnumerable<T>> QueryAsync<T>(IDbConnection dbCon, string sql, object parameters = null, int timeout = 60)
        {
            return await RetryPolicy.ExecuteAsync(
                async () => await dbCon.QueryAsync<T>(sql, parameters, commandTimeout: timeout)
            );
        }

        public async Task<int> ExecuteAsync(IDbConnection dbCon, string sql, object parameters = null, int timeout = 60, CommandType commandType = CommandType.Text)
        {
            return await RetryPolicy.ExecuteAsync(
                async () => await dbCon.ExecuteAsync(sql, parameters, commandTimeout: timeout, commandType: commandType)
            );
        }
    }
}
