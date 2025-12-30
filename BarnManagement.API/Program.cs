using BarnManagement.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using BarnManagement.Core.Interfaces;
using BarnManagement.Core.Entities;
using BarnManagement.Business.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json.Serialization;
using Serilog;
using Microsoft.OpenApi.Models;
using System.Reflection;

// Uygulama oluşturucu (Builder) başlatılıyor.
var builder = WebApplication.CreateBuilder(args);

// Serilog yapılandırması: Loglama ayarları 'appsettings.json' dosyasından okunur.
builder.Host.UseSerilog((context, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration));

// Controller servisleri ekleniyor ve Enum'ların string olarak serileştirilmesi sağlanıyor.
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

// OpenAPI/Swagger desteği ekleniyor (API dökümantasyonu için).
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo 
    { 
        Title = "Barn Management API", 
        Version = "v1",
        Description = "Barn Management - Çiftlik ve Hayvan Yönetim Sistemi API",
        Contact = new OpenApiContact
        {
            Name = "Barn Management Support",
            Email = "support@barnmanagement.com"
        }
    });

    // JWT Yetkilendirme desteği ekle
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Token girişi yapın. Örnek: 'Bearer {token}'"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });

    // XML yorumlarını Swagger'a dahil et
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    options.IncludeXmlComments(xmlPath);
});

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
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Barn Management API v1");
        options.RoutePrefix = "swagger"; // Swagger UI'ya /swagger adresinden erişilecek
    });
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
