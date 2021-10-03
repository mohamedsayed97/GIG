using ICP_ABC.Areas.Funds.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ICP_ABC.Areas.Subscriptions.Models
{
    public class UploadModificationsViewModel
    {
       
        [Required(ErrorMessage = "Please select file")]
        [FileExt(Allow = ".xls,.xlsx", ErrorMessage = "Only excel file")]
        public HttpPostedFileBase file { get; set; }

        
    }
}
