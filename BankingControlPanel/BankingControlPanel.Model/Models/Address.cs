using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingControlPanel.Model.Models
{
    public class Address
    {
        public int AddressId { get; set; }
        [Required(ErrorMessage = "Country is required.")]
        public string Country { get; set; }
        [Required(ErrorMessage = "City is required.")]
        public string City { get; set; }
        public string? Street { get; set; }
        public string? ZipCode { get; set; }
        public bool IsActive { get; set; }
        public int ClientId { get; set; }
    }
}
