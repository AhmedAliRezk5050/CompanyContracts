using System.Data;
using AspNetCore.Reporting;
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
public class FundersController : Controller
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IWebHostEnvironment _webHostEnvironment;
    private readonly UserManager<AppUser> _userManager;

    public FundersController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment, UserManager<AppUser> userManager)
    {
        _unitOfWork = unitOfWork;
        _webHostEnvironment = webHostEnvironment;
        _userManager = userManager;
    }
    
    [HttpGet]
    [Authorize(Permissions.Funders.View)]
    public async Task<IActionResult> Index()
    {
        try
        {
            var funders = (await _unitOfWork
                .FunderRepository
                .GetAllAsync(include: x
                    => x.Include(e => e.User)));
            return View(funders);
        }
        catch (Exception e)
        {
            Log.Error("ContractsController - Index[GET] - {@message}", e.Message);
            TempData["home-temp-msg"] = "unknown-error";
            return RedirectToAction("Index", "Home");
        }
    }

    [HttpGet]
    [Authorize(Permissions.Funders.Add)]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [Authorize(Permissions.Funders.Add)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(FunderViewModel model)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var funder = new Funder();

            funder.Name = model.Name;
            funder.Address = model.Address;
            funder.PhoneNumber = model.PhoneNumber;
            funder.ContactEmployee = model.ContactEmployee;
            funder.MainNumber = model.MainNumber;
            funder.SubNumber = model.SubNumber;
            funder.User = await GetAuthenticatedUser();

            _unitOfWork.FunderRepository.Add(funder);

            var isSuccessSave = await _unitOfWork.SaveAsync();


            if (isSuccessSave)
            {
                TempData["funders-index-temp-msg"] = "create-success";
                return RedirectToAction(nameof(Index));
            }

            TempData["funders-index-temp-msg"] = "unknown-error";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception e)
        {
            TempData["funders-index-temp-msg"] = "unknown-error";
            Log.Error("FundersController - Create[GET] - {@message}", e.Message);
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpGet]
    [Authorize(Permissions.Funders.Edit)]
    public async Task<IActionResult> Edit(int id)
    {
        try
        {
            var funder = await _unitOfWork
                .FunderRepository
                .GetFirstOrDefaultAsync(x => x.Id == id);

            if (funder is null)
            {
                return NotFound();
            }

            var model = new FunderViewModel
            {
                Id = funder.Id,
                Name = funder.Name,
                Address = funder.Address,
                ContactEmployee = funder.ContactEmployee,
                PhoneNumber = funder.PhoneNumber,
                MainNumber = funder.MainNumber,
                SubNumber = funder.SubNumber
            };

            return View(model);
        }
        catch (Exception e)
        {
            TempData["funders-index-temp-msg"] = "unknown-error";
            Log.Error("FundersController - Edit[GET] - {@message}", e.Message);
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpPost]
    [Authorize(Permissions.Funders.Edit)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(FunderViewModel model)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var funder = await _unitOfWork
                .FunderRepository
                .GetFirstOrDefaultAsync(x => x.Id == model.Id);

            if (funder is null)
            {
                return NotFound();
            }

            funder.Name = model.Name;
            funder.Address = model.Address;
            funder.ContactEmployee = model.ContactEmployee;
            funder.PhoneNumber = model.PhoneNumber;
            funder.MainNumber = model.MainNumber;
            funder.SubNumber = model.SubNumber;
            funder.User = await GetAuthenticatedUser();

            _unitOfWork.FunderRepository.Update(funder);

            var isSuccessSave = await _unitOfWork.SaveAsync();

            if (isSuccessSave)
            {
                TempData["funders-index-temp-msg"] = "edit-success";
                return RedirectToAction(nameof(Index));
            }

            TempData["funders-index-temp-msg"] = "edit-error";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception e)
        {
            TempData["funders-index-temp-msg"] = "unknown-error";
            Log.Error("FundersController - Edit[POST] - {@message}", e.Message);
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpGet]
    [Authorize(Permissions.Funders.View)]
    public async Task<IActionResult> Details(int id)
    {
        try
        {
            var funder = await _unitOfWork
                .FunderRepository
                .GetFirstOrDefaultAsync(x => x.Id == id);

            if (funder is null)
            {
                return NotFound();
            }

            var funderViewModel = new FunderViewModel
            {
                Id = funder.Id,
                Name = funder.Name,
                Address = funder.Address,
                PhoneNumber = funder.PhoneNumber,
                ContactEmployee = funder.ContactEmployee,
                MainNumber = funder.MainNumber,
                SubNumber = funder.SubNumber
            };

            return View(funderViewModel);
        }
        catch (Exception e)
        {
            TempData["funders-index-temp-msg"] = "unknown-error";
            Log.Error("FundersController - Details[GET] - {@message}", e.Message);
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpGet]
    [Authorize(Permissions.Funders.Delete)]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var funder = await _unitOfWork
                .FunderRepository
                .GetFirstOrDefaultAsync(x => x.Id == id);

            if (funder is null)
            {
                return NotFound();
            }

            _unitOfWork.FunderRepository.Remove(funder);

            var isSuccessSave = await _unitOfWork.SaveAsync();

            if (isSuccessSave)
            {
                TempData["funders-index-temp-msg"] = "delete-success";
                return RedirectToAction(nameof(Index));
            }

            TempData["funders-index-temp-msg"] = "delete-error";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception e)
        {
            TempData["funders-index-temp-msg"] = "unknown-error";
            Log.Error("FundersController - Delete[GET] - {@message}", e.Message);
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpGet]
    [Authorize(Permissions.Funders.View)]
    public async Task<ActionResult> GetFundersPdf()
    {
        try
        {
            var funders = await _unitOfWork.FunderRepository.GetAllAsync();

            if (!funders.Any())
            {
                TempData["funders-index-temp-msg"] = "unknown-error";
                return RedirectToAction(nameof(Index));
            }

            var dt = new DataTable();

            dt.Columns.Add("Name");
            dt.Columns.Add("Address");
            dt.Columns.Add("PhoneNumber");
            dt.Columns.Add("ContactEmployee");
            dt.Columns.Add("MainNumber");
            dt.Columns.Add("SubNumber");

            foreach (var funder in funders)
            {
                DataRow row = dt.NewRow();
                row["Name"] = funder.Name;
                row["Address"] = funder.Address;
                row["PhoneNumber"] = funder.PhoneNumber;
                row["ContactEmployee"] = funder.ContactEmployee;
                row["MainNumber"] = funder.MainNumber;
                row["SubNumber"] = funder.SubNumber;
                dt.Rows.Add(row);
            }
            
            var fundersInfoDataTable = new DataTable();
            fundersInfoDataTable.Columns.Add("PrintDate");
            fundersInfoDataTable.Columns.Add("User");

            var fundersInfoDataTableRow = fundersInfoDataTable.NewRow();
            fundersInfoDataTableRow["PrintDate"] = DateTime.Now.ToString("yyyy-MM-dd HH:m");
            fundersInfoDataTableRow["User"] = User.Identity.Name;
            fundersInfoDataTable.Rows.Add(fundersInfoDataTableRow);
            
            
            var mimetype = String.Empty;
            var extension = 1;

            var reportPath = $"{_webHostEnvironment.WebRootPath}\\reports\\funders.rdlc";
            var pdfReportName = $"{Guid.NewGuid()}.pdf";
            
            var localReport = new LocalReport(reportPath);
            localReport.AddDataSource("funders", dt);
            localReport.AddDataSource("fundersInfo", fundersInfoDataTable);
            var result = localReport.Execute(RenderType.Pdf, extension, null, mimetype);


            return File(result.MainStream, "application/pdf", pdfReportName);
        }
        catch (Exception e)
        {
            TempData["funders-index-temp-msg"] = "unknown-error";
            Log.Error("FundersController - GetFundersPdf[GET] - {@message}", e.Message);
            return RedirectToAction(nameof(Index));
        }
    }
    
    private Task<AppUser> GetAuthenticatedUser()
    {
        return _userManager.GetUserAsync(User);
    }
}