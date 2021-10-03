using System.Web.Mvc;

namespace ICP_ABC.Areas.NavRpt
{
    public class NavRptAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "NavRpt";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "NavRpt_default",
                "NavRpt/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}