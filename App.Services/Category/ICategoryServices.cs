using App.Services.Category.Create;
using App.Services.Dto;

namespace App.Services.Category;

public interface ICategoryService
{
    Task<ServiceResult.ServiceResult<int>> CreateCategoryAsync(CreateCategoryRequest request);
    
    Task<ServiceResult.ServiceResult> DeleteCategoryAsync(int id);
    
    Task<ServiceResult.ServiceResult<List<CategoryDto>>> GetAllCategoriesAsync();

    Task<ServiceResult.ServiceResult<CategoryWithProductsDto>>GetCategoryWithProductsByIdAsync(int id);

    Task<ServiceResult.ServiceResult<List<CategoryWithProductsDto>>> GetAllCategoriesWithProductsAsync();
}