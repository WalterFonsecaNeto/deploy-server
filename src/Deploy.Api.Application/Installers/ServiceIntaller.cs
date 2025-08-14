using Deploy.Api.Core.Interfaces.Services;
using Deploy.Api.Core.Services;


namespace Deploy.Api.Application.Installers
{
    public static class ServiceInstaller
    {
        public static void AddCustomServices(this IServiceCollection services)
        {
            services.AddScoped<IDeployService, DeployService>();
        }
    }
}