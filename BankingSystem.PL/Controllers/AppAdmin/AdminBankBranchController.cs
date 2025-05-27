using AutoMapper;
using BankingSystem.BLL.Interfaces;
using BankingSystem.DAL.Models;
using BankingSystem.PL.ViewModels;
using BankingSystem.PL.ViewModels.Admin;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;
using System.Security.Claims;

namespace BankingSystem.PL.Controllers.AppAdmin
{
    public class AdminBankBranchController(IUnitOfWork unitOfWork) : Controller
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;


        public IActionResult GetBankDetails(string id)
        {
            var Bank = _unitOfWork.Repository<Bank>().GetAllIncluding(
                        b => b.Admin,
                        b => b.Branches
                    ).FirstOrDefault();

            ViewBag.accountsNumber = _unitOfWork.Repository<Account>().GetAll().Count();
            ViewBag.tellersNumber = _unitOfWork.Repository<Teller>().GetAll().Count();
            ViewBag.customersNumber = _unitOfWork.Repository<Customer>().GetAll().Count();
            return View(Bank);
        }

        public IActionResult GetAllBranches()
        {
            var AdminID = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var Branches = _unitOfWork.Repository<Branch>().GetAllIncluding(
                                                                            b => b.MyManager,
                                                                            b => b.Loans,
                                                                            b => b.Customers,
                                                                            b => b.Tellers,
                                                                            b => b.Departments,
                                                                            b => b.Savings,
                                                                            b => b.Reservations).ToList();
            return View(Branches);
        }


        // GET: Branch/Create
        public IActionResult Create()
        {
            // Populate banks dropdown
            ViewBag.Banks = new SelectList(_unitOfWork.Repository<Bank>().GetAll(), "Id", "Name");
            return View();
        }

        // POST: Branch/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(BranchVM model, string? returnSection)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Create Branch entity from view model
                    var branch = new Branch
                    {
                        Name = model.Name,
                        Location = model.Location,
                        CreatedAt = DateTime.Now,
                        Opens = model.Opens,
                        Closes = model.Closes,
                        BankId = model.BankId
                    };

                    // Add branch to repository
                    _unitOfWork.Repository<Branch>().Add(branch);

                    // Create default savings account for the branch
                    var branchSavings = new Savings
                    {
                        BranchId = branch.Id,
                        Currency = "EGP",
                        Branch = branch,
                        Balance = 1000000, // Initial branch funds
                    };

                    _unitOfWork.Repository<Savings>().Add(branchSavings);

                    // Save changes
                    _unitOfWork.Complete();

