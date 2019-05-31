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
using System.Windows.Threading;
using System.Data.SqlClient;

namespace Trabalhos
{
    /// <summary>
    /// Interaction logic for GerirTarefas.xaml
    /// </summary>
    public partial class GerirTarefas : Page
    {
        SqlCommand queryTodasTarefas = new SqlCommand("SELECT Key_Tarefa, Key_Trabalho, Key_Servico, Desconto FROM Tarefa WHERE Key_Trabalho = @KeyTrabalho");
        SqlCommand queryTodosTempos = new SqlCommand("SELECT Key_Tempo, Key_Tarefa, DataInicio, DataFim FROM Tempo WHERE Key_Tarefa = @KeyTarefa");
        SqlCommand queryTodosServicos = new SqlCommand("SELECT Key_Servico, Nome, Preco FROM Servico");
        SqlCommand queryIndexTarefa = new SqlCommand("SELECT Key_Tarefa FROM Tarefa WHERE Key_Tarefa = @KeyTarefa");
        SqlCommand queryIndexTempo = new SqlCommand("SELECT Key_Tempo FROM Tempo WHERE Key_Tempo = @KeyTempo");
        SqlCommand queryInserirTarefa = new SqlCommand("INSERT INTO Tarefa (Key_Tarefa, Key_Trabalho, Key_Servico, Desconto) VALUES (@KeyTarefa, @KeyTrabalho, @KeyServico, @Desconto)");
        SqlCommand queryInserirTempo = new SqlCommand("INSERT INTO Tempo (Key_Tempo, Key_Tarefa, DataInicio, DataFim) VALUES (@KeyTempo, @KeyTarefa, @DataInicio, @DataFim)");
        SqlCommand queryAtualizarTarefa = new SqlCommand("UPDATE Tarefa SET Key_Servico = @KeyServico, Desconto = @Desconto WHERE Key_Tarefa = @KeyTarefa");
        SqlCommand queryApagarTarefa = new SqlCommand("DELETE FROM Tarefa WHERE Key_Tarefa = @KeyTarefa");
        SqlCommand queryApagarTodosTempos = new SqlCommand("DELETE FROM Tempo WHERE Key_Tarefa = @KeyTarefa");
        SqlCommand queryApagarTempo = new SqlCommand("DELETE FROM Tempo WHERE Key_Tempo = @Key_Tempo");

        SqlDataReader Reader;

        DispatcherTimer temporizador = new DispatcherTimer();
        DispatcherTimer sliderTemp = new DispatcherTimer();

        List<Servico> servicos = new List<Servico>();
        List<Tarefa> tarefas = new List<Tarefa>();
        List<Tempo> tempos = new List<Tempo>();

        List<ListaTarefas> listaTarefa = new List<ListaTarefas>();
        List<ListaTempo> listaTempo = new List<ListaTempo>();

        //Gerar chave aleatória 
        public static string RandomString(int length)
        {
            Random random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            return new string(Enumerable.Range(1, length).Select(_ => chars[random.Next(chars.Length)]).ToArray());
        }

        DateTime DataInicio;
        DateTime DataFim;

        bool KeyValido = false;
        bool ServicoValido = false;
        bool TempoValido = false;
        bool DataInicioValido = false;
        bool DataFimValido = false;

        bool Adicionar = false;

