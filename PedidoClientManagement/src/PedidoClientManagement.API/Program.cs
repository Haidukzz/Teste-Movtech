using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using PedidoClientManagement.API.Data;

var builder = WebApplication.CreateBuilder(args);

// 1) Lê connection string (suporta formato URI e key-value)
var rawConn =
    Environment.GetEnvironmentVariable("DATABASE_URL")
    ?? builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string não configurada.");

// Converte formato URI (postgresql://user:pass@host:port/db) para key-value do Npgsql
var connectionString = ConverterParaNpgsql(rawConn);
Console.WriteLine("→ Conectando ao banco de dados...");

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

// Auto-aplica migrações no startup
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

if (app.Environment.IsDevelopment())
    app.UseDeveloperExceptionPage();

app.UseCors();

var df = new DefaultFilesOptions();
df.DefaultFileNames.Clear();
df.DefaultFileNames.Add("index.html");
app.UseDefaultFiles(df);

app.UseStaticFiles();
app.UseAuthorization();
app.MapControllers();
app.Run();

// Converte URI postgresql:// para formato key-value aceito pelo Npgsql
static string ConverterParaNpgsql(string input)
{
    if (!input.StartsWith("postgresql://") && !input.StartsWith("postgres://"))
        return input; // já está no formato key-value

    var uri = new Uri(input);
    var partes = uri.UserInfo.Split(':', 2);
    var usuario = partes[0];
    var senha    = partes.Length > 1 ? partes[1] : "";
    var banco    = uri.AbsolutePath.TrimStart('/');
    var porta    = uri.Port > 0 ? uri.Port : 5432;

    return $"Host={uri.Host};Port={porta};Database={banco};" +
           $"Username={usuario};Password={senha};" +
           $"SSL Mode=Require;Trust Server Certificate=true";
}
