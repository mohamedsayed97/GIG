using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ICP_ABC.Reports._UsersRpt.Model
{
    public class _UsersRptVM
    {
        public List<UserStatus> Statuses { get; set; }
        public int status { get; set; }

        [Display(Name = "From Date")]
        [Required]
        public DateTime? FromDate { get; set; }

        [Display(Name = "To Date")]
        [Required]
        public DateTime? ToDate { get; set; }
    }
    public class UserStatus
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}