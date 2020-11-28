using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1.Models
{
    public class OrderDetails
    {
        [Key]
        public int OrderDetailId { get; set; }
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        [Range(1, Int32.MaxValue, ErrorMessage = "Ingresar solo valores positivos")]
        public int Quantity { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal Cost { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal SubTotal { get; set; }
        public DateTime Creation { get; set; }
        public Producto Product { get; set; }
        public Orders Order { get; set; }
    }
}
