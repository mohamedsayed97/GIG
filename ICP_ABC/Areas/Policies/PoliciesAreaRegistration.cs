using System.Web.Mvc;

namespace ICP_ABC.Areas.Policies
{
    public class PoliciesAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Policies";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "Policies_default",
                "Policies/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional },
                    namespaces: new[] { "ICP_ABC.Areas.Policies.Controllers" }
            );

            
        }
    }
}