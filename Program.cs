using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// Configuración de controladores y supresión de validación forzada en campos opcionales
builder.Services.AddControllersWithViews(options =>
{
    // Permite que campos como sObservaciones, sPuntoVerde, etc. se envíen vacíos sin error "The field is required"
    options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true;
});

// Configuración de Autenticación basada en Cookies y Roles (Estudiante, Organización, Admin)
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Cuenta/Login";
        options.LogoutPath = "/Cuenta/Logout";
        options.AccessDeniedPath = "/Cuenta/AccesoDenegado";
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
        options.Cookie.Name = "EcoCokie_Auth";
    });

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Middleware de Autenticación y Autorización
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Voluntariado}/{action=Index}/{id?}");

app.Run();
