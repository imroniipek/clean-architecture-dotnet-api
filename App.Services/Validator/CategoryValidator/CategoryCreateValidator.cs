using App.Services.Category.Create;
using FluentValidation;

namespace App.Services.Validator.CategoryValidator;

public class CategoryCreateValidator:AbstractValidator<CreateCategoryRequest>
{
    public CategoryCreateValidator()
    {
        RuleFor(x=>x.Name).NotEmpty().WithMessage("Ürün adı boş olamaz.")
            .MinimumLength(3).WithMessage("Ürün adı en az 3 karakter olmalıdır.")
            .MaximumLength(250).WithMessage("Ürün adı en fazla 100 karakter olabilir.");
    }
}