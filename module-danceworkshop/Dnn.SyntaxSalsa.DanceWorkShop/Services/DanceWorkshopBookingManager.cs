using DotNetNuke.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DanceWorkShop_Dnn.Dnn.SyntaxSalsa.DanceWorkShop.Services
{
    public class DanceWorkshopBookingManager : IDanceWorkshopBookingManager
    {
        public DanceWorkshopBooking FindBookingByID(int bookingID, int currentUserID, bool isAdmin)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<DanceWorkshopBooking>();
                var booking = rep.GetById(bookingID);
                if (booking != null && (booking.CreatedBy == currentUserID || isAdmin))
                {
                    return booking;
                }
                return null;
            }
        }

        public DanceWorkshopBooking CreateBooking(DanceWorkshopBooking booking, int capacity)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                if (!IsSlotAvailable(booking.SessionID, capacity))
                {
                    throw new DanceWorkshopException("Timeslot unavailable.");
                }
                var session = ctx.GetRepository<DanceWorkshopSession>().GetById(booking.SessionID);
                if (session.Start.ToUniversalTime() - DateTime.UtcNow < TimeSpan.FromHours(24))
                {
                    throw new DanceWorkshopException("Cannot book before minimum scheduling treshold.");
                }
                ctx.GetRepository<DanceWorkshopBooking>().Insert(booking);
                return booking;
            }
        }

        public bool IsSlotAvailable(int sessionID, int capacity)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var bookings = ctx.GetRepository<DanceWorkshopBooking>()
                                  .Find("WHERE SessionID = @0 AND IsCancelled = 0", sessionID);
                return bookings.Count() < capacity;
            }
        }

        public void CancelBooking(int bookingID, int currentUserID, bool isAdmin)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<DanceWorkshopBooking>();
                var booking = rep.GetById(bookingID);
                if (booking == null) return;
                if (booking.CreatedBy != currentUserID && !isAdmin)
                {
                    throw new DanceWorkshopException("Permission denied.");
                }
                if (!booking.IsCancelled)
                {
                    booking.IsCancelled = true;
                    rep.Update(booking);
                }
            }
        }

        public IEnumerable<DanceWorkshopBooking> FindBookingsByParticipant(int participantID, DateTime fromDate, DateTime toDate)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                return ctx.GetRepository<DanceWorkshopBooking>()
                          .Find("WHERE CreatedBy = @0 AND IsCancelled = 0", participantID)
                          .Where(b => b.CreatedAt >= fromDate && b.CreatedAt <= toDate);
            }
        }

        public IEnumerable<DanceWorkshopBooking> FindBookingsByDate(DateTime fromDate, DateTime toDate, bool findAll)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var bookings = ctx.GetRepository<DanceWorkshopBooking>().Get();
                if (!findAll) bookings = bookings.Where(b => !b.IsCancelled);
                return bookings;
            }
        }
    }
}