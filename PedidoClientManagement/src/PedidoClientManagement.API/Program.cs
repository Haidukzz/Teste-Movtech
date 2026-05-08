using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using PedidoClientManagement.API.Data;

var builder = WebApplication.CreateBuilder(args);

// 1) Connection string via env var (DATABASE_URL) ou appsettings
var connectionString =
    Environment.GetEnvironmentVariable("DATABASE_URL")
    ?? builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string não configurada. Defina a variável de ambiente DATABASE_URL.");

Console.WriteLine($"→ Conectando ao banco de dados...");

// 2) EF + PostgreSQL
builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseNpgsql(connectionString));

// 3) CORS
builder.Services.AddCors(opt =>
    opt.AddDefaultPolicy(p => p.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));

// 4) Controllers + JSON cycles off
builder.Services.AddControllers()
    .AddJsonOptions(opts =>
    {
        opts.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        opts.JsonSerializerOptions.WriteIndented = true;
    });

// 5) Arquivos estáticos
builder.Services.AddDirectoryBrowser();

var app = builder.Build();

// **Auto-aplica migrações no startup**
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

if (app.Environment.IsDevelopment())
    app.UseDeveloperExceptionPage();

app.UseCors();

// default files -> wwwroot/index.html
var df = new DefaultFilesOptions();
df.DefaultFileNames.Clear();
df.DefaultFileNames.Add("index.html");
app.UseDefaultFiles(df);

app.UseStaticFiles();

app.UseAuthorization();
app.MapControllers();

app.Run();
