using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingControlPanel.Data.Models
{
    public class PaginationResult
    {
        public List<Client> Clients { get; set; }   // List of items for the current page
        public int TotalPages { get; set; }   // Total number of pages
        public int CurrentPage { get; set; }
        public int TotalCount { get; set; }
        public int TotalRecords { get; set; }
        public int PageSize { get; set; }     // Number of items per pag
        public string FilterSex { get; set; }  // Store the 'Sex' filter as a string ("Male" or "Female")

    }
}

