namespace App.Services.Dto;

public record CategoryWithProductsDto(int Id, string Name, List<ProductDto> Products );
