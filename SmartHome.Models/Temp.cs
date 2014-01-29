using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome.Models
{
    public class Temp
    {
        public int Id { get; set; }

        [Required]
        public System.DateTime DateTimeUpdate { get; set; }

        [Required]
        public double Temperature { get; set; }

        public Nullable<int> SensorId { get; set; }

        [Required]
        public virtual Sensor Sensor { get; set; }
    }
}
