using App.Repository.Context;
using App.Repository.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;

namespace App.Repository.Repo.Abstract;

public class GenericRepository<T, T1>(AppDbContext context)
    : IGenericRepository<T, T1>
    where T : class, IBaseEntity<T1>
    where T1 : struct
{
    
    //Context degiskeni burdan alammın sebebi, icinde Dbset diyerek bazı tablelarımı olusturup onlar uzerinde işlem yapıyorum//

    public readonly DbSet<T> DbSet = context.Set<T>();


    public Task<bool> AnyAsync(T1 id) => DbSet.AnyAsync(x => x.Id.Equals(id));
   

    public IQueryable<T> GetAll()=>DbSet.AsNoTracking();
    

    public async ValueTask<T?> GetByIdAsync(int id)=>await DbSet.FindAsync(id);
   
    public async ValueTask AddAsync(T entity)
    {
         await DbSet.AddAsync(entity);
    }

    public void Update(T entity)=>  DbSet.Update(entity);
    

    public void Delete(T entity)=>DbSet.Remove(entity);
    
}