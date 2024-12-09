using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace BankingControlPanel.Model.Models
{
    public class PaginationResult
    {

        public List<Client> Clients { get; set; }  
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int TotalCount { get; set; }
        public int PageSize { get; set; }
        public int TotalRecords { get; set; }

    }
}
