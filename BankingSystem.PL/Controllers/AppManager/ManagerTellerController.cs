using AutoMapper;
using BankingSystem.BLL.Interfaces;
using BankingSystem.DAL.Models;
using BankingSystem.PL.ViewModels.Auth;
using BankingSystem.PL.ViewModels.Manager;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using System.Security.Claims;


namespace BankingSystem.PL.Controllers
{
    [Authorize(Roles = "Manager")]
    public class ManagerTellerController(IUnitOfWork unitOfWork, IMapper mapper, UserManager<ApplicationUser> userManager) : Controller
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;
        private readonly UserManager<ApplicationUser> _userManager = userManager;

        [HttpGet]
        public ActionResult GetAllTellers(string id)
        {
            var manager = _unitOfWork.Repository<DAL.Models.Manager>().GetSingleIncluding(b => b.Id == id);
            //if (manager == null)
            //{
            //    return NotFound($"Manager with ID {id} not found.");
            //}

            var branchId = manager.BranchId;
            //if (branchId == null)
            //{
            //    return NotFound($"Branch with ID {branchId} not found.");
            //}

            var employees = _unitOfWork.Repository<Teller>()
                .GetAllIncluding(e => e.Branch, e => e.Department)
                .Where(e => e.BranchId == branchId)
                .ToList();

            var tellerViewModels = _mapper.Map<List<TellerDetailsViewModel>>(employees);
            return View(tellerViewModels);
        }
        [HttpGet]
        public ActionResult GetTellerDetails(string id)
        {
            var employee = _unitOfWork.Repository<Teller>()
                .GetSingleIncluding(e => e.Id == id, e => e.Branch, e => e.Department);

            //if (employee == null)
            //{
            //    return NotFound();
            //}

            var tellerDetailsViewModel = _mapper.Map<TellerDetailsViewModel>(employee);

            tellerDetailsViewModel.BranchName = employee.Branch?.Name;
            tellerDetailsViewModel.DepartmentName = employee.Department?.Name ?? "No Departments";

            return View(tellerDetailsViewModel);
        }
        [HttpGet]
        public ActionResult CreateTeller()
        {
            var managerId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var manager = _unitOfWork.Repository<DAL.Models.Manager>().GetSingleIncluding(m => m.Id == managerId);

            //if (manager == null)
            //    return NotFound("Manager not found");

            ViewData["FixedRole"] = "Teller";
            return View("~/Views/Account/Register.cshtml");
        }
        [HttpPost]
        public async Task<ActionResult> CreateTeller(RegisterViewModel UserToRegister)
        {
            ViewData["FixedRole"] = "Teller";
            var managerId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var manager = _unitOfWork.Repository<DAL.Models.Manager>().GetSingleIncluding(m => m.Id == managerId);

            //if (manager == null || manager.BranchId == null)
            //    return NotFound("Manager or Branch not found");

            if (UserToRegister != null && ModelState.IsValid)
            {
                UserToRegister.Id = Guid.NewGuid().ToString();
                var teller = _mapper.Map<Teller>(UserToRegister);
                teller.Id = UserToRegister.Id;

                teller.BranchId = manager.BranchId;
                teller.ManagerId = manager.Id;

                IdentityResult result = await _userManager.CreateAsync(teller, UserToRegister.Password);

                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(teller, UserToRegister.Role);
                    //await _userManager.AddToRoleAsync(teller, "Teller");
                    return RedirectToAction(nameof(GetAllTellers), new { id = managerId });
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }
                return View(nameof(Register), UserToRegister);
        }

        [HttpGet]
        public ActionResult EditTeller(string id)
        {
            var teller = _unitOfWork.Repository<Teller>()
                .GetSingleIncluding(e => e.Id == id, e => e.Branch, e => e.Branch.MyManager, e => e.Department);
            //if (teller == null)
            //{
            //    return NotFound();
            //}

            var tellerDetailsViewModel = _mapper.Map<TellerDetailsViewModel>(teller);
            tellerDetailsViewModel.BranchName = teller.Branch?.Name;
            tellerDetailsViewModel.DepartmentName = teller.Department?.Name;

            if (teller.Branch?.MyManager == null)
            {
                return NotFound("The branch does not have a manager assigned.");
            }

            return View(tellerDetailsViewModel);
        }
        [HttpPost]
        public ActionResult EditTeller(TellerDetailsViewModel tellerDetailsViewModel)
        {
            if (ModelState.IsValid)
            {
                var teller = _unitOfWork.Repository<Teller>()
                    .GetSingleIncluding(e => e.Id == tellerDetailsViewModel.Id, e => e.Branch, e => e.Department, e => e.Branch.MyManager);
                //if (teller == null)
                //{
                //    return NotFound();
                //}

                _mapper.Map(tellerDetailsViewModel, teller);
                _unitOfWork.Complete();
                return RedirectToAction("GetAllTellers", new { id = teller.ManagerId });
            }
            return View(tellerDetailsViewModel);
        }
        [HttpGet]
        public ActionResult DeleteTeller(string id)
        {
            var teller = _unitOfWork.Repository<Teller>()
                .GetSingleIncluding(e => e.Id == id, e => e.Branch, e => e.Branch.MyManager, e => e.Department);
            //if (teller == null)
            //{
            //    return NotFound();
            //}
            var tellerDetailsViewModel = _mapper.Map<TellerDetailsViewModel>(teller);
            tellerDetailsViewModel.BranchName = teller.Branch?.Name;
            tellerDetailsViewModel.DepartmentName = teller.Department?.Name;
            return View(tellerDetailsViewModel);
        }
        [HttpPost]
        public ActionResult DeleteTeller(TellerDetailsViewModel tellerDetails)
        {
            var teller = _unitOfWork.Repository<Teller>()
                .GetSingleIncluding(e => e.Id == tellerDetails.Id, e => e.Branch, e => e.Department, e => e.Branch.MyManager);
            //if (teller == null)
            //{
            //    return NotFound();
            //}
            _unitOfWork.Repository<Teller>().Delete(teller);
            _unitOfWork.Complete();
            return RedirectToAction("GetAllTellers", new { id = teller.Branch.MyManager.Id });
        }
    }
}
