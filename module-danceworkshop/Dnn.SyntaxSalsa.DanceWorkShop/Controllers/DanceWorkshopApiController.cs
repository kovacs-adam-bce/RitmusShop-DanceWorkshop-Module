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
    [SupportedModules("Dnn.SyntaxSalsa.DanceWorkShop")]
    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.View)]
    public class DanceWorkshopApiController : DnnApiController
    {
        private readonly IDanceWorkshopBookingManager _bookingManager;

        public DanceWorkshopApiController()
        {
            _bookingManager = new DanceWorkshopBookingManager();
        }

        [HttpGet]
        [AllowAnonymous]
        [ActionName("WorkshopsList")]
        public HttpResponseMessage WorkshopsList()
        {
            try
            {
                using (IDataContext ctx = DataContext.Instance())
                {
                    var workshops = ctx.GetRepository<DanceWorkshop>().Get(ActiveModule.ModuleID).ToList();
                    return Request.CreateResponse(HttpStatusCode.OK, workshops);
                }
            }
            catch (Exception ex) { return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message); }
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
                    var workshops = ctx.GetRepository<DanceWorkshop>().Get().ToList();

                    // JAVÍTÁS: Megkeressük a bejelentkezett felhasználóhoz tartozó résztvevő ID-t az email címe alapján
                    int currentParticipantId = -1;
                    if (UserInfo.UserID > 0)
                    {
                        var p = ctx.GetRepository<DanceWorkshopParticipant>().Find("WHERE ParticipantMail = @0", UserInfo.Email).FirstOrDefault();
                        if (p != null) currentParticipantId = p.ParticipantID;
                    }

                    var result = sessions.Select(s => {
                        var ws = workshops.FirstOrDefault(w => w.WorkshopID == s.WorkshopID);

                        // JAVÍTÁS: A naptár akkor jelzi sajátnak, ha a résztvevő ID-ja egyezik
                        var userBooking = bookings.FirstOrDefault(b => b.SessionID == s.SessionID && b.CreatedBy == currentParticipantId);

                        return new
                        {
                            s.SessionID,
                            s.WorkshopID,
                            WorkshopName = ws?.Name ?? "Workshop",
                            WorkshopLevel = ws?.Level ?? "",
                            s.Start,
                            s.Capacity,
                            IsFull = bookings.Count(b => b.SessionID == s.SessionID) >= s.Capacity,
                            UserBookingID = userBooking?.BookingID ?? 0
                        };
                    });
                    return Request.CreateResponse(HttpStatusCode.OK, result);
                }
            }
            catch (Exception ex) { return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message); }
        }

        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        [ActionName("GetAllBookings")]
        public HttpResponseMessage GetAllBookings()
        {
            try
            {
                using (IDataContext ctx = DataContext.Instance())
                {
                    var bookings = ctx.GetRepository<DanceWorkshopBooking>().Get().ToList();
                    var sessions = ctx.GetRepository<DanceWorkshopSession>().Get().ToList();
                    var workshops = ctx.GetRepository<DanceWorkshop>().Get().ToList();
                    var participants = ctx.GetRepository<DanceWorkshopParticipant>().Get().ToList();

                    var result = bookings.Select(b => {
                        var session = sessions.FirstOrDefault(s => s.SessionID == b.SessionID);
                        var workshop = workshops.FirstOrDefault(w => w.WorkshopID == (session?.WorkshopID ?? 0));
                        var person = participants.FirstOrDefault(p => p.ParticipantID == b.CreatedBy);

                        return new
                        {
                            b.BookingID,
                            b.IsCancelled,
                            SessionStart = session?.Start,
                            WorkshopName = (workshop?.Name ?? "Ismeretlen") + (workshop != null && !string.IsNullOrEmpty(workshop.Level) ? " (" + workshop.Level + ")" : ""),
                            Username = person != null ? person.ParticipantName : "Vendég"
                        };
                    }).OrderByDescending(x => x.SessionStart).ToList();

                    return Request.CreateResponse(HttpStatusCode.OK, result);
                }
            }
            catch (Exception ex) { return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message); }
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
                    CreatedBy = participantId,
                    CreatedAt = DateTime.UtcNow
                };
                _bookingManager.CreateBooking(booking, 10);
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true });
            }
            catch (Exception ex) { return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message); }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("CancelBooking")]
        public HttpResponseMessage CancelBooking([FromUri] int id)
        {
            try
            {
                // JAVÍTÁS: 'id' itt a BookingID, így a Guest/User is le tudja mondani a sajátját
                _bookingManager.CancelBooking(id, UserInfo.UserID, UserInfo.IsSuperUser);
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true });
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { message = ex.Message });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        [ActionName("SaveWorkshop")]
        public HttpResponseMessage SaveWorkshop(DanceWorkshop workshop)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<DanceWorkshop>();
                if (workshop.WorkshopID > 0) rep.Update(workshop);
                else { workshop.ModuleId = ActiveModule.ModuleID; rep.Insert(workshop); }
                return Request.CreateResponse(HttpStatusCode.OK, workshop);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        [ActionName("SaveSession")]
        public HttpResponseMessage SaveSession(DanceWorkshopSession session)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<DanceWorkshopSession>();
                if (session.SessionID > 0) rep.Update(session);
                else rep.Insert(session);
                return Request.CreateResponse(HttpStatusCode.OK, session);
            }
        }

        private int EnsureParticipant(string name, string email, string phone)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<DanceWorkshopParticipant>();
                var participant = rep.Find("WHERE ParticipantMail = @0", email).FirstOrDefault();
                if (participant == null)
                {
                    participant = new DanceWorkshopParticipant { ParticipantName = name, ParticipantMail = email, ParticipantPhone = phone };
                    rep.Insert(participant);
                }
                else
                {
                    // JAVÍTÁS: Ha a név változott az adott emailhez, frissítjük
                    participant.ParticipantName = name;
                    participant.ParticipantPhone = phone;
                    rep.Update(participant);
                }
                return participant.ParticipantID;
            }
        }
    }

    // JAVÍTÁS: Ez az osztály KÖTELEZŐ a namespace-en belül, a hibaüzenet elkerüléséhez!
    public class BookingRequest
    {
        public int SessionID { get; set; }
        public string ParticipantName { get; set; }
        public string ParticipantMail { get; set; }
        public string ParticipantPhone { get; set; }
    }
}