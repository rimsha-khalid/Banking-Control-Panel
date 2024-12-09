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
        public int TotalRecords { get; set; }
        public int TotalPages { get; set; }
        public string Filter { get; set; }        // Store Sex enum as string (e.g., "Male" or "Female")
    }
}
