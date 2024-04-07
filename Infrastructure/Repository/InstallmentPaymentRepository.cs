using Core.Entities;
using Core.Interfaces;
using Infrastructure.Data;

namespace Infrastructure.Repository;

public class InstallmentPaymentRepository : BaseRepository<InstallmentPayment>, IInstallmentPaymentRepository
{
    public InstallmentPaymentRepository(AppDbContext context) : base(context)
    {
    }
}