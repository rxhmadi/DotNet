using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BarangAPI.Data;
using BarangAPI.Models;
using BarangAPI.DTOs;

namespace BarangAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BarangController : ControllerBase
    {
        private readonly AppDbContext _context;

        public BarangController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/barang
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Barang>>> GetAllBarang()
        {
            var barangList = await _context.Barang.ToListAsync();
            return Ok(barangList);
        }

        // GET: api/barang/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Barang>> GetBarangById(int id)
        {
            var barang = await _context.Barang.FindAsync(id);

            if (barang == null)
            {
                return NotFound(new { message = $"Barang dengan ID {id} tidak ditemukan" });
            }

            return Ok(barang);
        }

        // POST: api/barang
        [HttpPost]
        public async Task<ActionResult<Barang>> CreateBarang(BarangCreateDto barangDto)
        {
            var barang = new Barang
            {
                NamaBarang = barangDto.NamaBarang,
                Harga = barangDto.Harga,
                Stok = barangDto.Stok,
                IdKategori = barangDto.IdKategori
            };

            _context.Barang.Add(barang);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetBarangById), new { id = barang.IdBarang }, barang);
        }

        // PUT: api/barang/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBarang(int id, BarangUpdateDto barangDto)
        {
            var barang = await _context.Barang.FindAsync(id);

            if (barang == null)
            {
                return NotFound(new { message = $"Barang dengan ID {id} tidak ditemukan" });
            }

            // Update properties
            barang.NamaBarang = barangDto.NamaBarang;
            barang.Harga = barangDto.Harga;
            barang.Stok = barangDto.Stok;
            barang.IdKategori = barangDto.IdKategori;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return Conflict(new { message = "Data telah diubah oleh pengguna lain. Silakan refresh dan coba lagi." });
            }

            return Ok(new { message = "Barang berhasil diupdate", data = barang });
        }

        // DELETE: api/barang/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBarang(int id)
        {
            var barang = await _context.Barang.FindAsync(id);

            if (barang == null)
            {
                return NotFound(new { message = $"Barang dengan ID {id} tidak ditemukan" });
            }

            _context.Barang.Remove(barang);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Barang berhasil dihapus" });
        }

        // POST: api/barang/update-massal
        [HttpPost("update-massal")]
        public async Task<IActionResult> UpdateMassal(UpdateMassalRequestDto request)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                foreach (var update in request.Updates)
                {
                    var barang = await _context.Barang.FindAsync(update.IdBarang);

                    if (barang == null)
                    {
                        throw new Exception($"Barang dengan ID {update.IdBarang} tidak ditemukan");
                    }

                    barang.Stok += update.TambahStok;

                    if (barang.Stok < 0)
                    {
                        throw new Exception($"Stok barang {barang.NamaBarang} tidak boleh negatif");
                    }
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return Ok(new { message = "Update massal berhasil dilakukan" });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return BadRequest(new { message = "Update massal gagal", error = ex.Message });
            }
        }
    }
}
