using Core.Entities;
using Core.Interfaces;
using Infrastructure.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CompanyContracts.Models;
using Serilog;

namespace CompanyContracts.Controllers;

[Authorize]
public class DestructionsController : Controller
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly UserManager<AppUser> _userManager;

    public DestructionsController(IUnitOfWork unitOfWork, UserManager<AppUser> userManager)
    {
        _unitOfWork = unitOfWork;
        _userManager = userManager;
    }

    [HttpGet]
    [Authorize(Permissions.Destructions.View)]
    public async Task<IActionResult> Index()
    {
        try
        {
            var destructions = await _unitOfWork
                .DestructionRepository
                .GetAllAsync(include: x =>
                    x.Include(a => a.Contract)
                        .Include(d => d.User)
                    );

            var destructionViewModelList = destructions
                .Select(d => new DestructionViewModel
                {
                    ContractNumber = d.Contract.ContractNumber,
                    Amount = d.Amount,
                    Date = d.Date,
                    UserName = d.User?.UserName ?? "--"
                }).ToList();

            return View(destructionViewModelList);
        }
        catch (Exception e)
        {
            TempData["home-temp-msg"] = "unknown-error";
            Log.Error("DestructionsController - Index[GET] - {@message}", e.Message);
            return RedirectToAction("Index", "Home");
        }
    }

    [HttpGet]
    [Authorize(Permissions.Destructions.Add)]
    public async Task<IActionResult> Create(int contractId)
    {
        try
        {
            var contract = await _unitOfWork
                .ContractRepository
                .GetFirstOrDefaultAsync(x => x.Id == contractId);

            if (contract is null)
            {
                return NotFound();
            }

            var destructionViewModel = new DestructionViewModel
            {
                ContractNumber = contract.ContractNumber,
                ContractId = contract.Id
            };

            return View(destructionViewModel);
        }
        catch (Exception e)
        {
            TempData["destructions-index-temp-msg"] = "unknown-error";
            Log.Error("DestructionsController - Create[GET] - {@message}", e.Message);
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpPost]
    [Authorize(Permissions.Destructions.Add)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(DestructionViewModel model)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return View(new DestructionViewModel
                {
                    ContractNumber = (await GetContract(model.ContractId.Value)).ContractNumber
                });
            }

            var contract = await GetContract(model.ContractId.Value);

            if (contract is null)
            {
                return NotFound();
            }

            if (contract.IsPaid)
            {
                TempData["contracts-index-temp-msg"] = "pay-done";
                return RedirectToAction("Index", "Contracts");
            }

            var contractHasInstallmentPayments = contract.InstallmentPayments.Count;
            
            var constLastInstallmentPaymentIsFullyPaid = false;

            if (contractHasInstallmentPayments > 0)
            {
                constLastInstallmentPaymentIsFullyPaid = contract.InstallmentPayments.Last().IsPaid;
            }
            
            if (contractHasInstallmentPayments == 0 || !constLastInstallmentPaymentIsFullyPaid)
            {
                TempData["destructions-index-temp-msg"] = "unknown-error";
                return RedirectToAction(nameof(Index));
            }

            var destruction = new Destruction
            {
                Amount = model.Amount.Value,
                Date = model.Date.Value,
                ContractId = model.ContractId.Value,
                User = await GetAuthenticatedUser()
            };

            _unitOfWork.DestructionRepository.Add(destruction);

            var isSuccessSave = await _unitOfWork.SaveAsync();

            if (isSuccessSave)
            {
                TempData["destructions-index-temp-msg"] = "create-success";
                return RedirectToAction(nameof(Index));
            }

            TempData["destructions-index-temp-msg"] = "create-error";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception e)
        {
            TempData["destructions-index-temp-msg"] = "unknown-error";
            Log.Error("DestructionsController - Create[POST] - {@message}", e.Message);
            return RedirectToAction(nameof(Index));
        }
    }

    private Task<Contract?> GetContract(int id)
    {
        return _unitOfWork
            .ContractRepository
            .GetFirstOrDefaultAsync(x => x.Id == id,
                include: x => 
                    x.Include(e => e.InstallmentPayments));
    }
    
    private Task<AppUser> GetAuthenticatedUser()
    {
        return _userManager.GetUserAsync(User);
    }
}