using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace Trabalhos
{
    public static class Functions
    {
        public static decimal Clamp(decimal value, decimal max = 100, decimal clampMax = 1)
        {
            return value * clampMax / max;
        }
    }

    public static class InterPages
    {
        public static string KeyTrabalho;
        public static string NomeTrabalho;
    }

    public static class DataBase
    {
        public static SqlConnection conexao;
#if DEBUG
        public static string stringConexao = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=E:\PAP\Trabalhos\Trabalhos\WorkPlanner.mdf;Integrated Security=True;MultipleActiveResultSets=True"; //E:\PAP\Trabalhos\Trabalhos\WorkPlanner.mdf //C:\Users\Muhdex\source\repos\Muhdo\Trabalhos\Trabalhos\WorkPlanner.mdf
#else
        public static string stringConexao = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\WorkPlanner.mdf;Integrated Security=True;MultipleActiveResultSets=True";
#endif
    }

    public static class Configuracoes
    {
        public static int TempoCopia;
        public static string LocalCopia;
        public static int IdadeMinima;
        public static int ContactoPreferivel;
        public static decimal ServicoPrecoMinimo;
    }

    public static class EditarClienteCampos
    {
        public static string ChaveCliente;
        public static string Nome;
        public static DateTime? DataNascimento;
        public static string Sexo;
        public static string Morada;
        public static string CodigoPostal;
        public static string Localidade;
        public static string Email;
        public static long Telemovel;
        public static long Telefone;
    }

    public static class EditarServicoCampos
    {
        public static string ChaveServico;
        public static string Nome;
        public static decimal? Preco;
    }

    public static class EditarTarefaCampos
    {
        public static string ChaveTarefa;
        public static string Servico;
        public static List<Tempo> tempos = new List<Tempo>();
        public static DateTime? DataInicio;
        public static DateTime? DataFim;
        public static double Desconto;
    }

    public static class EditarTrabalhoCampos
    {
        public static string ChaveTrabalho;
        public static string Cliente;
        public static string Trabalho;
        public static string Descricao;
        public static List<TrabalhoTarefas> Tarefas = new List<TrabalhoTarefas>(); 
    }
}
