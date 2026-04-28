using DanceWorkShop_Dnn.Dnn.SyntaxSalsa.DanceWorkShop.Models;
using DanceWorkShop_Dnn.Dnn.SyntaxSalsa.DanceWorkShop.Services;
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
    [SupportedModules("DanceWorkShop")]
    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.View)]
    public class DanceWorkshopApiController : DnnApiController
    {
        private readonly IDanceWorkshopBookingManager _bookingManager;

        public DanceWorkshopApiController()
        {
            _bookingManager = new DanceWorkshopBookingManager();
        }

        // --- NAPTÁR ÉS FOGLALÁS FUNKCIÓK ---

        [HttpGet]
        [AllowAnonymous]
        [ActionName("WorkshopsList")]
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
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { message = ex.Message });
            }
        }

        [HttpGet]
        [AllowAnonymous]
        [ActionName("SessionsList")]
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
                        IsFull = bookings.Count(b => b.SessionID == s.SessionID) >= s.Capacity,
                        // Itt nézzük meg, hogy a bejelentkezett felhasználónak van-e foglalása
                        UserBookingID = bookings.FirstOrDefault(b => b.SessionID == s.SessionID && b.CreatedBy == UserInfo.UserID)?.BookingID ?? 0
                    });

                    return Request.CreateResponse(HttpStatusCode.OK, result);
                }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { message = ex.Message });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("CreateBooking")]
        public HttpResponseMessage CreateBooking(BookingRequest request)
        {
            try
            {
                int participantId = EnsureParticipant(request.ParticipantName, request.ParticipantMail, request.ParticipantPhone);

                var booking = new DanceWorkshopBooking
                {
                    SessionID = request.SessionID,
                    CreatedBy = UserInfo.UserID > 0 ? UserInfo.UserID : participantId, // Ha be van jelentkezve, a UserID-t mentjük
                    CreatedAt = DateTime.UtcNow
                };

                var result = _bookingManager.CreateBooking(booking, 10);
                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (DanceWorkshopException ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { message = "Hiba: " + ex.Message });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("CancelBooking")]
        public HttpResponseMessage CancelBooking(int id)
        {
            try
            {
                _bookingManager.CancelBooking(id, UserInfo.UserID, UserInfo.IsSuperUser);
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true });
            }
            catch (DanceWorkshopException ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { message = ex.Message });
            }
        }

        // --- MANAGEMENT / ADMIN FUNKCIÓK (EZ HIÁNYZOTT!) ---

        [HttpPost]
        [ValidateAntiForgeryToken]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        [ActionName("SaveWorkshop")]
        public HttpResponseMessage SaveWorkshop(DanceWorkshop workshop)
        {
            try
            {
                using (IDataContext ctx = DataContext.Instance())
                {
                    var rep = ctx.GetRepository<DanceWorkshop>();
                    if (workshop.WorkshopID > 0) { rep.Update(workshop); }
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
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { message = ex.Message });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        [ActionName("SaveSession")]
        public HttpResponseMessage SaveSession(DanceWorkshopSession session)
        {
            try
            {
                using (IDataContext ctx = DataContext.Instance())
                {
                    var rep = ctx.GetRepository<DanceWorkshopSession>();
                    if (session.SessionID > 0) { rep.Update(session); }
                    else { rep.Insert(session); }
                    return Request.CreateResponse(HttpStatusCode.OK, session);
                }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { message = ex.Message });
            }
        }

        // --- SEGÉDMETÓDUSOK ---

        private int EnsureParticipant(string name, string email, string phone)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<DanceWorkshopParticipant>();
                var participant = rep.Find("WHERE ParticipantMail = @0", email).FirstOrDefault();

                if (participant == null)
                {
                    participant = new DanceWorkshopParticipant
                    {
                        ParticipantName = name,
                        ParticipantMail = email,
                        ParticipantPhone = phone
                    };
                    rep.Insert(participant);
                }
                return participant.ParticipantID;
            }
        }
    }

    public class BookingRequest
    {
        public int SessionID { get; set; }
        public string ParticipantName { get; set; }
        public string ParticipantMail { get; set; }
        public string ParticipantPhone { get; set; }
    }
}