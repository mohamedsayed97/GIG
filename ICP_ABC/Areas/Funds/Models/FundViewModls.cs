using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ICP_ABC.Areas.Funds.Models
{
   
    public class CreateFundViewModel
    {
        public decimal PriceTol { get; set; }
        [DisplayName("Fund Code")]
        [Required]
        public string Code { get; set; }

        [DisplayName("Nominal Value (Initial)")]
        [Required(ErrorMessage = "Nominal Value is required")]
        //[Range(1,Double.MaxValue,ErrorMessage = "Nominal Value must be greater than or equal one.")]
        public decimal nomval { get; set; }
      

        public int? FUNSDND { get; set; }

        [DisplayName("Fund Name")]
        [Required]
        public string Name { get; set; }

        //[Required]
        //[StringLength(10, ErrorMessage = "Max ISIN Must Be 10 Characters.", MinimumLength = 1)]
        [DisplayName("ISIN Code")]
        public string ISIN { get; set; }
        [Required]
        [DisplayName("Currency")]
        
        public int CurrencyID { get; set; }
       // [Required]
        [DisplayName("Min units per individual")]
    
        public int? MinInd { get; set; }
        //[Required]
        [DisplayName("Max units per individual")]
        public int? MaxInd { get; set; }
        //[Required]
        [DisplayName("Min units per corporate")]
        
        public int? MinCor { get; set; }
        //[Required]
        [DisplayName("Max units per corporate")]
        public int? MaxCor { get; set; }

        
        public int? cper_flag { get; set; }

        [DisplayName("Bank Sead Capital")]
        //[Required]
        public decimal? ceiling { get; set; }

        [DisplayName("Minimum client holding")]
        //[Required]
        public int? min_pos { get; set; }

        [DisplayName("Minimum Fund Utilization")]
        //[Required]
        public int? Min_hold_units { get; set; }

        [Required]
        public int no_ics { get; set; }

        [DisplayName("Issuer")]
        public int SponsorID { get; set; }
        [DisplayName("Subscription fees")]
        public decimal? SubFeesBar { get; set; }
        [DisplayName("Redemption fees")]
        public decimal? RedFeesBar { get; set; }
        [DisplayName("Subscription fees acc no.")]
        public string SubFeesAcc { get; set; }
        [DisplayName("Redemption fees acc no.")]
        public string RedemFeesAcc { get; set; }
        public decimal? OtherSubFees { get; set; }
        public decimal? OtherRedFees { get; set; }
        [DisplayName("Client Type")]
        //[Required]
        public int? CustTypeID { get; set; }
        [DisplayName(" Fund Account number")]
        public string FundAcc { get; set; }
        [DisplayName(" Other sub fees acc no.")]
        public string OtherSubAcc { get; set; }
        [DisplayName("Other red fees acc no.")]
        public string OtherRedAcc { get; set; }
        [DisplayName("Type")]
        public Type CboType { get; set; }
        //icprice
        [Required]
        [Range(0, 5, ErrorMessage = "ICprice Must be between 0 to 5")]
        public int ICprice { get; set; }


        [DataType(DataType.Date, ErrorMessage = "Date only")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DisplayName("Starting IPO")]
        //[Required]
        public DateTime? StartDate { get; set; }

     
        [DataType(DataType.Date, ErrorMessage = "Date only")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DisplayName("Ending IPO")]
        //[Required]
        public DateTime? EndDate { get; set; }
        //FundDay & FundAuthDay 
        //public virtual FundDay FundDay { get; set; }
        //public virtual FundAuthDay FundAuthDay { get; set; }
        //public List<Day> Days { get; set; }

        public bool? SpecialCase { get; set; }

    }

    public class EditFundViewModel
    {
        public decimal PriceTol { get; set; }

        [Required]
        [DisplayName("Fund Code")]
        public string Code { get; set; }

        [DisplayName("Nominal Value (Initial)")]
        [Required(ErrorMessage = "Nominal Value is required")]
        //[Range(1,Double.MaxValue,ErrorMessage = "Nominal Value must be greater than or equal one.")]
        public decimal nomval { get; set; }

        public int? FUNSDND { get; set; }

        [DisplayName("Fund Name")]
        [Required]
        public string Name { get; set; }

        [DisplayName("ISIN Code")]
        //[Required]
        //[StringLength(10, ErrorMessage = "Max ISIN Must Be 10 Characters.", MinimumLength = 1)]
        public string ISIN { get; set; }

        [DisplayName("Currency")]
        public int CurrencyID { get; set; }
        [DisplayName("Min units per individual")]
        //[Required]
        public int? MinInd { get; set; }
        [DisplayName("Max units per individual")]
        //[Required]
        public int? MaxInd { get; set; }
        [DisplayName("Min units per corporate")]
        //[Required]
        public int? MinCor { get; set; }
        [DisplayName("Max units per corporate")]
        //[Required]
        public int? MaxCor { get; set; }
        [DisplayName("Issuer")]
        public int SponsorID { get; set; }
        [DisplayName("Subscription fees")]
        public decimal? SubFeesBar { get; set; }
        [DisplayName("Redemption fees")]
        public decimal? RedFeesBar { get; set; }
        [DisplayName("Subscription fees acc no.")]
        public string SubFeesAcc { get; set; }
        [DisplayName("Redemption fees acc no.")]
        public string RedemFeesAcc { get; set; }
        [DisplayName("Other sub fees")]
        public decimal? OtherSubFees { get; set; }
        [DisplayName("Other red fees")]
        public decimal? OtherRedFees { get; set; }
        [DisplayName("Client Type")]
        //[Required]
        public int? CustTypeID { get; set; }
        [DisplayName("Fund Account number")]
        public string FundAcc { get; set; }
        [DisplayName("Other sub fees acc no.")]
        public string OtherSubAcc { get; set; }
        [DisplayName("Other red fees acc no.")]
        public string OtherRedAcc { get; set; }
        [DisplayName("Type")]
        public Type CboType { get; set; }
        public bool Check { get; set; }
        public string Checker { get; set; }
        [DefaultValue(0)]
        public bool Auth { get; set; }
        public string Auther { get; set; }

        //icprice
        [Required]
        [Range(0, 5, ErrorMessage = "ICprice Must be between 0 to 5")]
        public int ICprice { get; set; }
        [Required]
        public int no_ics { get; set; }
        public bool? SpecialCase { get; set; }


        //[Required]
        public int? cper_flag { get; set; }

        [DisplayName("Bank Sead Capital")]
        //[Required]
        public decimal? ceiling { get; set; }

        [DisplayName("Minimum client holding")]
        //[Required]
        public int? min_pos { get; set; }

        [DisplayName("Minimum Fund Utilization")]
        //[Required]
        public int? Min_hold_units { get; set; }

        [DataType(DataType.Date, ErrorMessage = "Date only")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DisplayName("Starting IPO")]
        //[Required]
        public DateTime StartDate { get; set; }


        [DataType(DataType.Date, ErrorMessage = "Date only")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DisplayName("Ending IPO")]
        //[Required]
        public DateTime EndDate { get; set; }
    }


    public class DetailsFundViewModel
    {

        [Required]
        public int no_ics { get; set; }

        public decimal PriceTol { get; set; }
        [DisplayName("Fund Code")]
        [Required]
        public string Code { get; set; }
        [DisplayName("Fund Name")]
        [Required]
        public string Name { get; set; }
        [DisplayName("ISIN Code")]
        //[Required]
        //[StringLength(10, ErrorMessage = "Max ISIN Must Be 10 Characters.", MinimumLength = 1)]
        public string ISIN { get; set; }
        [DisplayName("Currency")]
        public int? CurrencyID { get; set; }
        [DisplayName("Min units per individual")]
        public int? MinInd { get; set; }
        [DisplayName("Max units per individual")]
        public int? MaxInd { get; set; }
        [DisplayName("Min units per corporate")]
        public int? MinCor { get; set; }
        [DisplayName("Max units per corporate")]
        public int? MaxCor { get; set; }
        [DisplayName("Issuer")]
        public int SponsorID { get; set; }
        [DisplayName("Subscription fees")]
        public decimal? SubFeesBar { get; set; }
        [DisplayName("Redemption fees")]
        public decimal? RedFeesBar { get; set; }
        [DisplayName("Subscription fees acc no.")]
        public string SubFeesAcc { get; set; }
        [DisplayName("Redemption fees acc no.")]
        public string RedemFeesAcc { get; set; }
        [DisplayName("Other sub fees")]
        public decimal OtherSubFees { get; set; }
        [DisplayName("Other Red fees")]
        public decimal OtherRedFees { get; set; }
        [DisplayName("Client Type")]
        //[Required]
        public int? CustTypeID { get; set; }
        [DisplayName("Fund Account number")]
        public string FundAcc { get; set; }
        [DisplayName("Other sub fees acc no.")]
        public string OtherSubAcc { get; set; }
        [DisplayName("Other red fees acc no.")]
        public string OtherRedAcc { get; set; }
        [DisplayName("Type")]
        public Type CboType { get; set; }
        public bool Check { get; set; }
        public string Checker { get; set; }
        [DefaultValue(0)]
        public bool Auth { get; set; }
        public string Auther { get; set; }

        //---------------
        public bool AuthForEditAndDelete { get; set; }

        //icprice
        [Required]
        [Range(0, 5, ErrorMessage = "ICprice Must be between 0 to 5")]
        public int ICprice { get; set; }

        public bool? SpecialCase { get; set; }


        //[Required]
        public int? cper_flag { get; set; }

        [DisplayName("Bank Sead Capital")]
        //[Required]
        public decimal? ceiling { get; set; }

        [DisplayName("Minimum client holding")]
        //[Required]
        public int? min_pos { get; set; }

        [DisplayName("Minimum Fund Utilization")]
        //[Required]
        public int? Min_hold_units { get; set; }

        [DataType(DataType.Date, ErrorMessage = "Date only")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DisplayName("Starting IPO")]
        //[Required]
        public DateTime StartDate { get; set; }


        [DataType(DataType.Date, ErrorMessage = "Date only")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DisplayName("Ending IPO")]
        //[Required]
        public DateTime EndDate { get; set; }

        [DisplayName("Nominal Value (Initial)")]
        [Required(ErrorMessage = "Nominal Value is required")]
        //[Range(1,Double.MaxValue,ErrorMessage = "Nominal Value must be greater than or equal one.")]
        public decimal nomval { get; set; }

        public int? FUNSDND { get; set; }
    }
}