using Microsoft.EntityFrameworkCore;
using TrilhaNetAzureDesafio.Models;

namespace TrilhaNetAzureDesafio.Context
{
    // Classe de contexto que herda de DbContext, responsável por
    // mapear os modelos (entidades para tabelas no banco de dados.)
    public class RHContext : DbContext
    {
        // Construtor recebe as opções de configuração (connection string, provider, etc.)
        public RHContext(DbContextOptions<RHContext> options) : base(options)
        {

        }

        // Representa a tabela Funcionarios no banco.
        // A entidade Funcionario será mapeada automaticamente pelo EF Core.
        public DbSet<Funcionario> Funcionarios { get; set; } = null!;
    }
}
