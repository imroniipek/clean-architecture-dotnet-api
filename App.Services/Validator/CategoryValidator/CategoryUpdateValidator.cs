using App.Services.Category.Update;
using FluentValidation;

namespace App.Services.Validator.CategoryValidator;

public class CategoryUpdateValidator:AbstractValidator<UpdateCategoryRequest>
{
    public CategoryUpdateValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0).WithMessage("Id Alanı 0 dan Büyük Olmalıdır").NotEmpty().NotNull();
        
        RuleFor(x=>x.Name).NotEmpty().WithMessage("Ürün adı boş olamaz.")
            .MinimumLength(3).WithMessage("Ürün adı en az 3 karakter olmalıdır.")
            .MaximumLength(250).WithMessage("Ürün adı en fazla 100 karakter olabilir.");
    }
}