using BookCore.Data;
using BookCore.Services;
using Microsoft.EntityFrameworkCore;

var constructor = WebApplication.CreateBuilder(args);

var cadenaConexion = constructor.Configuration
    .GetConnectionString("ConexionBookCore")
    ?? throw new InvalidOperationException(
        "No se encontró la conexión ConexionBookCore en appsettings.json.");

// Conectamos Entity Framework con la base de datos BookCore.
constructor.Services.AddDbContext<BookCoreContexto>(opciones =>
    opciones.UseSqlServer(cadenaConexion));

// Servicio que maneja el módulo de categorías.
constructor.Services.AddScoped<
    ICategoriaServicio,
    CategoriaServicio>();

// Servicio que maneja el módulo de autores.
constructor.Services.AddScoped<
    IAutorServicio,
    AutorServicio>();

// Este servicio maneja los usuarios que solicitan libros.
constructor.Services.AddScoped<
    IUsuarioBibliotecaServicio,
    UsuarioBibliotecaServicio>();
// Este servicio maneja libros, filtros y asociaciones con autores.
constructor.Services.AddScoped<
    ILibroServicio,
    LibroServicio>();
// Este servicio administra las copias físicas de los libros.
constructor.Services.AddScoped<
    IEjemplarServicio,
    EjemplarServicio>();
// Este servicio registra préstamos y devoluciones.
constructor.Services.AddScoped<
    IPrestamoServicio,
    PrestamoServicio>();

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