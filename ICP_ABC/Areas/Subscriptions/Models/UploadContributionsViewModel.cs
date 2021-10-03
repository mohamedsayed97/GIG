using ICP_ABC.Areas.Funds.Models;
using ICP_ABC.Areas.Policies.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ICP_ABC.Areas.Subscriptions.Models
{
    public class UploadContributionsViewModel
    {
        //[Required(ErrorMessage = "Please select fund")]
        //public int FundId { get; set; }

        //[Required(ErrorMessage = "Please insert unit price")]
        //public decimal icprice { get; set; }

        //public List<Fund> Funds { get; set; }
        //[Required(ErrorMessage = "Please select Policy")]
        //public string PolicyNo { get; set; }

        //public List<Policy> Policies { get; set; }


        [Required(ErrorMessage = "Please select file")]
        [FileExt(Allow = ".xls,.xlsx", ErrorMessage = "Only excel file")]
        public HttpPostedFileBase file { get; set; }

 



       
        //public DateTime valueDate { get; set; }

       // [Remote("IsAlreadyPriced", "Subscription", HttpMethod = "POST", AdditionalFields = "valueDate , FundId", ErrorMessage = "There is no price for this data.")]
        [Required(ErrorMessage = "Please select value date")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        [DataType(DataType.Date)]
        public DateTime processingDate { get; set; }
    }
}