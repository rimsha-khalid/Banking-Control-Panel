using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingControlPanel.Model.Models
{
    public class Search
    {
        public int SearchId { get; set; } // Unique identifier for the search
        [Required(ErrorMessage = "Search parameter is required.")]
        public string SearchRecord { get; set; }  // Represents parameters used for searching
        public DateTime SearchAt { get; set; } // Represents the date and time when Admin search a particular record
        public bool IsSuccessful { get; set; } // Indicates whether the search operation was successful
    }
}
