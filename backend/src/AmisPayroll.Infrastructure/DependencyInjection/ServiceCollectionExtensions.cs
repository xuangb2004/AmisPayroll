using AmisPayroll.Application.Interfaces.Repositories;
using AmisPayroll.Infrastructure.Context;
using AmisPayroll.Infrastructure.Repositories;
using Dapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AmisPayroll.Infrastructure.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            DefaultTypeMap.MatchNamesWithUnderscores = true;

            var connectionString = configuration.GetConnectionString("DefaultConnection");

            services.AddSingleton<IDbConnectionFactory>(new MySqlConnectionFactory(connectionString));

            // services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));
            services.AddScoped<ISalaryCompositionRepository, SalaryCompositionRepository>();
            // services.AddScoped<IOrganizationRepository, OrganizationRepository>();
            // services.AddScoped<IGridConfigRepository, GridConfigRepository>();

            return services;
        }
    }
}