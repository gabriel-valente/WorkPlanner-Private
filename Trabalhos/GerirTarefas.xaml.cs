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
        SqlCommand queryApagarTempo = new SqlCommand("DELETE FROM Tempo WHERE Key_Tarefa = @KeyTarefa");

        SqlDataReader Reader;

        DispatcherTimer temporizador = new DispatcherTimer();

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

        bool KeyValido = false;
        bool ServicoValido = false;
        bool DataInicioValido = false;
        bool DataFimValido = false;

        //Iniciação
        public GerirTarefas()
        {
            InitializeComponent();

            Lbl_Trabalho.Content = ("Tarefas do trabalho {0}", InterPages.KeyTrabalho);

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

            Lst_Tarefas.ItemsSource = listaTarefa;
            Lst_Tarefas.Items.Refresh();

            Cb_Servico.ItemsSource = servicos;
            Cb_Servico.Items.Refresh();
        }

        //Funçoes de butoes e lista
        //Butao adicionar nova tarefa
        private void Btn_AdicionarTarefa_Click(object sender, RoutedEventArgs e)
        {
            LimparCampos();

            Lbl_CodigoTarefa.Content = ReservarChave("Tarefa");

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
                Lbl_Desconto.Content = ("{0} %", Sld_Desconto.Value);
                Lbl_Preco.Content = ("{0} €", preco);

                Btn_AtualizarCliente.IsEnabled = true;

                try
                {
                    DataBase.conexao.Open();
                    queryProcurarTrabalhosCliente.Connection = DataBase.conexao;
                    queryProcurarTrabalhosCliente.Parameters.AddWithValue("@Key_Cliente", clientes[Lst_Clientes.SelectedIndex].ChaveCliente);

                    Reader = queryProcurarTrabalhosCliente.ExecuteReader();
                    queryProcurarTrabalhosCliente.Parameters.Clear();

                    if (Reader.HasRows)
                    {
                        Reader.Read();

                        trabalhosCliente = Convert.ToInt64(Reader["Contagem"].ToString());

                        if (trabalhosCliente > 0)
                        {
                            Btn_ApagarCliente.IsEnabled = false;
                        }
                        else if (trabalhosCliente == 0)
                        {
                            Btn_ApagarCliente.IsEnabled = true;
                        }
                    }
                    else
                    {
                        Btn_ApagarCliente.IsEnabled = true;
                    }

                    Reader.Close();
                    DataBase.conexao.Close();
                }
                catch (Exception ex)
                {
                    Lbl_Erros.Text = "Erro Inesperado!\nVerifique a lista de erros conhecidos.\nErro: " + ex;
                }
            }
            else if (Lst_Clientes.SelectedIndex == -1)
            {
                Btn_AtualizarCliente.IsEnabled = false;
                Btn_ApagarCliente.IsEnabled = false;
            }
        }

        //Funçoes de validação
        //Validar nome do cliente
        private void Tb_NomeCliente_TextChanged(object sender, TextChangedEventArgs e)
        {
            temporizador.Stop();

            Tb_NomeCliente.Text = Tb_NomeCliente.Text.TrimStart();
            char[] nome = Tb_NomeCliente.Text.ToCharArray();

            if (!string.IsNullOrEmpty(Tb_NomeCliente.Text.TrimStart()) && !char.IsUpper(nome[0]))
            {
                nome[0] = char.ToUpper(nome[0]);
                Tb_NomeCliente.Text = new string(nome);
                Tb_NomeCliente.SelectionStart = Tb_NomeCliente.Text.Length;
            }

            for (int i = 0; i < nome.Length; i++)
            {

                if (!ValidarNome.IsMatch(nome[i].ToString()) | (i >= 1 && char.IsWhiteSpace(nome[i]) & char.IsWhiteSpace(nome[i - 1])))
                {
                    Tb_NomeCliente.Text = Tb_NomeCliente.Text.Remove(i, 1);
                    Array.Clear(nome, 0, nome.Length);
                    nome = Tb_NomeCliente.Text.TrimStart().ToCharArray();
                    Tb_NomeCliente.SelectionStart = i;
                }
            }

            for (int i = 0; i < nome.Length; i++)
            {
                if (i > 0 && char.IsWhiteSpace(nome[i - 1]) && !char.IsUpper(nome[i]))
                {
                    nome[i] = char.ToUpper(nome[i]);
                    Tb_NomeCliente.Text = new string(nome);
                    Tb_NomeCliente.SelectionStart = i + 1;
                }

                if (i > 0 && char.IsLetter(nome[i - 1]) && char.IsUpper(nome[i]))
                {
                    nome[i] = char.ToLower(nome[i]);
                    Tb_NomeCliente.Text = new string(nome);
                    Tb_NomeCliente.SelectionStart = i + 1;
                }
            }

            NomeValido = false;

            temporizador.Start();

            AtualizarBotoes();
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
            }
            catch (Exception ex)
            {
                Lbl_Erros.Text = "Erro Inesperado!\nVerifique a lista de erros conhecidos.\nErro: " + ex;
                Btn_AdicionarTarefa.IsEnabled = false;
                Btn_AtualizarTarefa.IsEnabled = false;
            }

            Reader.Close();
            DataBase.conexao.Close();
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
            //if (!NomeValido || !PrecoValido)
            //{
            //    Btn_GuardarServico.IsEnabled = false;
            //    Btn_GuardarAlteracoes.IsEnabled = false;
            //}
            //else if (NomeValido && PrecoValido)
            //{
            //    Btn_GuardarServico.IsEnabled = true;
            //    Btn_GuardarAlteracoes.IsEnabled = true;
            //}
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
                    queryTodasTarefas.Parameters.AddWithValue("@KeyTarefa", key);
                    Reader = queryTodasTarefas.ExecuteReader();

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
