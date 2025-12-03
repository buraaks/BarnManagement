using BarnManagement.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// DbContext ? Code-First SQL baðlantýsý
builder.Services.AddDbContext<AppDbContext>(options =>
	options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// OpenAPI (Swagger)
builder.Services.AddOpenApi();

var app = builder.Build();

// Swagger sadece Development modunda
if (app.Environment.IsDevelopment())
{
	app.MapOpenApi();
}

app.UseHttpsRedirection();

app.Run();
