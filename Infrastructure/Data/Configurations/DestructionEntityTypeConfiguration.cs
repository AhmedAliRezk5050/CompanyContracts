using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

public class DestructionEntityTypeConfiguration : IEntityTypeConfiguration<Destruction>
{
    public void Configure(EntityTypeBuilder<Destruction> builder)
    {
        builder.Property(d => d.Amount).HasPrecision(17, 2);


        builder.HasOne(x => x.Contract)
            .WithMany(c => c.Destructions)
            .HasForeignKey(w => w.ContractId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasOne(c => c.User)  
            .WithMany(x => x.Destructions)
            .HasForeignKey(c => c.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}