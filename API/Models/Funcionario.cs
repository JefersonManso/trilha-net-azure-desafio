using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TrilhaNetAzureDesafio.Models
{
    public class Funcionario
    {
        public Funcionario()
        {
            Nome = string.Empty;
            Endereco = string.Empty;
            Ramal = string.Empty;
            EmailProfissional = string.Empty;
            Departamento = string.Empty;
        }

        public Funcionario(int id, string nome, string endereco, string ramal, string emailProfissional, string departamento, decimal salario, DateTime dataAdmissao)
        {
            Id = id;
            Nome = nome;
            Endereco = endereco;
            Ramal = ramal;
            EmailProfissional = emailProfissional;
            Departamento = departamento;
            Salario = salario;
            DataAdmissao = dataAdmissao;
        }

        public int Id { get; set; }

        [Required(ErrorMessage = "O campo Nome é obrigatório")]
        [MaxLength(100)]
        public string Nome { get; set; }

        [MaxLength(200)]
        public string Endereco { get; set; }

        [MaxLength(200)]
        public string Ramal { get; set; }

        [Required(ErrorMessage = "O campo Email Profissional é obrigatório")]
        [EmailAddress]
        [MaxLength(100)]   
        public string EmailProfissional { get; set; }

        [Required(ErrorMessage = "O campo Departamento é obrigatório")]
        [MaxLength(50)]
        public string Departamento { get; set; }

        [Column(TypeName = "decimal(18,2)")]

        [Range(0, double.MaxValue, ErrorMessage = "O campo Salário deve ser maior ou igual a zero")]
        public decimal Salario { get; set; }
        public DateTimeOffset? DataAdmissao { get; set; }
    }
}