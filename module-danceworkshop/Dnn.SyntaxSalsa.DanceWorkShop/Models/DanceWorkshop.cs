using DotNetNuke.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DanceWorkShop_Dnn.Dnn.SyntaxSalsa.DanceWorkShop
{
    [TableName("DanceWorkshops")]
    [PrimaryKey("WorkshopID", AutoIncrement = true)]
    [Scope("ModuleId")]
    public class DanceWorkshop
    {
        public int WorkshopID { get; set; }
        public int ModuleId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Price { get; set; }
        public string Level { get; set; }
    }
}
