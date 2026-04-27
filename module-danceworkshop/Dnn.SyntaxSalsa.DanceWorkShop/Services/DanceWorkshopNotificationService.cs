using DotNetNuke.Services.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DanceWorkShop_Dnn.Dnn.SyntaxSalsa.DanceWorkShop.Services
{
    public class DanceWorkshopNotificationService
    {
        public void SendBookingNotification(string adminEmail, DanceWorkshopBooking booking, string workshopName, string username)
        {
            if (string.IsNullOrWhiteSpace(adminEmail)) return;

            var subject = "Új workshop foglalás érkezett";
            var body = $@"<h3>Új foglalás történt a naptárban</h3>
                         <p><b>Workshop:</b> {workshopName}</p>
                         <p><b>Résztvevő:</b> {username}</p>
                         <p><b>Rögzítve:</b> {booking.CreatedAt.ToString("yyyy-MM-dd HH:mm")}</p>
                         <hr/>
                         <p>Ez egy automatikus üzenet a DanceWorkshop modulból.</p>";

            Mail.SendEmail(adminEmail, adminEmail, subject, body);
        }
    }
}
