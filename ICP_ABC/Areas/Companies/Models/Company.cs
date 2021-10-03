using ICP_ABC.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ICP_ABC.Areas.Companies.Models
{
    public class Company
    {
        [Key , DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } 

        public bool Chk { get; set; }
        public string Checker { get; set; }

        public bool Auth { get; set; }
        public string Auther { get; set; }
        public DeleteFlag DeletFlag { get; set; } = DeleteFlag.NotDeleted;
        public DateTime SysDate { get; set; } = DateTime.Now;

    
    }
}

