using BookCore.Data;
using Microsoft.EntityFrameworkCore;

var constructor = WebApplication.CreateBuilder(args);

var cadenaConexion = constructor.Configuration
    .GetConnectionString("ConexionBookCore")
    ?? throw new InvalidOperationException(
        "No se encontró la conexión ConexionBookCore en appsettings.json.");

// Aquí dejamos conectado Entity Framework con la base BookCore.
constructor.Services.AddDbContext<BookCoreContexto>(opciones =>
    opciones.UseSqlServer(cadenaConexion));

constructor.Services.AddControllersWithViews();

var aplicacion = constructor.Build();

if (!aplicacion.Environment.IsDevelopment())
{
    aplicacion.UseExceptionHandler("/Home/Error");
    aplicacion.UseHsts();
}

aplicacion.UseHttpsRedirection();
aplicacion.UseRouting();

aplicacion.UseAuthorization();

aplicacion.MapStaticAssets();

aplicacion.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

aplicacion.Run();