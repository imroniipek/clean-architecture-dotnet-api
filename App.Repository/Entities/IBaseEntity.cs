namespace App.Repository.Entities;

public interface IBaseEntity<T>
{
    public T Id { get; set; }
}