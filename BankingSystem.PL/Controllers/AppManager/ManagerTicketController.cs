using AutoMapper;
using BankingSystem.BLL.Interfaces;
using BankingSystem.BLL;
using BankingSystem.DAL.Models;
using BankingSystem.PL.ViewModels.Manager;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BankingSystem.PL.Controllers.AppManager
{
    public class ManagerTicketController(IUnitOfWork unitOfWork, IMapper mapper) : Controller
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;

        public IActionResult GetAllTickets()
        {
            var ManagerID = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            //if (ManagerID == null) return NotFound();

            var branch = _unitOfWork.Repository<Branch>().GetSingleIncluding(b => b.MyManager.Id == ManagerID);

            var Ticket = _unitOfWork.Repository<SupportTicket>()
                .GetAllIncluding(c => c.Customer!, t => t.Teller, a => a.Account)
                .Where(b => b.Account.BranchId == branch.Id)
                .ToList();

            //if (Ticket == null) return NotFound();
            
            foreach (var ticket in Ticket)
            {
                ticket.Teller = _unitOfWork.Repository<Teller>().GetSingleIncluding(t => t.Id == ticket.TellerId);
            }


            var TicketsViewModel = _mapper.Map<List<TicketsViewModel>>(Ticket);
            return View(TicketsViewModel);
        }

        public IActionResult PreviewTicket(int TicketId)
        {
            var Ticket = _unitOfWork.Repository<SupportTicket>().GetSingleIncluding(t => t.Id == TicketId, 
                                                                                    a => a.Account,
                                                                                    c => c.Customer,
                                                                                    t => t.Teller,
                                                                                    d => d.Customer.FinancialDocument);
            //if (Ticket == null) return NotFound();

            var LoanDetails = _mapper.Map<SupportTicket, TicketDetailsView>(Ticket);

            return View(LoanDetails);
        }



    }
}
