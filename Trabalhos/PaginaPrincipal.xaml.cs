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

namespace Trabalhos
{
    /// <summary>
    /// Interaction logic for PaginaPrincipal.xaml
    /// </summary>
    public partial class PaginaPrincipal : Page
    {
        public List<Conteudo2> lista = new List<Conteudo2>();

        public PaginaPrincipal()
        {
            InitializeComponent();

            int length = 50;

            for (int i = 0; i < length; i++)
            {
                lista.Add(new Conteudo2 { Cliente = "Cliente " + i.ToString(), Trabalho = "Trabalho " + i.ToString() });
            }

            Lst_TrabalhosConcluidos.ItemsSource = lista;
            Lst_TrabalhosNaoConcluidos.ItemsSource = lista;
            Lst_TrabalhosPagos.ItemsSource = lista;
            Lst_TrabalhosNaoPagos.ItemsSource = lista;

            Lst_TrabalhosConcluidos.Items.Refresh();
            Lst_TrabalhosNaoConcluidos.Items.Refresh();
            Lst_TrabalhosPagos.Items.Refresh();
            Lst_TrabalhosNaoPagos.Items.Refresh();
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
    }

    public class Conteudo2
    {
        public string Cliente { get; set; }
        public string Trabalho { get; set; }
    }
}
