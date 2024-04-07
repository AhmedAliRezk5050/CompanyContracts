using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

public class FunderEntityTypeConfiguration : IEntityTypeConfiguration<Funder>
{
    public void Configure(EntityTypeBuilder<Funder> builder)
    {
        builder.HasIndex(a => new
        {
            a.Name,
            a.MainNumber,
            a.SubNumber
        }).IsUnique();
        builder.HasIndex(a => a.PhoneNumber).IsUnique();
        
        builder.HasOne(c => c.User)  
            .WithMany(x => x.Funders)
            .HasForeignKey(c => c.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}