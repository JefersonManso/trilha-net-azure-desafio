using System;
using System.Text.Json;
using Azure;
using Azure.Data.Tables;

namespace TrilhaNetAzureDesafio.Models
{
    // Classe que registra um log de alterações de funcionário no Azure table storage
    public class FuncionarioLog : Funcionario, ITableEntity
    {
        public FuncionarioLog()
        {
            PartitionKey = string.Empty;
            RowKey = string.Empty;
            JSON = string.Empty;
        } 

        public FuncionarioLog(Funcionario funcionario, TipoAcao tipoAcao, string partitionKey, string rowKey)
        {
            if (funcionario == null)
                throw new ArgumentNullException(nameof(funcionario));
                
            // Copiando dados do funcionário
            Id = funcionario.Id;
            Nome = funcionario.Nome;
            Endereco = funcionario.Endereco;
            Ramal = funcionario.Ramal;
            EmailProfissional = funcionario.EmailProfissional;
            Departamento = funcionario.Departamento;
            Salario = funcionario.Salario;
            DataAdmissao = funcionario.DataAdmissao;

            // Metadados do log
            TipoAcao = tipoAcao;
            JSON = JsonSerializer.Serialize(funcionario);

            // Dados obrigatórios do ITableEntity
            PartitionKey = string.IsNullOrWhiteSpace(partitionKey) ? "Funcionario" : partitionKey;
            RowKey = string.IsNullOrWhiteSpace(rowKey) ? Guid.NewGuid().ToString() : rowKey;

            ETag = ETag.All;
        }

        // Tipo de operação realizada (ex: Inserção, Atualização, Exclusão)
        public TipoAcao TipoAcao { get; set; }

        // Snapshot em JSON do funcionário no momento do log
        public string JSON { get; set; } 

        // Propriedades obrigatórias do Azure table storage
        public string PartitionKey { get; set; } 
        public string RowKey { get; set; } 
        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }
    }
}