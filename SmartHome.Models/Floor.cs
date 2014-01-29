using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome.Models
{
    public class Floor
    {
        public Floor()
        {
            this.Rooms = new HashSet<Room>();
        }
        public int Id { get; set; }        
        public string Name { get; set; }      
        public int HouseId { get; set; }
        public virtual House House { get; set; }

        public virtual ICollection<Room> Rooms { get; set; }
    }
}