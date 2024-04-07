using Core.Interfaces;
using Infrastructure.Data;

namespace Infrastructure.Repository;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _dbContext;

    public IContractRepository ContractRepository { get; set; }
    public IFunderRepository FunderRepository { get; set; }
    
    public IInstallmentPaymentRepository InstallmentPaymentRepository { get; set; } = null!;
    public IDestructionRepository DestructionRepository { get; set; } = null!;

    public UnitOfWork(AppDbContext dbContext)
    {
        _dbContext = dbContext;
        ContractRepository = new ContractRepository(dbContext);
        FunderRepository = new FunderRepository(dbContext);
        InstallmentPaymentRepository = new InstallmentPaymentRepository(dbContext);
        DestructionRepository = new DestructionRepository(dbContext);
    }

    public async Task<bool> SaveAsync()
    {
        return await _dbContext.SaveChangesAsync() > 0;
    }
}