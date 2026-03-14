/*
        * DbContextEventData ise, o bariyerden geçen aracın (yani senin veritabanı işleminin) içindeki her şeyi içeren "dosyadır".
        * * Bu dosyanın içinde sadece "context" yok, işlemin teknik künyesine dair birçok bilgi var.
        * Anlamana yardımcı olması için DbContextEventData içindeki en önemli parçaları bir tabloyla özetleyeyim:
        *
        *Parametre             Ne Anlama Gelir?                                Ne İçin Kullanırsın?
        .Context              O an çalışan DbContext nesnesidir.              Değişen verileri (Entity) yakalamak, ChangeTracker'a bakmak için.
        .EventId              EF Core'un bu işleme verdiği benzersiz kimlik.  Loglama yaparken işlemleri birbirinden ayırmak için.
       .LogLevel              Bu olayın ciddiyet seviyesi (Info, Warning vb.).    Sadece kritik durumlarda kod çalıştırmak için.
       .IsAsync               İşlemin async mi yoksa sync mi olduğu.          Kodun asenkron akışa uygun olup olmadığını kontrol etmek için.
       .StartTime             İşlemin başladığı an.                           Performans ölçümü yapmak için

        */
using App.Repository.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace App.Repository.Interceptors;

public class AuditDbContextInterceptor : SaveChangesInterceptor
{
    private static readonly Dictionary<EntityState, Action<DbContext, IAudiEntity>> Behaviors = new()
    {
        [EntityState.Added] = AddBehavior,
        [EntityState.Modified] = UpdateBehavior
    };

    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData,
        InterceptionResult<int> result)
    {
        ApplyAudit(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        ApplyAudit(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private static void ApplyAudit(DbContext? context)
    {
        if (context == null) return;

        foreach (var entry in context.ChangeTracker.Entries())
        {
            if (entry.Entity is IAudiEntity audiEntity &&
                Behaviors.TryGetValue(entry.State, out var behavior))
            {
                behavior(context, audiEntity);
            }
        }
    }

    private static void AddBehavior(DbContext context, IAudiEntity audiEntity)
    {
        audiEntity.CreatedAt = DateTime.UtcNow;
        context.Entry(audiEntity).Property(x => x.UpdatedAt).IsModified = false;
    }

    private static void UpdateBehavior(DbContext context, IAudiEntity audiEntity)
    {
        audiEntity.UpdatedAt = DateTime.UtcNow;
        context.Entry(audiEntity).Property(x => x.CreatedAt).IsModified = false;
    }
}