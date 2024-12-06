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
    public class Address // Represents the address of clients
    {
        [Key]
        public int AddressId { get; set; } // Unique Identifier for address
        [Required(ErrorMessage = "Country is required.")]
        public string Country { get; set; } // Represents country
        [Required(ErrorMessage = "City is required.")]
        public string City { get; set; } // Represents city
        public string? Street { get; set; } // Represents street (number or name)
        public string? ZipCode { get; set; } // Represents city's zipcode

        [ForeignKey("Client")]
        public int ClientId { get; set; } // ForeignKey of associated client

        [JsonIgnore]
        public virtual Client? Client{ get; set; }  //Navigation property of cleint

    }
}
