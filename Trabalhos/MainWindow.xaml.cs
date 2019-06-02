using System;
using System.Collections.Generic;
using System.Configuration;
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

            try
            {
                Configuracoes.TempoCopia = Convert.ToInt32(ConfigurationManager.AppSettings["CopiaSeguranca"]);
                Configuracoes.LocalCopia = ConfigurationManager.AppSettings["LocalCopiaSeguranca"];
                Configuracoes.IdadeMinima = Convert.ToInt32(ConfigurationManager.AppSettings["IdadeMinima"]);
                Configuracoes.ContactoPreferivel = Convert.ToInt32(ConfigurationManager.AppSettings["ContactoPreferivel"]);
                Configuracoes.ServicoPrecoMinimo = Convert.ToDecimal(ConfigurationManager.AppSettings["ServicoPrecoMinimo"]);

                Frm_Principal.Content = new PaginaPrincipal();
            }
            catch (Exception)
            {
                Grd_PrimeiroLogin.Visibility = Visibility.Visible;
            }
        }

        private void Btn_EditarConfiguracao_Click(object sender, RoutedEventArgs e)
        {
            Grd_PrimeiroLogin.Visibility = Visibility.Hidden;
            Frm_Principal.Content = new Definicoes();
        }

        private void Btn_FecharPrograma_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        //private void Btn_Query_Click(object sender, RoutedEventArgs e)
        //{
        //    OleDbConnection myConnection = new OleDbConnection();
        //    SqlConnection conexao = new SqlConnection();
        //    string myConnectionString = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=E:\PAP\Trabalhos\Trabalhos\CodPostal.mdb";
        //    string conexaoString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=E:\PAP\Trabalhos\Trabalhos\WorkPlanner.mdf;Integrated Security=True";
        //    myConnection.ConnectionString = myConnectionString;
        //    conexao.ConnectionString = conexaoString;
        //    myConnection.Open();
        //    conexao.Open();

        //    OleDbCommand queryBuscar = new OleDbCommand("SELECT DISTINCT Localidades.LOCALIDADE, Arterias.ART_DESIG, COD_POSTAL.CP FROM(Arterias INNER JOIN (COD_POSTAL INNER JOIN Localidades ON Localidades.LLLL = COD_POSTAL.LLLL) ON COD_POSTAL.ART_COD = Arterias.ART_COD) ORDER BY COD_POSTAL.CP");
        //    SqlCommand queryInserir = new SqlCommand("INSERT INTO CodigoPostal(CodPostal, Localidade, Rua) VALUES (@CodPostal, @Localidade, @Rua)");
        //    SqlCommand queryApagar = new SqlCommand("DELETE FROM CodigoPostal");
        //    SqlCommand queryResetar = new SqlCommand("DBCC CHECKIDENT('CodigoPostal', RESEED, 0)");

        //    queryBuscar.Connection = myConnection;

        //    OleDbDataReader reader = queryBuscar.ExecuteReader();

        //    Console.WriteLine("Começou a Inserir Dados Na Lista!");

        //    while (reader.Read())
        //    {
        //        codPostal.Add(new CodigoPostal { CodPostal = Convert.ToString(reader["CP"].ToString()), Localidade = Convert.ToString(reader["LOCALIDADE"].ToString()), Rua = Convert.ToString(reader["ART_DESIG"].ToString()) });
        //    }

        //    Console.WriteLine("Tamanho da Lista: " + codPostal.Count);

        //    myConnection.Close();

        //    queryApagar.Connection = conexao;
        //    queryApagar.ExecuteNonQuery();
        //    queryResetar.Connection = conexao;
        //    queryResetar.ExecuteNonQuery();

        //    queryInserir.Connection = conexao;

        //    int index = 0;

        //    foreach (CodigoPostal item in codPostal)
        //    {
        //        index++;

        //        queryInserir.Parameters.AddWithValue("@CodPostal", item.CodPostal);
        //        queryInserir.Parameters.AddWithValue("@Localidade", item.Localidade);
        //        queryInserir.Parameters.AddWithValue("@Rua", item.Rua);

        //        int result = queryInserir.ExecuteNonQuery();

        //        queryInserir.Parameters.Clear();

        //        if (index % 1000 == 0)
        //        {
        //            Console.WriteLine("Inseriu: " + index);
        //        }

        //        if (result < 0)
        //            Console.WriteLine("Erro a Inserir Dados! ", result);
        //    }

        //    Console.WriteLine("Supostamente Inseriu Tudo!");
        //    conexao.Close();
        //}
    }
}
