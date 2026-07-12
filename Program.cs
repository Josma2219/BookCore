<<<<<<< Updated upstream
namespace BookCore
=======
using BookCore.Data;
using Microsoft.EntityFrameworkCore;
using BookCore.Services;

var constructor = WebApplication.CreateBuilder(args);

var cadenaConexion = constructor.Configuration
    .GetConnectionString("ConexionBookCore")
    ?? throw new InvalidOperationException(
        "No se encontró la conexión ConexionBookCore en appsettings.json.");

// Aquí dejamos conectado Entity Framework con la base BookCore.
constructor.Services.AddDbContext<BookCoreContexto>(opciones =>
    opciones.UseSqlServer(cadenaConexion));
// Dejamos el servicio listo para usarlo desde los controladores.
constructor.Services.AddScoped<
    ICategoriaServicio,
    CategoriaServicio>();

constructor.Services.AddControllersWithViews();

var aplicacion = constructor.Build();

if (!aplicacion.Environment.IsDevelopment())
>>>>>>> Stashed changes
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            var app = builder.Build();

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
        }
    }
}

// Prueba 2

// Prueba 4
