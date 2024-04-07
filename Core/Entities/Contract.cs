namespace Core.Entities;

public class Contract : BaseEntity
{
    public string ContractNumber { get; set; } = null!;
    public decimal BasicFundingAmount { get; set; }
    public double InterestRatio { get; set; }
    public decimal AdministrativeFees { get; set; }
    public decimal AdvancePayment { get; set; }
    public int InstallmentsCount { get; set; }

    public decimal TotalFundingAmount { get; set; }
    public decimal TotalInstallmentsAmount { get; set; }

    public DateTime FirstInstallmentBeginningDate { get; set; }

    public DateTime LastInstallmentDate { get; set; }

    public string Notes { get; set; } = null!;

    public bool IsPaid { get; set; }

    public int FunderId { get; set; }
    public Funder Funder { get; set; } = null!;

    public double TaxRatio { get; set; }
    
    public List<InstallmentPayment> InstallmentPayments { get; set; } = null!;
    
    public List<Destruction> Destructions { get; set; } = null!;
}