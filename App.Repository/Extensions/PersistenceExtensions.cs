using App.Repository.Context;
using App.Repository.Interceptors;
using App.Repository.Repo.Abstract;
using App.Repository.Repo.CategoryRepo;
using App.Repository.Repo.ProductRepository;
using App.Repository.UnitOfWork;
using Microsoft.EntityFrameworkCore;

namespace App.Api.Extensions;

public class PersistenceExtensions
{
    public static IServiceCollection AddPersistence(IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("SqlServer");

        services.AddScoped<AuditDbContextInterceptor>();

        services.AddDbContext<AppDbContext>((sp, options) =>
        {
            var interceptor = sp.GetRequiredService<AuditDbContextInterceptor>();
            options.UseSqlServer(connectionString);
            options.AddInterceptors(interceptor);
        });

        services.AddScoped(typeof(IGenericRepository<,>), typeof(GenericRepository<,>));

        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }
}