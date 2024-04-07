namespace Core.Entities;

public class InstallmentPayment : BaseEntity
{
    public double InstallmentNumber { get; set; }
    
    public decimal InstallmentAmount { get; set; }
    
    public decimal PaymentAmount { get; set; }
    
    public decimal OtherPaymentsAmount { get; set; }
    
    public decimal RemainingPaymentAmount { get; set; }
    
    public bool IsNet { get; set; }
    
    public bool IsBank { get; set; }
    
    public DateTime PaymentDate { get; set; }

    public string? BankRefNumber { get; set; }
    
    public string? TransferredBankAccountNumber { get; set; }
    public string BankStatement { get; set; } = null!;

    public string Notes { get; set; } = null!;
    
    public int ContractId { get; set; }
    public Contract Contract { get; set; } = null!;

    public bool IsPaid { get; set; }

    public bool HasForcePay { get; set; }
}