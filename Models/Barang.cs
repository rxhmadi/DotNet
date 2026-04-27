using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BarangAPI.Models
{
    public class Barang
    {
        [Key]
        public int IdBarang { get; set; }

        [Required]
        [StringLength(100)]
        public string NamaBarang { get; set; } = string.Empty;

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Harga { get; set; }

        [Required]
        public int Stok { get; set; }

        [Required]
        public int IdKategori { get; set; }

        // Concurrency Control menggunakan RowVersion
        [Timestamp]
        public byte[]? RowVersion { get; set; }
    }
}
