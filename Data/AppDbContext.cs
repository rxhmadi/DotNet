using Microsoft.EntityFrameworkCore;
using BarangAPI.Models;

namespace BarangAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Barang> Barang { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Konfigurasi tambahan untuk tabel Barang
            modelBuilder.Entity<Barang>(entity =>
            {
                entity.HasKey(e => e.IdBarang);
                entity.Property(e => e.NamaBarang).IsRequired();
                entity.Property(e => e.Harga).HasColumnType("decimal(18,2)");
                entity.Property(e => e.RowVersion).IsRowVersion();
            });
        }
    }
}
