using AutoMapper;
using BankingSystem.BLL.Interfaces;
using BankingSystem.DAL.Models;
using BankingSystem.PL.Controllers.AppTeller;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace BankingSystem.PL.Controllers.CreationHub
{
    public class CreateUsingSignalR : Hub
    {
        public async Task NotifyCustomerCreated(string customerId, string tellerId)
        {
            await Clients.All.SendAsync("ReceiveCustomerCreated", customerId, tellerId);
        }

        public async Task NotifyCustomerCreationFailed(string error)
        {
            await Clients.Caller.SendAsync("ReceiveCustomerCreationFailed", error);
        }
    }
}
