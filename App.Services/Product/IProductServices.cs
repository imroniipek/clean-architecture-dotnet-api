using App.Services.Dto;
using App.Services.Product.Create;
using App.Services.Product.Update;

namespace App.Services.Product;

public interface IProductService
{
    public Task<ServiceResult.ServiceResult<List<ProductDto>>> GetTopPriceProductAsync(int count);

    public Task<ServiceResult.ServiceResult<ProductDto>> GetProductByIdAsync(int id);

    public Task<ServiceResult.ServiceResult<List<ProductDto>>> GetAllOfProductAsync();

    public Task<ServiceResult.ServiceResult> DeleteProductAsync(int productId);
    
    public Task<ServiceResult.ServiceResult<List<ProductDto>>> GetPagedAllListedAsync(int pageNumber,int pageSize);
    
    public Task<ServiceResult.ServiceResult<ProductCreateResponse>> CreateProductAsync(ProductCreateRequest request);
    
    public Task<ServiceResult.ServiceResult> UpdateProductAsync(int productId, ProductUpdateRequest request);

    public Task<ServiceResult.ServiceResult> UpdateTheProductCount(int productId, int count);
}