                    TempData["SuccessMessage"] = $"Branch '{branch.Name}' created successfully.";
                    return RedirectToAction("Index", "Admin", new { activeSection = returnSection });
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, $"Error creating branch: {ex.Message}");
                }
            }

            return View(model);
        }
        // Helper method to get available managers for the dropdown
        private SelectList GetAvailableManagers(Branch existingBranch, string selectedManagerId)
        {
            var availableManagers = _unitOfWork.Repository<Manager>().GetAll()
                .Where(m => m.BranchId == null || m.Id == existingBranch.ManagerId)
                .ToList();

            return new SelectList(availableManagers, nameof(Manager.Id), nameof(Manager.FullName), selectedManagerId);
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var branch = _unitOfWork.Repository<Branch>().GetSingleIncluding(b => b.Id == id, b => b.MyManager!);

            if (branch == null)
                return NotFound();

            var branchVM = new BranchVM
            {
                Id = branch.Id,
                Name = branch.Name,
                Location = branch.Location,
                Opens = branch.Opens,
                Closes = branch.Closes,
                IsDeleted = branch.IsDeleted,
                ManagerId = branch.ManagerId,
                ManagerFirstName = branch.MyManager?.FirstName,
                ManagerLastName = branch.MyManager?.LastName,
                ManagerEmail = branch.MyManager?.Email,
                ManagerSSN = branch.MyManager?.SSN
            };

            ViewBag.Managers = GetAvailableManagers(branch, branch.ManagerId);
            return View(branchVM);
        }

        [HttpPost]
        public IActionResult Edit(BranchVM model, string? returnSection)
        {
            var existingBranch = _unitOfWork.Repository<Branch>()
                .GetSingleIncluding(b => b.Id == model.Id, b => b.MyManager!);

            if (existingBranch == null)
                return NotFound();

            ViewBag.Managers = GetAvailableManagers(existingBranch, model.ManagerId);

            model.Name = existingBranch.Name;
            model.Location = existingBranch.Location;
            model.Opens = existingBranch.Opens;
            model.Closes = existingBranch.Closes;
            model.IsDeleted = existingBranch.IsDeleted;

            model.ManagerFirstName = existingBranch.MyManager?.FirstName;
            model.ManagerLastName = existingBranch.MyManager?.LastName;
            model.ManagerEmail = existingBranch.MyManager?.Email;
            model.ManagerSSN = existingBranch.MyManager?.SSN;

            if (!ModelState.IsValid)
                return View(model);

            try
            {
                var currentManagerId = existingBranch.ManagerId;

                if (model.ManagerId != currentManagerId)
                {
                    if (!string.IsNullOrEmpty(currentManagerId))
                    {
                        var previousManager = _unitOfWork.Repository<Manager>().GetAll()
                            .FirstOrDefault(m => m.Id == currentManagerId);

                        if (previousManager != null)
                        {
                            previousManager.BranchId = null;
                            _unitOfWork.Repository<Manager>().Update(previousManager);
                        }
                    }

                    if (!string.IsNullOrEmpty(model.ManagerId))
                    {
                        var newManager = _unitOfWork.Repository<Manager>().GetAll()
                            .FirstOrDefault(m => m.Id == model.ManagerId);

                        if (newManager == null)
                        {
                            ModelState.AddModelError("ManagerId", "Selected manager does not exist");
                            return View(model);
                        }

                        newManager.BranchId = existingBranch.Id;
                        _unitOfWork.Repository<Manager>().Update(newManager);
                        existingBranch.ManagerId = newManager.Id;
                        existingBranch.MyManager = newManager;
                    }
                    else
                    {
                        existingBranch.ManagerId = null;
                        existingBranch.MyManager = null;
                    }
                }

                _unitOfWork.Repository<Branch>().Update(existingBranch);
                _unitOfWork.Complete();

                TempData["SuccessMessage"] = "Branch updated successfully";
                return RedirectToAction("Index", "Admin", new { activeSection = returnSection });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An error occurred while saving: " + ex.Message);
                return View(model);
            }
        }



        [HttpPost]
        public IActionResult Delete(int id, string? returnSection)
        {
            // Get the branch to be deleted
            var branch = _unitOfWork.Repository<Branch>().GetSingleIncluding(
                b => b.Id == id,
                b => b.MyManager,
                b => b.Tellers,
                b => b.Departments,
                b => b.Customers,
                b => b.Loans,
                b => b.Savings,
                b => b.Reservations
            );

            // Check if branch exists
            if (branch == null)
            {
                return NotFound();
            }

            try
            {
                // Check for related entities that might prevent deletion
                if (branch.Customers.Any() || branch.Loans.Any() || branch.Savings.Any())
                {
                    TempData["ErrorMessage"] = "Cannot delete branch because it has associated customers, loans, savings accounts.";
                    return RedirectToAction("Index", "Admin", new { activeSection = returnSection });
                }

                // Handle the branch manager relationship first
                if (branch.MyManager != null)
                {
                    // Unassign this branch from the manager
                    branch.MyManager.BranchId = null;
                    _unitOfWork.Repository<Manager>().Update(branch.MyManager);
                }

                // Reassign or remove tellers
                foreach (var teller in branch.Tellers)
                {
                    // Either delete tellers or set BranchId to null
                    teller.BranchId = null;
                    _unitOfWork.Repository<Teller>().Update(teller);
                }

                // Now that relationships are handled, delete the branch
                _unitOfWork.Repository<Branch>().Delete(branch);
                _unitOfWork.Complete();

                TempData["SuccessMessage"] = $"Branch '{branch.Name}' has been successfully deleted.";
            }
            catch (Exception ex)
            {
                // Log the error
                // _logger.LogError(ex, "Error deleting branch with ID {Id}", id);

                TempData["ErrorMessage"] = $"An error occurred while trying to delete the branch: {ex.Message}";
            }

            return RedirectToAction("Index", "Admin", new { activeSection = returnSection });
        }
    }
}