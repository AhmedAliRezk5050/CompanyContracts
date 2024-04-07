using Microsoft.AspNetCore.Identity;

namespace Core.Entities;

public class AppUser : IdentityUser
{
    public List<Contract> Contracts { get; set; } = null!;
    public List<Destruction> Destructions { get; set; } = null!;
    public List<Funder> Funders { get; set; } = null!;
    public List<InstallmentPayment> InstallmentPayments { get; set; } = null!;
}