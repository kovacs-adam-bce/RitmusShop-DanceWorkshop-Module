using DotNetNuke.Data;
using System;
using System.Collections.Generic;
using System.Linq;

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


                if (booking == null || (booking.CreatedBy != currentUserID && !isAdmin))
                {
                    return null;
                }

                return booking;
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

                var sessionRep = ctx.GetRepository<DanceWorkshopSession>();
                var session = sessionRep.GetById(booking.SessionID);

                if (session == null || (session.Start.ToUniversalTime() - DateTime.UtcNow).TotalHours < 24)
                {
                    throw new DanceWorkshopException("Cannot book before minimum scheduling treshold.");
                }

                booking.CreatedAt = DateTime.UtcNow;
                booking.IsCancelled = false;

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

                return ctx.ExecuteQuery<DanceWorkshopBooking>(
                    System.Data.CommandType.Text,
                    "SELECT b.* FROM DanceWorkshopBookings b " +
                    "INNER JOIN DanceWorkshopSessions s ON b.SessionID = s.SessionID " +
                    "WHERE b.CreatedBy = @0 AND b.IsCancelled = 0 AND s.Start >= @1 AND s.Start <= @2",
                    participantID, fromDate.ToUniversalTime(), toDate.ToUniversalTime());
            }
        }

        public IEnumerable<DanceWorkshopBooking> FindBookingsByDate(DateTime fromDate, DateTime toDate, bool findAll)
        {
            using (IDataContext ctx = DataContext.Instance())
            {

                string sql = "SELECT b.* FROM DanceWorkshopBookings b " +
                             "INNER JOIN DanceWorkshopSessions s ON b.SessionID = s.SessionID " +
                             "WHERE s.Start >= @0 AND s.Start <= @1";


                if (!findAll) sql += " AND b.IsCancelled = 0";

                return ctx.ExecuteQuery<DanceWorkshopBooking>
                    (
                    System.Data.CommandType.Text,
                    sql,
                    fromDate.ToUniversalTime(),
                    toDate.ToUniversalTime());
            }
        }
    }
}