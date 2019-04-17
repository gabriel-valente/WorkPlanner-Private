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
    /// Interaction logic for GerirTrabalhos.xaml
    /// </summary>
    public partial class GerirTrabalhos : Page
    {
        public GerirTrabalhos()
        {
            InitializeComponent();

            List<Tarefa> tarefas = new List<Tarefa>();

            tarefas.Add(new Tarefa { ChaveTarefa = "asdfasd", ChaveTrabalho = "fdsgfdgsdff", ChaveServico = 21, DataInicio = DateTime.Now.AddHours(-1), DataFim = DateTime.Now, TempoDecorrido = TimeSpan.FromHours(1.50), Desconto = Convert.ToDecimal(0.3), Preco = Convert.ToDecimal(3.2) });
            tarefas.Add(new Tarefa { ChaveTarefa = "asdfasd", ChaveTrabalho = "fdsgfdgsdff", ChaveServico = 21, DataInicio = DateTime.Now.AddHours(-1), DataFim = DateTime.Now, TempoDecorrido = TimeSpan.FromHours(1.50), Desconto = Convert.ToDecimal(0.3), Preco = Convert.ToDecimal(3.2) });
            tarefas.Add(new Tarefa { ChaveTarefa = "asdfasd", ChaveTrabalho = "fdsgfdgsdff", ChaveServico = 21, DataInicio = DateTime.Now.AddHours(-1), DataFim = DateTime.Now, TempoDecorrido = TimeSpan.FromHours(1.50), Desconto = Convert.ToDecimal(0.3), Preco = Convert.ToDecimal(3.2) });
            tarefas.Add(new Tarefa { ChaveTarefa = "asdfasd", ChaveTrabalho = "fdsgfdgsdff", ChaveServico = 21, DataInicio = DateTime.Now.AddHours(-1), DataFim = DateTime.Now, TempoDecorrido = TimeSpan.FromHours(1.50), Desconto = Convert.ToDecimal(0.3), Preco = Convert.ToDecimal(3.2) });
            tarefas.Add(new Tarefa { ChaveTarefa = "asdfasd", ChaveTrabalho = "fdsgfdgsdff", ChaveServico = 21, DataInicio = DateTime.Now.AddHours(-1), DataFim = DateTime.Now, TempoDecorrido = TimeSpan.FromHours(1.50), Desconto = Convert.ToDecimal(0.3), Preco = Convert.ToDecimal(3.2) });
            tarefas.Add(new Tarefa { ChaveTarefa = "asdfasd", ChaveTrabalho = "fdsgfdgsdff", ChaveServico = 21, DataInicio = DateTime.Now.AddHours(-1), DataFim = DateTime.Now, TempoDecorrido = TimeSpan.FromHours(1.50), Desconto = Convert.ToDecimal(0.3), Preco = Convert.ToDecimal(3.2) });
            tarefas.Add(new Tarefa { ChaveTarefa = "asdfasd", ChaveTrabalho = "fdsgfdgsdff", ChaveServico = 21, DataInicio = DateTime.Now.AddHours(-1), DataFim = DateTime.Now, TempoDecorrido = TimeSpan.FromHours(1.50), Desconto = Convert.ToDecimal(0.3), Preco = Convert.ToDecimal(3.2) });
            tarefas.Add(new Tarefa { ChaveTarefa = "asdfasd", ChaveTrabalho = "fdsgfdgsdff", ChaveServico = 21, DataInicio = DateTime.Now.AddHours(-1), DataFim = DateTime.Now, TempoDecorrido = TimeSpan.FromHours(1.50), Desconto = Convert.ToDecimal(0.3), Preco = Convert.ToDecimal(3.2) });
            tarefas.Add(new Tarefa { ChaveTarefa = "asdfasd", ChaveTrabalho = "fdsgfdgsdff", ChaveServico = 21, DataInicio = DateTime.Now.AddHours(-1), DataFim = DateTime.Now, TempoDecorrido = TimeSpan.FromHours(1.50), Desconto = Convert.ToDecimal(0.3), Preco = Convert.ToDecimal(3.2) });

            Lst_Tarefas.ItemsSource = tarefas;
        }
    }
}
