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
using System.Data.SqlClient;
using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;

namespace Trabalhos
{
    /// <summary>
    /// Interaction logic for PaginaPrincipal.xaml
    /// </summary>
    public partial class PaginaPrincipal : Page
    {
        SqlCommand queryContarClientes = new SqlCommand("SELECT COUNT(Key_Cliente) AS 'Contagem' FROM Cliente");
        SqlCommand queryContarTrabalhos = new SqlCommand("SELECT COUNT(Key_Trabalho) AS 'Contagem' FROM Trabalho");
        SqlCommand queryContarServicos = new SqlCommand("SELECT COUNT(Key_Servico) AS 'Contagem' FROM Servico");

        SqlCommand queryTodosTrabalhos = new SqlCommand("SELECT Key_Trabalho, Pago FROM Trabalho");
        SqlCommand queryTodosServicos = new SqlCommand("SELECT Key_Servico, Preco FROM Servico");
        SqlCommand queryTodasTarefas = new SqlCommand("SELECT Key_Tarefa, Key_Trabalho, Key_Servico, Desconto FROM Tarefa");
        SqlCommand queryTodosTempos = new SqlCommand("SELECT Key_Tempo, Key_Tarefa, DataInicio, DataFim FROM Tempo");

        SqlDataReader Reader;

        public PaginaPrincipal()
        {
            InitializeComponent();

            DataBase.conexao = new SqlConnection(DataBase.stringConexao);

            CarregarCircular();
            CarregarLinhas();
        }

        private void Chart_OnDataClick(object sender, ChartPoint chartpoint)
        {
            var chart = (PieChart)chartpoint.ChartView;

            foreach (PieSeries series in chart.Series)
                series.PushOut = 1;

            var selectedSeries = (PieSeries)chartpoint.SeriesView;
            selectedSeries.PushOut = 8;
        }

        private void Btn_GerirClientes_Click(object sender, RoutedEventArgs e)
        {
            ((MainWindow)Application.Current.MainWindow).Frm_Principal.Content = new GerirClientes();
        }

        private void Btn_Definicoes_Click(object sender, RoutedEventArgs e)
        {
            ((MainWindow)Application.Current.MainWindow).Frm_Principal.Content = new Definicoes();
        }

        private void Btn_GerirServicos_Click(object sender, RoutedEventArgs e)
        {
            ((MainWindow)Application.Current.MainWindow).Frm_Principal.Content = new GerirServicos();
        }

        private void Btn_GerirTrabalhos_Click(object sender, RoutedEventArgs e)
        {
            ((MainWindow)Application.Current.MainWindow).Frm_Principal.Content = new GerirTrabalhos();
        }

        void CarregarCircular()
        {
            Func<ChartPoint, string> PieLabel = chartPoint => string.Format("{0}, {1} ({2:P})", chartPoint.SeriesView.Title, chartPoint.Y, chartPoint.Participation);

            double contagemClientes = 0;
            double contagemTrabalhos = 0;
            double contagemServicos = 0;

            try
            {
                DataBase.conexao.Open();
                queryContarClientes.Connection = DataBase.conexao;

                Reader = queryContarClientes.ExecuteReader();

                Reader.Read();
                contagemClientes = Convert.ToDouble(Reader["Contagem"].ToString());

                if (contagemClientes > 0)
                {
                    Chrt_Pie.Series.Add(new PieSeries { Title = "Clientes", StrokeThickness = 1, DataLabels = true, LabelPoint = PieLabel, Values = new ChartValues<double> { contagemClientes } });
                }

                Reader.Close();

                queryContarTrabalhos.Connection = DataBase.conexao;
                Reader = queryContarTrabalhos.ExecuteReader();

                Reader.Read();
                contagemTrabalhos = Convert.ToDouble(Reader["Contagem"].ToString());

                if (contagemTrabalhos > 0)
                {
                    Chrt_Pie.Series.Add(new PieSeries { Title = "Trabalhos", StrokeThickness = 1, DataLabels = true, LabelPoint = PieLabel, Values = new ChartValues<double> { contagemTrabalhos } });
                }

                Reader.Close();

                queryContarServicos.Connection = DataBase.conexao;
                Reader = queryContarServicos.ExecuteReader();

                Reader.Read();
                contagemServicos = Convert.ToDouble(Reader["Contagem"].ToString());

                if (contagemServicos > 0)
                {
                    Chrt_Pie.Series.Add(new PieSeries { Title = "Serviços", StrokeThickness = 1, DataLabels = true, LabelPoint = PieLabel, Values = new ChartValues<double> { contagemServicos } });
                }

                Reader.Close();
                queryContarClientes.Connection.Close();
                queryContarTrabalhos.Connection.Close();
                queryContarServicos.Connection.Close();
                DataBase.conexao.Close();
            }
            catch (Exception ex)
            {
            }

            if (contagemClientes == 0 && contagemTrabalhos == 0 && contagemServicos== 0)
            {
                Chrt_Pie.Visibility = Visibility.Hidden;
                Lbl_Pie.Visibility = Visibility.Hidden;
            }
        }

        void CarregarLinhas()
        {
            List<Tuple<string, decimal>> trabalhos = new List<Tuple<string, decimal>>();
            List<Tuple<string, decimal>> servicos = new List<Tuple<string, decimal>>();
            List<Tuple<string, string, string, decimal>> tarefas = new List<Tuple<string, string, string, decimal>>();
            List<Tuple<string, string, DateTime, DateTime>> tempo = new List<Tuple<string, string, DateTime, DateTime>>();

            DataBase.conexao.Open();
            queryTodosTrabalhos.Connection = DataBase.conexao;
            Reader = queryTodosTrabalhos.ExecuteReader();

            if (Reader.HasRows)
            {
                while (Reader.Read())
                {
                    trabalhos.Add(new Tuple<string, decimal>(Convert.ToString(Reader["Key_Trabalho"].ToString()), Convert.ToDecimal(Reader["Pago"].ToString())));
                }

                Reader.Close();


            }
            else
            {
                Chrt_Lines.Visibility = Visibility.Hidden;
                Lbl_Lines.Visibility = Visibility.Visible;
            }

            Reader.Close();
            DataBase.conexao.Close();
        }
    }
}
