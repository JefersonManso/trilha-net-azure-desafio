using Microsoft.EntityFrameworkCore;
using TrilhaNetAzureDesafio.Context;

var builder = WebApplication.CreateBuilder(args);

// ==================== Configuração do DbContext ==================== //
// Conecta o EF Core ao Azure SQL Database usando a connection string
builder.Services.AddDbContext<RHContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("ConexaoPadrao")));

// ==================== Serviços MVC/Controllers ==================== //

builder.Services.AddControllers();

// ==================== Swagger / OpenAPI ==================== //
// Facilita testes da API e documentação
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Swagger
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
