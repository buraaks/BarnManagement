##  Phase 1: Temel Altyapı ve Kimlik Doğrulama

### Proje Kurulumu

- [x]  .NET 10 Web API projesi oluşturma
- [x]  Solution yapısı organizasyonu
- API Layer
- Business Layer
- Data Access Layer
- Core/Domain Layer
- [x]  Git repository kurulumu ve branch stratejisi
- [x]  SQL Server bağlantısı ve Code-First yapılandırması
- [x]  Serilog entegrasyonu
- Console logging
- File logging (JSON formatında)
- Log seviyeleri konfigürasyonu
- [x]  appsettings.json ve environment ayarları

### Sprint 1.2: Authentication & User Management


- [x]  `POST /api/auth/register` - Kullanıcı kaydı
- [x] Email validasyonu
- [x] Password hashing (BCrypt)
- [x] Başlangıç bakiyesi (0)
- [x]  `POST /api/auth/login` - JWT token üretimi
- [x] Token expiration (60dk)
- [x]  `GET /api/users/me` - Kullanıcı profili
- [ ]  `PUT /api/users/me` - Profil güncelleme (Sonraya bırakıldı)
- [x]  `GET /api/users/me/balance` - Bakiye sorgulama

**Database Migrations:**

- [x]  User tablosu (Id, Email, UserName, PasswordHash, Balance) (InitialCreate ile yapıldı)

**Testler:**

- [x]  Manual .http tests

---

##  Phase 2: Core Business Logic

### Sprint 2.1: Çiftlik Yönetimi

**API Endpoints:**

- [x]  `POST /api/farms` - Çiftlik oluşturma
- [x]  `GET /api/farms/{id}` - Çiftlik detayları
- [x]  `PUT /api/farms/{id}` - Çiftlik güncelleme
- [x]  `DELETE /api/farms/{id}` - Çiftlik silme
- [x]  `GET /api/farms` - Kullanıcının çiftlikleri (user/me yerine)

**Database:**

- [x]  Farm tablosu migration (InitialCreate ile yapıldı)
- [x]  User-Farm ilişkisi (1-N)

**Business Rules:**

- [x]  Authorization (sadece çiftlik sahibi işlem yapabilir)

**UI (BarnManagement.UI):**

- [x]  WinForms projesi oluşturuldu
- [x]  LoginForm ve RegisterForm
- [x]  API Client servisi
- [x]  MainForm temel yapısı

### Sprint 2.2: Hayvan Yönetimi

**API Endpoints:**

- [x]  `POST /api/farms/{farmId}/animals/buy` - Hayvan satın alma
    - Bakiye kontrolü
    - Fiyat hesaplama
    - Stok kontrolü (opsiyonel)
- [x]  `POST /api/animals/{id}/sell` - Hayvan satma
    - Satış fiyatı hesaplama (yaş, tür vb.)
    - Bakiye güncelleme
- [x]  `GET /api/animals` - Hayvan listesi (filtreleme)
- [x]  `GET /api/animals/{id}` - Hayvan detayı
- [x]  `GET /api/farms/{farmId}/animals` - Çiftliğin hayvanları

**Database:**

- [x]  Animal tablosu migration
    - Species, BirthDate, LifeSpanDays
    - ProductionInterval, NextProductionAt
    - PurchasePrice, SellPrice
- [x]  Farm-Animal ilişkisi (1-N)
- [ ]  AnimalType lookup tablosu (opsiyonel)

**Business Rules:**

- [ ]  Yaşam süresi takibi (Background Worker bekleniyor - Sprint 3.2)
- [ ]  Üretim zamanı hesaplama (Background Worker bekleniyor - Sprint 3.2)
- [ ]  Fiyat dinamiği (Temel mantık var, worker ile gelişecek)
- [x]  Bakiye işlemleri (transaction safety)

---

##  Phase 3: Ürün Sistemi ve Otomasyon

### Sprint 3.1: Ürün Yönetimi 

**API Endpoints:**

- [x]  `GET /api/products` - Ürün listesi (filtreleme)
- [x]  `GET /api/products/{id}` - Ürün detayı
- [x]  `POST /api/products/{id}/sell` - Ürün satışı
- [x]  `GET /api/farms/{farmId}/products` - Çiftliğin ürünleri

**Database:**

- [x]  Product tablosu migration
    - AnimalId, ProductType
    - ProducedAt, SalePrice
    - (IsSold, SoldAt kaldırıldı - Satıldığında siliniyor)
- [x]  Animal-Product ilişkisi (1-N)

**Business Rules:**

- [x]  Ürün satış validasyonu
- [x]  Bakiye güncelleme
- [x]  Satış transaction logging

### Sprint 3.2: Background Worker - Otomasyon 

**Worker Services:**

- [x]  **ProductionWorker** (IHostedService)
    - Her 1 dakikada bir çalışır
    - `NextProductionAt <= Now` kontrolü
    - Otomatik ürün oluşturma
    - NextProductionAt güncelleme
    - Loglama
- [x]  **AnimalLifecycleWorker**
    - Her gece 00:00'da çalışır
    - `BirthDate + LifeSpanDays <= Now` kontrolü
    - Hayvanları soft-delete veya hard-delete
    - Bildirim/log oluşturma
- [ ]  **PriceFluctuationWorker** (opsiyonel)
    - Pazar fiyatlarını günceller

**Testing:**

- [ ]  Worker unit testleri
- [ ]  Integration testleri (test database)
- [ ]  Performance testleri (çok sayıda hayvan simülasyonu)

---

##  Phase 4: Testing, Optimizasyon ve Deployment

### Sprint 4.1: Testing ve Hata Yönetimi 

**Test Coverage:**

- [ ]  Unit testler (tüm business logic)
- [ ]  Integration testler (API endpoints)
- [ ]  Authentication/Authorization testleri
- [ ]  Concurrency testleri
- [ ]  Error handling testleri

**Hata Senaryoları:**

- [ ]  Yetersiz bakiye
- [ ]  Duplicate satış girişimleri
- [ ]  JWT expiration/invalid token
- [ ]  SQL connection failures
- [ ]  Concurrency conflicts
- [ ]  Validation errors

**Logging Enhancement:**

- [ ]  Tüm critical operations için log noktaları
- [ ]  Structured logging (JSON)
- [ ]  Error tracking (opsiyonel: Seq/ELK)
- [ ]  Performance metrics

### Sprint 4.2: Optimizasyon ve Deployment  

**Performance:**

- [ ]  Database indexleme stratejisi
- [ ]  Query optimization (N+1 problemleri)
- [ ]  Caching stratejisi (Redis opsiyonel)
- [ ]  API response time iyileştirmeleri

**Documentation:**

- [ ]  Swagger/OpenAPI dokümantasyonu
- [ ]  API endpoint örnekleri
- [ ]  Postman collection
- [ ]  [README.md](http://README.md) ve kurulum kılavuzu

**Deployment:**

- [ ]  Production environment setup
- [ ]  Connection string management
- [ ]  Environment-specific configurations
- [ ]  Health check endpoints
- [ ]  Monitoring setup