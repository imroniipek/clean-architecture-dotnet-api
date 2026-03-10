

namespace App.Repository.Entities;

public class Product:IBaseEntity<int>,IAudiEntity
{
    public int Id { get; set; }

    public string Name { get; set; } = default!;

    public decimal Price { get; set; }

    public int Count { get; set; }

    public int CategoryId { get; set; }

    public Category Category { get; set; } = default!;
    
    public DateTime CreatedAt { get; set; }
    
    public DateTime? UpdatedAt { get; set; }
}