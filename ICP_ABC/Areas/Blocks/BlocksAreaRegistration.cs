using System.Web.Mvc;

namespace ICP_ABC.Areas.Blocks
{
    public class BlocksAreaRegistration : AreaRegistration 
    {

        public override string AreaName 
        {
            get 
            {
                return "Blocks";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "Blocks_default",
                "Blocks/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}