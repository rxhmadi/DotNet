# 📖 PANDUAN TESTING LENGKAP - API Barang Gudang

## 🎯 Tujuan Testing
Memastikan semua fitur API berjalan sesuai requirement tugas:
1. ✅ CRUD lengkap (GET, POST, PUT, DELETE)
2. ✅ Transaction dengan rollback
3. ✅ Concurrency control dengan RowVersion

---

## 🚀 LANGKAH 1: Persiapan

### Pastikan Aplikasi Berjalan
1. Buka terminal/command prompt
2. Masuk ke folder project: `cd D:\febri2`
3. Jalankan: `dotnet run --urls "http://localhost:8080"`
4. Tunggu sampai muncul: `Now listening on: http://localhost:8080`
5. Buka browser: **http://localhost:8080** (akan redirect ke Swagger)

---

## 🧪 LANGKAH 2: Testing CRUD (45 Poin)

### A. POST - Tambah Barang Baru ✅

**Di Swagger UI:**
1. Cari **POST /api/barang** (kotak hijau)
2. Klik untuk expand
3. Klik tombol **"Try it out"**
4. Copy-paste JSON ini ke kotak Request body:

```json
{
  "namaBarang": "Laptop ASUS ROG",
  "harga": 15000000,
  "stok": 10,
  "idKategori": 1
}
```

5. Klik **"Execute"**
6. **Hasil yang diharapkan:**
   - Response Code: **201 Created**
   - Response Body berisi data barang dengan `idBarang: 1`

**Screenshot/Catat:** Response body untuk laporan

---

### B. POST - Tambah Barang Kedua ✅

Ulangi langkah A dengan data berbeda:

```json
{
  "namaBarang": "Mouse Gaming Logitech",
  "harga": 500000,
  "stok": 25,
  "idKategori": 2
}
```

**Hasil:** `idBarang: 2`

---

### C. POST - Tambah Barang Ketiga ✅

```json
{
  "namaBarang": "Keyboard Mechanical",
  "harga": 1200000,
  "stok": 15,
  "idKategori": 2
}
```

**Hasil:** `idBarang: 3`

---

### D. GET - Lihat Semua Barang ✅

1. Cari **GET /api/barang** (kotak biru)
2. Klik **"Try it out"**
3. Klik **"Execute"**
4. **Hasil yang diharapkan:**
   - Response Code: **200 OK**
   - Response Body: Array berisi 3 barang

```json
[
  {
    "idBarang": 1,
    "namaBarang": "Laptop ASUS ROG",
    "harga": 15000000,
    "stok": 10,
    "idKategori": 1,
    "rowVersion": "AAAAAAAAB9E="
  },
  {
    "idBarang": 2,
    "namaBarang": "Mouse Gaming Logitech",
    "harga": 500000,
    "stok": 25,
    "idKategori": 2,
    "rowVersion": "AAAAAAAAB9I="
  },
  {
    "idBarang": 3,
    "namaBarang": "Keyboard Mechanical",
    "harga": 1200000,
    "stok": 15,
    "idKategori": 2,
    "rowVersion": "AAAAAAAAB9M="
  }
]
```

---

### E. GET by ID - Lihat Detail Barang ✅

1. Cari **GET /api/barang/{id}** (kotak biru)
2. Klik **"Try it out"**
3. Isi parameter **id**: `1`
4. Klik **"Execute"**
5. **Hasil yang diharapkan:**
   - Response Code: **200 OK**
   - Response Body: Detail barang ID 1 saja

---

### F. PUT - Update Barang ✅

1. Cari **PUT /api/barang/{id}** (kotak orange)
2. Klik **"Try it out"**
3. Isi parameter **id**: `1`
4. Isi Request body:

```json
{
  "namaBarang": "Laptop ASUS ROG Strix G15",
  "harga": 18000000,
  "stok": 20,
  "idKategori": 1
}
```

