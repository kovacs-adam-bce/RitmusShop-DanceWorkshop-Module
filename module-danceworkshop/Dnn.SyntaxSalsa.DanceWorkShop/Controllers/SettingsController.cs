using DotNetNuke.Collections;
using DotNetNuke.Security;
using DotNetNuke.Web.Mvc.Framework.ActionFilters;
using DotNetNuke.Web.Mvc.Framework.Controllers;
using System.Web.Mvc;

namespace DanceWorkShop_Dnn.Dnn.SyntaxSalsa.DanceWorkShop.Controllers
{
    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
    [DnnHandleError]
    public class SettingsController : DnnController
    {
        [HttpGet]
        public ActionResult Settings()
        {
            var settings = new Models.Settings
            {
                DefaultCapacity = ModuleContext.Configuration.ModuleSettings.GetValueOrDefault("DanceWorkshop_Capacity", 10),
                AdminEmail = ModuleContext.Configuration.ModuleSettings.GetValueOrDefault("DanceWorkshop_AdminEmail", ""),
                CancelThreshold = ModuleContext.Configuration.ModuleSettings.GetValueOrDefault("DanceWorkshop_CancelThreshold", 24)
            };

            return View(settings);
        }

        [HttpPost]
        [ValidateInput(false)]
        [DotNetNuke.Web.Mvc.Framework.ActionFilters.ValidateAntiForgeryToken]
        public ActionResult Settings(Models.Settings settings)
        {
            ModuleContext.Configuration.ModuleSettings["DanceWorkshop_Capacity"] = settings.DefaultCapacity.ToString();
            ModuleContext.Configuration.ModuleSettings["DanceWorkshop_AdminEmail"] = settings.AdminEmail;
            ModuleContext.Configuration.ModuleSettings["DanceWorkshop_CancelThreshold"] = settings.CancelThreshold.ToString();

            return RedirectToDefaultRoute();
        }
    }
}