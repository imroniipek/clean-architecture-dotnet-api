namespace App.Repository.Repo.Abstract;

public interface IGenericRepository<T,T1> where T:class where T1:struct
{

    Task<bool> AnyAsync(T1 id);
    IQueryable<T> GetAll();
    
    ValueTask<T?> GetByIdAsync(int id);

    ValueTask AddAsync(T entity);

    void Update(T entity);

    void Delete(T entity);
}