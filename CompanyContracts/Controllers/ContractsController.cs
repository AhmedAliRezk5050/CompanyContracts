using System.Data;
using System.Linq.Expressions;
using AspNetCore.Reporting;
using Core.Entities;
using Core.Interfaces;
using Infrastructure.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CompanyContracts.Models;
using CompanyContracts.Utility;
using Serilog;

namespace CompanyContracts.Controllers;

[Authorize]
public class ContractsController : Controller
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IWebHostEnvironment _webHostEnvironment;
    private readonly UserManager<AppUser> _userManager;

    public ContractsController(IUnitOfWork unitOfWork,
        IWebHostEnvironment webHostEnvironment, UserManager<AppUser> userManager)
    {
        _unitOfWork = unitOfWork;
        _webHostEnvironment = webHostEnvironment;
        _userManager = userManager;
    }

    [HttpGet]
    [Authorize(Permissions.Contracts.View)]
    public async Task<IActionResult> Index()
    {
        try
        {
            var contractDetailsViewModelList = (await _unitOfWork.ContractRepository.GetAllAsync(
                include: x => x.Include(
                        a => a.InstallmentPayments
                    ).Include(a => a.Funder)
                    .Include(e => e.Destructions)
                    .Include(w => w.User)
            )).Select(x => new ContractDetailsViewModel()
            {
                Id = x.Id,
                ContractNumber = x.ContractNumber,
                Notes = x.Notes,
                FunderName = x.Funder.Name,
                InstallmentAmount = CalculateInstallmentAmount(x),
                InstallmentsCount = x.InstallmentsCount,
                FirstInstallmentBeginningDate = x.FirstInstallmentBeginningDate,
                TotalFundingAmount = Math.Round(x.TotalFundingAmount, 2),
                TotalInstallmentsAmount = Math.Round(x.TotalInstallmentsAmount, 2),
                AdvancePayment = x.AdvancePayment,
                PaidInstallmentsCount = GetPaidInstallmentsCount(x),
                PaidInstallments = GetPaidInstallments(x),
                NonPaidInstallments = GetNonPaidInstallments(x),
                RemainingInstallmentsCount = GetRemainingInstallmentsCount(x),
                UserName = x.User?.UserName ?? "--"
            }).OrderBy(x => x.RemainingInstallmentsCount).ToList();


            var funders = await _unitOfWork.FunderRepository.GetAllAsync();
            ViewBag.ContractSelectList = new SelectList(contractDetailsViewModelList, "Id", "ContractNumber");
            ViewBag.FunderSelectList = new SelectList(funders, "Id", "Name");

            return View(contractDetailsViewModelList);
        }
        catch (Exception e)
        {
            TempData["home-temp-msg"] = "unknown-error";
            Log.Error("ContractsController - Index - {@message}", e.Message);
            return RedirectToAction("Index", "Home");
        }
    }


    [HttpGet]
    [Authorize(Permissions.Contracts.Add)]
    public async Task<IActionResult> Create()
    {
        try
        {
            var contractModel = new ContractViewModel();

            contractModel.FunderSelectList = await GetFunderSelectList();

            return View(contractModel);
        }
        catch (Exception e)
        {
            TempData["contracts-index-temp-msg"] = "create-error";
            Log.Error("ContractsController - Create[GET] - {@message}", e.Message);
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpPost]
    [Authorize(Permissions.Contracts.Add)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ContractViewModel model)
    {
        try
        {
            ModelState.Remove(nameof(model.FunderSelectList));

            if (!ModelState.IsValid)
            {
                model.FunderSelectList = await GetFunderSelectList();
                return View(model);
            }

            var contract = new Contract();

            contract.ContractNumber = model.ContractNumber;
            contract.BasicFundingAmount = model.BasicFundingAmount;
            contract.InterestRatio = model.InterestRatio.Value;
            contract.TaxRatio = model.TaxRatio.Value; //
            contract.AdministrativeFees = model.AdministrativeFees.Value;
            contract.AdvancePayment = model.AdvancePayment.Value; //
            contract.InstallmentsCount = model.InstallmentsCount.Value;
            contract.FirstInstallmentBeginningDate = model.FirstInstallmentBeginningDate.Value;
            contract.LastInstallmentDate = model.LastInstallmentDate.Value;
            contract.Notes = model.Notes;
            contract.FunderId = model.FunderId.Value;
            contract.TotalFundingAmount = CalculateContractTotalFundingAmount(contract);
            contract.TotalInstallmentsAmount = CalculateTotalInstallmentsAmount(contract);
            contract.User = await GetAuthenticatedUser();

            _unitOfWork.ContractRepository.Add(contract);

            var isSuccessSave = await _unitOfWork.SaveAsync();


            if (isSuccessSave)
            {
                TempData["contracts-index-temp-msg"] = "create-success";
                return RedirectToAction(nameof(Index));
            }

            model.FunderSelectList = await GetFunderSelectList();

            TempData["contracts-create-temp-msg"] = "create-error";

            return View(model);
        }
        catch (Exception e)
        {
            TempData["contracts-index-temp-msg"] = "unknown-error";
            Log.Error("ContractsController - Create[POST] - {@message}", e.Message);
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpGet]
    [Authorize(Permissions.Contracts.Edit)]
    public async Task<IActionResult> Edit(int id)
    {
        try
        {
            var contract = await _unitOfWork
                .ContractRepository
                .GetFirstOrDefaultAsync(x => x.Id == id,
                    x => x.Include(a => a.InstallmentPayments));

            if (contract is null)
            {
                return NotFound();
            }


            var model = new ContractViewModel();

            model.Id = contract.Id;
            model.ContractNumber = contract.ContractNumber;
            model.FunderId = contract.FunderId;
            model.FunderSelectList = await GetFunderSelectList();
            model.Notes = contract.Notes;
            model.HasInstallmentPayments = HasInstallmentPayments(contract);

            if (!HasInstallmentPayments(contract))
            {
                model.BasicFundingAmount = contract.BasicFundingAmount;
                model.InterestRatio = contract.InterestRatio;
                model.TaxRatio = contract.TaxRatio;
                model.AdministrativeFees = contract.AdministrativeFees;
                model.AdvancePayment = contract.AdvancePayment;
                model.InstallmentsCount = contract.InstallmentsCount;
                model.FirstInstallmentBeginningDate = contract.FirstInstallmentBeginningDate;
                model.LastInstallmentDate = contract.LastInstallmentDate;
            }


            return View(model);
        }
        catch (Exception e)
        {
            TempData["contracts-index-temp-msg"] = "unknown-error";
            Log.Error("ContractsController - Edit[GET] - {@message}", e.Message);
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpPost]
    [Authorize(Permissions.Contracts.Edit)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(ContractViewModel model)
    {
        try
        {
            if (ModelState.IsValid)
            {
                model.FunderSelectList = await GetFunderSelectList();
                return View(model);
            }

            var contract = await _unitOfWork
                .ContractRepository
                .GetFirstOrDefaultAsync(x => x.Id == model.Id,
                    x =>
                        x.Include(a => a.InstallmentPayments));

            if (contract is null)
            {
                TempData["contracts-index-temp-msg"] = "edit-error";
                return RedirectToAction(nameof(Index));
            }


            contract.ContractNumber = model.ContractNumber;
            contract.Notes = model.Notes;
            contract.FunderId = model.FunderId.Value;
            contract.User = await GetAuthenticatedUser();

            if (!contract.InstallmentPayments.Any())
            {
                contract.BasicFundingAmount = model.BasicFundingAmount;
                contract.InterestRatio = model.InterestRatio.Value;
                contract.TaxRatio = model.TaxRatio.Value;
                contract.AdministrativeFees = model.AdministrativeFees.Value;
                contract.AdvancePayment = model.AdvancePayment.Value;
                contract.InstallmentsCount = model.InstallmentsCount.Value;
                contract.FirstInstallmentBeginningDate = model.FirstInstallmentBeginningDate.Value;
                contract.LastInstallmentDate = model.LastInstallmentDate.Value;
                contract.TotalFundingAmount = CalculateContractTotalFundingAmount(contract);
                contract.TotalInstallmentsAmount = CalculateTotalInstallmentsAmount(contract);
            }

            _unitOfWork.ContractRepository.Update(contract);

            var isSuccessSave = await _unitOfWork.SaveAsync();

            if (isSuccessSave)
            {
                TempData["contracts-index-temp-msg"] = "edit-success";
                return RedirectToAction(nameof(Index));
            }

            TempData["contracts-index-temp-msg"] = "edit-error";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception e)
        {
            TempData["contracts-index-temp-msg"] = "unknown-error";
            Log.Error("ContractsController - Edit[POST] - {@message}", e.Message);
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpGet]
    [Authorize(Permissions.InstallmentPayments.View)]
    public async Task<IActionResult> PayInstallment(int id)
    {
        try
        {
            var contract = await _unitOfWork
                .ContractRepository
                .GetFirstOrDefaultAsync(x => x.Id == id,
                    x =>
                        x.Include(a => a.InstallmentPayments)
                            .Include(t => t.Destructions)
                );

            if (contract is null)
            {
                return NotFound();
            }

            if (contract.IsPaid)
            {
                TempData["contracts-index-temp-msg"] = "pay-done";
                return RedirectToAction(nameof(Index));
            }

            var contractHasPrevInstallmentPayments = HasInstallmentPayments(contract);

            var isFirstInstallmentPayment = !HasInstallmentPayments(contract);

            var isPaidLastInstallmentPayment = contractHasPrevInstallmentPayments &&
                                               IsPaidLastInstallmentPayment(contract);

            var lastPaidInstallmentPaymentIndex = GetLastPaidInstallmentPaymentIndex(contract);

            var contractInstallmentPaymentsAfterLastPaidInstallment =
                contract.InstallmentPayments.Select(x => x).ToList();
            contractInstallmentPaymentsAfterLastPaidInstallment.RemoveRange(0, lastPaidInstallmentPaymentIndex + 1);

            var totalPrepaidInstallmentPayments = isPaidLastInstallmentPayment
                ? 0
                : contractInstallmentPaymentsAfterLastPaidInstallment
                    .Sum(x => x.PaymentAmount + x.OtherPaymentsAmount);

            var requiredPayment = isFirstInstallmentPayment || isPaidLastInstallmentPayment
                ? 0
                : CalculateInstallmentAmount(contract) -
                  totalPrepaidInstallmentPayments;

            var installmentNumber = isPaidLastInstallmentPayment
                ? contract.InstallmentPayments.Where(x => x.IsPaid).Count() + 1
                : Math.Ceiling(contract.InstallmentPayments.LastOrDefault()?.InstallmentNumber ?? 1);

            var model = new InstallmentPaymentViewModel
            {
                ContractId = contract.Id,
                ContractNumber = contract.ContractNumber,
                InstallmentAmount = CalculateInstallmentAmount(contract),
                InstallmentNumber = installmentNumber,
                RemainingPaymentAmount = requiredPayment
            };


            return View(model);
        }
        catch (Exception e)
        {
            Log.Error("ContractsController - PayInstallment[GET] - {@message}", e.Message);
            TempData["contracts-index-temp-msg"] = "unknown-error";
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpPost]
    [Authorize(Permissions.InstallmentPayments.Add)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> PayInstallment(InstallmentPaymentViewModel model)
    {
        try
        {
            ModelState.Remove(nameof(model.ContractNumber));
            ModelState.Remove(nameof(model.InstallmentNumber));
            ModelState.Remove(nameof(model.InstallmentAmount));
            ModelState.Remove(nameof(model.RemainingPaymentAmount));

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var contract = await _unitOfWork
                .ContractRepository
                .GetFirstOrDefaultAsync(x => x.Id == model.ContractId,
                    x => x.Include(a => a.InstallmentPayments)
                        .Include(e => e.Destructions));


            if (!model.HasForcePay && (model.PaymentAmount + model.OtherPaymentsAmount > model.InstallmentAmount))
            {
                TempData["contracts-index-temp-msg"] = "pay-unbalanced";
                return RedirectToAction(nameof(Index));
            }

            var isFirstInstallment = contract.InstallmentPayments.Count == 0;

            var isPendingPay = IsPendingPay(model);

            var isCompletePay = IsCompletePay(model);

            var isCompletePendingPay = IsCompletePendingPay(model, isPendingPay);

            double nextInstallmentPaymentNumber = 0;

            if (isPendingPay && !model.HasForcePay)
            {
                if (isCompletePendingPay)
                {
                    nextInstallmentPaymentNumber = contract.InstallmentPayments.Count(x => x.IsPaid) + 1;
                }
                else
                {
                    var lastInstallmentNumber = contract.InstallmentPayments.LastOrDefault()?.InstallmentNumber ?? 0;

                    nextInstallmentPaymentNumber = Math.Round(lastInstallmentNumber +
                                                              (
                                                                  ((double)model.PaymentAmount.Value +
                                                                   (double)model.OtherPaymentsAmount)) /
                                                              (double)model.InstallmentAmount, 2);
                }
            }
            else
            {
                nextInstallmentPaymentNumber = contract.InstallmentPayments.Where(x => x.IsPaid).Count() + 1;
            }


            if (nextInstallmentPaymentNumber == contract.InstallmentsCount)
            {
                contract.IsPaid = true;
            }


            var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "Uploads");
            var uniqueFileName = Guid.NewGuid() + "_" + model.BankStatementFile.FileName;
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            await using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await model.BankStatementFile.CopyToAsync(fileStream);
            }


            var totalPaidAmount = model.PaymentAmount.Value + model.OtherPaymentsAmount;
            var remainingPaymentAmount = model.RemainingPaymentAmount;
            var installmentAmount = model.InstallmentAmount;
            decimal updatedRemainingPaymentAmount = 0;

            if (remainingPaymentAmount == 0)
            {
                updatedRemainingPaymentAmount = installmentAmount - totalPaidAmount;
            }
            else
            {
                updatedRemainingPaymentAmount = remainingPaymentAmount - totalPaidAmount;
            }

            var installmentPayment = new InstallmentPayment();

            if (model.HasForcePay)
            {
                installmentPayment.InstallmentAmount = model.PaymentAmount.Value + model.OtherPaymentsAmount;
                installmentPayment.RemainingPaymentAmount = 0;
                installmentPayment.IsPaid = true;
                installmentPayment.HasForcePay = true;
            }
            else
            {
                installmentPayment.InstallmentAmount = model.InstallmentAmount;
                installmentPayment.RemainingPaymentAmount = updatedRemainingPaymentAmount;
                installmentPayment.IsPaid = isCompletePay || isCompletePendingPay;
            }

            installmentPayment.InstallmentNumber = nextInstallmentPaymentNumber;
            installmentPayment.PaymentAmount = model.PaymentAmount.Value;
            installmentPayment.OtherPaymentsAmount = model.OtherPaymentsAmount;
            installmentPayment.IsNet = model.PaymentMethod == "net";
            installmentPayment.IsBank = model.PaymentMethod == "bank";
            installmentPayment.PaymentDate = model.PaymentDate.Value;
            installmentPayment.BankRefNumber = model.BankRefNumber;
            installmentPayment.TransferredBankAccountNumber = model.TransferredBankAccountNumber;
            installmentPayment.BankStatement = uniqueFileName;
            installmentPayment.ContractId = model.ContractId;
            installmentPayment.Notes = model.Notes;
            installmentPayment.User = await GetAuthenticatedUser();

            //  installmentPayment = new InstallmentPayment
            // {
            //     InstallmentNumber = nextInstallmentPaymentNumber,
            //     InstallmentAmount = model.InstallmentAmount,
            //     PaymentAmount = model.PaymentAmount.Value,
            //     OtherPaymentsAmount = model.OtherPaymentsAmount,
            //     IsNet = model.PaymentMethod == "net",
            //     IsBank = model.PaymentMethod == "bank",
            //     PaymentDate = model.PaymentDate.Value,
            //     BankRefNumber = model.BankRefNumber,
            //     TransferredBankAccountNumber = model.TransferredBankAccountNumber,
            //     BankStatement = uniqueFileName,
            //     ContractId = model.ContractId,
            //     Notes = model.Notes,
            //     RemainingPaymentAmount = updatedRemainingPaymentAmount,
            //     IsPaid = isCompletePay || isCompletePendingPay,
            //     User = await GetAuthenticatedUser()
            // };

            _unitOfWork.InstallmentPaymentRepository.Add(installmentPayment);

            var isSuccessSave = await _unitOfWork.SaveAsync();

            if (isSuccessSave)
            {
                return RedirectToAction(nameof(Index));
            }

            TempData["contracts-index-temp-msg"] = "pay-error";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception e)
        {
            TempData["contracts-index-temp-msg"] = "unknown-error";
            Log.Error("ContractsController - PayInstallment[POST] - {@message}", e.Message);
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpGet]
    [Authorize(Permissions.Contracts.View)]
    public async Task<IActionResult> Details(int id)
    {
        try
        {
            var contract = await _unitOfWork
                .ContractRepository
                .GetFirstOrDefaultAsync(x => x.Id == id,
                    x =>
                        x.Include(a => a.InstallmentPayments)
                            .Include(a => a.Funder)
                            .Include(q => q.Destructions)
                );

            if (contract is null)
            {
                return NotFound();
            }

            var paidInstallmentsCount = contract.InstallmentPayments.LastOrDefault()?.InstallmentNumber ?? 0;

            var model = new ContractDetailsViewModel()
            {
                ContractNumber = contract.ContractNumber,
                BasicFundingAmount = contract.BasicFundingAmount,
                InterestRatio = contract.InterestRatio,
                TaxRatio = contract.TaxRatio,
                AdministrativeFees = contract.AdministrativeFees,
                AdvancePayment = contract.AdvancePayment,
                InstallmentsCount = contract.InstallmentsCount,
                FirstInstallmentBeginningDate = contract.FirstInstallmentBeginningDate,
                LastInstallmentDate = contract.LastInstallmentDate,
                Notes = contract.Notes,
                FunderId = contract.FunderId,
                FundingNetAmount = CalculateContractTotalFundingAmount(contract),
                TotalInstallmentsAmount = CalculateTotalInstallmentsAmount(contract),
                InstallmentAmount = CalculateInstallmentAmount(contract),
                FunderSelectList = await GetFunderSelectList(),
                PaidInstallmentsCount = paidInstallmentsCount,
                RemainingInstallmentsCount = GetRemainingInstallmentsCount(contract),
                IsPaid = GetPaymentStatusInWords(contract),
                PaidInstallments = GetPaidInstallments(contract),
                NonPaidInstallments = GetNonPaidInstallments(contract),
                FunderMainNumber = contract.Funder.MainNumber ?? "--",
                FunderSubNumber = contract.Funder.SubNumber ?? "--",
            };

            return View(model);
        }
        catch (Exception e)
        {
            TempData["contracts-index-temp-msg"] = "unknown-error";
            Log.Error("ContractsController - Details[GET] - {@message}", e.Message);
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpGet]
    [Authorize(Permissions.Contracts.View)]
    public async Task<IActionResult> InstallmentPayments(int contractId)
    {
        try
        {
            var contract = await _unitOfWork
                .ContractRepository
                .GetFirstOrDefaultAsync(
                    x => x.Id == contractId,
                    x =>
                        x.Include(a => a.InstallmentPayments)
                            .ThenInclude(q => q.User)
                );

            if (contract is null || !HasInstallmentPayments(contract))
            {
                return NotFound();
            }

            var installmentPaymentViewModelList =
                contract.InstallmentPayments.Select(i => new InstallmentPaymentViewModel()
                {
                    Id = i.Id,
                    InstallmentNumber = i.InstallmentNumber,
                    InstallmentAmount = i.InstallmentAmount,
                    ContractNumber = i.Contract.ContractNumber,
                    PaymentAmount = i.PaymentAmount,
                    OtherPaymentsAmount = i.OtherPaymentsAmount,
                    PaymentMethod = GetPaymentMethodInWords(i),
                    PaymentDate = i.PaymentDate,
                    Notes = i.Notes,
                    RemainingPaymentAmount = i.RemainingPaymentAmount,
                    BankRefNumber = i.BankRefNumber,
                    TransferredBankAccountNumber = i.TransferredBankAccountNumber,
                    UserName = i.User?.UserName ?? "--"
                });

            ViewBag.ContractNumber = installmentPaymentViewModelList.First().ContractNumber;

            return View(installmentPaymentViewModelList);
        }
        catch (Exception e)
        {
            TempData["contracts-index-temp-msg"] = "unknown-error";
            Log.Error("ContractsController - InstallmentPayments[GET] - {@message}", e.Message);
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpGet]
    [Authorize(Permissions.Contracts.View)]
    public async Task<IActionResult> InstallmentPayment(int id)
    {
        try
        {
            var installmentPayment = await _unitOfWork
                .InstallmentPaymentRepository
                .GetFirstOrDefaultAsync(x => x.Id == id,
                    x => x.Include(a => a.Contract)
                );

            if (installmentPayment is null)
            {
                return NotFound();
            }

            var installmentPaymentViewModel = new InstallmentPaymentViewModel()
            {
                Id = installmentPayment.Id,
                InstallmentNumber = installmentPayment.InstallmentNumber,
                InstallmentAmount = installmentPayment.InstallmentAmount,
                ContractNumber = installmentPayment.Contract.ContractNumber,
                PaymentAmount = installmentPayment.PaymentAmount,
                OtherPaymentsAmount = installmentPayment.OtherPaymentsAmount,
                RemainingPaymentAmount = installmentPayment.RemainingPaymentAmount,
                PaymentMethod = GetPaymentMethodInWords(installmentPayment),
                PaymentDate = installmentPayment.PaymentDate,
                BankRefNumber = installmentPayment.BankRefNumber,
                TransferredBankAccountNumber = installmentPayment.TransferredBankAccountNumber,
                Notes = installmentPayment.Notes,
                BankStatementFileName = installmentPayment.BankStatement
            };

            return View(installmentPaymentViewModel);
        }
        catch (Exception e)
        {
            TempData["contracts-index-temp-msg"] = "unknown-error";
            Log.Error("ContractsController - InstallmentPayment[GET] - {@message}", e.Message);
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpGet]
    [Authorize(Permissions.InstallmentPayments.Edit)]
    public async Task<IActionResult> EditInstallmentPayment(int id)
    {
        try
        {
            var installmentPayment = await _unitOfWork
                .InstallmentPaymentRepository
                .GetFirstOrDefaultAsync(x => x.Id == id);

            if (installmentPayment is null)
            {
                return NotFound();
            }

            var editInstallmentPaymentViewModel = new EditInstallmentPaymentViewModel
            {
                Id = installmentPayment.Id,
                PaymentDate = installmentPayment.PaymentDate,
                BankRefNumber = installmentPayment.BankRefNumber,
                TransferredBankAccountNumber = installmentPayment.TransferredBankAccountNumber,
                Notes = installmentPayment.Notes,
            };

            return View(editInstallmentPaymentViewModel);
        }
        catch (Exception e)
        {
            TempData["contracts-index-temp-msg"] = "unknown-error";
            Log.Error("ContractsController - EditInstallmentPayment[GET] - {@message}", e.Message);
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpPost]
    [Authorize(Permissions.InstallmentPayments.Edit)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditInstallmentPayment(EditInstallmentPaymentViewModel model)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var installmentPayment = await _unitOfWork
                .InstallmentPaymentRepository
                .GetFirstOrDefaultAsync(x => x.Id == model.Id);

            if (installmentPayment is null)
            {
                return NotFound();
            }

            installmentPayment.PaymentDate = model.PaymentDate.Value;
            installmentPayment.BankRefNumber = model.BankRefNumber;
            installmentPayment.TransferredBankAccountNumber = model.TransferredBankAccountNumber;
            installmentPayment.Notes = model.Notes;
            installmentPayment.User = await GetAuthenticatedUser();


            _unitOfWork.InstallmentPaymentRepository.Update(installmentPayment);

            var isSuccessSave = await _unitOfWork.SaveAsync();

            if (isSuccessSave)
            {
                return RedirectToAction(nameof(Index));
            }


            TempData["contracts-index-temp-msg"] = "unknown-error";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception e)
        {
            TempData["contracts-index-temp-msg"] = "unknown-error";
            Log.Error("ContractsController - EditInstallmentPayment[POST] - {@message}", e.Message);
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpGet]
    [Authorize(Permissions.Contracts.Delete)]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var contract = await _unitOfWork
                .ContractRepository
                .GetFirstOrDefaultAsync(x => x.Id == id);

            if (contract is null)
            {
                return NotFound();
            }

            _unitOfWork.ContractRepository.Remove(contract);

            var isSuccessSave = await _unitOfWork.SaveAsync();

            if (isSuccessSave)
            {
                TempData["contracts-index-temp-msg"] = "delete-success";
                return RedirectToAction(nameof(Index));
            }

            TempData["contracts-index-temp-msg"] = "delete-error";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception e)
        {
            TempData["contracts-index-temp-msg"] = "delete-error";
            Log.Error("ContractsController - Delete[GET] - {@message}", e.Message);
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpGet]
    [Authorize(Permissions.Contracts.View)]
    public IActionResult DownloadBankStatementFile(string fileName)
    {
        try
        {
            if (string.IsNullOrEmpty(fileName))
            {
                return NotFound();
            }

            var filePath = Path.Combine(_webHostEnvironment.WebRootPath, "Uploads", fileName);

            if (!System.IO.File.Exists(filePath))
            {
                return NotFound();
            }

            var fileStream = System.IO.File.OpenRead(filePath);
            return File(fileStream, "application/octet-stream", fileName);
        }
        catch (Exception e)
        {
            TempData["contracts-index-temp-msg"] = "unknown-error";
            Log.Error("ContractsController - DownloadBankStatementFile[GET] - {@message}", e.Message);
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpGet]
    [Authorize(Permissions.Contracts.View)]
    public async Task<ActionResult> GetContractsPdf(int? funderId, DateTime? fromDate, DateTime? toDate, bool isPaid,
        bool getAll)
    {
        try
        {
            var contractDetailsViewModelList =
                await GetContractDetailsViewModelList(funderId, fromDate, toDate, isPaid, getAll);

            var contractsDataTable = new DataTable();

            contractsDataTable.Columns.Add("count");
            contractsDataTable.Columns.Add("ContractNumber");
            contractsDataTable.Columns.Add("Notes");
            contractsDataTable.Columns.Add("FunderName");
            contractsDataTable.Columns.Add("InstallmentAmount");
            contractsDataTable.Columns.Add("InstallmentsCount");
            contractsDataTable.Columns.Add("FirstInstallmentBeginningDate");
            contractsDataTable.Columns.Add("TotalFundingAmount");
            contractsDataTable.Columns.Add("TotalInstallmentsAmount");
            contractsDataTable.Columns.Add("AdvancePayment");
            contractsDataTable.Columns.Add("PaidInstallmentsCount");
            contractsDataTable.Columns.Add("PaidInstallments");
            contractsDataTable.Columns.Add("NonPaidInstallments");
            contractsDataTable.Columns.Add("RemainingInstallmentsCount");

            decimal totalFundingAmountTotals = 0;
            decimal paidInstallmentsTotals = 0;
            decimal nonPaidInstallmentsTotals = 0;
            double paidInstallmentsCountTotals = 0;
            double remainingInstallmentsCountTotals = 0;
            decimal installmentAmountTotals = 0;
            double installmentsCountTotals = 0;
            decimal advancePaymentTotals = 0;
            decimal totalInstallmentsAmountTotals = 0;

            for (var i = 0; i < contractDetailsViewModelList.Count; i++)
            {
                var row = contractsDataTable.NewRow();
                row["count"] = i + 1;
                row["ContractNumber"] = contractDetailsViewModelList[i].ContractNumber;
                row["Notes"] = contractDetailsViewModelList[i].Notes;
                row["FunderName"] = contractDetailsViewModelList[i].FunderName;
                row["InstallmentAmount"] = Helpers.FormatWithCommas(contractDetailsViewModelList[i].InstallmentAmount);
                row["InstallmentsCount"] = $"{contractDetailsViewModelList[i].InstallmentsCount}" + "شهر";
                row["FirstInstallmentBeginningDate"] = contractDetailsViewModelList[i].FirstInstallmentBeginningDate
                    .ToString("dd/MM/yyyy");
                row["TotalFundingAmount"] =
                    Helpers.FormatWithCommas(contractDetailsViewModelList[i].TotalFundingAmount);
                row["TotalInstallmentsAmount"] =
                    Helpers.FormatWithCommas(contractDetailsViewModelList[i].TotalInstallmentsAmount);
                row["AdvancePayment"] = Helpers.FormatWithCommas(contractDetailsViewModelList[i].AdvancePayment);
                row["PaidInstallmentsCount"] = contractDetailsViewModelList[i].PaidInstallmentsCount;
                row["PaidInstallments"] = Helpers.FormatWithCommas(contractDetailsViewModelList[i].PaidInstallments);
                row["NonPaidInstallments"] =
                    Helpers.FormatWithCommas(contractDetailsViewModelList[i].NonPaidInstallments);
                row["RemainingInstallmentsCount"] = contractDetailsViewModelList[i].RemainingInstallmentsCount;

                totalFundingAmountTotals += contractDetailsViewModelList[i].TotalFundingAmount;
                paidInstallmentsTotals += contractDetailsViewModelList[i].PaidInstallments;
                nonPaidInstallmentsTotals += contractDetailsViewModelList[i].NonPaidInstallments;
                paidInstallmentsCountTotals += contractDetailsViewModelList[i].PaidInstallmentsCount;
                remainingInstallmentsCountTotals += contractDetailsViewModelList[i].RemainingInstallmentsCount;
                installmentAmountTotals += contractDetailsViewModelList[i].InstallmentAmount;
                advancePaymentTotals += contractDetailsViewModelList[i].AdvancePayment;
                installmentsCountTotals += contractDetailsViewModelList[i].InstallmentsCount;
                totalInstallmentsAmountTotals += contractDetailsViewModelList[i].TotalInstallmentsAmount;


                contractsDataTable.Rows.Add(row);
            }

            var contractsInfoDataTable = new DataTable();

            contractsInfoDataTable.Columns.Add("PrintDate");
            contractsInfoDataTable.Columns.Add("User");
            contractsInfoDataTable.Columns.Add("Company");

            contractsInfoDataTable.Columns.Add("TotalFundingAmount");
            contractsInfoDataTable.Columns.Add("PaidInstallmentsTotals");
            contractsInfoDataTable.Columns.Add("NonPaidInstallmentsTotals");
            contractsInfoDataTable.Columns.Add("PaidInstallmentsCountTotals");
            contractsInfoDataTable.Columns.Add("RemainingInstallmentsCountTotals");
            contractsInfoDataTable.Columns.Add("InstallmentAmountTotals");
            contractsInfoDataTable.Columns.Add("InstallmentsCountTotals");
            contractsInfoDataTable.Columns.Add("AdvancePaymentTotals");
            contractsInfoDataTable.Columns.Add("TotalInstallmentsAmountTotals");

            var contractsInfoDataTableRow = contractsInfoDataTable.NewRow();
            contractsInfoDataTableRow["PrintDate"] = DateTime.Now.ToString("yyyy-MM-dd HH:m");
            contractsInfoDataTableRow["User"] = User.Identity.Name;
            contractsInfoDataTableRow["Company"] = funderId is not null && contractDetailsViewModelList.Count > 0
                ? contractDetailsViewModelList.First().FunderName
                : funderId is null
                    ? "جميع جهات التمويل"
                    : "";

            contractsInfoDataTableRow["TotalFundingAmount"] = Helpers.FormatWithCommas(totalFundingAmountTotals);
            contractsInfoDataTableRow["PaidInstallmentsTotals"] = Helpers.FormatWithCommas(paidInstallmentsTotals);
            contractsInfoDataTableRow["NonPaidInstallmentsTotals"] =
                Helpers.FormatWithCommas(nonPaidInstallmentsTotals);
            contractsInfoDataTableRow["PaidInstallmentsCountTotals"] =
                Helpers.FormatWithCommas(paidInstallmentsCountTotals);
            contractsInfoDataTableRow["RemainingInstallmentsCountTotals"] =
                Helpers.FormatWithCommas(remainingInstallmentsCountTotals);
            contractsInfoDataTableRow["InstallmentAmountTotals"] = Helpers.FormatWithCommas(installmentAmountTotals);
            contractsInfoDataTableRow["InstallmentsCountTotals"] = Helpers.FormatWithCommas(installmentsCountTotals);
            contractsInfoDataTableRow["AdvancePaymentTotals"] = Helpers.FormatWithCommas(advancePaymentTotals);
            contractsInfoDataTableRow["TotalInstallmentsAmountTotals"] =
                Helpers.FormatWithCommas(totalInstallmentsAmountTotals);

            contractsInfoDataTable.Rows.Add(contractsInfoDataTableRow);

            var mimetype = String.Empty;
            var extension = 1;

            string reportPath;

            if (funderId is not null)
            {
                reportPath = $"{_webHostEnvironment.WebRootPath}\\reports\\contracts.rdlc";
            }
            else
            {
                reportPath = $"{_webHostEnvironment.WebRootPath}\\reports\\allContracts.rdlc";
            }


            var pdfReportName = $"{Guid.NewGuid()}.pdf";

            var localReport = new LocalReport(reportPath);

            localReport.AddDataSource("contracts", contractsDataTable);

            localReport.AddDataSource("contractsInfo", contractsInfoDataTable);
            var result = localReport.Execute(RenderType.Pdf, extension, null, mimetype);
            return File(result.MainStream, "application/pdf", pdfReportName);
        }
        catch (Exception e)
        {
            TempData["contracts-index-temp-msg"] = "unknown-error";
            Log.Error("ContractsController - GetContractsPdf[GET] - {@message}", e.Message);
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpGet]
    [Authorize(Permissions.InstallmentPayments.View)]
    public async Task<ActionResult> GetInstallmentPaymentsPdf(int contractId)
    {
        try
        {
            var contract = await _unitOfWork
                .ContractRepository
                .GetFirstOrDefaultAsync(x => x.Id == contractId,
                    x => x.Include(a => a.InstallmentPayments)
                        .Include(e => e.Funder)
                        .Include(s => s.Destructions)
                );

            if (contract is null)
            {
                NotFound();
            }

            var dt = new DataTable();
            dt.Columns.Add("PaymentDate");
            dt.Columns.Add("PaymentMethod");
            dt.Columns.Add("BankRefNumber");
            dt.Columns.Add("TransferredBankAccountNumber");
            dt.Columns.Add("InstallmentNumber");
            dt.Columns.Add("InstallmentAmount");
            dt.Columns.Add("PaymentAmount");
            dt.Columns.Add("OtherPaymentsAmount");
            dt.Columns.Add("RemainingPaymentAmount");
            dt.Columns.Add("Notes");
            dt.Columns.Add("ContractNumber");

            var contractNumber = contract.ContractNumber;

            foreach (var payment in contract.InstallmentPayments)
            {
                var row = dt.NewRow();
                row["PaymentDate"] = payment.PaymentDate.ToString("dd/MM/yyyy hh:mm");
                row["PaymentMethod"] = payment.IsBank ? "تحويل" : "شبكة";
                row["BankRefNumber"] = payment.BankRefNumber;
                row["TransferredBankAccountNumber"] = payment.TransferredBankAccountNumber;
                row["InstallmentNumber"] = payment.InstallmentNumber;
                row["InstallmentAmount"] = Helpers.FormatWithCommas(payment.InstallmentAmount);
                row["PaymentAmount"] = Helpers.FormatWithCommas(payment.PaymentAmount);
                row["OtherPaymentsAmount"] = Helpers.FormatWithCommas(payment.OtherPaymentsAmount);
                row["RemainingPaymentAmount"] = Helpers.FormatWithCommas(payment.RemainingPaymentAmount);
                row["Notes"] = payment.Notes;
                row["ContractNumber"] = contractNumber;
                dt.Rows.Add(row);
            }

            var fundingNetAmount = CalculateContractTotalFundingAmount(contract);
            var paidInstallments = GetPaidInstallments(contract);
            var nonPaidInstallments = GetNonPaidInstallments(contract);
            var paidInstallmentsCount = contract.InstallmentPayments.LastOrDefault()?.InstallmentNumber ?? 0;
            var remainingInstallmentsCount = GetRemainingInstallmentsCount(contract);


            var installmentPaymentsInfoDataTable = new DataTable();
            installmentPaymentsInfoDataTable.Columns.Add("PrintDate");
            installmentPaymentsInfoDataTable.Columns.Add("User");
            installmentPaymentsInfoDataTable.Columns.Add("ContractNumber");
            installmentPaymentsInfoDataTable.Columns.Add("Funder");

            installmentPaymentsInfoDataTable.Columns.Add("FundingNetAmount");
            installmentPaymentsInfoDataTable.Columns.Add("PaidInstallments");
            installmentPaymentsInfoDataTable.Columns.Add("NonPaidInstallments");
            installmentPaymentsInfoDataTable.Columns.Add("PaidInstallmentsCount");
            installmentPaymentsInfoDataTable.Columns.Add("RemainingInstallmentsCount");
            installmentPaymentsInfoDataTable.Columns.Add("AdvancePayment");

            var installmentPaymentsInfoRow = installmentPaymentsInfoDataTable.NewRow();

            installmentPaymentsInfoRow["PrintDate"] = DateTime.Now.ToString("yyyy-MM-dd HH:m");
            installmentPaymentsInfoRow["User"] = User.Identity.Name;
            installmentPaymentsInfoRow["ContractNumber"] = contractNumber;
            installmentPaymentsInfoRow["Funder"] = contract.Funder.Name;

            installmentPaymentsInfoRow["FundingNetAmount"] = Helpers.FormatWithCommas(fundingNetAmount);
            installmentPaymentsInfoRow["PaidInstallments"] = Helpers.FormatWithCommas(paidInstallments);
            installmentPaymentsInfoRow["NonPaidInstallments"] = Helpers.FormatWithCommas(nonPaidInstallments);
            installmentPaymentsInfoRow["PaidInstallmentsCount"] = paidInstallmentsCount;
            installmentPaymentsInfoRow["RemainingInstallmentsCount"] = remainingInstallmentsCount;
            installmentPaymentsInfoRow["AdvancePayment"] = Helpers.FormatWithCommas(contract.AdvancePayment);

            installmentPaymentsInfoDataTable.Rows.Add(installmentPaymentsInfoRow);

            var mimetype = String.Empty;
            var extension = 1;

            var reportPath = $"{_webHostEnvironment.WebRootPath}\\reports\\installmentPayments.rdlc";
            var pdfReportName = $"{Guid.NewGuid()}.pdf";

            var localReport = new LocalReport(reportPath);
            localReport.AddDataSource("installmentPayments", dt);
            localReport.AddDataSource("installmentPaymentsInfo", installmentPaymentsInfoDataTable);
            var result = localReport.Execute(RenderType.Pdf, extension, null, mimetype);
            return File(result.MainStream, "application/pdf", pdfReportName);
        }
        catch (Exception e)
        {
            TempData["contracts-index-temp-msg"] = "unknown-error";
            Log.Error("ContractsController - GetInstallmentPaymentsPdf[GET] - {@message}", e.Message);
            return RedirectToAction(nameof(Index));
        }
    }

    private async Task<IActionResult> DeleteInstallmentPayment(int id)
    {
        try
        {
            var installmentPayment =
                await _unitOfWork
                    .InstallmentPaymentRepository
                    .GetFirstOrDefaultAsync(x => x.Id == id,
                        x => x.Include(a => a.Contract)
                            .ThenInclude(r => r.InstallmentPayments)
                    );

            if (installmentPayment is null)
            {
                TempData["contracts-index-temp-msg"] = "unknown-error";
                return RedirectToAction(nameof(Index));
            }

            if (installmentPayment.IsPaid)
            {
                TempData["contracts-index-temp-msg"] = "unknown-error";
                return RedirectToAction(nameof(Index));
            }

            var contract = installmentPayment.Contract;

            var contractIsPaid = contract.IsPaid;


            var isLastInstallmentPayment =
                Helpers.HasElementsAfter(contract.InstallmentPayments, installmentPayment);

            var contractHasOneInstallmentPayment = contract.InstallmentPayments.Count == 1;

            var installmentPayments = contract.InstallmentPayments;
            installmentPayments.Remove(installmentPayment);

            if (!isLastInstallmentPayment && !contractHasOneInstallmentPayment)
            {
                foreach (var p in installmentPayments)
                {
                    if (installmentPayment.IsPaid)
                    {
                        p.InstallmentNumber--;
                    }
                }
            }

            if (contractIsPaid)
            {
                contract.IsPaid = false;
            }

            _unitOfWork.InstallmentPaymentRepository.Remove(installmentPayment);

            var isSuccessSave = await _unitOfWork.SaveAsync();

            if (isSuccessSave)
            {
                TempData["contracts-delete-pay-temp-msg"] = "delete-success";
                return RedirectToAction(nameof(Index));
            }

            TempData["contracts-index-temp-msg"] = "unknown-error";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception e)
        {
            TempData["contracts-index-temp-msg"] = "unknown-error";
            return RedirectToAction(nameof(Index));
        }
    }

    private async Task<SelectList> GetFunderSelectList()
    {
        var funders = await (_unitOfWork.FunderRepository.All.Select(x => new
        {
            x.Id,
            x.Name
        })).ToListAsync();

        return new SelectList(funders, "Id", "Name");
    }

    private decimal CalculateContractTotalFundingAmount(Contract contract)
    {
        var fundingAmount = contract.BasicFundingAmount;
        var fundingAmountAfterTax = fundingAmount + fundingAmount * (decimal)contract.TaxRatio;
        var fundingAmountAfterInterest =
            fundingAmountAfterTax + fundingAmountAfterTax * (decimal)contract.InterestRatio;
        return Math.Round(fundingAmountAfterInterest + contract.AdministrativeFees, 2);
    }

    private decimal CalculateTotalInstallmentsAmount(Contract contract)
    {
        return Math.Round(contract.TotalFundingAmount - contract.AdvancePayment, 2);
    }

    private static decimal CalculateTotalForcedInstallmentsAmount(Contract contract)
    {
        return contract.InstallmentPayments.Where(x => x.HasForcePay)
            .Sum(e => e.InstallmentAmount);
    }

    private decimal CalculateInstallmentAmount(Contract contract)
    {
        decimal amount;

        if (contract.Destructions.Any() || HasForcedInstallments(contract))
        {
            amount = GetNonPaidInstallments(contract) / (decimal)GetRemainingInstallmentsCount(contract);
        }
        // else if (HasForcedInstallments(contract))
        // {
        //     amount = Math.Round(contract.TotalInstallmentsAmount -
        //                         CalculateTotalForcedInstallmentsAmount(contract)
        //                         / contract.InstallmentsCount - GetForcedInstallmentsCount(contract), 2);
        // }
        else
        {
            amount = contract.TotalInstallmentsAmount / contract.InstallmentsCount;
        }

        return Math.Round(amount, 2);
    }

    private async Task<List<ContractDetailsViewModel>> GetContractDetailsViewModelList(int? funderId,
        DateTime? fromDate, DateTime? toDate, bool isPaid, bool getAll)
    {
        var contractDetailsViewModelList = (await _unitOfWork.ContractRepository.GetAllAsync(
            filters: new List<Expression<Func<Contract, bool>>>()
            {
                c => funderId == null || funderId == c.FunderId,
                c => fromDate == null || c.FirstInstallmentBeginningDate >= fromDate,
                c => toDate == null || c.FirstInstallmentBeginningDate <= toDate,
                c => getAll || c.IsPaid == isPaid
            },
            include: x => x.Include(
                    a => a.InstallmentPayments
                ).Include(a => a.Funder)
                .Include(e => e.Destructions)
        )).Select(x => new ContractDetailsViewModel()
        {
            Id = x.Id,
            ContractNumber = x.ContractNumber,
            Notes = x.Notes,
            FunderName = x.Funder.Name,
            InstallmentAmount = CalculateInstallmentAmount(x),
            InstallmentsCount = x.InstallmentsCount,
            FirstInstallmentBeginningDate = x.FirstInstallmentBeginningDate,
            TotalFundingAmount = x.TotalFundingAmount,
            TotalInstallmentsAmount = x.TotalInstallmentsAmount,
            AdvancePayment = x.AdvancePayment,
            PaidInstallmentsCount = GetPaidInstallmentsCount(x),
            PaidInstallments = GetPaidInstallments(x),
            NonPaidInstallments = GetNonPaidInstallments(x),
            RemainingInstallmentsCount = GetRemainingInstallmentsCount(x)
        }).OrderBy(s => s.RemainingInstallmentsCount).ToList();

        return contractDetailsViewModelList;
    }

    private double GetRemainingInstallmentsCount(Contract contract)
    {
        double addedInstallmentsCount = 0;

        if (contract.InstallmentPayments.Any())
        {
            addedInstallmentsCount = contract.InstallmentPayments.LastOrDefault()!.InstallmentNumber;
        }

        return Math.Round(contract.InstallmentsCount - addedInstallmentsCount, 2);
    }

    private static decimal GetNonPaidInstallments(Contract contract)
    {
        if (contract.IsPaid)
        {
            return 0;
        }

        return Math.Round(contract.TotalInstallmentsAmount -
                          GetPaidInstallments(contract) - GetDestructionsAmount(contract.Destructions)
            , 2);
    }

    private static decimal GetPaidInstallments(Contract contract)
    {
        return contract.InstallmentPayments.Sum(a => a.PaymentAmount + a.OtherPaymentsAmount);
    }

    private static int GetPaidInstallmentsCount(Contract contract)
    {
        return contract.InstallmentPayments.Count(a => a.IsPaid);
    }

    private static bool HasInstallmentPayments(Contract contract)
    {
        return contract.InstallmentPayments.Any();
    }

    private static int GetLastPaidInstallmentPaymentIndex(Contract contract)
    {
        return contract.InstallmentPayments.FindLastIndex(x => x.IsPaid);
    }

    private static bool IsPaidLastInstallmentPayment(Contract contract)
    {
        return contract.InstallmentPayments.LastOrDefault().IsPaid;
    }

    private static bool IsCompletePendingPay(InstallmentPaymentViewModel model, bool isPendingPay)
    {
        return isPendingPay &&
               model.RemainingPaymentAmount ==
               (model.PaymentAmount + model.OtherPaymentsAmount);
    }

    private static bool IsCompletePay(InstallmentPaymentViewModel model)
    {
        return model.InstallmentAmount == (model.PaymentAmount + model.OtherPaymentsAmount);
    }

    private static bool IsPendingPay(InstallmentPaymentViewModel model)
    {
        return (model.PaymentAmount + model.OtherPaymentsAmount) != model.InstallmentAmount;
    }

    private static string GetPaymentStatusInWords(Contract contract)
    {
        return contract.IsPaid ? "مسدد" : "غير مسدد";
    }


    private static string GetPaymentMethodInWords(InstallmentPayment i)
    {
        return i.IsNet ? "شبكة" : "تحويل";
    }

    private static decimal GetDestructionsAmount(List<Destruction> destructions)
    {
        return destructions.Sum(x => x.Amount);
    }

    private Task<AppUser> GetAuthenticatedUser()
    {
        return _userManager.GetUserAsync(User);
    }

    private Task<AppUser> GetUser(string id)
    {
        return _userManager.FindByIdAsync(id);
    }

    private static bool HasForcedInstallments(Contract contract)
    {
        return contract.InstallmentPayments.Any(i => i.HasForcePay);
    }

    private static int GetForcedInstallmentsCount(Contract contract)
    {
        return contract.InstallmentPayments.Count(i => i.HasForcePay);
    }

    private static decimal GetForcedInstallmentsAmount(Contract contract)
    {
        return contract.InstallmentPayments.Where(i => i.HasForcePay).Sum(e => e.InstallmentAmount);
    }
}