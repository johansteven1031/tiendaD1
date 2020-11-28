using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Models;

namespace WebApplication1.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<WebApplication1.Models.Producto> Producto { get; set; }
        public DbSet<WebApplication1.Models.Orders> Orders { get; set; }
        public DbSet<WebApplication1.Models.OrderDetails> OrderDetails { get; set; }
        public DbSet<WebApplication1.Models.Categoria> Categoria { get; set; }
        public DbSet<WebApplication1.Models.Marca> Marca { get; set; }
    }
}