        //Iniciação
        public GerirTarefas()
        {
            InitializeComponent();

              ////////////////////
             /// REMOVER ISTO ///
            ////////////////////
            InterPages.KeyTrabalho = "abc";

            Lbl_Trabalho.Content = "Tarefas do trabalho " + InterPages.KeyTrabalho;

            LigarBaseDados();

            foreach (Tarefa item in tarefas)
            {
                TimeSpan tempoDecorrido = new TimeSpan(0, 0, 0);
                foreach (Tempo itm in tempos)
                {
                    tempoDecorrido += itm.DataFim - itm.DataInicio;
                }

                listaTarefa.Add(new ListaTarefas { ChaveTarefa = item.ChaveTarefa, Servico = servicos.Find(lst => lst.ChaveServico == item.ChaveServico).Nome, Tempo = tempoDecorrido });
            }

            temporizador.Interval = new TimeSpan(0, 0, 1);
            temporizador.Tick += new EventHandler(Timer_Tick);

            sliderTemp.Interval = new TimeSpan(0, 0, 0, 0, 300);
            sliderTemp.Tick += new EventHandler(Slider_Tick);

            Lst_Tarefas.ItemsSource = listaTarefa;
            Lst_Tarefas.Items.Refresh();

            Cb_Servico.ItemsSource = servicos;
            Cb_Servico.DisplayMemberPath = "Nome";
            Cb_Servico.Items.Refresh();
        }

        //Funçoes de butoes e lista
        //Butao adicionar nova tarefa
        private void Btn_AdicionarTarefa_Click(object sender, RoutedEventArgs e)
        {
            decimal preco = 0;

            Adicionar = true;

            LimparCampos();

            if (EditarTarefaCampos.ChaveTarefa == null)
            {
                Lbl_CodigoTarefa.Content = ReservarChave("Tarefa");
            }
            else
            {
                Lbl_CodigoTarefa.Content = EditarTarefaCampos.ChaveTarefa;
            }

            Cb_Servico.SelectedItem = EditarTarefaCampos.Servico;

            listaTempo.Clear();

            foreach (Tempo item in EditarTarefaCampos.tempos)
            {
                TimeSpan tempoDecorrido;

                try
                {
                    tempoDecorrido = item.DataFim - item.DataInicio;
                    preco += servicos.Find(lst => lst.ChaveServico == tarefas.Find(lstt => lstt.ChaveTarefa == item.ChaveTarefa).ChaveServico).Preco;
                }
                catch (Exception)
                {
                    tempoDecorrido = new TimeSpan(0, 0, 0);
                }

                listaTempo.Add(new ListaTempo { ChaveTempo = item.ChaveTempo, DataInicio = item.DataInicio, DataFim = item.DataFim, TempoDecorrido = tempoDecorrido });
            }

            Lst_Tempo.ItemsSource = listaTempo;
            Lst_Tempo.Items.Refresh();

            Sld_Desconto.Value = EditarTarefaCampos.Desconto;
            Lbl_Preco.Content = preco * (1 - Functions.Clamp(Convert.ToDecimal(Sld_Desconto.Value))) + " €";

            Lbl_Servico.Visibility = Visibility.Hidden;
            Cb_Servico.Visibility = Visibility.Visible;
            Lst_Tempo.IsEnabled = true;
            Btn_AdicionarTempo.IsEnabled = true;
            Lbl_DataInicio.Visibility = Visibility.Hidden;
            Dp_DataInicio.Visibility = Visibility.Visible;
            Btn_LimparDataInicio.Visibility = Visibility.Visible;
            Btn_AtualDataInicio.Visibility = Visibility.Visible;
            Lbl_DataFim.Visibility = Visibility.Hidden;
            Dp_DataFim.Visibility = Visibility.Visible;
            Btn_LimparDataFim.Visibility = Visibility.Visible;
            Btn_AtualDataFim.Visibility = Visibility.Visible;
            Sld_Desconto.IsEnabled = true;
            Btn_GuardarTarefa.Visibility = Visibility.Visible;
            Btn_CancelarTarefa.Visibility = Visibility.Visible;
            Btn_AdicionarTarefa.Visibility = Visibility.Hidden;
            Btn_AtualizarTarefa.Visibility = Visibility.Hidden;
            Btn_ApagarTarefa.Visibility = Visibility.Hidden;
        }

