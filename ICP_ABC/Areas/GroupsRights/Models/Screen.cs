using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ICP_ABC.Areas.GroupsRights.Models
{
    public class Screen
    {
        [Key]
        public int ScreenID { get; set; }

        [StringLength(10)]
        [Index(IsUnique =true)]
        public string Code { get; set; }

        [StringLength(15)]
        public string Name { get; set; }


    }
}