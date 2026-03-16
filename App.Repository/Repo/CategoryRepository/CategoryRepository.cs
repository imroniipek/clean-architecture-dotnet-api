using App.Repository.Context;
using App.Repository.Entities;
using App.Repository.Repo.Abstract;
using Microsoft.EntityFrameworkCore;

namespace App.Repository.Repo.CategoryRepo;


public class CategoryRepository(AppDbContext context) :GenericRepository<Category,int>(context),ICategoryRepository
{
    public IQueryable<Category> GetAllCategoriesWithProducts() => DbSet.Include(x => x.ProductList);
    
        public async Task<Category?> GetCategoryWithProductsAsync(int categoryId)
        {
            return await DbSet
                .Include(x => x.ProductList)
                .FirstOrDefaultAsync(x => x.Id == categoryId);
        }
    
}