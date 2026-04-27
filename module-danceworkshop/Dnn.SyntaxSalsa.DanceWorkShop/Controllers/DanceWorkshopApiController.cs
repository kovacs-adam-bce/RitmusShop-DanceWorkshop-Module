using DanceWorkShop_Dnn.Dnn.SyntaxSalsa.DanceWorkShop.Models;
using DanceWorkShop_Dnn.Dnn.SyntaxSalsa.DanceWorkShop.Services;
using DotNetNuke.Collections;
using DotNetNuke.Data;
using DotNetNuke.Security;
using DotNetNuke.Web.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

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
                    var sessions = ctx.GetRepository<DanceWorkshopSession>().Get().ToList();
                    var bookings = ctx.GetRepository<DanceWorkshopBooking>().Find("WHERE IsCancelled = 0").ToList();

                    var result = sessions.Select(s => new {
                        s.SessionID,
                        s.WorkshopID,
                        s.Start,
                        s.Capacity,
                        IsFull = bookings.Count(b => b.SessionID == s.SessionID) >= s.Capacity
                    });

                    return Request.CreateResponse(HttpStatusCode.OK, result);
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

                var adminEmail = ActiveModule.ModuleSettings.GetValueOrDefault("DanceWorkshop_AdminEmail", "");
                if (!string.IsNullOrWhiteSpace(adminEmail))
                {
                    using (IDataContext ctx = DataContext.Instance())
                    {
                        var session = ctx.GetRepository<DanceWorkshopSession>().GetById(booking.SessionID);
                        if (session != null)
                        {
                            var workshop = ctx.GetRepository<DanceWorkshop>().GetById(session.WorkshopID);
                            var wsName = workshop != null ? workshop.Name : "Ismeretlen workshop";

                            var notificationService = new DanceWorkshopNotificationService();
                            notificationService.SendBookingNotification(adminEmail, booking, wsName, UserInfo.DisplayName);
                        }
                    }
                }

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

        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        public HttpResponseMessage GetAllBookings(DateTime? fromDate = null, DateTime? toDate = null)
        {
            try
            {
                var start = fromDate ?? DateTime.Now.AddDays(-30);
                var end = toDate ?? DateTime.Now.AddDays(90);

                var bookings = _bookingManager.FindBookingsByDate(start, end, true);

                using (IDataContext ctx = DataContext.Instance())
                {
                    var sessions = ctx.GetRepository<DanceWorkshopSession>().Get().ToList();
                    var workshops = ctx.GetRepository<DanceWorkshop>().Get(ActiveModule.ModuleID).ToList();

                    var result = bookings.Select(b => new
                    {
                        b.BookingID,
                        b.CreatedAt,
                        b.IsCancelled,
                        SessionStart = sessions.FirstOrDefault(s => s.SessionID == b.SessionID)?.Start,
                        WorkshopName = workshops.FirstOrDefault(w => w.WorkshopID == (sessions.FirstOrDefault(s => s.SessionID == b.SessionID)?.WorkshopID ?? 0))?.Name,
                        Username = UserInfo.Username
                    });

                    return Request.CreateResponse(HttpStatusCode.OK, result);
                }
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        public HttpResponseMessage SaveWorkshop(DanceWorkshop workshop)
        {
            try
            {
                using (IDataContext ctx = DataContext.Instance())
                {
                    var rep = ctx.GetRepository<DanceWorkshop>();
                    if (workshop.WorkshopID > 0)
                    {
                        rep.Update(workshop);
                    }
                    else
                    {
                        workshop.ModuleId = ActiveModule.ModuleID;
                        rep.Insert(workshop);
                    }
                    return Request.CreateResponse(HttpStatusCode.OK, workshop);
                }
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        public HttpResponseMessage SaveSession(DanceWorkshopSession session)
        {
            try
            {
                using (IDataContext ctx = DataContext.Instance())
                {
                    var rep = ctx.GetRepository<DanceWorkshopSession>();
                    if (session.SessionID > 0)
                    {
                        rep.Update(session);
                    }
                    else
                    {
                        rep.Insert(session);
                    }
                    return Request.CreateResponse(HttpStatusCode.OK, session);
                }
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

    }
}