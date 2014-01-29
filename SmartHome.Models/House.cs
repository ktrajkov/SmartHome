using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome.Models
{
    public class House
    {
        public House()
        {
            this.Floоrs = new HashSet<Floor>();
        }

        public int Id { get; set; }
        public int Idd { get; set; }

        public string Name { get; set; }
     
        public string Address { get; set; }

        public Nullable<System.DateTime> DateCreate { get; set; }

        public string ImageUrl { get; set; }
       
        public string SecretKey { get; set; }
        
        public string ServerIp { get; set; }

        public int ServerPort { get; set; }

        public string ReceiverIp { get; set; }
   
        public int TimeToCheckTemp { get; set; }
     
        public int TimeToSendTemp { get; set; }

        public Int16 MaxNumberPins { get; set; }

        public byte MaxArraySizeTermostats { get; set; }
     
        public byte MaxArraySizeSensors { get; set; }

        public string UserId { get; set; }


        public virtual ApplicationUser User { get; set; }

        public virtual ICollection<Floor> Floоrs { get; set; }
    }
}