# Barn Management Projesi - Kod Özeti

## Katman: API (BarnManagement.API)

Bu katman, dış dünya ile uygulamanın iş mantığı arasındaki köprüdür. İstemcilerden (Frontend, Mobile App vb.) gelen HTTP isteklerini karşılar, gerekli servisleri çağırır ve sonuçları uygun formatta (JSON) geri döner.

---

### 1. Dosya: Program.cs
**Konum:** `BarnManagement.API/Program.cs`

#### Görevi ve Sorumluluğu
Projenin giriş noktasıdır (Entry Point). Uygulamanın ayarlarının yapıldığı, servislerin (Dependency Injection) kaydedildiği ve HTTP istek işleme hattının (Middleware Pipeline) yapılandırıldığı yerdir.

#### İletişim Kurduğu Katmanlar
- **DataAccess:** Veritabanı bağlantısını (`AppDbContext`) yapılandırır.
- **Business:** İş mantığı servislerini (`Service` sınıfları) ve arka plan işçilerini (`Worker` sınıfları) sisteme tanıtır.
- **Core:** Arayüzler (`Interface`) ve varlıklar (`Entity`) üzerinden bağımlılıkları tanımlar.

#### Önemli Kod Parçaları
- `builder.Services.AddDbContext<AppDbContext>(...)`: SQL Server veritabanı bağlantısını kurar.
- `builder.Services.AddAuthentication(...)`: JWT (JSON Web Token) tabanlı kimlik doğrulamayı aktif eder.
- `builder.Services.AddScoped<IInterface, Service>()`: Servisleri (örneğin `IAuthService`, `IFarmService`) sisteme ekler.
- `builder.Services.AddHostedService<...>()`: Hayvan yaşam döngüsü ve üretim gibi arka plan işlemlerini başlatır.
- `app.UseAuthentication()` ve `app.UseAuthorization()`: Güvenlik katmanlarını devreye alır.

#### Kullanım Senaryosu
Uygulama ilk başlatıldığında bu dosya çalışır. Veritabanına bağlanır, güvenlik ayarlarını yükler ve gelen istekleri dinlemeye başlar.

#### Bağımlılıklar
- ASP.NET Core Framework
- Entity Framework Core
- Serilog (Loglama için)
- JWT Bearer (Kimlik doğrulama için)

---

### 2. Dosya: Controllers\AuthController.cs
**Konum:** `BarnManagement.API/Controllers/AuthController.cs`

#### Görevi ve Sorumluluğu
Kullanıcıların sisteme kayıt olması (Register) ve giriş yapması (Login) işlemlerini yönetir. Token üretimi için iş katmanını tetikler.

#### İletişim Kurduğu Katmanlar
- **Service (Business):** `IAuthService` üzerinden kayıt ve giriş işlemlerini gerçekleştirir.

#### Önemli Kod Parçaları
- `Register(RegisterRequest request)`: Yeni kullanıcı oluşturur.
- `Login(LoginRequest request)`: Kullanıcı bilgilerini doğrular ve JWT token döner.

#### Kullanım Senaryosu
Bir kullanıcı "Kayıt Ol" butonuna bastığında buraya istek gelir. Başarılı olursa kullanıcıya "Kayıt Başarılı" döner. "Giriş Yap" dediğinde ise, kimlik bilgileri doğruysa bir erişim anahtarı (Token) verir.

#### Bağımlılıklar
- `IAuthService` (Constructor Injection ile gelir)

---

### 3. Dosya: Controllers\UsersController.cs
**Konum:** `BarnManagement.API/Controllers/UsersController.cs`

#### Görevi ve Sorumluluğu
Giriş yapmış kullanıcının kendi profil bilgilerini ve bakiyesini görüntülemesini sağlar. Ayrıca hesabı sıfırlama (reset) gibi kullanıcıya özel işlemleri yönetir.

#### İletişim Kurduğu Katmanlar
- **Service (Business):** `IUserService` ile iletişim kurar.

