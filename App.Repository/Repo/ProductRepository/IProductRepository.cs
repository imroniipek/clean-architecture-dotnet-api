using App.Repository.Entities;
using App.Repository.Repo.Abstract;

namespace App.Repository.Repo.ProductRepository;

public interface IProductRepository:IGenericRepository<Product,int>
{
    public Task<List<Product>> TheTopSellingProducts(int count);
    
}