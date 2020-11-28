using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1.Models
{
    public class Orders
    {
        [Key]
        public int OrderId { get; set; }
        public string UserId { get; set; }
        public bool Finished { get; set; }
        public DateTime Creation { get; set; } 
    }
}
