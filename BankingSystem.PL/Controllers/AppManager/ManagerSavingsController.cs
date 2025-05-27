using AutoMapper;
using BankingSystem.BLL.Interfaces;
using BankingSystem.DAL.Models;
using BankingSystem.PL.ViewModels.Manager;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;

namespace BankingSystem.PL.Controllers.AppManager
{

    [Authorize(Roles = "Manager")]
    public class ManagerSavingsController(IUnitOfWork unitOfWork, IMapper mapper) : Controller
    {
        readonly private IUnitOfWork _unitOfWork = unitOfWork;
        readonly private IMapper _mapper = mapper;

        [HttpGet]
        public ActionResult GetAllSavings()
        {
            var managerId = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
            var manager = _unitOfWork.Repository<DAL.Models.Manager>().GetSingleIncluding(b => b.Id == User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            //if (manager?.BranchId == null) return NotFound("Manager or branch not found");


            var savings = _unitOfWork.Repository<Savings>()
                .GetAll()
                .Where(s => s.BranchId == manager.BranchId)
                .ToList();

            var viewModel = _mapper.Map<List<SavingsViewModel>>(savings);

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateSaving(SavingsViewModel model)
        {
            if (!ModelState.IsValid)
            {
                // Return to view with errors
                TempData["Error"] = "Invalid data";
                return RedirectToAction(nameof(GetAllSavings), new { id = TempData["ManagerId"] });
            }

            var saving = _unitOfWork.Repository<Savings>().Get(model.Id);
            //if (saving == null)
            //{
            //    return NotFound();
            //}

            // Selective update
            saving.Currency = model.Currency;
            saving.Balance = model.Balance;

            try
            {
                _unitOfWork.Complete();
                TempData["Success"] = "Update successful";
            }
            catch (Exception)
            {
                TempData["Error"] = "Update failed";
            }

            // Preserve manager ID across redirect
            return RedirectToAction(nameof(GetAllSavings), new { id = User.FindFirst(ClaimTypes.NameIdentifier)!.Value });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddSaving(SavingsViewModel model)
        {
            var managerId = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
            var manager = _unitOfWork.Repository<DAL.Models.Manager>().GetSingleIncluding(b => b.Id == managerId);
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Invalid data";
                return RedirectToAction(nameof(GetAllSavings), new { id = User.FindFirst(ClaimTypes.NameIdentifier)!.Value });
            }

            Savings newSaving = new()
            {
                Currency = model.Currency,
                Balance = model.Balance,
                BranchId = (int)manager!.BranchId!
            };

            try
            {
                _unitOfWork.Repository<Savings>().Add(newSaving);
                _unitOfWork.Complete();
                TempData["Success"] = "Saving added successfully";
            }
            catch (Exception)
            {
                TempData["Error"] = "Failed to add saving";
            }

            return RedirectToAction(nameof(GetAllSavings), new { id = managerId });
        }
    }
}