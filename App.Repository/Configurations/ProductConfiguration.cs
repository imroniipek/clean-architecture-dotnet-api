using App.Repository.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace App.Repository.Configurations;

public class ProductConfiguration:IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.HasKey(x => x.Id);

        //Kolonun özellikleri boş bırakılabilir mi max uzunlugu,null olabilir mi vs ayarlamamızı sağlar.
        builder.Property(x => x.Name).IsRequired().HasMaxLength(150);

        //Mevcut sütun üzerinde "hızlı erişim yolu" kurar.Sorgu hızı (Performance) ve Kısıtlama sağlar/
        builder.HasIndex(x => x.Name).IsUnique();
            
        builder.Property(x => x.Price).IsRequired().HasColumnType("decimal(18,2)");

        builder.Property(x => x.Count).IsRequired();
    }
}