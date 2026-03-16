using App.Services.Category;
using App.Services.Product;
using Microsoft.Extensions.DependencyInjection;

namespace App.Services.Extensions;

public static class ApplicationServiceExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<IProductService, ProductService>();

        return services;
    }
}