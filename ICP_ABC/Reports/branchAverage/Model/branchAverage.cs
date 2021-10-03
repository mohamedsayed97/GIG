using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using ICP_ABC.Areas.Funds.Models;


namespace ICP_ABC.Reports.branchAverage.Model
{
    public class branchAverageVM
    {
        public List<Fund> Funds { get; set; }
        public List<ClientType> ClientTypes { get; set; }

        [Display(Name = "From Date")]
        [Required]
        public DateTime? FromDate { get; set; }

        [Display(Name = "To Date")]
        [Required]
        public DateTime? ToDate { get; set; }

        public int? Fund { get; set; }
        public int? ClientType { get; set; }

        public bool byCustomer { get; set; }
        public bool ByBranch { get; set; }

    }
    public class ClientType
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}