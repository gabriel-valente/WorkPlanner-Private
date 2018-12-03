using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using System.Data.OleDb;
using System.Data.SqlClient;

namespace Trabalhos
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //List<CodigoPostal> codPostal = new List<CodigoPostal>();

        public MainWindow()
        {
            InitializeComponent();

            Frm_Principal.Content = new PaginaPrincipal();
        }

        private void Btn_Query_Click(object sender, RoutedEventArgs e)
        {
            //OleDbConnection myConnection = new OleDbConnection();
            //SqlConnection conexao = new SqlConnection();
            //string myConnectionString = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=E:\PAP\Trabalhos\Trabalhos\CodPostal.mdb";
            //string conexaoString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=E:\PAP\Trabalhos\Trabalhos\Trabalhos.mdf;Integrated Security=True";
            //myConnection.ConnectionString = myConnectionString;
            //conexao.ConnectionString = conexaoString;
            //myConnection.Open();
            //conexao.Open();

            //OleDbCommand queryBuscar = new OleDbCommand("SELECT DISTINCT Localidades.LOCALIDADE, Arterias.ART_DESIG, COD_POSTAL.CP FROM(Arterias INNER JOIN (COD_POSTAL INNER JOIN Localidades ON Localidades.LLLL = COD_POSTAL.LLLL) ON COD_POSTAL.ART_COD = Arterias.ART_COD) ORDER BY COD_POSTAL.CP");
            //SqlCommand queryInserir = new SqlCommand("INSERT INTO CodigoPostal(CodPostal, Localidade, Rua) VALUES (@CodPostal, @Localidade, @Rua)");
            //SqlCommand queryApagar = new SqlCommand("DELETE FROM CodigoPostal");
            //SqlCommand queryResetar = new SqlCommand("DBCC CHECKIDENT('CodigoPostal', RESEED, 0)");

            //queryBuscar.Connection = myConnection;

            //OleDbDataReader reader = queryBuscar.ExecuteReader();

            //System.Diagnostics.Debug.WriteLine("Começou a Inserir Dados Na Lista!");

            //while (reader.Read())
            //{
            //    codPostal.Add(new CodigoPostal { CodPostal = Convert.ToString(reader["CP"].ToString()), Localidade = Convert.ToString(reader["LOCALIDADE"].ToString()), Rua = Convert.ToString(reader["ART_DESIG"].ToString()) });
            //}

            //System.Diagnostics.Debug.WriteLine("Tamanho da Lista: " + codPostal.Count);

            //myConnection.Close();

            //queryApagar.Connection = conexao;
            //queryApagar.ExecuteNonQuery();
            //queryResetar.Connection = conexao;
            //queryResetar.ExecuteNonQuery();

            //queryInserir.Connection = conexao;

            //int index = 0;

            //foreach (CodigoPostal item in codPostal)
            //{
            //    index++;

            //    queryInserir.Parameters.AddWithValue("@CodPostal", item.CodPostal);
            //    queryInserir.Parameters.AddWithValue("@Localidade", item.Localidade);
            //    queryInserir.Parameters.AddWithValue("@Rua", item.Rua);

            //    int result = queryInserir.ExecuteNonQuery();

            //    queryInserir.Parameters.Clear();

            //    if (index % 100 == 0)
            //    {
            //        System.Diagnostics.Debug.WriteLine("Inseriu: " + index);
            //    }

            //    if (result < 0)
            //        System.Diagnostics.Debug.WriteLine("Erro a Inserir Dados! ", result);
            //}

            //System.Diagnostics.Debug.WriteLine("Supostamente Inseriu Tudo!");
            //conexao.Close();
        }

        //private void Btn_VerificarDados_Click(object sender, RoutedEventArgs e)
        //{
        //    SqlConnection conexao;
        //    string stringConexao = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|Trabalhos.mdf; Integrated Security=True";
        //    SqlCommand queryContarClientes = new SqlCommand("SELECT COUNT(Key_Cliente) AS 'Clientes' FROM Cliente");
        //    SqlCommand queryContarTrabalhos = new SqlCommand("SELECT COUNT(Key_Trabalho) AS 'Trabalhos' FROM Trabalho");
        //    SqlCommand queryContarServicos = new SqlCommand("SELECT COUNT(Key_Servico) AS 'Servicos' FROM Servico");
        //    SqlDataReader Reader;

        //    ContagemBaseDados quantidades = new ContagemBaseDados();

        //    Btn_VerificarDados.Visibility = Visibility.Hidden;

        //    try
        //    {
        //        conexao = new SqlConnection(stringConexao);

        //        conexao.Open();
        //        queryContarClientes.Connection = conexao;
        //        Reader = queryContarClientes.ExecuteReader();

        //        Reader.Read();
        //        quantidades.QuantidadeClientes = Convert.ToInt32(Reader["Clientes"].ToString());

        //        Reader.Close();

        //        queryContarTrabalhos.Connection = conexao;
        //        Reader = queryContarTrabalhos.ExecuteReader();

        //        Reader.Read();
        //        quantidades.QuantidadeTrabalhos = Convert.ToInt32(Reader["Trabalhos"].ToString());

        //        Reader.Close();

        //        queryContarServicos.Connection = conexao;
        //        Reader = queryContarServicos.ExecuteReader();

        //        Reader.Read();
        //        quantidades.QuantidadeServicos = Convert.ToInt32(Reader["Servicos"].ToString());

        //        Reader.Close();
        //        conexao.Close();

        //        Tb_Dados.Visibility = Visibility.Visible;

        //        Tb_Dados.DataContext = quantidades;
        //    }
        //    catch (Exception ex)
        //    {
        //        System.Diagnostics.Debug.WriteLine(ex.ToString());
        //        Tb_Erro.Visibility = Visibility.Visible;
        //    }
        //}

        //private void Btn_EditarConfiguracao_Click(object sender, RoutedEventArgs e)
        //{
        //    Frm_Principal.Content = new Definicoes();
        //    Grd_SemFicheiro.Visibility = Visibility.Hidden;
        //}

        //private void Btn_FecharPrograma_Click(object sender, RoutedEventArgs e)
        //{
        //    this.Close();
        //}
    }

    public class ContagemBaseDados
    {
        public int QuantidadeClientes { get; set; }
        public int QuantidadeTrabalhos { get; set; }
        public int QuantidadeServicos { get; set; }
    }
}
