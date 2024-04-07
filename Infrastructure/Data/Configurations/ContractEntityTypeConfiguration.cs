using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

public class ContractEntityTypeConfiguration : IEntityTypeConfiguration<Contract>
{
    public void Configure(EntityTypeBuilder<Contract> builder)
    {
        builder.HasIndex(x => x.ContractNumber).IsUnique();
        
        builder.Property(entry => entry.BasicFundingAmount).HasPrecision(17, 2);
        builder.Property(entry => entry.AdministrativeFees).HasPrecision(17, 2);
        builder.Property(entry => entry.AdvancePayment).HasPrecision(17, 2);
        builder.Property(entry => entry.TotalFundingAmount).HasPrecision(17, 2);
        builder.Property(entry => entry.TotalInstallmentsAmount).HasPrecision(17, 2);
        builder.Property(entry => entry.InstallmentsCount).HasPrecision(17, 2);

        builder.HasOne(x => x.Funder)
            .WithMany(x => x.Contracts)
            .HasForeignKey(x => x.FunderId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.InstallmentPayments)
            .WithOne(x => x.Contract)
            .HasForeignKey(x => x.ContractId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
        
        
        builder.HasOne(c => c.User)  
            .WithMany(x => x.Contracts)
            .HasForeignKey(c => c.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}