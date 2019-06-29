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

namespace Trabalhos
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            try
            {
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
    }
}
