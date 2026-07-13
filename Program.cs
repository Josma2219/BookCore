using BookCore.Data;
using BookCore.Models.Entidades;
using BookCore.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var constructor = WebApplication.CreateBuilder(args);

var cadenaConexion = constructor.Configuration
    .GetConnectionString("ConexionBookCore")
    ?? throw new InvalidOperationException(
        "No se encontró la conexión ConexionBookCore en appsettings.json.");

// Conexión de Entity Framework con SQL Server.
constructor.Services.AddDbContext<BookCoreContexto>(opciones =>
    opciones.UseSqlServer(cadenaConexion));

// Servicios de los módulos administrativos.
constructor.Services.AddScoped<
    ICategoriaServicio,
    CategoriaServicio>();

constructor.Services.AddScoped<
    IAutorServicio,
    AutorServicio>();

constructor.Services.AddScoped<
    IUsuarioBibliotecaServicio,
    UsuarioBibliotecaServicio>();

constructor.Services.AddScoped<
    ILibroServicio,
    LibroServicio>();

constructor.Services.AddScoped<
    IEjemplarServicio,
    EjemplarServicio>();

constructor.Services.AddScoped<
    IPrestamoServicio,
    PrestamoServicio>();

// Servicio que valida el usuario y la contraseña.
constructor.Services.AddScoped<
    IAccesoServicio,
    AccesoServicio>();

// Servicio de consulta pública del catálogo.
constructor.Services.AddScoped<
    ICatalogoServicio,
    CatalogoServicio>();
// Servicio encargado del resumen administrativo.
constructor.Services.AddScoped<
    IPanelAdministrativoServicio,
    PanelAdministrativoServicio>();

// Genera y verifica las contraseñas protegidas.
constructor.Services.AddScoped<
    IPasswordHasher<UsuarioAdministrativo>,
    PasswordHasher<UsuarioAdministrativo>>();

// La sesión administrativa se guarda en una cookie protegida.
constructor.Services
    .AddAuthentication(
        CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(opciones =>
    {
        opciones.LoginPath =
            "/Cuenta/IniciarSesion";

        opciones.AccessDeniedPath =
            "/Cuenta/AccesoDenegado";

        opciones.Cookie.Name =
            "BookCore.Sesion";

        opciones.Cookie.HttpOnly = true;

        opciones.Cookie.SameSite =
            SameSiteMode.Lax;

        opciones.Cookie.SecurePolicy =
            CookieSecurePolicy.SameAsRequest;

        opciones.ExpireTimeSpan =
            TimeSpan.FromHours(8);

        opciones.SlidingExpiration = true;
    });

constructor.Services.AddAuthorization();

constructor.Services.AddControllersWithViews();

var aplicacion = constructor.Build();

if (!aplicacion.Environment.IsDevelopment())
{
    aplicacion.UseExceptionHandler("/Home/Error");
    aplicacion.UseHsts();
}

aplicacion.UseHttpsRedirection();
aplicacion.UseRouting();

// Primero se identifica al usuario.
aplicacion.UseAuthentication();

// Después se revisa si tiene permiso.
aplicacion.UseAuthorization();

aplicacion.MapStaticAssets();

aplicacion.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

// Solo se crea automáticamente durante el desarrollo local.
if (aplicacion.Environment.IsDevelopment())
{
    await InicializadorAdministrador
        .CrearAsync(aplicacion.Services);
}

aplicacion.Run();