using System.Web.Mvc;

namespace ICP_ABC.Areas.Group
{
    public class GroupAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Group";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "Group_default",
                "Group/{controller}/{action}/{id}",
                new {controller="Group", action = "Index", id = UrlParameter.Optional }
                
            );
        }
    }
}