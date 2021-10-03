using ICP_ABC.Areas.Customers.Models;
using ICP_ABC.Areas.Policies.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ICP_ABC.Areas.Subscriptions.Models
{
    public class EmployeePolicy
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public string CustomerId { get; set; }
      

        [Required]
        public int CompanyId { get; set; }
    

        [Required]
        public int PolicyId { get; set; }

        [Required]
        public bool isSurrendered { get; set; }
    }
}