        //Botao alterar tarefa selecionada
        private void Btn_AtualizarTarefa_Click(object sender, RoutedEventArgs e)
        {
            decimal preco = 0;

            KeyValido = false;
            ServicoValido = true;
            DataInicioValido = true;
            DataFimValido = true;

            temporizador.Start();

            Lbl_CodigoTarefa.Content = tarefas[Lst_Tarefas.SelectedIndex].ChaveTarefa;
            Cb_Servico.SelectedItem = servicos.Find(lista => lista.ChaveServico == tarefas[Lst_Tarefas.SelectedIndex].ChaveServico).Nome;

            listaTempo.Clear();

            foreach (Tempo item in tempos)
            {
                if (item.ChaveTarefa == tarefas[Lst_Tarefas.SelectedIndex].ChaveTarefa)
                {
                    TimeSpan tempoDecorrido;

                    try
                    {
                        tempoDecorrido = item.DataFim - item.DataInicio;
                        preco += servicos.Find(lst => lst.ChaveServico == tarefas.Find(lstt => lstt.ChaveTarefa == item.ChaveTarefa).ChaveServico).Preco;
                    }
                    catch (Exception)
                    {
                        tempoDecorrido = new TimeSpan(0, 0, 0);
                    }

                    listaTempo.Add(new ListaTempo { ChaveTempo = item.ChaveTempo, DataInicio = item.DataInicio, DataFim = item.DataFim, TempoDecorrido = tempoDecorrido } );
                }
            }   

            Lst_Tempo.ItemsSource = listaTempo;
            Lst_Tempo.Items.Refresh();

            Sld_Desconto.Value = Convert.ToDouble(tarefas[Lst_Tarefas.SelectedIndex].Desconto);
            Lbl_Desconto.Content = tarefas[Lst_Tarefas.SelectedIndex].Desconto;

            Lbl_Preco.Content = preco;

            Lbl_Servico.Visibility = Visibility.Hidden;
            Cb_Servico.Visibility = Visibility.Visible;
            Lst_Tempo.IsEnabled = true;
            Btn_AdicionarTempo.IsEnabled = true;
            Lbl_DataInicio.Visibility = Visibility.Hidden;
            Dp_DataInicio.Visibility = Visibility.Visible;
            Btn_LimparDataInicio.Visibility = Visibility.Visible;
            Btn_AtualDataInicio.Visibility = Visibility.Visible;
            Lbl_DataFim.Visibility = Visibility.Hidden;
            Dp_DataFim.Visibility = Visibility.Visible;
            Btn_LimparDataFim.Visibility = Visibility.Visible;
            Btn_AtualDataFim.Visibility = Visibility.Visible;
            Sld_Desconto.IsEnabled = true;
            Btn_GuardarTarefa.Visibility = Visibility.Visible;
            Btn_CancelarTarefa.Visibility = Visibility.Visible;
            Btn_AdicionarTarefa.Visibility = Visibility.Hidden;
            Btn_AtualizarTarefa.Visibility = Visibility.Hidden;
            Btn_ApagarTarefa.Visibility = Visibility.Hidden;
        }

        //Botao apagar tarefa selecionada
        private void Btn_ApagarTarefa_Click(object sender, RoutedEventArgs e)
        {
            BloquearFundo.Visibility = Visibility.Visible;
            Grd_ValidarApagar.Visibility = Visibility.Visible;
        }

        //Botao confirmar apagar tarefa
        private void Btn_ConfirmarApagar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DataBase.conexao.Open();
                queryApagarTempo.Connection = DataBase.conexao;
                queryApagarTempo.Parameters.AddWithValue("@KeyTarefa", tarefas[Lst_Tarefas.SelectedIndex].ChaveTarefa);

                Reader = queryApagarTempo.ExecuteReader();
                queryApagarTempo.Parameters.Clear();

                Reader.Close();

                tempos.RemoveAll(rem => rem.ChaveTarefa == tarefas[Lst_Tarefas.SelectedIndex].ChaveTarefa);
                Lst_Tempo.Items.Refresh();

