using System.Net;
using App.Repository.Repo.CategoryRepo;
using App.Repository.Repo.ProductRepository;
using App.Repository.UnitOfWork;
using App.Services.Dto;
using App.Services.Exceptions;
using App.Services.Product.Create;
using App.Services.Product.Update;
using App.Services.ServiceResult;
using Microsoft.EntityFrameworkCore;

namespace App.Services.Product;

public class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICategoryRepository _categoryRepository;

    public ProductService(IProductRepository productRepository,IUnitOfWork unitOfWork, ICategoryRepository categoryRepository)
    {
        _productRepository = productRepository;
        _unitOfWork = unitOfWork;
        _categoryRepository = categoryRepository;
    }

    public async Task<ServiceResult<List<ProductDto>>> GetTopPriceProductAsync(int count)
    {
        var products = await _productRepository.TheTopSellingProducts(count);

        if (products.Count == 0)
        {
            return ServiceResult<List<ProductDto>>
                .Failed(["Db Classında ürün bulunmaktadır"],HttpStatusCode.NotFound);
        }

        var dtoList = products
            .Select(p => new ProductDto(p.Id, p.Name, p.Price, p.Count, p.CategoryId))
            .ToList();

        return ServiceResult<List<ProductDto>>.Success(dtoList);
    }

    public async Task<ServiceResult<List<ProductDto>>> GetAllOfProductAsync()
    {
        var productDtoList = await _productRepository
            .GetAll()
            .Select(product => new ProductDto(
                product.Id,
                product.Name,
                product.Price,
                product.Count,
                product.CategoryId))
            .ToListAsync();

        return ServiceResult<List<ProductDto>>.Success(productDtoList);
    }

    public async Task<ServiceResult<ProductDto>> GetProductByIdAsync(int id)
    {
        var product = await _productRepository.GetByIdAsync(id);

        if (product is null)
            throw new NotFoundException("Product", id);

        var productDto = new ProductDto(product.Id, product.Name, product.Price, product.Count, product.CategoryId);
        return ServiceResult<ProductDto>.Success(productDto);
    }

    public async Task<ServiceResult<List<ProductDto>>> GetPagedAllListedAsync(int pageNumber, int pageSize)
    {
        var products = await _productRepository.GetAll()
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var dtoList = products
            .Select(p => new ProductDto(p.Id, p.Name, p.Price, p.Count, p.CategoryId))
            .ToList();

        return ServiceResult<List<ProductDto>>.Success(dtoList);
    }

    public async Task<ServiceResult.ServiceResult> DeleteProductAsync(int productId)
    {
        var product = await _productRepository.GetByIdAsync(productId);

        if (product is null)
            throw new NotFoundException("Product", productId);

        _productRepository.Delete(product);
        await _unitOfWork.SaveAllChangesInDbAsync();
        return ServiceResult.ServiceResult.Success(HttpStatusCode.NoContent);
    }

    public async Task<ServiceResult.ServiceResult> UpdateProductAsync(int productId, ProductUpdateRequest request)
    {
        var product = await _productRepository.GetByIdAsync(productId);

        if (product is null)
            throw new NotFoundException("Product", productId);

        var categoryExists = await _categoryRepository.AnyAsync(request.CategoryId);
        if (!categoryExists)
            throw new NotFoundException("Category", request.CategoryId);

        var normalizedName = request.Name.Trim().ToLower();

        bool exists = await _productRepository
            .GetAll()
            .AnyAsync(x => x.Name.ToLower() == normalizedName && x.Id != productId);

        if (exists)
            throw new ConflictException("Bu ürün adı başka bir üründe zaten kullanılıyor.");

        product.Name = normalizedName;
        product.Price = request.Price;
        product.Count = request.Count;
        product.CategoryId = request.CategoryId;

        _productRepository.Update(product);
        await _unitOfWork.SaveAllChangesInDbAsync();

        return ServiceResult.ServiceResult.Success(HttpStatusCode.NoContent);
    }

    public async Task<ServiceResult.ServiceResult> UpdateTheProductCount(int productId, int count)
    {
        var theProduct = await _productRepository.GetByIdAsync(productId);

        if (theProduct is null)
            throw new NotFoundException("Product", productId);

        if (count < 0)
            throw new ConflictException("Stok sayısı negatif olamaz.");

        theProduct.Count = count;
        _productRepository.Update(theProduct);
        await _unitOfWork.SaveAllChangesInDbAsync();
        return ServiceResult.ServiceResult.Success(HttpStatusCode.NoContent);
    }

    public async Task<ServiceResult<ProductCreateResponse>> CreateProductAsync(ProductCreateRequest request)
    {
        var normalizedName = request.Name.Trim().ToLower();

        var categoryExists = await _categoryRepository.AnyAsync(request.CategoryId);
        
        if (!categoryExists)
            throw new NotFoundException("Category", request.CategoryId);

        bool exists = await _productRepository
            .GetAll()
            .AnyAsync(x => x.Name.ToLower() == normalizedName);

        if (exists)
            throw new ConflictException("Bu ürün veritabanında zaten bulunmaktadır.");

        var product = new Repository.Entities.Product
        {
            Name = normalizedName,
            Price = request.Price,
            Count = request.Count,
            CategoryId = request.CategoryId
        };

        await _productRepository.AddAsync(product);
        await _unitOfWork.SaveAllChangesInDbAsync();

        return ServiceResult<ProductCreateResponse>
            .SuccessForCreated(new ProductCreateResponse(product.Id), $"/api/products/{product.Id}");
    }
}