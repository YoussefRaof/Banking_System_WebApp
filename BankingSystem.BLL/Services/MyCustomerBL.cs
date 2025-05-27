using BankingSystem.BLL.Interfaces;
using BankingSystem.DAL.Data;
using BankingSystem.DAL.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BankingSystem.BLL.Services
{
    public class MyCustomerBL : IGenericRepository<Customer>, ISearchPaginationRepo<Customer>
    {
        private readonly BankingSystemContext _context;

        public MyCustomerBL(BankingSystemContext context)
        {
            _context = context;
        }


        public IEnumerable<Customer> GetAllByPagination(string? ID, string? filter, out int totalRecords, out int totalPages, int pageNumber = 1)
        {
            throw new NotImplementedException();
        }
        public Customer? Get(int id, string? UID = "", long number = 0)
        {
            throw new NotImplementedException();
        }


        public IQueryable<Customer> GetAll(string? userID = "", int flag = 1)
        {
            return _context.Customers
                .Include(c => c.Branch)
                .Where(c => c.Id == userID);
        }


        public IEnumerable<Customer> Search(string search, string? tellerID)
        {
            if (search == null)
                return _context.Customers
                        .Include(c => c.Branch)
                            .ThenInclude(c => c.Tellers)
                        .ToList();
            //.Where(c => c.Branch.Tellers.FirstOrDefault()!.Id == tellerID);
        
            var query = _context.Customers
                .Include(c => c.Branch)
                    .ThenInclude(b => b.Tellers)
                .Where(c => c.SSN.ToString()
                    .Contains(ISearchPaginationRepo<Customer>.MyRegex().Replace(search.Trim(), " ")));
        
            if (!query.Any())
                query = _context.Customers
                .Include(c => c.Branch)
                    .ThenInclude(b => b.Tellers)
                .Where(c => (c.FirstName + " " + c.LastName).ToLower().Trim()
                    .Contains(ISearchPaginationRepo<Customer>.MyRegex().Replace(search.ToLower().Trim(), " ")));
        
        
            return query;
        }



        public void Add(Customer Entity)
        {
            throw new NotImplementedException();
        }

        public void Delete(Customer Entity)
        {
            throw new NotImplementedException();
        }


        public IEnumerable<Customer> GetAllIncluding(params Expression<Func<Customer, object>>[] includes)
        {
            throw new NotImplementedException();
        }

        public Customer? GetSingleIncluding(Expression<Func<Customer, bool>> predicate, params Expression<Func<Customer, object>>[] includes)
        {
            throw new NotImplementedException();
        }

        public void Update(Customer Entity, string? TellerId = "")
        {
            throw new NotImplementedException();
        }

        public Task<bool> ExistsAsync(Expression<Func<Customer, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public Customer? GetSingleDeepIncluding(Expression<Func<Customer, bool>> predicate, params Func<IQueryable<Customer>, IIncludableQueryable<Customer, object>>[] includes)
        {
            throw new NotImplementedException();
        }
    }
}
