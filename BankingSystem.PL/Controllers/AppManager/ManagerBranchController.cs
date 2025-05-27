using BankingSystem.BLL;
using BankingSystem.BLL.Interfaces;
using BankingSystem.DAL.Models;
using BankingSystem.PL.ViewModels.Manager;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BankingSystem.PL.Controllers.AppManager
{
    public class ManagerBranchController(IUnitOfWork unitOfWork) : Controller
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        public ActionResult GetBranchDetails(string id) //managerID
        {
            var Manager = _unitOfWork.Repository<DAL.Models.Manager>().GetSingleIncluding(b => b.Id == id);

            var Branches = _unitOfWork.Repository<Branch>().GetSingleIncluding(
                                                                b => b.Id == Manager.BranchId,
                                                                l => l.Loans,
                                                                c => c.Customers,
                                                                t => t.Tellers,
                                                                d => d.Departments,
                                                                s => s.Savings
                                                                );

            //if (Branches == null)
            //    return RedirectToAction("Error", "Home");


            var BranchDetails = new BranchDetailsViewModel
            {
                Id = Branches.Id,
                Name = Branches.Name,
                Location = Branches.Location,
                Opens = Branches.Opens,
                Closes = Branches.Closes,
                Loans = Branches.Loans,
                Customers = Branches.Customers,
                Tellers = Branches.Tellers,
                Departments = Branches.Departments,
                Savings = Branches.Savings,
            };
            return View(BranchDetails);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditBusinessHours(BranchWorkingTimeViewModel Timing)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!ModelState.IsValid)
            {
                // Return validation errors
                if (Request.Headers.XRequestedWith == "XMLHttpRequest")
                //if (Request.IsAjaxRequest())
                    return Json(new { success = false, errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });

                return RedirectToAction("GetBranchDetails", new { id = userId });
            }

            try
            {
                var branchWorkingTime = _unitOfWork.Repository<Branch>().Get(Timing.BranchId);
                if (branchWorkingTime == null) return NotFound();

                branchWorkingTime.Opens = Timing.Opens;
                branchWorkingTime.Closes = Timing.Closes;
                _unitOfWork.Complete();
                
                if (Request.Headers.XRequestedWith == "XMLHttpRequest")
                    //if (Request.IsAjaxRequest())
                    return Json(new { success = true });

                return RedirectToAction("GetBranchDetails", new { id = userId });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
                if (Request.Headers.XRequestedWith == "XMLHttpRequest")
                    return Json(new { success = false, error = ex.Message });

                return RedirectToAction("GetBranchDetails", new { id = userId });
            }
        }
    }
}
