using System.Web.Mvc;

namespace CF2025.Web.Admin.Areas.Prod
{
    public class ProdAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Prod";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "Prod_default",
                "Prod/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}