using Azure.Data.Tables;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using TrilhaNetAzureDesafio.Context;
using TrilhaNetAzureDesafio.Models;

namespace TrilhaNetAzureDesafio.Controllers;

// Marca a classe como Controller de API
[ApiController]

// Define a rota base como /Funcionário
[Route("[controller]")]
public class FuncionarioController : ControllerBase
{
    // Contexto do Entity Framework para acessar o banco SQL
    private readonly RHContext _context;

    // String de conexão do Azure Table Storage
    private readonly string _connectionString;

    // Nome da tabela no Azure Table Storage
    private readonly string _tableName;

    // Construtor da Controller: recebe o contexto e as configurações
    public FuncionarioController(RHContext context, IConfiguration configuration)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        
        // Busca a connection string configurada no appsettings.json
        _connectionString = configuration.GetValue<string>("ConnectionStrings:SAConnectionString")
                            ?? throw new ArgumentNullException(nameof(configuration), "Connection string não encontrada.");

        // Busca o nome da tabela configurada no appsettings.json
        _tableName = configuration.GetValue<string>("ConnectionStrings:AzureTableName")
                     ?? throw new ArgumentNullException(nameof(configuration), "Nome da tabela não encontado.");
    }

    // Método privado para criar/obter um cliente da tabela no Azure
    private TableClient GetTableClient()
    {
        // Cria o cliente de serviço
        var serviceClient = new TableServiceClient(_connectionString);

        // Cria o cliente para a tabela específica
        var tableClient = serviceClient.GetTableClient(_tableName);

        // Se a tabela não existir no Azure, cria automaticamente
        tableClient.CreateIfNotExists();
        return tableClient;
    }


    // ==================== Métodos CRUD ==================== //

    // GET /Funcionario/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> ObterPorId(int id)
    {
        try
        {
            // Busca funcionário no banco SQL pelo ID (chave primária)
            var funcionario = await _context.Funcionarios.FindAsync(id);

            // Retorna 404 se não encontrar
            if (funcionario == null)
                return NotFound();

            // Retorna 200 com o objeto
            return Ok(funcionario);
        }
        catch (Exception ex)
        {
            // Em caso de erro, retorna 500 e a mensagem
            return StatusCode(500, $"Erro ao buscar funcionário: {ex.Message}");
        }
    }

    // POST /Funcionario
    [HttpPost]
    public async Task<IActionResult> Criar(Funcionario funcionario)
    {
        try
        {
            // Adiciona no SQL Server
            await _context.Funcionarios.AddAsync(funcionario);

            // Chamar o método SaveChanges do _context para salvar no Banco SQL
            await _context.SaveChangesAsync();

            // Cria o cliente da tabela
            var tableClient = GetTableClient();

            // Cria um log para o Azure Table (ação = Inclusão)
            var funcionarioLog = new FuncionarioLog(funcionario, TipoAcao.Inclusao, funcionario.Departamento, funcionario.Id.ToString());

            // Chamar o método UpsertEntity para salvar no Azure Table
            await tableClient.UpsertEntityAsync(funcionarioLog);

            // Retorna 201 (Created) com rota de consulta por ID
            return CreatedAtAction(nameof(ObterPorId), new { id = funcionario.Id }, funcionario);
        }
        catch (Exception ex)
        {
            // Retorna 500 em caso de falha
            return StatusCode(500, $"Erro ao criar funcionário: {ex.Message}");
        }
    }


    // PUT /Funcionario/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> Atualizar(int id, Funcionario funcionario)
    {
        try
        {
            // Busca o funcionário no banco SQL
            var funcionarioBanco = await _context.Funcionarios.FindAsync(id);

            // Se não encontrar, 404
            if (funcionarioBanco == null)
                return NotFound();

            // Atualiza todos os campos
            funcionarioBanco.Nome = funcionario.Nome;
            funcionarioBanco.Endereco = funcionario.Endereco;
            funcionarioBanco.Ramal = funcionario.Ramal;
            funcionarioBanco.EmailProfissional = funcionario.EmailProfissional;
            funcionarioBanco.Departamento = funcionario.Departamento;
            funcionarioBanco.Salario = funcionario.Salario;
            funcionarioBanco.DataAdmissao = funcionario.DataAdmissao;


            // Chamar o método de Update do _context.Funcionarios para salvar no Banco SQL
            _context.Funcionarios.Update(funcionarioBanco);
            await _context.SaveChangesAsync();

            // Cria cliente do Azure Table
            var tableClient = GetTableClient();

            // Cria log de atualização
            var funcionarioLog = new FuncionarioLog(funcionarioBanco, TipoAcao.Atualizacao, funcionarioBanco.Departamento, funcionarioBanco.Id.ToString());

            // Chamar o método UpsertEntity para salvar no Azure Table
            await tableClient.UpsertEntityAsync(funcionarioLog);

            // Retorna 200 com o objeto atualizado
            return Ok(funcionarioBanco);
        }
        catch (Exception ex)
        {
           return StatusCode(500, $"Erro ao atualizar funcionário: {ex.Message}");
        }
    }


    // DELETE /Funcionario/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> Deletar(int id)
    {
        try
        {
            // Busca o funcionário no banco SQL
            var funcionarioBanco = await _context.Funcionarios.FindAsync(id);

            // 404 se não encontar
            if (funcionarioBanco == null)
                return NotFound();

            // Remove do _context.Funcionarios para salvar no Banco SQL
            _context.Funcionarios.Remove(funcionarioBanco);
            await _context.SaveChangesAsync();

            // Cria cliente do Azure Table
            var tableClient = GetTableClient();

            // Cria log de remoção
            var funcionarioLog = new FuncionarioLog(funcionarioBanco, TipoAcao.Remocao, funcionarioBanco.Departamento, funcionarioBanco.Id.ToString());

            // Chamar o método UpsertEntity para salvar no Azure Table
            await tableClient.UpsertEntityAsync(funcionarioLog);

            // Retorna 204 
            return NoContent();
        }
        catch (Exception ex)
        {
           return StatusCode(500, $"Erro ao deletar funcionário: {ex.Message}");
        }
    }
}
