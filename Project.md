# Barn Management API — Project.md (AI Guide)

> Bu dosya **projenin kaynak kural kitabı**dır. Yapay zeka (AI) veya geliştirici hangi adımı atması gerektiğini buradan kesin olarak öğrenecek.

---

## 1. Öncelik ve işlem sırası (Kritik — değişmez kural)
1. **Her çalışmada önce `Project.md` okunmalıdır.** Buradaki kurallar tek yetkili rehberdir.
2. Sonra **`Completed.md`** kontrol edilir — tamamlanmış işleri tekrar etme.
3. En son **`ToDo.md`** okunur ve yalnızca **TO-DO listesinin en üstündeki (aktif) madde** işlenir.
4. Bir görev tamamlandığında: tamamlanan madde **tüm içeriğiyle** `Completed.md`'ye taşınır (tarih+zaman eklenir) ve `ToDo.md`'den silinir veya işaretlenir.

> Not: AI, bu sırayı bozarsa önce uyarı ver ve ardından Project.md kurallarına geri dön.

---

## 2. Proje amacı (kısa)
Bu proje, kullanıcıların sanal çiftliklerini yönetebildiği, hayvan alım/satımı, ürün üretimi ve ürün satışından gelir elde edebildiği bir **.NET 10 Web API** uygulamasıdır. Veriler SQL Server'da tutulur ve Code-First (migrations) yaklaşımı kullanılır. Güvenlik JWT ile sağlanır. Loglama Serilog veya benzeri ile yapılır.

---

## 3. AI için temel kurallar (davranışsal)
- **Dosya rolü kesin**: `Project.md` — rehber; `ToDo.md` — yapılacak; `Completed.md` — bitti.
- **Tek görev**: Aynı anda yalnızca bir `ToDo` ele alınır.
- **Değişiklik kaydı**: Veritabanı şeması değişiklikleri migration ile yapılır ve commit edilir.
- **Güncelleme bildirimi**: Eğer AI kod/senaryo oluşturursa ilgili komutları (ef migrations, db update, run) `ToDo.md` içinde öner.
- **Hata yönetimi**: Tüm hata/istatistik/olaylar loglanmalıdır; böyle bir görev varsa `ToDo` listesine "Add Logging for X" şeklinde ekle.

---

## 4. Proje yapısı (öneri)
```
/BarnManagementAPI
  /src
    /BarnManagement.Api        (Web API)
    /BarnManagement.Core       (Domain modelleri, interfaces)
    /BarnManagement.DataAccess (EF Core, Repositories, Migrations)
    /BarnManagement.Business  (Business logic, Background services)
    /BarnManagement.Tests      (Unit/Integration tests)
  /docs
    Project.md
    ToDo.md
    Completed.md
```

---

## 5. Veri modelleri (örnek C# sınıfları)
**(Model isimlendirmeleri İngilizce olacak)**

### User
```csharp
public class User {
  public Guid Id { get; set; }
  public string UserName { get; set; }
  public string Email { get; set; }
  public decimal Balance { get; set; }
  public ICollection<Farm> Farms { get; set; }
}
```

### Farm
```csharp
public class Farm {
  public Guid Id { get; set; }
  public string Name { get; set; }
  public Guid OwnerId { get; set; }
  public User Owner { get; set; }
  public ICollection<Animal> Animals { get; set; }
}
```

### Animal
```csharp
public class Animal {
  public Guid Id { get; set; }
  public string Species { get; set; }
  public string Name { get; set; }
  public DateTime BirthDate { get; set; }
  public TimeSpan LifeSpan { get; set; } // lifetime length
  public DateTime? RemovedAt { get; set; }
  public Guid FarmId { get; set; }
  public Farm Farm { get; set; }
  public ICollection<Product> Products { get; set; }
}
```

### Product
```csharp
public class Product {
  public Guid Id { get; set; }
  public string Type { get; set; }
  public decimal SalePrice { get; set; }
  public DateTime ProducedAt { get; set; }
  public Guid AnimalId { get; set; }
  public Animal Animal { get; set; }
  public bool IsSold { get; set; }
}
```

---

