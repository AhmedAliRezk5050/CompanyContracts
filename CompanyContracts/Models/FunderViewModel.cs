using System.ComponentModel.DataAnnotations;

namespace CompanyContracts.Models;

public class FunderViewModel
{
    public int Id { get; set; }

    [Display(Name = "الاسم")]
    [Required(ErrorMessage = "اسم الجهة مطلوب")]
    public string Name { get; set; } = null!;

    [Display(Name = "العنوان")]
    [Required(ErrorMessage = "العنوان مطلوب")]
    public string Address { get; set; } = null!;

    [Display(Name = "رقم الجوال")]
    [Required(ErrorMessage = "رقم الجوال مطلوب")]
    public string PhoneNumber { get; set; } = null!;

    [Display(Name = "مسؤول التواصل")]
    [Required(ErrorMessage = "موظف التواصل مطلوب")]
    public string ContactEmployee { get; set; } = null!;
    
    [Display(Name = "رقم الجهة")]
    public string? MainNumber { get; set; }

    [Display(Name = "الرقم الفرعي")]
    public string? SubNumber { get; set; }
}