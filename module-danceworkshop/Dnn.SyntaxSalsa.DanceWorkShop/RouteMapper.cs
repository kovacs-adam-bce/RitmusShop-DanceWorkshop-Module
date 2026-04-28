using DotNetNuke.Web.Api;
using System.Web.Http;

namespace DanceWorkShop_Dnn.Dnn.SyntaxSalsa.DanceWorkShop
{
    public class RouteMapper : IServiceRouteMapper
    {
        public void RegisterRoutes(IMapRoute mapRouteManager)
        {
            mapRouteManager.MapHttpRoute(
                moduleFolderName: "YourRFProject/module-danceworkshop/Dnn.SyntaxSalsa.DanceWorkShop",
                routeName: "default",
                url: "{controller}/{action}/{id}",
                defaults: new { id = RouteParameter.Optional },
                namespaces: new[] { "DanceWorkShop_Dnn.Dnn.SyntaxSalsa.DanceWorkShop.Controllers" }
            );
        }
    }
}