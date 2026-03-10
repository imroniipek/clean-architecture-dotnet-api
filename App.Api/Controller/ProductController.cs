using App.Api.Filters;
using App.Repository.Entities;
using App.Services.Product;
using App.Services.Product.Create;
using App.Services.Product.Update;
using Microsoft.AspNetCore.Mvc;
namespace App.Api.Controller;

[ApiController]
[Route("api/[controller]")]

public class ProductController(IProductService service):CustomBaseController
{
    [HttpGet]
    public async Task<IActionResult> GetAll() => CreateActionResult(await service.GetAllOfProductAsync());
    
    
    [ServiceFilter(typeof(NotFoundFilter<Product,int>))]
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
        => CreateActionResult(await service.GetProductByIdAsync(id));

    
    
    [HttpGet("{pageIndex:int}/{pageSize:int}")]
    public async Task<IActionResult> GetPagedAllListAsync(int pageIndex, int pageSize) =>
        CreateActionResult(await service.GetPagedAllListedAsync(pageIndex, pageSize));
    
    [HttpPost]
    public async Task<IActionResult> Create(ProductCreateRequest request)
    {
        var result = await service.CreateProductAsync(request);
        return CreateActionResult(result);
    }
    //route constraint//
    
    [ServiceFilter(typeof(NotFoundFilter<Product,int>))]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, ProductUpdateRequest request)
        => CreateActionResult(await service.UpdateProductAsync(id, request));
    
    [ServiceFilter(typeof(NotFoundFilter<Product,int>))]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id) => CreateActionResult(await service.DeleteProductAsync(id));
}