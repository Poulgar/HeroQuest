using System.Globalization;
using HeroQuest.Data;
using HeroQuest.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.Sqlite;

var builder = WebApplication.CreateBuilder(args);

CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.InvariantCulture;

// Add services to the container.
builder.Services.AddControllersWithViews();

// Fijamos la ruta a la carpeta del proyecto para que Visual Studio y `dotnet run`
// usen la misma base y no creen por accidente otro HeroQuest.db en bin/Debug.
var databasePath = Path.Combine(builder.Environment.ContentRootPath, "DataBase", "HeroQuest.db");
Directory.CreateDirectory(Path.GetDirectoryName(databasePath)!);
var connectionString = new SqliteConnectionStringBuilder(
    builder.Configuration.GetConnectionString("DefaultConnection"))
{
    DataSource = databasePath
}.ToString();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));

var app = builder.Build();

var ruta = Path.Combine(
    AppContext.BaseDirectory,
    "Data",
    "HeroQuest_Heroes.csv");

// Los CSV de HeroQuest_csv se copian a la salida mediante HeroQuest.csproj.
var csvFolder = Path.Combine(AppContext.BaseDirectory, "HeroQuest_csv");

// Primero actualizamos el esquema y después importamos los datos en el mismo contexto.
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider
        .GetRequiredService<ApplicationDbContext>();

    await context.Database.MigrateAsync();

    var importer = new HeroeImportService(context);
    await importer.ImportarAsync(ruta);

    var csvImporter = new CsvImportService(context);
    await csvImporter.ImportAllAsync(csvFolder);

    // Los IDs sirven como índice alfabético, respetando las reglas de ordenación españolas.
    var alphabeticalIds = new AlphabeticalIdService(context);
    await alphabeticalIds.EnsureAlphabeticalIdsAsync();
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
