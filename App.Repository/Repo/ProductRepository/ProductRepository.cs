using App.Repository.Context;
using App.Repository.Entities;
using App.Repository.Repo.Abstract;
using Microsoft.EntityFrameworkCore;
namespace App.Repository.Repo.ProductRepository;

public class ProductRepository(AppDbContext context):GenericRepository<Product,int>(context),IProductRepository
{
    public async Task<List<Product>> TheTopSellingProducts(int count)=>  await DbSet.OrderByDescending(x => x.Price).Take(count).ToListAsync();
}