namespace Core.Interfaces;

public interface IUnitOfWork
{
    public IContractRepository ContractRepository { get; set; }
    public IFunderRepository FunderRepository { get; set; }
    public IInstallmentPaymentRepository InstallmentPaymentRepository { get; set; }
    public IDestructionRepository DestructionRepository { get; set; }
    Task<bool> SaveAsync();
}