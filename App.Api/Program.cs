using App.Api.ExceptionHandler;
using App.Api.Filters;
using App.Repository.Cache;
using App.Repository.Context;
using App.Repository.Interceptors;
using App.Repository.Repo.Abstract;
using App.Repository.Repo.CategoryRepo;
using App.Repository.Repo.ProductRepository;
using App.Repository.UnitOfWork;
using App.Services.Category;
using App.Services.Product;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("SqlServer");

builder.Services.AddScoped<AuditDbContextInterceptor>();

builder.Services.AddDbContext<AppDbContext>((sp, options) =>
{
    var interceptor = sp.GetRequiredService<AuditDbContextInterceptor>();
    options.UseSqlServer(connectionString);
    options.AddInterceptors(interceptor);
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAuthorization();
builder.Services.AddMemoryCache();
builder.Services.AddSingleton<ICacheService, AppCaching>();

builder.Services.AddScoped(typeof(IGenericRepository<,>), typeof(GenericRepository<,>));
builder.Services.AddScoped<CheckFilter>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddExceptionHandler<ConflictExceptionHandler>();
builder.Services.AddScoped(typeof(NotFoundFilter<,>));

builder.Services.AddExceptionHandler<NotFoundExceptionHandler>();
builder.Services.AddProblemDetails();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseExceptionHandler();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();