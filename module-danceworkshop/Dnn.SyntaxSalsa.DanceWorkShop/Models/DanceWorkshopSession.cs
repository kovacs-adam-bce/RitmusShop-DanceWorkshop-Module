using DotNetNuke.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DanceWorkShop_Dnn.Dnn.SyntaxSalsa.DanceWorkShop
{
    [TableName("DanceWorkshopSessions")]
    [PrimaryKey("SessionID", AutoIncrement = true)]
    public class DanceWorkshopSession
    {
        public int SessionID { get; set; }
        public int WorkshopID { get; set; }
        public DateTime Start { get; set; }
        public int Capacity { get; set; }
    }
}
