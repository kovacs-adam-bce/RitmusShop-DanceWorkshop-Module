using DotNetNuke.Web.Mvc.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotNetNuke.Web.Api;


namespace DanceWorkShop_Dnn.Dnn.SyntaxSalsa.DanceWorkShop
{
    public class RouteMapper : DotNetNuke.Web.Api.IServiceRouteMapper
    {
        public void RegisterRoutes(DotNetNuke.Web.Api.IMapRoute mapRouteManager)
        {
            mapRouteManager.MapHttpRoute(
                moduleFolderName: "DanceWorkShop",
                routeName: "default",
                url: "{controller}/{action}",
                defaults: new { },
                namespaces: new[] { "DanceWorkShop_Dnn.Dnn.SyntaxSalsa.DanceWorkShop.Controllers" }
            );
        }
    }
}