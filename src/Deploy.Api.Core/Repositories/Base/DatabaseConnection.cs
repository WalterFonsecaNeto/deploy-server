using Microsoft.Extensions.Configuration;
using System.Data;
using Deploy.Api.Core.Interfaces.Repositories.Base.IDbCon;
using MySql.Data.MySqlClient;


namespace Deploy.Api.Core.Repository.Base
{
    public class DatabaseConnection : IDatabaseConnection
    {
        private readonly string _mySqlConnString;
        private IDbConnection? _conexao;

        public DatabaseConnection(IConfiguration configuration)
        {
            _mySqlConnString = configuration.GetConnectionString("MySQLConectionString");

            if (string.IsNullOrWhiteSpace(_mySqlConnString))
                throw new ArgumentException("String de conexão mySQL não encontrada.");

        }

        public IDbConnection GetConexaoMySql()
        {
            return _conexao ??= new MySqlConnection(_mySqlConnString);
        }
    }
}