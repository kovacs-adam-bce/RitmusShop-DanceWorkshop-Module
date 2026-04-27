using DanceWorkShop_Dnn.Dnn.SyntaxSalsa.DanceWorkShop.Services;
using DotNetNuke.Web.Mvc.Framework.ActionFilters;
using DotNetNuke.Web.Mvc.Framework.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace DanceWorkShop_Dnn.Dnn.SyntaxSalsa.DanceWorkShop.Controllers
{
    [DnnHandleError]
    public class DanceWorkshopController : DnnController
    {
        private readonly IDanceWorkshopBookingManager _bookingManager;

        public DanceWorkshopController()
        {
            _bookingManager = new DanceWorkshopBookingManager();
        }

        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }
    }
}
