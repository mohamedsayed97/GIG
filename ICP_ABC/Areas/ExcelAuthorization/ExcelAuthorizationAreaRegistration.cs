using System.Web.Mvc;

namespace ICP_ABC.Areas.ExcelAuthorization
{
    public class ExcelAuthorizationAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "ExcelAuthorization";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "ExcelAuthorization_default",
                "ExcelAuthorization/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}