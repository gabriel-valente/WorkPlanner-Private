using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trabalhos
{
    public class Trabalho
    {
        public string ChaveTrabalho { get; set; }
        public string ChaveCliente { get; set; }
        public string Nome { get; set; }
        public string Descricao { get; set; }
        public string Pago { get; set; }
        public long DataCriacao { get; set; }
    }

    public class TrabalhoTarefas
    {
        public string ChaveTarefa { get; set; }
        public string Tarefa { get; set; }
        public TimeSpan Tempo { get; set; }
        public string Preco { get; set; }
    }

    public class Tempo
    {
        public string ChaveTempo { get; set; }
        public string ChaveTarefa { get; set; }
        public DateTime DataInicio { get; set; }
        public DateTime? DataFim { get; set; }
    }

    public class Tarefa
    {
        public string ChaveTarefa { get; set; }
        public string ChaveTrabalho { get; set; }
        public string ChaveServico { get; set; }
        public decimal Desconto { get; set; }
    }

    public class Servico
    {
        public string ChaveServico { get; set; }
        public string Nome { get; set; }
        public decimal Preco { get; set; }
    }

    public class Cliente
    {
        public string ChaveCliente { get; set; }
        public string Nome { get; set; }
        public DateTime DataNascimento { get; set; }
        public string Sexo { get; set; }
        public string Morada { get; set; }
        public string CodigoPostal { get; set; }
        public string Localidade { get; set; }
        public string Email { get; set; }
        public long Telemovel { get; set; }
        public long Telefone { get; set; }
        public string Contacto { get; set; }
    }

    public class CodigoPostal
    {
        public long ChaveCodigoPostal { get; set; }
        public string CodPostal { get; set; }
        public string Localidade { get; set; }
        public string Rua { get; set; }
    }
}