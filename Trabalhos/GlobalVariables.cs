using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace Trabalhos
{
    public static class InterPages
    {
        public static string KeyTrabalho;
    }

    public static class DataBase
    {
        public static SqlConnection conexao;
        public static string stringConexao = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|Trabalhos.mdf; Integrated Security=True";
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
        public static long ChaveCliente;
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
        public static long ChaveServico;
        public static string Nome;
        public static decimal? Preco;
    }
}
