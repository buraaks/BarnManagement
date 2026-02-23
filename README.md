# BarnManagement

BarnManagement, kullanıcıların kendi çiftliklerini yönetebildiği, hayvan satın alıp ürün (süt, yumurta, yün) üretebildiği ve bu ürünleri satarak bakiye yönetimi yapabildiği kapsamlı bir çiftlik simülasyonu uygulamasıdır.

## Proje Yapısı

Proje, sürdürülebilirlik ve test edilebilirlik için **N-Tier (Katmanlı) Mimari** üzerine inşa edilmiştir:

* **`BarnManagement.Core`**: Uygulamanın çekirdek katmanıdır. Veritabanı varlıkları (Entity), DTO'lar, enum'lar ve servis arayüzleri burada tanımlanır. Bağımsız bir katmandır.
* **`BarnManagement.DataAccess`**: Veri erişim katmanıdır. Entity Framework Core ve SQL Server yapılandırmaları, veritabanı bağlamı (`AppDbContext`) ve migration dosyaları burada bulunur.
* **`BarnManagement.Business`**: Uygulamanın iş mantığı katmanıdır. Tüm servis implementasyonları, validasyonlar ve simülasyonu yürüten arka plan işçileri (Workers) bu katmandadır.
* **`BarnManagement.API`**: .NET 10 tabanlı RESTful API katmanıdır. JWT kimlik doğrulaması sağlar ve dış dünyadan gelen istekleri Business katmanına yönlendirir.
* **`BarnManagement.Web`**: Nuxt 3 ve Vue 3 ile geliştirilmiş modern SPA frontend uygulamasıdır. API ile iletişim kurarak kullanıcı arayüzünü sağlar.
* **`BarnManagement.Tests`**: API uç noktalarını ve iş mantığını test eden birim ve entegrasyon testlerini içerir.

## Temel Özellikler

* **Güvenlik:** JWT (JSON Web Token) tabanlı kimlik doğrulama ve hashlenmiş şifre yönetimi (BCrypt).
* **Simülasyon Mantığı:** Arka planda çalışan servisler sayesinde hayvanlar zamanla yaşlanır, ölür ve periyodik olarak ürün üretir.
* **Ekonomi Sistemi:** Kullanıcıların bakiyeleri üzerinden hayvan alımı ve ürün satışı gerçekleştirilir.
* **Çiftlik Yönetimi:** Birden fazla çiftlik oluşturma, düzenleme ve silme.
* **Dark Mode:** Sistem tercihine veya manuel seçime göre karanlık/aydınlık tema desteği.
* **Performans:** Veritabanı seviyesinde indeksleme, optimize edilmiş sorgular ve akıllı polling stratejisi.
* **Loglama:** Serilog ile tüm kritik işlemlerin kayıt altına alınması.

## Kullanılan Teknolojiler

| Katman | Teknoloji |
|--------|-----------|
| Backend | .NET 10, ASP.NET Core Web API |
| Frontend | Nuxt 3, Vue 3, TypeScript |
| Veritabanı | SQL Server, Entity Framework Core |
| Güvenlik | JWT Authentication, BCrypt |
| Logging | Serilog (Console + File) |
| Testing | xUnit, FluentAssertions, Moq |
| Paket Yönetimi | NuGet (.NET), pnpm (Node.js) |

## Başlangıç

### Gereksinimler

* .NET 10 SDK
* SQL Server
* Node.js (v18+) ve pnpm

### Kurulum

1. **Veritabanı:** `BarnManagement.API/appsettings.json` dosyasındaki bağlantı dizesini (`ConnectionStrings:DefaultConnection`) kendi SQL Server ayarlarınıza göre güncelleyin.

2. **Migration:** Veritabanı şemasını oluşturun:
   ```bash
   dotnet ef database update --project BarnManagement.DataAccess --startup-project BarnManagement.API
   ```

3. **Frontend bağımlılıkları:**
   ```bash
   cd BarnManagement.Web
   pnpm install
   ```

4. **Çalıştırma:** Visual Studio üzerinden `BarnManagement.sln` dosyasını açın ve **API + Web (Full Stack)** launch profilini seçerek her iki projeyi aynı anda başlatın. Ya da terminal ile:
   ```bash
   # API (ayrı terminalde)
   dotnet run --project BarnManagement.API

   # Web (ayrı terminalde)
   cd BarnManagement.Web && pnpm dev
   ```

---
*Bu proje, modern yazılım mimarilerini ve simülasyon mantığını öğrenmek/uygulamak amacıyla geliştirilmiştir.*
