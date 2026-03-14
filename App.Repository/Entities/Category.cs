namespace App.Repository.Entities;

public class Category:IBaseEntity<int>,IAudiEntity
{
    public int Id { set; get; }

    public string Name { set; get; } = default!;

    public List<Product> ProductList { get; init; } = new List<Product>();

    public DateTime CreatedAt { get; set; }
    
    public DateTime? UpdatedAt { get; set; }
}