using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DotNetNuke.Web.Api;
using DotNetNuke.Security;
using DotNetNuke.Data;
using DanceWorkShop_Dnn.Dnn.SyntaxSalsa.DanceWorkShop.Models;
using DanceWorkShop_Dnn.Dnn.SyntaxSalsa.DanceWorkShop.Services;

namespace DanceWorkShop_Dnn.Dnn.SyntaxSalsa.DanceWorkShop.Controllers
{
    public class DanceWorkshopApiController : DnnApiController
    {
        private readonly IDanceWorkshopBookingManager _bookingManager;

        public DanceWorkshopApiController()
        {
            _bookingManager = new DanceWorkshopBookingManager();
        }

        [HttpGet]
        [AllowAnonymous]
        public HttpResponseMessage WorkshopsList()
        {
            try
            {
                using (IDataContext ctx = DataContext.Instance())
                {
                    var workshops = ctx.GetRepository<DanceWorkshop>().Get(ActiveModule.ModuleID);
                    return Request.CreateResponse(HttpStatusCode.OK, workshops);
                }
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public HttpResponseMessage SessionsList(int year, int week)
        {
            try
            {
                using (IDataContext ctx = DataContext.Instance())
                {
                    var sessions = ctx.GetRepository<DanceWorkshopSession>().Get();
                    return Request.CreateResponse(HttpStatusCode.OK, sessions);
                }
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.View)]
        public HttpResponseMessage GetBooking(int id)
        {
            var booking = _bookingManager.FindBookingByID(id, UserInfo.UserID, UserInfo.IsSuperUser);
            if (booking == null) return Request.CreateResponse(HttpStatusCode.NotFound);
            return Request.CreateResponse(HttpStatusCode.OK, booking);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.View)]
        public HttpResponseMessage Create(DanceWorkshopBooking booking)
        {
            try
            {
                booking.CreatedBy = UserInfo.UserID;
                var result = _bookingManager.CreateBooking(booking, 10);
                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (DanceWorkshopException ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex.Message);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.View)]
        public HttpResponseMessage Cancel(int id)
        {
            try
            {
                _bookingManager.CancelBooking(id, UserInfo.UserID, UserInfo.IsSuperUser);
                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (DanceWorkshopException ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }
    }
}