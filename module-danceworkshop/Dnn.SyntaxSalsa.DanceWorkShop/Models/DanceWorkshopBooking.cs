using DotNetNuke.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DanceWorkShop_Dnn.Dnn.SyntaxSalsa.DanceWorkShop
{
    [TableName("DanceWorkshopBookings")]
    [PrimaryKey("BookingID", AutoIncrement = true)]
    public class DanceWorkshopBooking
    {
        public int BookingID { get; set; }
        public int CreatedBy { get; set; }
        public int SessionID { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsCancelled { get; set; }
               
        [IgnoreColumn]

        public DanceWorkshopParticipant Participant { get; set; }
    }
}
