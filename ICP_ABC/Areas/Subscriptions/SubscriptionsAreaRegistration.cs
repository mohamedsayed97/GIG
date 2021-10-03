using System.Web.Mvc;

namespace ICP_ABC.Areas.Subscriptions
{
    public class SubscriptionsAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Subscriptions";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "Subscriptions_default",
                "Subscriptions/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}