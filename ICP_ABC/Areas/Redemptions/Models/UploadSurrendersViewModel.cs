using ICP_ABC.Areas.Funds.Models;
using ICP_ABC.Areas.Policies.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ICP_ABC.Areas.Redemptions.Models
{
    public class UploadSurrendersViewModel
    {
        //[Required(ErrorMessage = "Please select fund")]
        //public int FundId { get; set; }
        //public List<Fund> Funds { get; set; }

        //[Required(ErrorMessage = "Please select Policy")]
        //public string PolicyNo { get; set; }

        //public List<Policy> Policies { get; set; }

        [Required(ErrorMessage = "Please select file")]
        [FileExt(Allow = ".xls,.xlsx", ErrorMessage = "Only excel file")]
        public HttpPostedFileBase file { get; set; }
    }
}