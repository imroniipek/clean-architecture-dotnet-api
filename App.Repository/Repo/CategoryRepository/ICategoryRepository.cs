using App.Repository.Entities;
using App.Repository.Repo.Abstract;

namespace App.Repository.Repo.CategoryRepo;

public interface ICategoryRepository:IGenericRepository<Category,int>
{
    IQueryable<Category> GetAllCategoriesWithProducts();
    Task<Category?> GetCategoryWithProductsAsync(int categoryId);
}