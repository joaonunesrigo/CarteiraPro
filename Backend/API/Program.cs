using Application.Services.Ativos;
using Infrastructure.DataBase;
using Infrastructure.Repositories;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Infrastructure.ExternalServices;
using Infrastructure.Importacao;
using Application.Services.Carteira;

var builder = WebApplication.CreateBuilder(args);

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

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();