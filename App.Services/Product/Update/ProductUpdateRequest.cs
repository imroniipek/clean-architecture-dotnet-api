namespace App.Services.Product.Update;

public record ProductUpdateRequest(string Name, decimal Price, int Count,int CategoryId);
