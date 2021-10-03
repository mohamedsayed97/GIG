using ICP_ABC.Areas.Currencies.Models;
using ICP_ABC.Areas.Sponsors.Models;
using ICP_ABC.Areas.CustTypes.Models;
using ICP_ABC.Models;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ICP_ABC.Areas.Funds.Models
{
   public enum Type
    {        
        LocalMoneyMarker,
        LocalEquity,
        OffShore

    }
    [Table("fund")]
    public class Fund
    {
        [Key]
        public int FundID { get; set; }
        [Index(IsUnique = true)]
        [StringLength(4)]
        [Required]
        public string Code { get; set; }

        [DisplayName("Fund Name")]
        [Index(IsUnique = true)]
        [MaxLength(100)]
        [Column("fname")]
        public string Name { get; set; }
        [ForeignKey("Currency")]
        
        public int CurrencyID { get; set; }
        [ForeignKey("Sponsor")]
       
        public int SponsorID { get; set; }
        //-------------------
         
        [DataType(DataType.Date)]
       
        public DateTime StartDate { get; set; }
         
        [DataType(DataType.Date)]
       
        public DateTime EndDate { get; set; }
       
        public decimal ParView { get; set; }
       
        public int Units { get; set; }
        //[RegularExpression(@"\d+(\.\d{1,5})?", ErrorMessage = "Invalid ")]
       
        public decimal SubFeesBar { get; set; }
        //[RegularExpression(@"\d+(\.\d{1,5})?", ErrorMessage = "Invalid ")]
        
        public decimal RedFeesBar { get; set; }
        
        public int MinInd { get; set; }
        
        public int MaxInd { get; set; }
        
        public int MinCor { get; set; }
        
        public int MaxCor { get; set; }
      
        public decimal OtherSubFees { get; set; }
       
        public decimal OtherRedFees { get; set; }
        
        public int AdminFees { get; set; }
       
        public int EarlyFees { get; set; }
         
        [DataType(DataType.Date)]
       
        public DateTime SysDate { get; set; } = DateTime.Now;
         
        [DataType(DataType.Date)]
       
        public DateTime EntryDate { get; set; }
        [ForeignKey("ApplicationUser")]
        public string UserID { get; set; }
        [Required]
        public bool EditFlag { get; set; }

        public DeleteFlag DeletFlag { get; set; } = DeleteFlag.NotDeleted;

        [Required]
        [Column("inputer")]
        public string Maker { get; set; }

        public bool Chk { get; set; }
        public string Checker { get; set; }

        public bool Auth { get; set; }
        public string Auther { get; set; }
         
        [DataType(DataType.Date)]
       
        public DateTime Timestamp { get; set; }
        public decimal Nav { get; set; }
        public string Remark { get; set; }
        //[Index(IsUnique = true)]
        //[StringLength(10)]
        public string ISIN { get; set; }
         
        [DataType(DataType.Date)]
     
        public DateTime InvDate { get; set; }
       
        public string FundAcc { get; set; }
        
        public decimal MarkFees { get; set; }
       
        public string SubFeesAcc { get; set; }
      
        public string RedemFeesAcc { get; set; }
       
        public string ManageFeesAcc { get; set; }
       
        public string AdminFeesAcc { get; set; }
      
        public string OtherSubAcc { get; set; }
       
        public string OtherRedAcc { get; set; }
       
        public string EarlyFeesAcc { get; set; }
        //we'll make this enum
        // public int ClientType { get; set; }
       
        public int ProductEligable { get; set; }
       
        public string GuaranteeNotePer { get; set; }
       
        public string GuranteeNote { get; set; }
        
        public Type CboType { get; set; }
      
        public decimal UpFrontFees { get; set; }
       
        public string UpFrontAcc { get; set; }
        
        public string FreeText { get; set; }
       
        public int CouponBox { get; set; }
        public string UserLog { get; set; }
        public bool HasFundTime { get; set; }
        public bool HasICPrice { get; set; }
        public virtual Currency Currency { get; set; }
        public Sponsor Sponsor { get; set; }

        // Foreign key -----------------------------------

        [Display(Name = "Client Type")]
        public int? CustTypeID { get; set; }

        //[ForeignKey("CustTypeID")]
        //public virtual CustType custType { get; set; }

        //--------------------------------------------

        public virtual ApplicationUser ApplicationUser { get; set; }

        public bool sub_fees_type { get; set; }

        public decimal Min_Sub_Fees { get; set; }

        public Int16 acc_Sub_Fees_Type { get; set; }

        public decimal Max_Sub_Fees { get; set; }

        public bool max_sub_fees_type { get; set; }

        public bool Red_Fees_Type { get; set; }

        public decimal Min_Red_Fees { get; set; }

        public int acc_red_fees_type { get; set; }

        public decimal Max_Red_Fees { get; set; }

        public decimal Max_Red_Fees_type { get; set; }

         
        [DataType(DataType.Date)]
       
        public DateTime fmatlead { get; set; }

        public decimal ceiling { get; set; }

        public int cper_flag { get; set; }

        public int no_ics { get; set; }

        public decimal nomval { get; set; }

        public decimal subper { get; set; }

        public decimal redper { get; set; }

        public int FUNSDND { get; set; }

        public int min_pos { get; set; }

        public int Min_hold_units { get; set; }

        public string FAccType { get; set; }

        public string Fund_UAccNo { get; set; }

        public string susp_acc_no { get; set; }
         
        [DataType(DataType.Date)]
        
        public DateTime Inception_Date { get; set; }

        public int TransOrder { get; set; }

        public decimal PriceTol { get; set; }

         
        [DataType(DataType.Date)]
       
        public DateTime min_Date { get; set; }

        public string fNameAr { get; set; }

        public decimal amount { get; set; }

        public int Monthly { get; set; }

        public int Weekly { get; set; }

        public int MonthlyOption { get; set; }

        //-------------------------
        //public bool ViewTransaction { get; set; }

        //public bool View { get; set; }
        //Icprice 
        [Required]
        [Range(0, 5, ErrorMessage = "ICprice Must be between 0 to 5")]
        public int ICprice { get; set; }
        //-----InputType SpecialCase
        public bool SpecialCase { get; set; }


    }

}