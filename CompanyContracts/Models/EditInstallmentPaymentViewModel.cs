using System.ComponentModel.DataAnnotations;

namespace CompanyContracts.Models;

public class EditInstallmentPaymentViewModel
{
    public int Id { get; set; }
    [Display(Name = "تاريخ السداد")]
    [Required(ErrorMessage = "تاريخ السداد مطلوب")]
    public DateTime? PaymentDate { get; set; }
    
    [Display(Name = "رقم المرجع البنكي")]
    public string? BankRefNumber { get; set; }
    
    [Display(Name = "رقم الحساب البنكي المحول منه")]
    public string? TransferredBankAccountNumber { get; set; }
    
    [Display(Name = "ايضاحات")] 
    [Required(ErrorMessage = "الايضاحات مطلوبة")]
    public string Notes { get; set; } = null!;
}