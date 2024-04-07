using System.ComponentModel.DataAnnotations;

namespace CompanyContracts.Models;

public class DestructionViewModel
{
    public int Id { get; set; }

    [Display(Name = "القيمة")]
    [Required(ErrorMessage = "القيمة مطلوبة")]
    public decimal? Amount { get; set; }
    
    [Display(Name = "التاريخ")]
    [Required(ErrorMessage = "التاريخ مطلوب")]
    public DateTime? Date { get; set; }
    
    [Display(Name = "رقم العقد")]
    public string? ContractNumber { get; set; }
    
    public int? ContractId { get; set; }

    public string? UserName { get; set; }
}