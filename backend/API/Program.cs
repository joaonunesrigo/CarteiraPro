using System.Text;
using API.Auth;
using Application.Services.Ativos;
using Application.Services.Auth;
using Infrastructure.DataBase;
using Infrastructure.Repositories;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Infrastructure.ExternalServices;
using Infrastructure.Importacao;
using Application.Services.Carteira;
using Application.Services.Proventos;
using Application.Services.Operacoes;
using Application.Services.Carteiras;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.IdentityModel.Tokens;

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

builder.Services.AddMemoryCache();
builder.Services.AddHttpContextAccessor();

// Banco de dados
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Repositórios
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<ICarteiraRepository, CarteiraRepository>();
builder.Services.AddScoped<IAtivoRepository, AtivoRepository>();
builder.Services.AddScoped<IProventoRepository, ProventoRepository>();
builder.Services.AddScoped<IOperacaoRepository, OperacaoRepository>();

// Use Cases
builder.Services.AddScoped<PasswordHasher>();
builder.Services.AddScoped<RegistrarUsuarioService>();
builder.Services.AddScoped<LoginUsuarioService>();
builder.Services.AddScoped<GetCarteiraAtualService>();
builder.Services.AddScoped<GetCarteirasService>();
builder.Services.AddScoped<AddCarteiraService>();
builder.Services.AddScoped<UpdateCarteiraService>();
builder.Services.AddScoped<RemoveCarteiraService>();
builder.Services.AddScoped<AddAtivoService>();
builder.Services.AddScoped<GetAtivosService>();
builder.Services.AddScoped<RemoveAtivoService>();
builder.Services.AddScoped<RemoveAllAtivosService>();
builder.Services.AddScoped<GetCotacaoAtivoService>();
builder.Services.AddScoped<PreviewImportacaoB3Service>();
builder.Services.AddScoped<ImportarAtivosService>();
builder.Services.AddScoped<IB3PosicaoParser, B3PosicaoParser>();
builder.Services.AddScoped<IB3MovimentacaoParser, B3MovimentacaoParser>();
builder.Services.AddScoped<GetRentabilidadeAtivosService>();
builder.Services.AddScoped<GetResumoCarteiraService>();
builder.Services.AddScoped<GetEvolucaoPatrimonialService>();
builder.Services.AddScoped<CalcularPosicaoAtivoService>();
builder.Services.AddScoped<GetOperacoesService>();
builder.Services.AddScoped<AddOperacaoService>();
builder.Services.AddScoped<RemoveOperacaoService>();
builder.Services.AddScoped<GetProventosService>();
builder.Services.AddScoped<GetResumoProventosService>();
builder.Services.AddScoped<AddProventoService>();
builder.Services.AddScoped<RemoveProventoService>();
builder.Services.AddScoped<PreviewImportacaoProventosB3Service>();
builder.Services.AddScoped<ImportarProventosB3Service>();

// External Services
builder.Services.AddHttpClient<IBrapiService, BrapiService>();
builder.Services.AddSingleton<ICotacoesCache, CotacoesCache>();
builder.Services.AddSingleton<IHistoricoCotacoesCache, HistoricoCotacoesCache>();
builder.Services.AddHostedService<CotacoesRefreshService>();

// Auth
var jwtSection = builder.Configuration.GetSection("Jwt");
var jwtKey = jwtSection.GetValue<string>("Key")
    ?? "dev-only-change-this-secret-key-with-at-least-32-characters";
var jwtIssuer = jwtSection.GetValue<string>("Issuer") ?? "CarteiraPro";
var jwtAudience = jwtSection.GetValue<string>("Audience") ?? "CarteiraPro";

builder.Services.AddScoped<ICurrentUser, HttpCurrentUser>();
builder.Services.AddScoped<JwtTokenService>();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddControllers(options =>
{
    var policy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
    options.Filters.Add(new AuthorizeFilter(policy));
});
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
app.UseAuthentication();
app.UseAuthorization();
app.MapGet("/health", () => Results.Ok(new { status = "ok" }));

if (swaggerEnabled)
{
    app.MapGet("/", () => Results.Redirect("/swagger")).ExcludeFromDescription();
}

app.MapControllers();

app.Run();