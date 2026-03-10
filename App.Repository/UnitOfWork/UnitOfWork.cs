using App.Repository.Context;

namespace App.Repository.UnitOfWork;

public class UnitOfWork(AppDbContext context):IUnitOfWork
{
    public Task<int> SaveAllChangesInDbAsync() => context.SaveChangesAsync();
}