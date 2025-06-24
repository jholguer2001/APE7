using NOTAS_APE.Data;
using NOTAS_APE.Services;
using Microsoft.EntityFrameworkCore;
using NOTAS_APE.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Habilitar CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    });

    options.AddPolicy("PermitirFrontend", policy =>
    {
        policy.WithOrigins("http://127.0.0.1:5500", "http://localhost:5500")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Registrar DbContext con la cadena de conexi�n
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (string.IsNullOrEmpty(connectionString))
{
    throw new InvalidOperationException("La cadena de conexi�n 'DefaultConnection' no est� configurada.");
}

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// Registrar servicios y repositorios
builder.Services.AddScoped<IEstudianteRepository, EstudianteRepository>();
builder.Services.AddScoped<EstudianteService>();

builder.Services.AddScoped<INotaRepository, NotaRepository>();
builder.Services.AddScoped<NotaService>();

builder.Services.AddScoped<IPromedioRepository, PromedioRepository>();
builder.Services.AddScoped<PromedioService>();

// Agregar controladores y Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Middleware para entorno de desarrollo
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// CORS debe ir antes de autenticaci�n/autorizaci�n
app.UseCors("PermitirFrontend");

// Orden correcto para servir archivos est�ticos y p�gina por defecto (index.html)
app.UseDefaultFiles();  // Sirve autom�ticamente wwwroot/index.html
app.UseStaticFiles();   // Sirve archivos est�ticos (css, js, im�genes)

app.UseAuthorization();

app.MapControllers();

app.Run();
