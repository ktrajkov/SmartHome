using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome.Models
{
    public class Room
    {
        public Room()
        {
            this.Devices = new HashSet<Device>();           
        }

        public int Id { get; set; }

        [Required(ErrorMessage = "Field is required and should not be more than 30 characters")]
        [StringLength(30)]
        public string Name { get; set; }

        [Required]
        public int FloorId { get; set; }
      

        public int? SensorId { get; set; }

        public int? ThermostatId { get; set; }      

        public virtual Floor Floor { get; set; }

   
        public virtual Sensor Sensor { get; set; }

     
        public virtual Thermostat Thermostat { get; set; }

        public virtual ICollection<Device> Devices { get; set; }


     

      
    }
}