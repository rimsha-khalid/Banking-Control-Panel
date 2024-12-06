using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BankingControlPanel.Data.Models
{
    public class Pagination
    {

        [Key]
        public int Id { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }

        // Store filtering details as strings
        public string FilterFirstName { get; set; }  // Filter by First Name
        public string FilterLastName { get; set; }   // Filter by Last Name
        public string FilterSex { get; set; }        // Store Sex enum as string (e.g., "Male" or "Female")
        public string UserId { get; set; }   // Foreign Key of associated User

      
    }
}
