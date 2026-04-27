# Cara Testing API Barang

## URL Aplikasi
**Base URL**: `http://localhost:8080`

## Testing dengan Browser (Swagger UI)

Buka browser dan akses: **http://localhost:8080/swagger**

Di Swagger UI, Anda bisa langsung test semua endpoint dengan klik "Try it out".

## Testing dengan Postman

### 1. GET - Lihat Semua Barang
```
Method: GET
URL: http://localhost:8080/api/barang
```

Response (jika masih kosong):
```json
[]
```

### 2. POST - Tambah Barang Baru
```
Method: POST
URL: http://localhost:8080/api/barang
Headers: Content-Type: application/json
Body (raw JSON):
```
```json
{
  "namaBarang": "Laptop ASUS ROG",
  "harga": 15000000,
  "stok": 5,
  "idKategori": 1
}
```

Response:
```json
{
  "idBarang": 1,
  "namaBarang": "Laptop ASUS ROG",
  "harga": 15000000,
  "stok": 5,
  "idKategori": 1,
  "rowVersion": "AAAAAAAAB9E="
}
```

### 3. GET - Lihat Barang by ID
```
Method: GET
URL: http://localhost:8080/api/barang/1
```

### 4. PUT - Update Barang
```
Method: PUT
URL: http://localhost:8080/api/barang/1
Headers: Content-Type: application/json
Body (raw JSON):
```
```json
{
  "namaBarang": "Laptop ASUS ROG Strix",
  "harga": 16000000,
  "stok": 8,
  "idKategori": 1
}
```

### 5. DELETE - Hapus Barang
```
Method: DELETE
URL: http://localhost:8080/api/barang/1
```

### 6. POST - Update Massal (dengan Transaction)
```
Method: POST
URL: http://localhost:8080/api/barang/update-massal
Headers: Content-Type: application/json
Body (raw JSON):
```
```json
{
  "updates": [
    {
      "idBarang": 1,
      "tambahStok": 10
    },
    {
      "idBarang": 2,
      "tambahStok": 5
    }
  ]
}
```

**Catatan**: Endpoint ini akan menambah stok beberapa barang sekaligus. Jika salah satu gagal (misal ID tidak ada), semua perubahan akan di-rollback.

## Contoh Skenario Testing Lengkap

### Skenario 1: CRUD Dasar
1. POST - Tambah 3 barang berbeda
2. GET - Lihat semua barang (harus ada 3)
3. GET by ID - Lihat detail barang ID 1
4. PUT - Update barang ID 1 (ubah harga dan stok)
5. DELETE - Hapus barang ID 3
6. GET - Lihat semua barang (harus ada 2)

### Skenario 2: Testing Transaction
1. POST - Tambah 2 barang (ID 1 dan 2)
2. POST update-massal - Update stok keduanya (harus berhasil)
3. GET - Verifikasi stok bertambah
4. POST update-massal - Update dengan ID yang tidak ada (harus gagal dan rollback)

### Skenario 3: Testing Concurrency
1. GET barang ID 1 di 2 tab Postman berbeda
2. Di tab 1: PUT update stok jadi 10 → Berhasil
3. Di tab 2: PUT update stok jadi 20 (tanpa refresh) → Gagal dengan error 409 Conflict
4. Refresh data di tab 2, lalu update lagi → Berhasil

## Tips
- Gunakan Swagger UI untuk testing cepat
- Gunakan Postman untuk testing yang lebih kompleks
- Perhatikan response code:
  - 200 OK = Berhasil
  - 201 Created = Data berhasil dibuat
  - 404 Not Found = Data tidak ditemukan
  - 409 Conflict = Concurrency conflict
  - 400 Bad Request = Request tidak valid

## Troubleshooting

**Q: Response `[]` kosong?**
A: Normal! Database masih kosong. Tambah data dulu dengan POST.

**Q: Error 404 di endpoint?**
A: Pastikan URL benar, harus ada `/api/barang`

**Q: Error 409 Conflict saat update?**
A: Data sudah diubah user lain. Refresh data (GET) lalu update lagi.

**Q: Swagger tidak muncul?**
A: Pastikan akses `http://localhost:8080/swagger` (dengan /swagger)
