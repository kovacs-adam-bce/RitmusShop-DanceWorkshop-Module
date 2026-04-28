using DanceWorkShop_Dnn.Dnn.SyntaxSalsa.DanceWorkShop.Services;
using DotNetNuke.Web.Mvc.Framework.ActionFilters;
using DotNetNuke.Web.Mvc.Framework.Controllers;
using System;
using System.Web.Mvc;
using DotNetNuke.Security;

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

        [HttpGet]
        public ActionResult Edit(int sessionId, bool view = false)
        { 
            ViewBag.SessionID = sessionId;
            ViewBag.IsViewer = view;

            return View();
        }

        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        public ActionResult Management()
        {

            DateTime fromDate = DateTime.UtcNow;
            DateTime toDate = fromDate.AddDays(7);

            var bookings = _bookingManager.FindBookingsByDate(fromDate, toDate, true);

            return View(bookings);
        }

        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        public ActionResult Settings()
        {
            return View();
        }
    }
}