## 6. Veritabanı ve Migrations (kurallar)
- EF Core Code-First kullanılacak. Migration komutları örnek:
  - `dotnet ef migrations add InitialCreate --project BarnManagement.Infrastructure`
  - `dotnet ef database update --project BarnManagement.Infrastructure`
- Migration dosyaları source control (git) ile saklanmalı.
- DB connection string `appsettings.json` içinde, hassas veriler `User Secrets` veya environment variable ile yönetilmeli.

---

## 7. Kimlik doğrulama — JWT
- Kayıt ve giriş endpointleri: `/api/auth/register`, `/api/auth/login`.
- JWT payload içinde `sub` = user id, `roles` veya `scope` gerektiğinde kullan.
- Token expiry kısa tutulmalı (örn. 60 dakika) ve refresh token mekanizması opsiyonel ama tavsiye edilir.

---

## 8. API endpoint önerisi (kısa, tam liste ToDo'ya eklenebilir)
**Auth**
- POST `/api/auth/register`
- POST `/api/auth/login`

**User**
- GET `/api/users/{id}`
- PUT `/api/users/{id}`

**Farm**
- POST `/api/farms`
- GET `/api/farms/{id}`
- PUT `/api/farms/{id}`

**Animal**
- POST `/api/farms/{farmId}/animals` (purchase)
- DELETE `/api/farms/{farmId}/animals/{animalId}` (sell/remove)
- GET `/api/farms/{farmId}/animals`

**Product**
- GET `/api/farms/{farmId}/products`
- POST `/api/animals/{animalId}/products` (manual produce, or internal)
- POST `/api/products/{productId}/sell` (sells product and credits user)

---

## 9. İş mantığı (business rules)
1. **Hayvan satın alma**: Kullanıcı bakiye >= hayvan fiyatı ise bakiye düşer, hayvan oluşturulur ve `ToDo` veya log kaydı oluşturulur.
2. **Hayvan yaşam süresi**: `LifeSpan` dolduğunda hayvan otomatik olarak sistemden kaldırılır (RemovedAt set edilir). Bu işlem background service tarafından kontrol edilir.
3. **Üretim**: Hayvanlar belirli aralıklarla ürün üretir (ör: inek -> 24 saatte 1 süt). Üretimler `Product` nesnesi oluşturur.
4. **Ürün satışı**: `Product` satıldığında `IsSold=true` olur ve ürünün `SalePrice`'ı sahibin `User.Balance`'ına eklenir. İşlem atomik olmalıdır (transaction).
5. **Concurrency**: Satış/alış ve üretim işlemlerinde concurrency kontrolü (EF Core optimistic concurrency veya DB transaction) kullanılmalıdır.

---

## 10. Zamanlanmış işler / Background services
- Hayvan ömrü ve ürün üretimi için `IHostedService`/`BackgroundService` veya **Hangfire/Quartz.NET** kullanılabilir.
- Örnek: Her 10 dakikada bir çalışan servis:
  - Süresi dolmuş hayvanları kaldır.
  - Üretim zamanları gelen hayvanlardan product üret.
  - Üretilen ürünleri DB'ye ekle ve gerekli loglamayı yap.

---

## 11. Loglama ve Hata Yönetimi
- Serilog ile konsol+dosya+rolling file JSON formatı kullanılmalı.
- Hata, uyarı, bilgi ve izleme logları (Error, Warning, Information, Debug) mantıklı seviyede tutulmalı.
- Tüm önemli iş akışı adımları loglanmalı: kullanıcı işlemleri, satın alma/satış, üretim, yaşam süresi silme, migration hataları.

---

## 12. Testler
- Unit testler `BarnManagement.Tests` içinde.
- Integration testler: in-memory DB (Sqlite in-memory) ile API endpoint testleri.
- Kritik test senaryoları: satın alma (balance değişimi), ürün satışı (balance artışı), hayvan ömrü temizleme, üretim zamanlaması.

---

## 13. Konvansiyonlar (Naming/Style)
- Tüm public API route'ları ve DTO'lar İngilizce.
- DTO kullan: `AnimalDto`, `CreateAnimalRequest`, `ProductDto`, `UserProfileDto`.
- Commit mesajları açık olmalı: `feat: add animal purchase endpoint` / `fix: handle product sale concurrency`.

