using Application.Services.Ativos;
using Infrastructure.DataBase;
using Infrastructure.Repositories;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Infrastructure.ExternalServices;
using Infrastructure.Importacao;
using Application.Services.Carteira;

var builder = WebApplication.CreateBuilder(args);

var swaggerEnabled = builder.Configuration.GetValue(
    "Swagger:Enabled",
    builder.Environment.IsDevelopment());

var corsOrigins = builder.Configuration.GetSection("Cors:Origins").Get<string[]>()
    ?? ["http://localhost:5173"];

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
        policy.WithOrigins(corsOrigins)
            .AllowAnyHeader()
            .AllowAnyMethod());
});

// Banco de dados
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Repositórios
builder.Services.AddScoped<IAtivoRepository, AtivoRepository>();

// Use Cases
builder.Services.AddScoped<AddAtivoService>();
builder.Services.AddScoped<GetAtivosService>();
builder.Services.AddScoped<RemoveAtivoService>();
builder.Services.AddScoped<GetCotacaoAtivoService>();
builder.Services.AddScoped<PreviewImportacaoB3Service>();
builder.Services.AddScoped<ImportarAtivosService>();
builder.Services.AddScoped<IB3PosicaoParser, B3PosicaoParser>();
builder.Services.AddScoped<GetRentabilidadeAtivosService>();
builder.Services.AddScoped<GetResumoCarteiraService>();

// External Services
builder.Services.AddHttpClient<IBrapiService, BrapiService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var app = builder.Build();

if (builder.Configuration.GetValue("Database:AutoMigrate", false))
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

if (swaggerEnabled)
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "CarteiraPro API v1");
        options.RoutePrefix = "swagger";
        options.DocumentTitle = "CarteiraPro API";
    });
}

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseCors();
app.UseAuthorization();
app.MapGet("/health", () => Results.Ok(new { status = "ok" }));

if (swaggerEnabled)
{
    app.MapGet("/", () => Results.Redirect("/swagger")).ExcludeFromDescription();
}

app.MapControllers();

app.Run();