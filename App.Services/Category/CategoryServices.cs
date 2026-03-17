using System.Net;
using App.Repository.Cache;
using App.Repository.Repo.CategoryRepo;
using App.Repository.UnitOfWork;
using App.Services.Category.Create;
using App.Services.Category.Update;
using App.Services.Dto;
using App.Services.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace App.Services.Category;

public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICacheService _cacheService;

    private static readonly string CategoriesCacheKey = "categories-all";
    private static readonly string CategoriesWithProductsCacheKey = "categories-with--products";
    private static string CategoryWithProductsByIdCacheKey(int id) => $"categories:with-products:{id}";

    public CategoryService(ICategoryRepository categoryRepository, IUnitOfWork unitOfWork, ICacheService cacheService)
    {
        _categoryRepository = categoryRepository;
        _unitOfWork = unitOfWork;
        _cacheService = cacheService;
    }

    public async Task<ServiceResult.ServiceResult<int>> CreateCategoryAsync(CreateCategoryRequest request)
    {
        var categoryName = request.Name.Trim();

        var isExist = await _categoryRepository
            .GetAll()
            .AnyAsync(x => x.Name.ToLower() == categoryName.ToLower());

        if (isExist)
        {
            return ServiceResult.ServiceResult<int>.Failed(new[] { "Bu kategori zaten veritabanında mevcut." });
        }

        var category = new Repository.Entities.Category
        {
            Name = categoryName
        };

        await _categoryRepository.AddAsync(category);
        await _unitOfWork.SaveAllChangesInDbAsync();
        await _cacheService.RemoveAsync(CategoriesCacheKey);
        await _cacheService.RemoveAsync(CategoriesWithProductsCacheKey);
        return ServiceResult.ServiceResult<int>.SuccessForCreated(category.Id, $"/api/categories/{category.Id}");
    }

    public async Task<ServiceResult.ServiceResult> UpdateCategoryAsync(UpdateCategoryRequest request)
    {
        var category = await _categoryRepository.GetByIdAsync(request.Id);

        if (category is null)
        {
            throw new NotFoundException("Category", request.Id);
        }

        var normalizedName = request.Name.Trim().ToLower();

        var isNameUsedByAnotherCategory = await _categoryRepository
            .GetAll()
            .AnyAsync(x => x.Id != request.Id && x.Name.ToLower() == normalizedName);

        if (isNameUsedByAnotherCategory)
        {
            return ServiceResult.ServiceResult.Failed(
                HttpStatusCode.BadRequest,
                new[] { "Bu kategori adı başka bir kategoride zaten kullanılıyor." });
        }

        category.Name = request.Name.Trim();

        _categoryRepository.Update(category);
        await _unitOfWork.SaveAllChangesInDbAsync();

        await _cacheService.RemoveAsync(CategoriesCacheKey);
        await _cacheService.RemoveAsync(CategoriesWithProductsCacheKey);
        await _cacheService.RemoveAsync(CategoryWithProductsByIdCacheKey(request.Id));

        return ServiceResult.ServiceResult.Success(HttpStatusCode.NoContent);
    }

    public async Task<ServiceResult.ServiceResult> DeleteCategoryAsync(int id)
    {
        var category = await _categoryRepository.GetByIdAsync(id);

        if (category is null)
        {
            throw new NotFoundException("Kategori Bulunamadı", id);
        }

        _categoryRepository.Delete(category);
        await _unitOfWork.SaveAllChangesInDbAsync();

        await _cacheService.RemoveAsync(CategoriesCacheKey);
        await _cacheService.RemoveAsync(CategoriesWithProductsCacheKey);
        await _cacheService.RemoveAsync(CategoryWithProductsByIdCacheKey(id));

        return ServiceResult.ServiceResult.Success(HttpStatusCode.NoContent);
    }

    public async Task<ServiceResult.ServiceResult<List<CategoryDto>>> GetAllCategoriesAsync()
    {
        var cachedCategories = await _cacheService.GetAsync<List<CategoryDto>>(CategoriesCacheKey);

        if (cachedCategories is not null)
        {
            return ServiceResult.ServiceResult<List<CategoryDto>>.Success(cachedCategories);
        }

        var categories = await _categoryRepository
            .GetAll()
            .AsNoTracking()
            .Select(x => new CategoryDto(x.Id, x.Name))
            .ToListAsync();

        await _cacheService.AddAsync(CategoriesCacheKey, categories, TimeSpan.FromMinutes(5));

        return ServiceResult.ServiceResult<List<CategoryDto>>.Success(categories);
    }

    public async Task<ServiceResult.ServiceResult<CategoryWithProductsDto>> GetCategoryWithProductsByIdAsync(int id)
    {
        var cacheKey = CategoryWithProductsByIdCacheKey(id);

        var cachedCategory = await _cacheService.GetAsync<CategoryWithProductsDto>(cacheKey);

        if (cachedCategory is not null)
        {
            return ServiceResult.ServiceResult<CategoryWithProductsDto>.Success(cachedCategory);
        }

        var category = await _categoryRepository.GetCategoryWithProductsAsync(id);

        if (category is null)
        {
            return ServiceResult.ServiceResult<CategoryWithProductsDto>.Failed(new[] { "Kategori bulunamadı." });
        }

        var categoryDto = new CategoryWithProductsDto(
            category.Id,
            category.Name,
            category.ProductList.Select(p => new ProductDto(p.Id, p.Name, p.Price, p.Count, p.CategoryId)).ToList()
        );

        await _cacheService.AddAsync(cacheKey, categoryDto, TimeSpan.FromMinutes(10));

        return ServiceResult.ServiceResult<CategoryWithProductsDto>.Success(categoryDto);
    }

    public async Task<ServiceResult.ServiceResult<List<CategoryWithProductsDto>>> GetAllCategoriesWithProductsAsync()
    {
        var cachedCategories = await _cacheService.GetAsync<List<CategoryWithProductsDto>>(CategoriesWithProductsCacheKey);

        if (cachedCategories is not null)
        {
            return ServiceResult.ServiceResult<List<CategoryWithProductsDto>>.Success(cachedCategories);
        }

        var categories = await _categoryRepository
            .GetAllCategoriesWithProducts()
            .AsNoTracking()
            .ToListAsync();

        var result = categories.Select(category => new CategoryWithProductsDto(
            category.Id,
            category.Name,
            category.ProductList.Select(p => new ProductDto(p.Id, p.Name, p.Price, p.Count, p.CategoryId)).ToList()
        )).ToList();

        await _cacheService.AddAsync(CategoriesWithProductsCacheKey, result, TimeSpan.FromMinutes(10));

        return ServiceResult.ServiceResult<List<CategoryWithProductsDto>>.Success(result);
    }
}