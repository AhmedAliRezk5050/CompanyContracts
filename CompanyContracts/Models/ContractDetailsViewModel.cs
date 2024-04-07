using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CompanyContracts.Models;

public class ContractDetailsViewModel
{
    public int Id { get; set; }
    
    [Display(Name = "رقم العقد")]
    public string ContractNumber { get; set; } = null!;
    
    [Display(Name = "مبلغ التمويل")]
    public decimal BasicFundingAmount { get; set; }
    
    [Display(Name = "نسبة المرابحة")]
    public double InterestRatio { get; set; }
    
    [Display(Name = "المصروفات الادارية")]
    public decimal AdministrativeFees { get; set; }
    
    [Display(Name = "الدفعة المقدمة")]
    public decimal AdvancePayment { get; set; }
    
    [Display(Name = "عدد الاقساط")]
    public double InstallmentsCount { get; set; }
    
    [Display(Name = "تاريخ بداية اول قسط")]
    public DateTime FirstInstallmentBeginningDate { get; set; }
    
    [Display(Name = "تاريخ نهاية اخر قسط")]
    public DateTime LastInstallmentDate { get; set; }

    [Display(Name = "ضريبة القيمة المضافة")]
    public double TaxRatio { get; set; }
    
    [Display(Name = "البيان")]
    public string Notes { get; set; } = null!;

    [Display(Name = "جهة التمويل")]
    public int FunderId { get; set; }

    [Display(Name = "جهة التمويل")]
    // [Display(Name = "اسم الشركة")]
    public string FunderName { get; set; } = null!;

    [Display(Name = "رقم الجهة")]
    public string? FunderMainNumber { get; set; }
    
    [Display(Name = "الرقم الفرعي")]
    public string? FunderSubNumber { get; set; }

    [Display(Name = "اجمالي مبلغ التمويل")]
    public decimal FundingNetAmount { get; set; }
    
    [Display(Name = "مجموع مبلغ الاقساط")]
    public decimal TotalInstallmentsAmount { get; set; }
    
    [Display(Name = "اجمالي قيمة العقد")]
    public decimal TotalFundingAmount { get; set; }
    
    [Display(Name = "قيمة القسط")]
    public decimal InstallmentAmount { get; set; }
    
    public SelectList FunderSelectList { get; set; } = null!;

    [Display(Name = "عدد الاقساط المسددة")]
    public double PaidInstallmentsCount { get; set; }
    
    [Display(Name = "عدد الاقساط الغير المسددة")]
    public double RemainingInstallmentsCount { get; set; }

    [Display(Name = "حالة العقد")] public string IsPaid { get; set; } = null!;

    [Display(Name = "اجمالي المسدد")]
    public decimal PaidInstallments { get; set; }
    
    [Display(Name = "اجمالي الغير مسدد")]
    public decimal NonPaidInstallments { get; set; }

    public string UserName { get; set; } = null!;
}