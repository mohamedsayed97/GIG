using ICP_ABC.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ICP_ABC.Areas.Calendars.Models
{
    [Table("Calendar")]
    public class Calendar
    {
        [Key]
        public int CalendarID { get; set; }
        [StringLength(4)]
        [Required]
        [Index(IsUnique = true)]
        public string Code { get; set; }
        [StringLength(50)]
        [Required(ErrorMessage = "Name is required")]
        public string Vacation_Name { get; set; }
        [Required]
        [ForeignKey("ApplicationUser")]
        public string UserID { get; set; }
        [Required]
        public bool EditFlag { get; set; }
        public DeleteFlag DeletFlag { get; set; } = DeleteFlag.NotDeleted;
        [Required]
        public string Maker { get; set; }
        public bool Chk { get; set; }
        public string Checker { get; set; }
        public bool Auth { get; set; }
        public string Auther { get; set; }

        [Required(ErrorMessage = "Date is required, Must Be Today Or day in future Not before .")]
        public DateTime Vacation_date { get; set; }

        public DateTime SysDate { get; set; } = DateTime.Now;
        public ICollection<ApplicationUser> ApplicationUser { get; set; }

    }
}