namespace App.Repository.Entities;

public interface IAudiEntity
{
    public DateTime CreatedAt { get; set; }
    
    public DateTime? UpdatedAt { get; set; }
}