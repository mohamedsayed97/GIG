using System.Web.Mvc;

namespace ICP_ABC.Areas.FundTimes
{
    public class FundTimesAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "FundTimes";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "FundTimes_default",
                "FundTimes/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}