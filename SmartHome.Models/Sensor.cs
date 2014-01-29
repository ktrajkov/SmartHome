using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome.Models
{
    public class Sensor
    {
        public Sensor()
        {
            this.Temps=new HashSet<Temp>();
        }

        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]       
        public Int16 AttachedPin { get; set; }

        public double? CurrentTemp { get; set; }

        public int ArduinoArraySensorsId { get; set; }

        public bool AlarmCheck { get; set; }

        public double? MinTempAlert { get; set; }

        public double? MaxTempAlert { get; set; }

          [Required]
        public int RoomId { get; set; }
         
        public virtual Room Room { get;set; }
        public ICollection<Temp> Temps { get; set; }
    }
}