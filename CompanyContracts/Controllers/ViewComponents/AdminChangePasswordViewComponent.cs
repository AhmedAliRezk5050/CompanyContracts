using Microsoft.AspNetCore.Mvc;
using CompanyContracts.Models;

namespace CompanyContracts.Controllers.ViewComponents;

public class AdminChangePasswordViewComponent : ViewComponent
{
    public IViewComponentResult Invoke(AdminChangePasswordViewModel model)
    {
        return View(model);
    }
}