---

## 14. Güvenlik
- API tüm kritik endpoint'lerde `[Authorize]` kullan.
- Sensitive bilgileri (password) hashing (bcrypt/ASP.NET Identity) ile sakla.
- Rate limiting ve basic input validation (size, length, numeric ranges) ekle.

---

## 15. Deployment / Local çalışma talimatları
- `appsettings.Development.json` ve `appsettings.Production.json` kullan.
- Local DB için Docker SQL Server veya localdb kullanılabilir.
- `dotnet run` ile servisi başlatmadan önce migrations uygulayın.

---

## 16. `ToDo.md` ve `Completed.md` kullanım formatı (AI okunabilir)
**ToDo.md** örnek satır biçimi:
```
- [ ] <YYYY-MM-DD HH:mm> TASK-ID: Short description -- details or commands
```
Örnek:
```
- [ ] 2025-12-08 10:00 TASK-001: Add EF Core initial models and migration -- run: dotnet ef migrations add InitialCreate
```

**Completed.md** örnek satır biçimi:
```
- [x] <YYYY-MM-DD HH:mm> TASK-ID: Short description -- details (by @agent or @dev)
```

> AI: Bir task'ı tamamladığında `Completed.md`'ye yukarıdaki formatta ekle ve `ToDo.md`'den kaldır.

---

## 17. Örnek görev iş akışı (kısa)
1. `ToDo.md` okunur; en üstte `TASK-001` varsa ele al.
2. Gerekli migration/mODEL oluşturulur.
3. Kod commit edilir `git add/commit` ve açıkca commit mesajı eklenir.
4. `Completed.md`'ye taşınır: `- [x] 2025-12-08 12:30 TASK-001: ... -- by @AI`

---

## 18. Hangi durumlarda manuel uyarı verilmeli
- Migration DB uyumsuzluğu.
- Kritik güvenlik açıkları veya credentials eksik.
- Production'a migration atılmadan önce onay gerektiriyorsa.

---

## 19. Sık kullanılan komutlar (örnek)
- `dotnet ef migrations add <Name> --project BarnManagement.Infrastructure`
- `dotnet ef database update --project BarnManagement.Infrastructure`
- `dotnet run --project BarnManagement.Api`

---

## 20. Son notlar
- Bu dosya **projenin standartlarını ve AI davranışını** belirler. Değişiklik gerekiyorsa `ToDo.md`'ye "Update Project.md: reason" şeklinde ekle. Değişiklik yapılmadan önce ekip onayı veya açık bir görev girişi şarttır.

---

## 21. Veritabanı Şeması
Database Schema & DB Diagram Reference
Bu projede veritabanı şeması dbdiagram.md dosyasında tanımlanmıştır.
Veritabanı yapısı ve tablolar arası ilişkiler dbdiagram.md dosyasında yer alır.
Bu dosya, tek yetkili veritabanı referansıdır.
Entity modelleri, EF Core konfigürasyonları ve migrations bu diyagrama birebir uyumlu olmalıdır.
Kurallar:
Project.md içinde tablo alanları tekrar tanımlanmaz.
Şema ile kod arasında uyumsuzluk tespit edilirse:
Öncelik DB diagram dosyasındadır.
Kod ve migrations diyagrama göre düzeltilir.
Yeni tablo veya kolon ihtiyacı doğarsa:
Önce DB diagram dosyası güncellenir.
Ardından ilgili ToDo görevi oluşturulur.
Son olarak migration üretilir.
İlişki Kuralları (Özet)
users → farms : One-to-Many (owner)
farms → animals : One-to-Many
animals → products : One-to-Many
Not: Tüm foreign key ilişkileri ve veri türleri DB diagram dosyasında tanımlandığı şekliyle uygulanmalıdır.

---

*Hazır.* Bu `Project.md` projenin tüm ana kurallarını, veri modellerini ve AI için gereken davranışları içerir. `ToDo.md` ve `Completed.md` oluşturulur/formatlanırsa AI doğru sıra ve kurallara göre çalışacaktır.

