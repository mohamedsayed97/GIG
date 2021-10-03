using System.Web.Mvc;

namespace ICP_ABC.Areas.GroupsRights
{
    public class GroupsRightsAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "GroupsRights";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "GroupRight_default",
                "GroupsRights/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}