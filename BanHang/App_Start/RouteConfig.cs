using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace BanHang
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.MapRoute(
                name: "Products",
             url: "san-pham",
             defaults: new { controller = "Products", action = "Index", alias = UrlParameter.Optional },
             namespaces: new[] { "BanHang.Controllers" }
                );
            routes.MapRoute(
                name: "detailProducts",
             url: "chi-tiet/{alias}-p{id}",
             defaults: new { controller = "Products", action = "Details", alias = UrlParameter.Optional },
             namespaces: new[] { "BanHang.Controllers" }
                );
            routes.MapRoute(
       name: "vnpay_return",
       url: "vnpay_return",
       defaults: new { controller = "AddToCart", action = "VnpayReturn", alias = UrlParameter.Optional },
       namespaces: new[] { "BanHang.Controllers" }
   );
            routes.MapRoute(
            name: "NewsList",
            url: "tin-tuc",
            defaults: new { controller = "News", action = "Index", alias = UrlParameter.Optional },
            namespaces: new[] { "BanHang.Controllers" }
        );
            routes.MapRoute(
         name: "DetailNew",
         url: "{alias}-n{id}",
         defaults: new { controller = "News", action = "Detail", id = UrlParameter.Optional },
         namespaces: new[] { "BanHang.Controllers" }
     );
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                namespaces: new[] { "BanHang.Controllers" }
            );
        }
    }
}
