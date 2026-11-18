using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SistemaDeOrdenesDeReparacion.BL;

var builder = WebApplication.CreateBuilder(args);

// 1. Registrar MVC
builder.Services.AddControllersWithViews();

// 2. Registrar IHttpContextAccessor para que esté disponible en las vistas
builder.Services.AddHttpContextAccessor();


// 3. Registrar DistributedMemoryCache y Session
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddControllersWithViews(options =>
{
    options.ModelBindingMessageProvider.SetValueMustBeANumberAccessor(
        (fieldName) => $"El campo {fieldName} debe ser un valor numérico válido.");



});


builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.UI.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.UI.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();

var app = builder.Build();

// 5. Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// 6. Habilitar Session justo después de UseRouting() y antes de UseAuthorization()
app.UseSession();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Login}/{action=Index}/{id?}");

app.Run();
