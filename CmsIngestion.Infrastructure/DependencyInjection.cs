using CmsIngestion.Core.Interfaces;
using CmsIngestion.Infrastructure.Data;
using CmsIngestion.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CmsIngestion.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlite(configuration.GetConnectionString("DefaultConnection") ?? "Data Source=cms.db"));

            services.AddScoped<ICmsRepository, CmsRepository>();

            return services;
        }
    }
}
