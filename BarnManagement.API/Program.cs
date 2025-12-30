using BarnManagement.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using BarnManagement.Core.Interfaces;
using BarnManagement.Core.Entities;
using BarnManagement.Business.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Serilog;

// Uygulama oluşturucu (Builder) başlatılıyor.
var builder = WebApplication.CreateBuilder(args);

// Serilog yapılandırması: Loglama ayarları 'appsettings.json' dosyasından okunur.
builder.Host.UseSerilog((context, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration));

// Controller servisleri ekleniyor (API endpoint'lerini yönetmek için).
builder.Services.AddControllers();

// OpenAPI/Swagger desteği ekleniyor (API dökümantasyonu için).
builder.Services.AddOpenApi();

// Sağlık kontrolleri (Health Checks) ekleniyor. Veritabanı bağlantısı kontrol edilir.
builder.Services.AddHealthChecks()
    .AddDbContextCheck<AppDbContext>();

// Veritabanı Bağlantısı (Entity Framework Core): Bağlantı dizesi 'appsettings.json'dan alınır.
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Kimlik Doğrulama (Authentication) Yapılandırması: JWT (JSON Web Token) kullanılır.
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true, // Tokenı oluşturan kimliği doğrula
            ValidateAudience = true, // Tokenı kullanacak kitleyi doğrula
            ValidateLifetime = true, // Token süresinin dolup dolmadığını doğrula
            ValidateIssuerSigningKey = true, // İmzayı doğrula
            ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
            ValidAudience = builder.Configuration["JwtSettings:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:Secret"]!)) // Şifreleme anahtarı
        };
    });

// Yetkilendirme (Authorization) servisi ekleniyor.
builder.Services.AddAuthorization();

// Bağımlılık Enjeksiyonu (Dependency Injection - DI): Arayüzler ve sınıflar eşleştiriliyor.
builder.Services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>(); // Token üretici
builder.Services.AddScoped<IAuthService, AuthService>(); // Giriş/Kayıt servisi
builder.Services.AddScoped<IUserService, UserService>(); // Kullanıcı işlemleri
builder.Services.AddScoped<IFarmService, FarmService>(); // Çiftlik işlemleri
builder.Services.AddScoped<IAnimalService, AnimalService>(); // Hayvan işlemleri
builder.Services.AddScoped<IProductService, ProductService>(); // Ürün işlemleri
builder.Services.AddSingleton<IMarketService, MarketService>(); // Market (fiyat) servisi (tekil örnek)

// Arka Plan İşçileri (Background Workers): Otomatik üretim ve yaşam döngüsü yönetimi için.
builder.Services.AddHostedService<BarnManagement.Business.Workers.ProductionWorker>();
builder.Services.AddHostedService<BarnManagement.Business.Workers.AnimalLifecycleWorker>();

// Uygulama inşa ediliyor.
var app = builder.Build();

// Geliştirme ortamında Swagger/OpenAPI haritasını çıkar.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// Serilog ile gelen HTTP isteklerini otomatik logla.
app.UseSerilogRequestLogging();

// HTTP isteklerini HTTPS'e yönlendir.
app.UseHttpsRedirection();

// Kimlik doğrulama ve yetkilendirme middleware'lerini kullan.
app.UseAuthentication();
app.UseAuthorization();

// Controller ve Health Check endpoint'lerini eşleştir.
app.MapControllers();
app.MapHealthChecks("/health");

// Uygulamayı çalıştır.
app.Run();

// Test projelerinin Program sınıfına erişebilmesi için gerekli (Partial Program Class).
public partial class Program { }
