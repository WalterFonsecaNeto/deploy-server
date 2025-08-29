namespace Deploy.Api.Core.Interfaces.Repositories
{
      public interface IDominioRepository
      {
            Task<int> ValidarExistenciaDoDominioPeloSubdominioAsync(string subdominio);
      }
}