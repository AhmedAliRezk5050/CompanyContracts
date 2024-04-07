using System.ComponentModel.DataAnnotations;

namespace CompanyContracts.Models;

public class InstallmentPaymentViewModel
{
    public int Id { get; set; }

    [Display(Name = "رقم القسط")]
    public double InstallmentNumber { get; set; }
    
    [Display(Name = "مبلغ القسط")]
    public decimal InstallmentAmount { get; set; }

    [Display(Name = "رقم العقد")] public string ContractNumber { get; set; } = null!;

    [Display(Name = "المبلغ المسدد")]
    [Required(ErrorMessage = "المبلغ المسدد مطلوب")]
    public decimal? PaymentAmount { get; set; }

    [Display(Name = "مبلغ تعويضات حادث")] public decimal OtherPaymentsAmount { get; set; }
    [Display(Name = "المبلغ المتبقي")] public decimal RemainingPaymentAmount { get; set; }

    [Display(Name = "طريقة الدفع")]
    [Required(ErrorMessage = "طريقة الدفع مطلوبة")]
    public string? PaymentMethod { get; set; }

    [Display(Name = "تاريخ السداد")]
    [Required(ErrorMessage = "تاريخ السداد مطلوب")]
    public DateTime? PaymentDate { get; set; }

    [Display(Name = "رقم المرجع البنكي")] public string? BankRefNumber { get; set; }

    [Display(Name = "رقم الحساب البنكي المحول منه")]
    public string? TransferredBankAccountNumber { get; set; }


    [Display(Name = "مستند البنك")]
    [Required(ErrorMessage = "مستند البنك مطلوب")]
    public IFormFile BankStatementFile { get; set; } = null!;

    public string? BankStatementFileName { get; set; }

    [Display(Name = "ايضاحات")] 
    [Required(ErrorMessage = "الايضاحات مطلوبة")]
    public string Notes { get; set; } = null!;

    public int ContractId { get; set; }

    public string? UserName { get; set; }
    
    public bool HasForcePay { get; set; }
}