                queryApagarTarefa.Connection = DataBase.conexao;
                queryApagarTarefa.Parameters.AddWithValue("@KeyTarefa", tarefas[Lst_Tarefas.SelectedIndex].ChaveTarefa);

                Reader = queryApagarTarefa.ExecuteReader();
                queryApagarTarefa.Parameters.Clear();

                Reader.Close();
                DataBase.conexao.Close();

                tarefas.RemoveAt(Lst_Tarefas.SelectedIndex);
                Lst_Tarefas.Items.Refresh();

                Lbl_Erros.Text = "A tarefa foi apagada com sucesso!";
                LimparCampos();
            }
            catch (Exception ex)
            {
                Lbl_Erros.Text = "Erro Inesperado!\nVerifique a lista de erros conhecidos.\nErro: " + ex;
            }
        }

        //Botao cancelar apagar tarefa
        private void Btn_CancelarApagar_Click(object sender, RoutedEventArgs e)
        {
            Grd_ValidarApagar.Visibility = Visibility.Hidden;
            BloquearFundo.Visibility = Visibility.Hidden;
            LimparCampos();
        }

        //Lista tarefas
        private void Lst_Tarefas_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Lst_Tarefas.SelectedIndex >= 0)
            {
                decimal preco = 0;
                DataBase.conexao = new SqlConnection(DataBase.stringConexao);

                queryTodosTempos.Connection = DataBase.conexao;
                queryTodasTarefas.Parameters.AddWithValue("@KeyTarefa", tarefas[Lst_Tarefas.SelectedIndex].ChaveTarefa);
                Reader = queryTodosTempos.ExecuteReader();

                while (Reader.Read())
                {
                    tempos.Add(new Tempo { ChaveTempo = Convert.ToString(Reader["Key_Tempo"].ToString()), ChaveTarefa = Convert.ToString(Reader["Key_Tarefa"].ToString()), DataInicio = Convert.ToDateTime(Reader["DataIncio"] as DateTime?), DataFim = Convert.ToDateTime(Reader["DataFim"] as DateTime?) });
                }

                Reader.Close();

                Lbl_CodigoTarefa.Content = tarefas[Lst_Tarefas.SelectedIndex].ChaveTarefa;
                Cb_Servico.SelectedItem = servicos.Find(lst => lst.ChaveServico == tarefas[Lst_Tarefas.SelectedIndex].ChaveServico).Nome;

                listaTempo.Clear();

                foreach (Tempo item in tempos)
                {
                    TimeSpan tempoDecorrido;

                    try
                    {
                        tempoDecorrido = item.DataFim - item.DataInicio;
                        preco += servicos.Find(lst => lst.ChaveServico == tarefas.Find(lstt => lstt.ChaveTarefa == item.ChaveTarefa).ChaveServico).Preco;
                    }
                    catch (Exception)
                    {
                        tempoDecorrido = new TimeSpan(0, 0, 0);
                    }

                    listaTempo.Add(new ListaTempo { ChaveTempo = item.ChaveTempo, DataInicio = item.DataInicio, DataFim = item.DataFim, TempoDecorrido = tempoDecorrido });
                }

                Lst_Tempo.ItemsSource = listaTempo;
                Lst_Tempo.Items.Refresh();

                Sld_Desconto.Value = Convert.ToDouble(tarefas[Lst_Tarefas.SelectedIndex].Desconto);
                Lbl_Desconto.Content = Math.Round(Sld_Desconto.Value, 2) + "%";
                Lbl_Preco.Content = preco * (1 - Functions.Clamp(Convert.ToDecimal(Sld_Desconto.Value))) + " €";

                Btn_AtualizarTarefa.IsEnabled = true;
                Btn_ApagarTarefa.IsEnabled = true;
            }
            else if (Lst_Tarefas.SelectedIndex == -1)
            {
                Btn_AtualizarTarefa.IsEnabled = false;
                Btn_ApagarTarefa.IsEnabled = false;
            }
        }

        //Lista Tempo e Validar
        private void Lst_Tempo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataInicio = tempos[Lst_Tempo.SelectedIndex].DataInicio;
            DataFim = tempos[Lst_Tempo.SelectedIndex].DataFim;

            if (!Lst_Tempo.HasItems)
            {
                TempoValido = false;
            }
            else
            {
                TempoValido = true;
            }

            AtualizarBotoes();
        }

        //Botao voltar á data anterior ou limpar data
        private void Btn_LimparDataInicio_Click(object sender, RoutedEventArgs e)
        {
            if (DataInicio != null)
            {
                Dp_DataInicio.SelectedDate = DataInicio;
                Dp_DataInicio.DisplayDate = DataInicio;
            }
            else
            {
                Dp_DataInicio.SelectedDate = null;
                Dp_DataInicio.DisplayDate = DateTime.Today;
            }
        }

        //Botao colocar data atual
        private void Btn_AtualDataInicio_Click(object sender, RoutedEventArgs e)
        {
            Dp_DataInicio.SelectedDate = DateTime.Now;
            Dp_DataInicio.DisplayDate = DateTime.Now;
        }

        //Botao voltar á data anterior ou limpar data
        private void Btn_LimparDataFim_Click(object sender, RoutedEventArgs e)
        {
            if (DataFim != null)
            {
                Dp_DataFim.SelectedDate = DataFim;
                Dp_DataFim.DisplayDate = DataFim;
            }
            else
            {
                Dp_DataFim.SelectedDate = null;
                Dp_DataFim.DisplayDate = DateTime.Today;
            }
        }

        //Botao colocar data atual
        private void Btn_AtualDataFim_Click(object sender, RoutedEventArgs e)
        {
            Dp_DataFim.SelectedDate = DateTime.Now;
            Dp_DataFim.DisplayDate = DateTime.Now;
        }

        //Botao voltar para o menu principal
        private void Btn_Voltar_Click(object sender, RoutedEventArgs e)
        {
            if (Adicionar)
            {
                
                DateTime? dataInicio = Dp_DataInicio.SelectedDate;
                DateTime? dataFim = Dp_DataFim.SelectedDate;

                if (Dp_DataInicio.Text == null)
                {
                    dataInicio = Convert.ToDateTime("01/01/0001 00:00:00");
                }

                if (Dp_DataFim.Text == null)
                {
                    dataFim = Convert.ToDateTime("01/01/0001 00:00:00");
                }

                EditarTarefaCampos.ChaveTarefa = Convert.ToString(Lbl_CodigoTarefa.Content);
                EditarTarefaCampos.Servico = Cb_Servico.Text;

                EditarTarefaCampos.tempos.Clear();

                foreach (ListaTempo item in listaTempo)
                {
                    EditarTarefaCampos.tempos.Add(new Tempo { ChaveTempo = item.ChaveTempo, ChaveTarefa = null, DataInicio = item.DataInicio, DataFim = item.DataFim });
                }

                EditarTarefaCampos.DataInicio = dataInicio;
                EditarTarefaCampos.DataFim = dataFim;
                EditarTarefaCampos.Desconto = Sld_Desconto.Value;
            }

            ((MainWindow)Application.Current.MainWindow).Frm_Principal.GoBack();
        }

        //Funçoes de validação
        //Validar servico
        private void Cb_Servico_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Cb_Servico.SelectedIndex == -1)
            {
                ServicoValido = false;
            }
            else
            {
                ServicoValido = true;
            }

            AtualizarBotoes();
        }

        //Validar Data inicio
        private void Dp_DataInicio_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Dp_DataInicio.SelectedDate > DateTime.Now)
            {
                Dp_DataInicio.SelectedDate = null;
                Dp_DataInicio.DisplayDate = DateTime.Now;
                Lbl_Erros.Text = "A data de inicio não pode ser após a este momento!";
            }
            else if (Dp_DataInicio.SelectedDate == Convert.ToDateTime(null))
            {
                Dp_DataInicio.SelectedDate = null;
                Dp_DataInicio.DisplayDate = DateTime.Now;
                Lbl_Erros.Text = "A data de inicio tem de ter um valor.";
            }
            else if (Dp_DataInicio.SelectedDate >= Dp_DataFim.SelectedDate && Dp_DataFim.SelectedDate != Convert.ToDateTime(null))
            {
                Dp_DataInicio.SelectedDate = null;
                Dp_DataInicio.DisplayDate = DateTime.Now;
                Lbl_Erros.Text = "A data de inicio não pode ser após a data de fim";
            }
            else
            {
                Lbl_Erros.Text = null;
            }

            DateTime value;

            if (DateTime.TryParse(Dp_DataInicio.Text, out value))
            {
                DataInicioValido = true;
            }
            else
            {
                DataInicioValido = false;
            }

            AtualizarBotoes();
        }

        //Validar Data fim
        private void Dp_DataFim_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Dp_DataFim.SelectedDate > DateTime.Now)
            {
                Dp_DataInicio.SelectedDate = null;
                Dp_DataInicio.DisplayDate = DateTime.Now;
                Lbl_Erros.Text = "A data de fim não pode ser após a este momento!";
            }
            else if (Dp_DataInicio.SelectedDate >= Dp_DataFim.SelectedDate)
            {
                Dp_DataInicio.SelectedDate = null;
                Dp_DataInicio.DisplayDate = DateTime.Now;
                Lbl_Erros.Text = "A data de fim não pode ser antes da data de inicio!";
            }
            else
            {
                Lbl_Erros.Text = null;
            }

            DateTime value;

            if (Dp_DataFim.Text == null)
            {
                DataFimValido = true;
            }
            else if (DateTime.TryParse(Dp_DataFim.Text, out value))
            {
                DataFimValido = true;
            }
            else
            {
                DataFimValido = false;
            }

            AtualizarBotoes();
        }

        //Atribuir valor do slider ao label
        private void Sld_Desconto_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            sliderTemp.Stop();

            Lbl_Desconto.Content = Math.Round(Sld_Desconto.Value, 2) + "%";

            sliderTemp.Start();
        }

        //Funçoes gerais
        //Ligar com base de dados e ler todos os dados
        void LigarBaseDados()
        {
            try
            {
                DataBase.conexao = new SqlConnection(DataBase.stringConexao);

                DataBase.conexao.Open();
                queryTodasTarefas.Connection = DataBase.conexao;
                queryTodasTarefas.Parameters.AddWithValue("@KeyTrabalho", InterPages.KeyTrabalho);
                Reader = queryTodasTarefas.ExecuteReader();

                while (Reader.Read())
                {
                    tarefas.Add(new Tarefa { ChaveTarefa = Convert.ToString(Reader["Key_Tarefa"].ToString()), ChaveTrabalho = Convert.ToString(Reader["Key_Trabalho"].ToString()), ChaveServico = Convert.ToInt32(Reader["Key_Servico"].ToString()), Desconto = Convert.ToDecimal(Reader["Desconto"].ToString()) });
                }

                Reader.Close();

                queryTodosServicos.Connection = DataBase.conexao;
                Reader = queryTodosServicos.ExecuteReader();

                while (Reader.Read())
                {
                    servicos.Add(new Servico { ChaveServico = Convert.ToInt32(Reader["Key_Servico"].ToString()), Nome = Convert.ToString(Reader["Nome"].ToString()), Preco = Convert.ToDecimal(Reader["Preco"].ToString()) });
                }

                Reader.Close();
                DataBase.conexao.Close();
            }
            catch (Exception ex)
            {
                Lbl_Erros.Text = "Erro Inesperado!\nVerifique a lista de erros conhecidos.\nErro: " + ex;
                Btn_AdicionarTarefa.IsEnabled = false;
                Btn_AtualizarTarefa.IsEnabled = false;
            }
        }

        //Limpar todos os campos que estao indroduzidos
        void LimparCampos()
        {
            Lbl_CodigoTarefa.Content = null;
            Lbl_Servico.Content = null;
            Cb_Servico.Text = null;
            Cb_Servico.SelectedIndex = -1;
            Lst_Tempo.Items.Clear();
            Lst_Tempo.SelectedIndex = -1;
            Lbl_DataInicio.Content = null;
            Dp_DataInicio.SelectedDate = null;
            Dp_DataInicio.DisplayDate = DateTime.Now;
            Dp_DataFim.SelectedDate = null;
            Dp_DataFim.DisplayDate = DateTime.Now;
            Sld_Desconto.Value = 0;
            Lbl_Desconto.Content = null;
            Lbl_Preco.Content = null;
            Lst_Tarefas.SelectedIndex = -1;
            Lbl_Erros.Text = null;
            temporizador.Stop();
        }

        //Atualiza os botoes caso os campos estejam incorretos ou corretos
        void AtualizarBotoes()
        {
            if (!DataInicioValido || !DataFimValido)
            {
                Btn_AdicionarTempo.IsEnabled = false;
            }
            else if (DataInicioValido && DataFimValido)
            {
                Btn_AdicionarTempo.IsEnabled = true;
            }

            if (!KeyValido || !ServicoValido || !TempoValido)
            {
                Btn_AdicionarTarefa.IsEnabled = false;
            }
            else if (KeyValido && ServicoValido && TempoValido)
            {
                Btn_AdicionarTarefa.IsEnabled = true;
            }
        }

        //Verificar se chave existe na base de dados (Tabela: Tarefa, Tempo)
        string ReservarChave(string Tabela)
        {
            string key;

            DataBase.conexao = new SqlConnection(DataBase.stringConexao);
            DataBase.conexao.Open();

            while (true)
            {
                if (Tabela == "Tarefa")
                {
                    key = RandomString(20);
                    queryIndexTarefa.Connection = DataBase.conexao;
                    queryIndexTarefa.Parameters.AddWithValue("@KeyTarefa", key);
                    Reader = queryIndexTarefa.ExecuteReader();

                    if (!Reader.HasRows)
                    {
                        break;
                    }
                }
                else if (Tabela == "Tempo")
                {
                    key = RandomString(30);
                    queryIndexTempo.Connection = DataBase.conexao;
                    queryIndexTempo.Parameters.AddWithValue("@KeyTempo", key);
                    Reader = queryIndexTempo.ExecuteReader();

                    if (!Reader.HasRows)
                    {
                        break;
                    }
                }

                Reader.Close();
            }

            return key;
        }

        //Chama função para validar o nome aoo fim de 1 segundos
        private void Timer_Tick(object sender, EventArgs e)
        {
            temporizador.Stop();

            //VerificarServico();
        }

        //Chama função para atualizar o preço ao fim de 300ms
        private void Slider_Tick(object sender, EventArgs e)
        {
            sliderTemp.Stop();

            decimal preco = 0;

            foreach (Tempo item in EditarTarefaCampos.tempos)
            {
                preco += servicos.Find(lst => lst.ChaveServico == tarefas.Find(lstt => lstt.ChaveTarefa == item.ChaveTarefa).ChaveServico).Preco;
            }

            Lbl_Preco.Content = preco * (1 - Functions.Clamp(Convert.ToDecimal(Sld_Desconto.Value))) + " €";
        }
    }

    class ListaTarefas
    {
        public string ChaveTarefa { get; set; }
        public string Servico { get; set; }
        public TimeSpan Tempo { get; set; }
    }

    class ListaTempo
    {
        public string ChaveTempo { get; set; }
        public DateTime DataInicio { get; set; }
        public DateTime DataFim { get; set; }
        public TimeSpan TempoDecorrido { get; set; }
    }
}
