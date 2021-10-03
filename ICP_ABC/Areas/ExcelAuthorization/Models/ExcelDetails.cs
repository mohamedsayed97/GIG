using ICP_ABC.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ICP_ABC.Areas.ExcelAuthorization.Models
{
    public class ExcelDetails
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string Name { get; set; }

        public DateTime uploadDate { get; set; }

        public byte Screen { get; set; }

        public byte Status { get; set; }

        public byte[] FileContent { get; set; }

        public string ContentType { get; set; }

        public int FileSize { get; set; }

        public string FileExtension { get; set; }


        [Required]
        public string Maker { get; set; }

        public bool Chk { get; set; }
        public string Checker { get; set; }

        public bool Auth { get; set; }
        public string Auther { get; set; }

        public DateTime SysDate { get; set; } = DateTime.Now;
    }

    public enum ExcelStatus
    {
        Pending = 1,
        Deleted = 2,
        Approved = 3
    }

    public enum ExcelTransactionType
    {
        Addition = 1,
        Contribution = 2,
        Withdrawal = 3,
        Surrender = 4,
        Modification = 5
    }

}
