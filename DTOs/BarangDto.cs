namespace BarangAPI.DTOs
{
    public class BarangCreateDto
    {
        public string NamaBarang { get; set; } = string.Empty;
        public decimal Harga { get; set; }
        public int Stok { get; set; }
        public int IdKategori { get; set; }
    }

    public class BarangUpdateDto
    {
        public string NamaBarang { get; set; } = string.Empty;
        public decimal Harga { get; set; }
        public int Stok { get; set; }
        public int IdKategori { get; set; }
    }

    public class UpdateMassalDto
    {
        public int IdBarang { get; set; }
        public int TambahStok { get; set; }
    }

    public class UpdateMassalRequestDto
    {
        public List<UpdateMassalDto> Updates { get; set; } = new List<UpdateMassalDto>();
    }
}
