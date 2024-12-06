using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingControlPanel.Data.Models
{
    public class PaginationResult<T>
    {

        public List<T> Items { get; set; }   // List of items for the current page
        public int TotalCount { get; set; }   // Total number of items (across all pages)
        public int TotalPages { get; set; }   // Total number of pages
        public int PageNumber { get; set; }   // Current page number
        public int PageSize { get; set; }     // Number of items per page

        public string FilterFirstName { get; set; }
        public string FilterLastName { get; set; }
        public string FilterSex { get; set; }  // Store the 'Sex' filter as a string ("Male" or "Female")

    }
}