5. Klik **"Execute"**
6. **Hasil yang diharapkan:**
   - Response Code: **200 OK**
   - Message: "Barang berhasil diupdate"

7. **Verifikasi:** GET /api/barang/1 → Lihat data sudah berubah

---

### G. DELETE - Hapus Barang ✅

1. Cari **DELETE /api/barang/{id}** (kotak merah)
2. Klik **"Try it out"**
3. Isi parameter **id**: `3`
4. Klik **"Execute"**
5. **Hasil yang diharapkan:**
   - Response Code: **200 OK**
   - Message: "Barang berhasil dihapus"

6. **Verifikasi:** GET /api/barang → Sekarang hanya ada 2 barang (ID 1 dan 2)

---

## 💰 LANGKAH 3: Testing Transaction (10 Poin)

### A. Update Massal BERHASIL ✅

**Skenario:** Update stok 2 barang sekaligus (ID 1 dan 2)

1. Cari **POST /api/barang/update-massal**
2. Klik **"Try it out"**
3. Isi Request body:

```json
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

4. Klik **"Execute"**
5. **Hasil yang diharapkan:**
   - Response Code: **200 OK**
   - Message: "Update massal berhasil dilakukan"

6. **Verifikasi:** 
   - GET /api/barang/1 → Stok bertambah 5 (dari 20 jadi 25)
   - GET /api/barang/2 → Stok bertambah 10 (dari 25 jadi 35)

---

### B. Update Massal GAGAL (Rollback) ✅

**Skenario:** Update dengan ID yang tidak ada → Semua harus di-rollback

1. Catat stok barang ID 1 dan 2 saat ini (dari GET)
2. POST /api/barang/update-massal dengan:

```json
{
  "updates": [
    {
      "idBarang": 1,
      "tambahStok": 100
    },
    {
      "idBarang": 999,
      "tambahStok": 50
    }
  ]
}
```

3. **Hasil yang diharapkan:**
   - Response Code: **400 Bad Request**
   - Error message: "Barang dengan ID 999 tidak ditemukan"

4. **PENTING - Verifikasi Rollback:**
   - GET /api/barang/1 → Stok TIDAK berubah (masih 25)
   - Ini membuktikan transaction rollback bekerja!
   - Meskipun update ID 1 valid, karena ID 999 gagal, semua dibatalkan

---

## 🔒 LANGKAH 4: Testing Concurrency Control (10 Poin)

### Penjelasan Concurrency Control

**RowVersion** adalah mekanisme untuk mencegah konflik saat 2 user mengubah data yang sama bersamaan.

### Cara Kerja:
1. Setiap record punya `rowVersion` (byte array)
2. Setiap kali data di-update, `rowVersion` berubah otomatis
3. Jika user A dan B buka data yang sama, lalu A update dulu, maka B akan gagal update karena `rowVersion` sudah berbeda

### Simulasi Testing:

**Skenario:** 2 Admin mencoba update barang yang sama

1. **Admin A - GET barang ID 1:**
   - GET /api/barang/1
   - Catat `rowVersion`: misalnya `"AAAAAAAAB9E="`
   - Catat stok: misalnya `25`

2. **Admin B - GET barang ID 1 (bersamaan):**
   - GET /api/barang/1
   - Dapat data yang sama dengan Admin A
   - `rowVersion`: `"AAAAAAAAB9E="` (sama)

3. **Admin A - UPDATE barang ID 1:**
   - PUT /api/barang/1
   ```json
   {
     "namaBarang": "Laptop ASUS ROG Strix G15",
     "harga": 18000000,
     "stok": 30,
     "idKategori": 1
   }
   ```
   - **Berhasil!** Response 200 OK
   - `rowVersion` sekarang berubah jadi `"AAAAAAAAB9Q="` (contoh)

4. **Admin B - UPDATE barang ID 1 (tanpa refresh):**
   - PUT /api/barang/1
   ```json
   {
     "namaBarang": "Laptop ASUS ROG Strix G15",
     "harga": 18000000,
     "stok": 40,
     "idKategori": 1
   }
   ```
   - **GAGAL!** Response 409 Conflict
   - Message: "Data telah diubah oleh pengguna lain. Silakan refresh dan coba lagi."

5. **Penjelasan:**
   - Admin B gagal karena `rowVersion` di database sudah berubah
   - Ini mencegah Admin B menimpa perubahan Admin A
   - Admin B harus GET data terbaru dulu, baru bisa update

### Bukti untuk Laporan:
- Screenshot response 409 Conflict
- Jelaskan bahwa ini adalah **Optimistic Concurrency Control**
- Sistem mencegah **Lost Update Problem**

---

## 📊 LANGKAH 5: Verifikasi Database

Anda bisa cek database langsung di SQL Server:

1. Buka **SQL Server Management Studio (SSMS)** atau **Azure Data Studio**
2. Connect ke: `localhost\SQLEXPRESS`
3. Buka database: `GudangDB`
4. Query:

```sql
SELECT * FROM Barang;
```

5. Anda akan melihat semua data barang yang sudah Anda tambahkan via API

---

## ✅ CHECKLIST TESTING LENGKAP

Pastikan semua ini sudah Anda test:

### CRUD (45 poin)
- [ ] POST - Tambah barang baru (berhasil)
- [ ] GET - Lihat semua barang (berhasil)
- [ ] GET by ID - Lihat detail barang (berhasil)
- [ ] PUT - Update barang (berhasil)
- [ ] DELETE - Hapus barang (berhasil)

### Transaction (10 poin)
- [ ] POST update-massal - Semua berhasil (commit)
- [ ] POST update-massal - Ada yang gagal (rollback)
- [ ] Verifikasi rollback: data tidak berubah

### Concurrency (10 poin)
- [ ] RowVersion ada di setiap record
- [ ] Update bersamaan: yang kedua gagal dengan 409 Conflict
- [ ] Penjelasan Optimistic Concurrency Control

### Database (35 poin)
- [ ] Database GudangDB terbuat
- [ ] Tabel Barang ada dengan semua kolom
- [ ] Migration berhasil
- [ ] Data tersimpan di database

---

## 🎬 Tips untuk Demo/Presentasi

1. **Persiapan:**
   - Jalankan aplikasi sebelum demo
   - Buka Swagger UI di browser
   - Siapkan JSON request di notepad

2. **Urutan Demo:**
   - Tunjukkan Swagger UI (dokumentasi otomatis)
   - Demo CRUD: POST → GET → PUT → DELETE
   - Demo Transaction: berhasil dan gagal (rollback)
   - Jelaskan Concurrency Control dengan diagram

3. **Poin Plus:**
   - Tunjukkan database di SSMS
   - Jelaskan separation of concerns (Controller, Model, DbContext)
   - Tunjukkan code Transaction dan RowVersion

---

## 🐛 Troubleshooting

**Q: Response 404 Not Found?**
- Cek URL: harus `http://localhost:8080/api/barang`
- Pastikan aplikasi running

**Q: Response 400 Bad Request?**
- Cek format JSON (harus valid)
- Pastikan semua field required terisi

**Q: Response 409 Conflict?**
- Ini NORMAL untuk concurrency test
- Artinya data sudah diubah user lain
- GET data terbaru, lalu update lagi

**Q: Database error?**
- Pastikan SQL Server running
- Cek connection string di appsettings.json

---

## 📝 Catatan untuk Laporan

Sertakan dalam laporan:
1. Screenshot Swagger UI
2. Screenshot request & response untuk setiap endpoint
3. Screenshot database (query SELECT)
4. Penjelasan Transaction (commit & rollback)
5. Penjelasan Concurrency Control dengan RowVersion
6. Diagram arsitektur (Controller → DbContext → Database)

---

**Selamat mencoba! Semoga sukses dengan tugasnya! 🚀**
