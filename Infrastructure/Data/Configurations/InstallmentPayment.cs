using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

public class InstallmentPaymentEntityTypeConfiguration : IEntityTypeConfiguration<InstallmentPayment>
{
    public void Configure(EntityTypeBuilder<InstallmentPayment> builder)
    {
        builder.Property(entry => entry.InstallmentAmount).HasPrecision(17, 2);
        builder.Property(entry => entry.PaymentAmount).HasPrecision(17, 2);
        builder.Property(entry => entry.OtherPaymentsAmount).HasPrecision(17, 2);
        builder.Property(entry => entry.RemainingPaymentAmount).HasPrecision(17, 2);
        
        builder.HasOne(c => c.User)  
            .WithMany(x => x.InstallmentPayments)
            .HasForeignKey(c => c.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}