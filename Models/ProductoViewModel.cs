using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1.Models
{
    public class ProductoViewModel
    {
        [Display(Name = "Producto")]
        [Required(ErrorMessage = "Por favor ingrese un nombre")]
        public string Description { get; set; }
        [Display(Name = "Código")]
        [Required(ErrorMessage = "Por favor ingrese un código")]
        public string Code { get; set; }
        [Display(Name = "Precio")]
        [Required(ErrorMessage = "Por favor ingrese un precio")]
        public decimal Cost { get; set; }
        [Display(Name = "Activo?")]
        public bool Active { get; set; }
        [Display(Name = "Categoría")]
        public int CategoriaId { get; set; }
        [Display(Name = "Marca")]
        public int MarcaId { get; set; }

        [Required(ErrorMessage = "Seleccione una imagen")]
        [Display(Name = "Imagen")]
        public IFormFile ProfileImage { get; set; }
    }
}
