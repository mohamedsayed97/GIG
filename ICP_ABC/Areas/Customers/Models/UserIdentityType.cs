using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ICP_ABC.Areas.Customers.Models
{
    public class UserIdentityType
    {
        [Key]
        public int UserIdentityTypeID { get; set; }
        public string Name { get; set; }
        public string ShortName { get; set; }
    }
}