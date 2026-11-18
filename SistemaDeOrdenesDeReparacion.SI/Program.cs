using Microsoft.EntityFrameworkCore;
using SistemaDeOrdenesDeReparacion.BL;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<SistemaDeOrdenesDeReparacion.DA.DBContexto>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddScoped<IServicioDeCorreo, ServicioDeCorreo>();
builder.Services.AddScoped<IGestionLogin, GestionLogin>();
builder.Services.AddScoped<IGestionMecanicos, GestionMecanicos>();
builder.Services.AddScoped<IGestionCliente, GestionCliente>();
builder.Services.AddScoped<ICatalogoDeRepuestos, CatalogoDeRepuestos>();
builder.Services.AddScoped<ICatalogoDeServicios, CatalogoDeServicios>();
builder.Services.AddScoped<IGestionDeOrdenesDeTrabajo, GestionDeOrdenesDeTrabajo>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseMiddleware<SistemaDeOrdenesDeReparacion.SI.Autenticacion.ApiKeyMiddleware>();

app.UseAuthorization();

app.MapControllers();

app.Run();
