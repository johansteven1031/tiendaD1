using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1.Models
{
    public class Producto
    {
        [Key]
        public int ProductId { get; set; }
        [Display(Name = "Producto")]
        [Required(ErrorMessage = "Por favor ingrese un nombre")]
        public string Description { get; set; }
        [Display(Name = "Código")]
        [Required(ErrorMessage = "Por favor ingrese un código")]
        public string Code { get; set; }
        [Display(Name = "Precio")]
        [Required(ErrorMessage = "Por favor ingrese un precio")]
        [Column(TypeName = "decimal(18,1)")]
        public decimal Cost { get; set; }
        [Display(Name = "Activo?")]
        public bool Active { get; set; }
        public string Ruta { get; set; }
        [Display(Name = "Categoría")]
        public int CategoriaId { get; set; }
        [Display(Name = "Marca")]
        public int MarcaId { get; set; }
        public Categoria categoria { get; set; }
        public Marca marca { get; set; }
    }
}
