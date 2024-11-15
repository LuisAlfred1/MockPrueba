using Microsoft.EntityFrameworkCore;
using MockPrueba.Models;
using System.Collections.Generic;

namespace MockPrueba.Data
{
    public class AppDbContext: DbContext
    {
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Producto> Productos { get; set; }
        public DbSet<Venta> Ventas { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Venta>()
                .HasOne(v => v.Cliente)  // Relación con Cliente
                .WithMany()  // Aquí especificas la relación, si el Cliente tiene muchas Ventas.
                .HasForeignKey(v => v.IdCliente);  // Clave foránea para Cliente

            modelBuilder.Entity<Venta>()
                .HasOne(v => v.Producto)  // Relación con Producto
                .WithMany()  // Similar al anterior
                .HasForeignKey(v => v.IdProducto);  // Clave foránea para Producto
        }
    }
}
