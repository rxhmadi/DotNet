# Sistem Web API Manajemen Barang Gudang

## Deskripsi
Web API sederhana untuk mengelola data barang di gudang menggunakan ASP.NET Core, Entity Framework Core, dan SQL Server.

## Fitur
1. ✅ CRUD lengkap untuk data Barang
2. ✅ Entity Framework Core dengan SQL Server
3. ✅ Transaction Management untuk update massal
4. ✅ Concurrency Control menggunakan RowVersion
5. ✅ RESTful API dengan format JSON

## Struktur Database
**Tabel: Barang**
- IdBarang (PK, int)
- NamaBarang (string, required)
- Harga (decimal)
- Stok (int)
- IdKategori (int)
- RowVersion (byte[], untuk concurrency control)

## Cara Menjalankan

### 1. Setup Database
Pastikan SQL Server sudah terinstall dan berjalan.

Edit connection string di `appsettings.json` sesuai konfigurasi SQL Server Anda:
```json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost;Database=GudangDB;Trusted_Connection=True;TrustServerCertificate=True;"
}
```

### 2. Jalankan Migration
```bash
dotnet ef migrations add InitialCreate
dotnet ef database update
```

### 3. Jalankan Aplikasi
```bash
dotnet run
```

API akan berjalan di: `https://localhost:5001` atau `http://localhost:5000`

## Endpoint API

### 1. GET All Barang
```
GET /api/barang
```
Response: Array of Barang objects

### 2. GET Barang by ID
```
GET /api/barang/{id}
```
Response: Single Barang object

### 3. POST Create Barang
```
POST /api/barang
Content-Type: application/json

{
  "namaBarang": "Laptop ASUS",
  "harga": 8500000,
  "stok": 10,
  "idKategori": 1
}
```

### 4. PUT Update Barang
```
PUT /api/barang/{id}
Content-Type: application/json

{
  "namaBarang": "Laptop ASUS ROG",
  "harga": 9500000,
  "stok": 8,
  "idKategori": 1
}
```

### 5. DELETE Barang
```
DELETE /api/barang/{id}
```

### 6. POST Update Massal (dengan Transaction)
```
POST /api/barang/update-massal
Content-Type: application/json

{
  "updates": [
    {
      "idBarang": 1,
      "tambahStok": 5
    },
    {
      "idBarang": 2,
      "tambahStok": 10
    }
  ]
}
```

## Penjelasan Fitur

### Transaction Management
Endpoint `update-massal` menggunakan `BeginTransaction()` untuk memastikan:
- Jika salah satu update gagal, semua perubahan di-rollback
- Jika semua berhasil, perubahan di-commit
- Atomicity terjaga dalam operasi batch

### Concurrency Control
- Menggunakan properti `RowVersion` (byte array) dengan atribut `[Timestamp]`
- Otomatis mendeteksi konflik saat dua user mengubah data yang sama
- Mengembalikan error `DbUpdateConcurrencyException` jika terjadi race condition
- Implementasi Persistence Transparency: logika database terpisah di `AppDbContext`

### Persistence Transparency
- Data access logic ada di `AppDbContext` (Data layer)
- Business logic ada di `Controller` (Application layer)
- Model terpisah di folder `Models`
- DTO terpisah di folder `DTOs`

## Testing dengan Postman
1. Import collection atau buat request manual
2. Set base URL: `https://localhost:5001`
3. Test semua endpoint sesuai contoh di atas
4. Untuk update massal, pastikan IdBarang yang digunakan sudah ada di database

## Teknologi
- ASP.NET Core 8.0
- Entity Framework Core 8.0
- SQL Server
- Swagger/OpenAPI untuk dokumentasi

## Catatan Penting
- Pastikan SQL Server berjalan sebelum menjalankan aplikasi
- Jalankan migration sebelum testing
- RowVersion akan otomatis di-generate oleh database
- Gunakan Swagger UI di `/swagger` untuk testing cepat
