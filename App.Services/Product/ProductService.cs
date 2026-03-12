using System.Net;
using App.Repository.Cache;
using App.Services.Dto;
using App.Services.Product.Create;
using App.Services.ServiceResult;
using App.Repository.Repo.ProductRepository;
using App.Repository.UnitOfWork;
using App.Services.Exceptions;
using App.Services.Product.Update;
using Microsoft.EntityFrameworkCore;

namespace App.Services.Product;

public class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICacheService _cache;
    private static readonly string CacheKey = "product-list";

    public ProductService(IProductRepository productRepository, IUnitOfWork unitOfWork, ICacheService cache)
    {
        _productRepository = productRepository;
        _unitOfWork = unitOfWork;
        _cache = cache;
    }

    public async Task<ServiceResult<List<ProductDto>>> GetTopPriceProductAsync(int count)
    {
        var products = await _productRepository.TheTopSellingProducts(count);

        if (products is null || products.Count == 0)
            throw new NotFoundException("Ürünler bulunamadı");

        var dtoList = products
            .Select(p => new ProductDto(p.Id, p.Name, p.Price, p.Count, p.CategoryId))
            .ToList();

        return ServiceResult<List<ProductDto>>.Success(dtoList);
    }

    public async Task<ServiceResult<List<ProductDto>>> GetAllOfProductAsync()
    {
        var cachedProducts = await _cache.GetAsync<List<ProductDto>>(CacheKey);

        if (cachedProducts is not null)
        {
            return ServiceResult<List<ProductDto>>.Success(cachedProducts);
        }

        var productDtoList = await _productRepository
            .GetAll()
            .Select(product => new ProductDto(
                product.Id,
                product.Name,
                product.Price,
                product.Count,
                product.CategoryId))
            .ToListAsync();

        await _cache.AddAsync(CacheKey, productDtoList, TimeSpan.FromMinutes(5));

        return ServiceResult<List<ProductDto>>.Success(productDtoList);
    }

    public async Task<ServiceResult<ProductDto>> GetProductByIdAsync(int id)
    {
        var product = await _productRepository.GetByIdAsync(id);

        if (product is null)
            throw new NotFoundException("Ürün bulunamadı");

        var productDto = new ProductDto(product.Id, product.Name, product.Price, product.Count, product.CategoryId);
        return ServiceResult<ProductDto>.Success(productDto);
    }

    public async Task<ServiceResult<List<ProductDto>>> GetPagedAllListedAsync(int pageNumber, int pageSize)
    {
        var products = await _productRepository.GetAll()
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        if (!products.Any())
            throw new NotFoundException("Ürünler bulunamadı");

        var dtoList = products
            .Select(p => new ProductDto(p.Id, p.Name, p.Price, p.Count, p.CategoryId))
            .ToList();

        return ServiceResult<List<ProductDto>>.Success(dtoList);
    }

    public async Task<ServiceResult.ServiceResult> DeleteProductAsync(int productId)
    {
        var product = await _productRepository.GetByIdAsync(productId);

        if (product is null)
            throw new NotFoundException("Ürün bulunamadı");

        _productRepository.Delete(product);

        await _unitOfWork.SaveAllChangesInDbAsync();
        await _cache.RemoveAsync(CacheKey);

        return ServiceResult.ServiceResult.Success(HttpStatusCode.NoContent);
    }

    public async Task<ServiceResult.ServiceResult> UpdateProductAsync(int productId, ProductUpdateRequest request)
    {
        var product = await _productRepository.GetByIdAsync(productId);

        if (product is null)
            throw new NotFoundException("Ürün bulunamadı");

        product.Name = request.Name.ToLower();
        product.Price = request.Price;
        product.Count = request.Count;
        product.CategoryId = request.CategoryId;

        _productRepository.Update(product);

        await _unitOfWork.SaveAllChangesInDbAsync();
        await _cache.RemoveAsync(CacheKey);

        return ServiceResult.ServiceResult.Success(HttpStatusCode.NoContent);
    }

    public async Task<ServiceResult.ServiceResult> UpdateTheProductCount(int productId, int count)
    {
        var theProduct = await _productRepository.GetByIdAsync(productId);

        if (theProduct is null)
            throw new NotFoundException("Ürün bulunamadı");

        theProduct.Count = count;

        _productRepository.Update(theProduct);
        await _unitOfWork.SaveAllChangesInDbAsync();
        await _cache.RemoveAsync(CacheKey);

        return ServiceResult.ServiceResult.Success(HttpStatusCode.NoContent);
    }

    public async Task<ServiceResult<ProductCreateResponse>> CreateProductAsync(ProductCreateRequest request)
    {
        var normalizedName = request.Name.ToLower();

        bool exists = await _productRepository
            .GetAll()
            .AnyAsync(x => x.Name.ToLower() == normalizedName);

        if (exists)
            return ServiceResult<ProductCreateResponse>.Failed(new[] { "Db'de bu ürün bulunmaktadır" });

        var product = new Repository.Entities.Product
        {
            Name = normalizedName,
            Price = request.Price,
            Count = request.Count,
            CategoryId = request.CategoryId
        };

        await _productRepository.AddAsync(product);
        await _unitOfWork.SaveAllChangesInDbAsync();
        await _cache.RemoveAsync(CacheKey);

        return ServiceResult<ProductCreateResponse>
            .SuccessForCreated(new ProductCreateResponse(product.Id), $"/api/products/{product.Id}");
    }
}