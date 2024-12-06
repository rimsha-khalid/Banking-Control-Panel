using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingControlPanel.Data.Models
{
    public class Filter
    {
        [Key]
        public int FilterId { get; set; }  // Unique identifier for the filter
        [Required (ErrorMessage ="Filter parameter is required.")]
        public string? FilterParams { get; set; } // Represents parameters used for filtering 
        public DateTime FilterAt { get; set; } // Represents the date and time when the filter was applied
        public bool IsSuccessful { get; set; } // Indicates whether the filter operation was successful
    }
}
