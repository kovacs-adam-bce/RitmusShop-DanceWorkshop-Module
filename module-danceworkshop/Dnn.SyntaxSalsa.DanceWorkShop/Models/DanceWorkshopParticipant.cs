using DotNetNuke.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DanceWorkShop_Dnn.Dnn.SyntaxSalsa.DanceWorkShop
{
    [TableName("DanceWorkshopParticipants")]
    [PrimaryKey("ParticipantID", AutoIncrement = true)]
    public class DanceWorkshopParticipant
    {
        public int ParticipantID { get; set; }
        public string ParticipantName { get; set; }
        public string ParticipantMail { get; set; }
        public string ParticipantPhone { get; set; }
    }
}
