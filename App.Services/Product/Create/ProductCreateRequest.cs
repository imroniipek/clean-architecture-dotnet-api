namespace App.Services.Product.Create;
//Record sadece veri tasıyan nesnelerdir.Immutabledır değiştirelemez//

public record ProductCreateRequest(string Name,decimal Price ,int Count,int CategoryId);