using AutoMapper;
using BankingSystem.BLL.Interfaces;
using BankingSystem.DAL.Models;
using BankingSystem.PL.ViewModels.Manager;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Elfie.Diagnostics;
using System.Security.Claims;

namespace BankingSystem.PL.Controllers.AppManager
{
    public class ManagerLoanController(IUnitOfWork unitOfWork, IMapper mapper) : Controller
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;
        public IActionResult GetAllLoans()
        {
            var ManagerID = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            //if (ManagerID == null) return NotFound();

            var branch = _unitOfWork.Repository<Branch>().GetSingleIncluding(b => b.MyManager.Id == ManagerID, b => b.MyManager);
            //if (branch == null) return NotFound("Not found.");

            var Loans = _unitOfWork.Repository<Loan>()
                .GetAllIncluding(c => c.Customer, a => a.Account)
                .Where(b => b.BranchId == branch.Id)
                .ToList();
            //if (Loans == null) return NotFound();

            var loansViewModel = _mapper.Map<List<LoansViewModel>>(Loans);
            return View(loansViewModel);
        }

        public IActionResult PreviewLoan(int LoanId)
        {
            var Loan = _unitOfWork.Repository<Loan>().GetSingleIncluding(l => l.Id == LoanId,
                                                                        a => a.Account,
                                                                        c => c.Customer,
                                                                        c => c.Customer.FinancialDocument);
            //if (Loan == null) return NotFound();

            var LoanDetails = _mapper.Map<Loan, LoanDetailsViewModel>(Loan);

            return View(LoanDetails);
        }

        public IActionResult DenyLoan(int LoanId)
        {
            var Loan = _unitOfWork.Repository<Loan>().GetSingleIncluding(l => l.Id == LoanId, a => a.Account, c => c.Customer);
            //if (Loan == null) return NotFound();

            Loan.LoanStatus = LoanStatus.Denied;
            _unitOfWork.Complete();

            return RedirectToAction(nameof(GetAllLoans));
        }

        public IActionResult AcceptLoan(int LoanId)
        {
            var Loan = _unitOfWork.Repository<Loan>().GetSingleIncluding(l => l.Id == LoanId, a => a.Account, c => c.Customer);
            //if (Loan == null) return NotFound();

            Loan.LoanStatus = LoanStatus.Accepted;

            _unitOfWork.Complete();

            return RedirectToAction(nameof(GetAllLoans));
        }


        [HttpGet]
        public IActionResult DownloadDocument(string customerId, int loanId)
        {
            //customerId = "0578fd86-26c8-4fb4-a347-a0e8997a7a34";

            //if (string.IsNullOrEmpty(customerId))
            //{
            //    return NotFound("CustomerId is required.");
            //}

            var managerId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var branch = _unitOfWork.Repository<Branch>()
                .GetSingleIncluding(b => b.ManagerId == managerId, b => b.Customers);

            //if (branch == null)
            //    return NotFound("Branch not found for the logged-in manager.");

            var customer = _unitOfWork.Repository<Customer>()
                .GetAllIncluding(c => c.FinancialDocument, c => c.Loans)
                .FirstOrDefault(c => c.Id == customerId && c.BranchId == branch.Id);

            //if (customer == null)
            //{
            //    return NotFound($"Customer not found. customerId: {customerId}, branchId: {branch.Id}");
            //}

            bool hasLoan = customer.Loans.Any(l => l.Id == loanId);

            //if (!hasLoan)
            //    return NotFound("Customer does not have this loan.");

            var document = customer.FinancialDocument.FirstOrDefault(d => d.LoanId == loanId);

            if (document == null || document.FileData == null)
                return NotFound("Document not found or file data is missing.");

            ViewBag.CustomerId = document.CustomerId;

            return File(document.FileData, document.ContentType, document.FileName);
        }



    }
}
