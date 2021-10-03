using System.Web.Mvc;

namespace ICP_ABC.Areas.FundRights
{
    public class FundRightsAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "FundRights";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "FundRights_default",
                "FundRights/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}