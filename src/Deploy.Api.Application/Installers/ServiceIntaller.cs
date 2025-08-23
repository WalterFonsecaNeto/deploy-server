using Deploy.Api.Core.Interfaces.Services;
using Deploy.Api.Core.Interfaces.Repositories;
using Deploy.Api.Core.Repositories;
using Deploy.Api.Core.Interfaces.Repositories.Base.IDbCon;
using Deploy.Api.Core.Repository.Base;
using Deploy.Api.Core.Services;


namespace Deploy.Api.Application.Installers
{
    public static class ServiceInstaller
    {
        public static void AddCustomServices(this IServiceCollection services)
        {
            //Conections
            services.AddScoped<IDatabaseConnection, DatabaseConnection>();

            //Services
            services.AddScoped<IUsuarioService, UsuarioService>();
            services.AddScoped<IDeployService, DeployService>();

            //Repositories
            services.AddScoped<IUsuarioRepository, UsuarioRepository>();
        }
    }
}