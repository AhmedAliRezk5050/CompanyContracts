using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CompanyContracts.Models;

public class ContractViewModel
{
    public int Id { get; set; }

    [Display(Name = "رقم العقد")]
    [Required(ErrorMessage = "رقم العقد مطلوب")]
    public string ContractNumber { get; set; } = null!;
    
    [Display(Name = "مبلغ التمويل")]
    [Required(ErrorMessage = "مبلغ التمويل")]
    public decimal BasicFundingAmount { get; set; }
    
    [Display(Name = "نسبة المرابحة")]
    [Required(ErrorMessage = "نسبة الفائدة مطلوبة")]
    public double? InterestRatio { get; set; }
    
    [Display(Name = "المصروفات الادارية")]
    [Required(ErrorMessage = "المصروفات الادارية مطلوبة")]
    public decimal? AdministrativeFees { get; set; }
    
    [Display(Name = "الدفعة المقدمة")]
    [Required(ErrorMessage = "الدفعة المقدمة مطلوبة")]
    public decimal? AdvancePayment { get; set; }
    
    [Display(Name = "عدد الاقساط")]
    [Required(ErrorMessage = "عدد الاقساط مطلوبة")]
    public int? InstallmentsCount { get; set; }
    
    [Display(Name = "تاريخ بداية اول قسط")]
    [Required(ErrorMessage = "تاريخ بداية اول قسط مطلوب")]
    public DateTime? FirstInstallmentBeginningDate { get; set; }
    
    [Display(Name = "تاريخ نهاية اخر قسط")]
    [Required(ErrorMessage = "تاريخ نهاية اخر قسط")]
    public DateTime? LastInstallmentDate { get; set; }
    
    
    [Display(Name = "ضريبة القيمة المضافة")]
    [Required(ErrorMessage = "ضريبة القيمة المضافة مطلوبة")]
    public double? TaxRatio { get; set; }
    
    [Display(Name = "الايضاحات")]
    [Required(ErrorMessage = "الايضاحات مطلوبة")]
    public string Notes { get; set; } = null!;

    [Display(Name = "جهة التمويل")]
    [Required(ErrorMessage = "جهة التمويل مطلوبة")]
    public int? FunderId { get; set; }

    public bool HasInstallmentPayments { get; set; }
    
    public SelectList FunderSelectList { get; set; } = null!;
    
    [Display(Name = "اجمالي الاقساط")]
    public decimal TotalInstallmentsAmount { get; set; }
    
    [Display(Name = "اجمالي العقد")]
    public decimal TotalFundingAmount { get; set; }

    [Display(Name = "المبلغ بعد الضريبة")]
    public decimal AfterTaxFundingAmount { get; set; }
    
    [Display(Name = "المبلغ بعد نسبة المرابحة")]
    public decimal AfterInterestRatioFundingAmount { get; set; }

    [Display(Name = "مبلغ الضريبة")]
    public decimal TaxAmount { get; set; }
    

    [Display(Name = "مبلغ المرابحة")]
    public decimal InterestRatioAmount { get; set; }

    [Display(Name = "قيمة القسط")]
    public decimal InstallmentAmount { get; set; }
}