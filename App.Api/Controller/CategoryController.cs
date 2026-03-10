using App.Services.Category;
using App.Services.Category.Create;
using Microsoft.AspNetCore.Mvc;

namespace App.Api.Controller;

[ApiController]
[Route("api/[controller]")]
public class CategoryController(ICategoryService service) : CustomBaseController
{
    [HttpGet]
    public async Task<IActionResult> GetAll()
        => CreateActionResult(await service.GetAllCategoriesAsync());

    [HttpGet("{id:int}/products")]
    public async Task<IActionResult> GetCategoryWithProductsById(int id)
        => CreateActionResult(await service.GetCategoryWithProductsByIdAsync(id));

    [HttpGet("with-products")]
    public async Task<IActionResult> GetAllCategoriesWithProducts()
        => CreateActionResult(await service.GetAllCategoriesWithProductsAsync());

    [HttpPost]
    public async Task<IActionResult> Create(CreateCategoryRequest request)
    {
        var result = await service.CreateCategoryAsync(request);
        return CreateActionResult(result);
    }
    
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
        => CreateActionResult(await service.DeleteCategoryAsync(id));
}