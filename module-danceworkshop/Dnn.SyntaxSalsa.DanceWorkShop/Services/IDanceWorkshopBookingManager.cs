using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DanceWorkShop_Dnn.Dnn.SyntaxSalsa.DanceWorkShop.Services
{
    public interface IDanceWorkshopBookingManager
    {

        DanceWorkshopBooking FindBookingByID(int bookingID, int currentUserID, bool isAdmin);

        IEnumerable<DanceWorkshopBooking> FindBookingsByParticipant(int participantID, DateTime fromDate, DateTime toDate);

        IEnumerable<DanceWorkshopBooking> FindBookingsByDate(DateTime fromDate, DateTime toDate, bool findAll);

        DanceWorkshopBooking CreateBooking(DanceWorkshopBooking booking, int capacity);

        void CancelBooking(int bookingID, int currentUserID, bool isAdmin);

        bool IsSlotAvailable(int sessionID, int capacity);
    }
}
