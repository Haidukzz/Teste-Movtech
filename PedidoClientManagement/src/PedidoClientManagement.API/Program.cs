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

var connectionString = ConverterParaNpgsql(rawConn);
Console.WriteLine("→ Conectando ao banco de dados...");

// 2) EF + PostgreSQL
builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseNpgsql(connectionString));

// 3) CORS
builder.Services.AddCors(opt =>
    opt.AddDefaultPolicy(p => p.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));

// 4) Controllers + JSON
builder.Services.AddControllers()
    .AddJsonOptions(opts =>
    {
        opts.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        opts.JsonSerializerOptions.WriteIndented = true;
    });

// 5) Arquivos estáticos
builder.Services.AddDirectoryBrowser();

var app = builder.Build();

// Auto-aplica migrações e garante colunas adicionadas manualmente
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();

    // Garante que a coluna Endereco existe, independente do histórico de migrations
    var conn = db.Database.GetDbConnection();
    conn.Open();
    using var cmd = conn.CreateCommand();
    cmd.CommandText = @"
        ALTER TABLE ""Clientes""
        ADD COLUMN IF NOT EXISTS ""Endereco"" TEXT NOT NULL DEFAULT '';";
    cmd.ExecuteNonQuery();
    conn.Close();
    Console.WriteLine("→ Coluna Endereco verificada/criada com sucesso.");
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
        return input;

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
