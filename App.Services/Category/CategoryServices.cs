using System.Net;
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

    public CategoryService(ICategoryRepository categoryRepository, IUnitOfWork unitOfWork)
    {
        _categoryRepository = categoryRepository;
        _unitOfWork = unitOfWork;
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

        return ServiceResult.ServiceResult<int>.SuccessForCreated(category.Id, $"/api/categories/{category.Id}");
    }

    public async Task<ServiceResult.ServiceResult> UpdateCategoryAsync(UpdateCategoryRequest request)
    {
        var category = await _categoryRepository.GetByIdAsync(request.Id);

        if (category is null)
        {
            throw new NotFoundException("Kategori Bulunamadı");
        }

        var normalizedName = request.Name.Trim().ToLower();

        var isNameUsedByAnotherCategory = await _categoryRepository
            .GetAll()
            .AnyAsync(x => x.Id != request.Id && x.Name.ToLower() == normalizedName);

        if (isNameUsedByAnotherCategory)
        {
            return ServiceResult.ServiceResult.Failed(HttpStatusCode.BadRequest,new[] { "Bu kategori adı başka bir kategoride zaten kullanılıyor." });
        }

        category.Name = request.Name.Trim();

        _categoryRepository.Update(category);
        await _unitOfWork.SaveAllChangesInDbAsync();

        return ServiceResult.ServiceResult.Success(HttpStatusCode.NoContent);
    }

    public async Task<ServiceResult.ServiceResult> DeleteCategoryAsync(int id)
    {
        var category = await _categoryRepository.GetByIdAsync(id);

        if (category is null)
        {
            throw new NotFoundException("Kategori Bulunamadı");
        }

        _categoryRepository.Delete(category);
        await _unitOfWork.SaveAllChangesInDbAsync();

        return ServiceResult.ServiceResult.Success(HttpStatusCode.NoContent);
    }

    public async Task<ServiceResult.ServiceResult<List<CategoryDto>>> GetAllCategoriesAsync()
    {
        var categories = await _categoryRepository
            .GetAll()
            .AsNoTracking()
            .Select(x => new CategoryDto(x.Id, x.Name))
            .ToListAsync();

        if (categories is null || categories.Count == 0)
        {
            throw new NotFoundException("Categoriler Listesi Bulunamadı");
        }

        return ServiceResult.ServiceResult<List<CategoryDto>>.Success(categories);
    }

    public async Task<ServiceResult.ServiceResult<CategoryWithProductsDto>> GetCategoryWithProductsByIdAsync(int id)
    {
        var category = await _categoryRepository.GetCategoryWithProductsAsync(id);

        if (category is null)
        {
            return ServiceResult.ServiceResult<CategoryWithProductsDto>.Failed(new[] { "Kategori bulunamadı." });
        }

        var categoryDto = new CategoryWithProductsDto(
            category.Id,
            category.Name,
            category.ProductList is null
                ? new List<ProductDto>()
                : category.ProductList.Select(p => new ProductDto(
                    p.Id,
                    p.Name,
                    p.Price,
                    p.Count,
                    p.CategoryId
                )).ToList()
        );

        return ServiceResult.ServiceResult<CategoryWithProductsDto>.Success(categoryDto);
    }

    public async Task<ServiceResult.ServiceResult<List<CategoryWithProductsDto>>> GetAllCategoriesWithProductsAsync()
    {
        var categories = await _categoryRepository
            .GetAllCategoriesWithProducts()
            .AsNoTracking()
            .ToListAsync();

        var result = categories.Select(category => new CategoryWithProductsDto(
            category.Id,
            category.Name,
            category.ProductList is null
                ? new List<ProductDto>()
                : category.ProductList.Select(p => new ProductDto(
                    p.Id,
                    p.Name,
                    p.Price,
                    p.Count,
                    p.CategoryId
                )).ToList()
        )).ToList();

        return ServiceResult.ServiceResult<List<CategoryWithProductsDto>>.Success(result);
    }

}