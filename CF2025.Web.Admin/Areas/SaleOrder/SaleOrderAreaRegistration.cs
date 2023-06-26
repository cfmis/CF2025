using System.Web.Mvc;

namespace CF2025.Web.Admin.Areas.SaleOrder
{
    public class SaleOrderAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "SaleOrder";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "SaleOrder_default",
                "SaleOrder/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}