#### Önemli Kod Parçaları
- `[Authorize]`: Bu denetleyicideki tüm metotların sadece giriş yapmış kullanıcılar tarafından çağrılabileceğini belirtir.
- `GetMe()`: Token içindeki kimlik bilgisini (`ClaimTypes.NameIdentifier`) kullanarak kullanıcının detaylarını getirir.
- `GetBalance()`: Kullanıcının güncel bakiyesini sorgular.
- `ResetAccount()`: Kullanıcının tüm ilerlemesini (çiftlikler, hayvanlar, bakiye) sıfırlar.

#### Kullanım Senaryosu
Kullanıcı profil sayfasına girdiğinde, adı, e-postası ve güncel parası bu dosya üzerinden sorgulanır.

#### Bağımlılıklar
- `IUserService`

---

### 4. Dosya: Controllers\FarmsController.cs
**Konum:** `BarnManagement.API/Controllers/FarmsController.cs`

#### Görevi ve Sorumluluğu
Kullanıcının çiftliklerini yönetmesini sağlar. Çiftlik oluşturma, listeleme, güncelleme ve silme (CRUD) işlemlerini yapar.

#### İletişim Kurduğu Katmanlar
- **Service (Business):** `IFarmService` ile iletişim kurar.

#### Önemli Kod Parçaları
- `CreateFarm(...)`: Yeni bir çiftlik kurar.
- `GetUserFarms()`: Kullanıcının sahip olduğu tüm çiftlikleri listeler.
- `GetFarmById(Guid id)`: Belirli bir çiftliğin detaylarını getirir.
- `UpdateFarm(...)` ve `DeleteFarm(...)`: Çiftlik bilgilerini günceller veya siler.

#### Kullanım Senaryosu
Oyun ekranında "Yeni Çiftlik Kur" dendiğinde buraya istek atılır. "Çiftliklerim" ekranında ise liste buradan çekilir.

#### Bağımlılıklar
- `IFarmService`

---

### 5. Dosya: Controllers\AnimalsController.cs
**Konum:** `BarnManagement.API/Controllers/AnimalsController.cs`

#### Görevi ve Sorumluluğu
Hayvanlarla ilgili işlemleri yönetir. Hayvan satın alma, satma ve bir çiftlikteki hayvanları listeleme gibi fonksiyonları barındırır.

#### İletişim Kurduğu Katmanlar
- **Service (Business):** `IAnimalService` ile iletişim kurar.

#### Önemli Kod Parçaları
- `BuyAnimal(...)`: Belirli bir çiftliğe yeni hayvan satın alır. Kullanıcının bakiyesini kontrol eder ve düşer.
- `SellAnimal(...)`: Mevcut bir hayvanı satar ve karşılığını bakiyeye ekler.
- `GetFarmAnimals(...)`: Çiftlikteki inek, tavuk gibi hayvanların listesini döner.

#### Kullanım Senaryosu
Kullanıcı marketten bir "İnek" satın aldığında `BuyAnimal` çalışır. İneği sattığında ise `SellAnimal` devreye girer.

#### Bağımlılıklar
- `IAnimalService`

---

### 6. Dosya: Controllers\ProductsController.cs
**Konum:** `BarnManagement.API/Controllers/ProductsController.cs`

#### Görevi ve Sorumluluğu
Hayvanların ürettiği (süt, yumurta vb.) ürünlerin yönetimini sağlar. Ürünleri listeleme ve satma işlemlerini yapar.

#### İletişim Kurduğu Katmanlar
- **Service (Business):** `IProductService` ile iletişim kurar.

#### Önemli Kod Parçaları
- `SellProduct(...)`: Tek bir ürünü satar.
- `SellAllProducts(...)`: Çiftlikteki satılabilir tüm ürünleri tek seferde satar (Toplu Satış).
- `GetFarmProducts(...)`: Çiftlik deposundaki ürünleri listeler.

#### Kullanım Senaryosu
Kullanıcı deposunda biriken 100 litre sütü "Hepsini Sat" butonuyla sattığında `SellAllProducts` metodu çalışır ve kazanılan para bakiyeye eklenir.

#### Bağımlılıklar
- `IProductService`
