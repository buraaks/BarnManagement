# BarnManagement Proje Rehberi 🌾🐄

Bu döküman, projenin mimari yapısını, iş mantığını ve teknik detaylarını sıfırdan bir kurulum sırasıyla açıklamaktadır.

---

### 1. Proje Kurulumu ve Mimari Yapı (The Foundation)
Proje, sürdürülebilirlik ve test edilebilirlik için **N-Tier (Katmanlı) Mimari** üzerine inşa edilmiştir.

*   **Core (Çekirdek):** Bağımsız katmandır. Varlıklar (`User`, `Animal`, `Farm`, `Product`), DTO'lar ve servis arayüzleri (IJwtTokenGenerator, IAnimalService vb.) burada bulunur.
*   **DataAccess (Veri Erişimi):** EF Core ve SQL Server yapılandırmasının bulunduğu yerdir. `AppDbContext` tüm veritabanı şemasını (Tables, Indexes, Relationships) yönetir.
*   **Business (İş Mantığı):** Uygulamanın beynidir. Servisler (Auth, Farm, Animal, Product) ve simülasyonu yürüten arka plan işçileri (Workers) buradadır.
*   **API:** .NET 10 tabanlı dış katmandır. JWT güvenliğini sağlar ve istekleri Business katmanına yönlendirir.
*   **UI:** Kullanıcı etkileşimi için geliştirilmiş, API ile konuşan WinForms masaüstü uygulamasıdır.

---

### 2. Authentication & User Management (Güvenlik)
Kullanıcıların sisteme erişimi ve finansal güvenliği bu katmanda sağlanır.

*   **JWT (JSON Web Token):** Standart kimlik doğrulama mekanizmasıdır. Kullanıcı giriş yaptığında bir token alır ve bu token ile yetkili işlemleri gerçekleştirir.
*   **Bakiye Yönetimi:** Her kullanıcının bir `Balance` alanı vardır. Bu simülasyon ekonomisinin temelidir.
*   **Şifre Güvenliği:** Kullanıcı şifreleri veritabanına asla düz metin olarak yazılmaz, güvenli hash algoritmaları ile saklanır.

---

### 3. Çiftlik Yönetimi (Farm Management)
Hayvanların barınacağı ve ürünlerin stoklanacağı temel birimdir.

*   **Mülkiyet:** Her çiftlik bir kullanıcıya bağlıdır (User-Farm 1:N ilişkisi).
*   **Kısıtlama:** Bir kullanıcı sadece kendi çiftliğine hayvan alabilir veya ürün satabilir. Bu kontrol servis katmanında yapılır.

---

### 4. Hayvan Yönetimi (Animal Management)
Ekonomik döngünün motoru olan canlı varlıkların yönetimidir.

*   **Satın Alma:** Kullanıcı hayvan aldığında bakiye kontrolü yapılır, tutar düşülür ve hayvana türüne göre (Cow, Sheep, Chicken) ömür ve üretim aralığı atanır.
*   **Yaşam Döngüsü:** Hayvan doğduğu andan itibaren yaşlanmaya başlar. Ömrü bittiğinde sistemden otomatik olarak silinir.
*   **Üretim Zamanlaması:** Her hayvanın bir sonraki üretim saati (`NextProductionAt`) veritabanında takip edilir.

---

### 5. Ürün Yönetimi (Product Management)
Hayvanların ürettiği değerlerin (Süt, Yumurta, Yün) yönetimidir.

*   **Stok Mantığı:** Üretilen ürünler hayvan bazlı değil, **çiftlik bazlı (Farm stokunda)** tutulur. Bu, büyük ölçekli yönetimde performans sağlar.
*   **Satış:** Ürünler satıldığında, kod içerisindeki sabit fiyatlar (Milk: 15, Egg: 2.5, Wool: 50) üzerinden hesaplama yapılır ve bakiye artırılır.

---

### 6. Test ve Hata Yönetimi (Stability)
Sistemin doğruluğunu ve hata anındaki davranışlarını yönetir.

*   **Integration Tests:** API endpoint'leri gerçek bir veritabanı (SQL Server - BarnManagementDb) üzerinde 33 farklı senaryo ile test edilir.
*   **Loglama (Serilog):** Uygulamanın yaptığı her kritik işlem (Satın alma, hata alma, DB silme) JSON formatında loglanır.
*   **Validation:** Geçersiz istekler (yetersiz bakiye, yanlış çiftlik ID vb.) kontrol edilerek anlamlı hata mesajları döndürülür.

---

### 7. Optimizasyon (Performance)
Sistemin hızlı çalışması için uygulanan teknikler.

*   **Veritabanı İndeksleri:** `Farms`, `Animals` ve `Products` tablolarına `FarmId` bazlı indeksler eklenmiştir. Bu sayede binlerce kayıt arasından çiftlik detaylarını getirmek saniyeler yerine milisaniyeler sürer.
*   **Health Checks:** Uygulama ve DB durumunu izlemek için `/health` noktası eklenmiştir.

---

### 8. Background Worker - Otomasyon (Automation)
Simülasyonun arka planda kendiliğinden akmasını sağlayan servislerdir.

*   **ProductionWorker:** Her 2 saniyede bir (simülasyonda 1 ay/gün gibi düşünebiliriz) veritabanını tarar, saati gelmiş hayvanlar için "Ürün" üretir ve bir sonraki üretim saatini ayarlar.
*   **AnimalLifecycleWorker:** Hayvanların yaşını kontrol eder ve ömrü dolanları sistemden kaldırarak çiftliğin temiz kalmasını sağlar.

---
*Bu rehber, BarnManagement projesinin teknik altyapısını anlamak için hazırlanmıştır.*
