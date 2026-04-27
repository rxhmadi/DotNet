# Penjelasan Concurrency Control & Transparency

## 1. Concurrency Control dengan RowVersion

### Implementasi
Pada model `Barang`, terdapat properti:
```csharp
[Timestamp]
public byte[]? RowVersion { get; set; }
```

Properti ini berfungsi sebagai **Optimistic Concurrency Control**.

### Cara Kerja
1. Setiap kali record di-update, SQL Server otomatis mengubah nilai `RowVersion`
2. Entity Framework membandingkan `RowVersion` saat update dengan nilai di database
3. Jika berbeda (artinya ada user lain yang sudah mengubah), maka akan terjadi `DbUpdateConcurrencyException`

### Simulasi Race Condition

**Skenario:**
- Admin A dan Admin B membuka data Barang ID 1 (Stok = 10) di waktu yang sama
- Admin A mengubah stok menjadi 15 dan menyimpan → **BERHASIL**
- Admin B (masih melihat stok 10) mengubah stok menjadi 20 dan menyimpan → **GAGAL**

**Yang Terjadi:**
1. Saat Admin A menyimpan, `RowVersion` berubah dari `0x00000001` menjadi `0x00000002`
2. Saat Admin B mencoba menyimpan, EF Core mendeteksi `RowVersion` yang dikirim (`0x00000001`) tidak sama dengan yang di database (`0x00000002`)
3. Exception `DbUpdateConcurrencyException` dilempar
4. Controller menangkap exception dan mengembalikan response:
   ```json
   {
     "message": "Data telah diubah oleh pengguna lain. Silakan refresh dan coba lagi."
   }
   ```

### Kode Penanganan di Controller
```csharp
try
{
    await _context.SaveChangesAsync();
}
catch (DbUpdateConcurrencyException)
{
    return Conflict(new { message = "Data telah diubah oleh pengguna lain. Silakan refresh dan coba lagi." });
}
```

## 2. Transaction Management

### Implementasi di Endpoint Update Massal
```csharp
using var transaction = await _context.Database.BeginTransactionAsync();

try
{
    // Proses update multiple barang
    foreach (var update in request.Updates)
    {
        // Update stok
        // Jika ada error, throw exception
    }
    
    await _context.SaveChangesAsync();
    await transaction.CommitAsync(); // Semua berhasil
}
catch (Exception ex)
{
    await transaction.RollbackAsync(); // Ada yang gagal, batalkan semua
    return BadRequest(new { message = "Update massal gagal", error = ex.Message });
}
```

### Manfaat Transaction
- **Atomicity**: Semua update berhasil atau semua dibatalkan
- **Consistency**: Database tetap konsisten
- **Isolation**: Perubahan tidak terlihat sampai di-commit
- **Durability**: Setelah commit, data permanen tersimpan

## 3. Persistence Transparency

### Prinsip
Logika akses database terpisah dari logika bisnis.

### Implementasi dalam Project

#### Layer 1: Data Access (AppDbContext.cs)
```csharp
public class AppDbContext : DbContext
{
    public DbSet<Barang> Barang { get; set; }
    // Konfigurasi database mapping
}
```
**Tanggung Jawab:** Mengelola koneksi database, mapping entity ke tabel

#### Layer 2: Model (Barang.cs)
```csharp
public class Barang
{
    public int IdBarang { get; set; }
    public string NamaBarang { get; set; }
    // ... properties lainnya
}
```
**Tanggung Jawab:** Representasi struktur data

#### Layer 3: DTO (BarangDto.cs)
```csharp
public class BarangCreateDto
{
    public string NamaBarang { get; set; }
    // ... properties untuk input
}
```
**Tanggung Jawab:** Data transfer antara client dan server

#### Layer 4: Business Logic (BarangController.cs)
```csharp
public class BarangController : ControllerBase
{
    private readonly AppDbContext _context;
    
    public async Task<ActionResult> CreateBarang(BarangCreateDto dto)
    {
        // Logika bisnis: validasi, transformasi, dll
        var barang = new Barang { ... };
        _context.Barang.Add(barang);
        await _context.SaveChangesAsync();
    }
}
```
**Tanggung Jawab:** Logika bisnis, validasi, orchestration

### Keuntungan Persistence Transparency
1. **Maintainability**: Mudah diubah tanpa mempengaruhi layer lain
2. **Testability**: Bisa mock `AppDbContext` untuk unit testing
3. **Flexibility**: Bisa ganti database (SQL Server → PostgreSQL) tanpa ubah controller
4. **Separation of Concerns**: Setiap layer punya tanggung jawab jelas

## Testing Concurrency

### Cara Test dengan Postman

1. **Request 1 - Admin A GET data:**
   ```
   GET /api/barang/1
   ```
   Response: `{ "idBarang": 1, "stok": 10, "rowVersion": "AAAAAAAAB9E=" }`

2. **Request 2 - Admin B GET data (waktu bersamaan):**
   ```
   GET /api/barang/1
   ```
   Response: `{ "idBarang": 1, "stok": 10, "rowVersion": "AAAAAAAAB9E=" }`

3. **Request 3 - Admin A UPDATE:**
   ```
   PUT /api/barang/1
   { "namaBarang": "Laptop", "harga": 8000000, "stok": 15, "idKategori": 1 }
   ```
   Response: `200 OK` → RowVersion berubah

4. **Request 4 - Admin B UPDATE (dengan data lama):**
   ```
   PUT /api/barang/1
   { "namaBarang": "Laptop", "harga": 8000000, "stok": 20, "idKategori": 1 }
   ```
   Response: `409 Conflict` → "Data telah diubah oleh pengguna lain"

## Kesimpulan

Sistem ini mengimplementasikan:
- ✅ **Concurrency Control** dengan RowVersion untuk mencegah lost update
- ✅ **Transaction Management** untuk operasi batch yang atomic
- ✅ **Persistence Transparency** dengan pemisahan layer yang jelas
- ✅ **Race Condition Handling** dengan optimistic locking
