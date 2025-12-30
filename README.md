# BarnManagement 

BarnManagement, kullanıcıların kendi çiftliklerini yönetebildiği, hayvan satın alıp ürün (süt, yumurta, yün) üretebildiği ve bu ürünleri satarak bakiye yönetimi yapabildiği kapsamlı bir çiftlik simülasyonu uygulamasıdır.

## 📁 Proje Yapısı

Proje, sürdürülebilirlik ve test edilebilirlik için **N-Tier (Katmanlı) Mimari** üzerine inşa edilmiştir:

*   **`BarnManagement.Core`**: Uygulamanın çekirdek katmanıdır. Veritabanı varlıkları (Entity), DTO'lar ve servis arayüzleri burada tanımlanır. Bağımsız bir katmandır.
*   **`BarnManagement.DataAccess`**: Veri erişim katmanıdır. Entity Framework Core ve SQL Server yapılandırmaları, veritabanı bağlamı (`AppDbContext`) ve migration dosyaları burada bulunur.
*   **`BarnManagement.Business`**: Uygulamanın iş mantığı katmanıdır. Tüm servis implementasyonları, validasyonlar ve simülasyonu yürüten arka plan işçileri (Workers) bu katmandadır.
*   **`BarnManagement.API`**: .NET 10 tabanlı RESTful API katmanıdır. JWT kimlik doğrulaması sağlar ve dış dünyadan gelen istekleri Business katmanına yönlendirir.
*   **`BarnManagement.UI`**: Kullanıcı etkileşimi için geliştirilmiş WinForms masaüstü uygulamasıdır. API ile asenkron olarak haberleşir.
*   **`BarnManagement.Tests`**: API uç noktalarını ve iş mantığını test eden entegrasyon testlerini içerir.

## ✨ Temel Özellikler

*   **Güvenlik:** JWT (JSON Web Token) tabanlı kimlik doğrulama ve hashlenmiş şifre yönetimi.
*   **Simülasyon Mantığı:** Arka planda çalışan servisler sayesinde hayvanlar zamanla yaşlanır, ölür ve periyodik olarak ürün üretir.
*   **Ekonomi Sistemi:** Kullanıcıların bakiyeleri üzerinden hayvan alımı ve ürün satışı gerçekleştirilir.
*   **Performans:** Veritabanı seviyesinde indeksleme ve optimize edilmiş sorgular.
*   **Loglama:** Serilog ile tüm kritik işlemlerin (satın alma, hatalar, vb.) kayıt altına alınması.

## 🛠 Kullanılan Teknolojiler

*   **Backend:** .NET 10
*   **Veritabanı:** SQL Server & Entity Framework Core
*   **Frontend:** Windows Forms (WinForms)
*   **Güvenlik:** JWT Authentication
*   **Logging:** Serilog
*   **Testing:** xUnit & Integration Tests

## 🚀 Başlangıç

1.  **Veritabanı:** `appsettings.json` dosyasındaki bağlantı dizesini (`ConnectionString`) kendi SQL Server ayarlarınıza göre güncelleyin.
2.  **Migration:** `Update-Database` komutu ile veritabanı şemasını oluşturun.
3.  **Çalıştırma:** Visual Studio üzerinden `BarnManagement.sln` dosyasını açın ve hem `API` hem de `UI` projelerini aynı anda başlatacak şekilde yapılandırın.

---
*Bu proje, modern yazılım mimarilerini ve simülasyon mantığını öğrenmek/uygulamak amacıyla geliştirilmiştir.*
