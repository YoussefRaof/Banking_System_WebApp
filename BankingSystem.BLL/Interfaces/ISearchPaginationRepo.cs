using BankingSystem.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BankingSystem.BLL.Interfaces
{
    public interface ISearchPaginationRepo<T> where T : class
    {
        public IEnumerable<T> Search(string search, string? userID = "");
        public IEnumerable<T> GetAllByPagination(string? ID, string? filter, out int totalRecords, out int totalPages, int pageNumber = 1);
    
    
        public static Regex MyRegex()
        {
            return new Regex(@"\s+");
        }
        
    }
}
