using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data.SqlClient;

namespace Trabalhos
{
    /// <summary>
    /// Interaction logic for GerirTarefas.xaml
    /// </summary>
    public partial class GerirTarefas : Page
    {
        SqlCommand queryTodasTarefas = new SqlCommand("SELECT Key_Tarefa, Key_Trabalho, Tarefa.Key_Servico AS 'Servico', Desconto FROM Tarefa INNER JOIN Servico ON Tarefa.Key_Servico = Servico.Key_Servico WHERE Key_Trabalho = @KeyTrabalho ORDER BY Nome");
        SqlCommand queryTodosTempos = new SqlCommand("SELECT Key_Tempo, Key_Tarefa, DataInicio, DataFim FROM Tempo WHERE Key_Tarefa = @KeyTarefa ORDER BY DataInicio, DataFim");
        SqlCommand queryTodosServicos = new SqlCommand("SELECT Key_Servico, Nome, Preco FROM Servico ORDER BY Nome");
        SqlCommand queryIndexTarefa = new SqlCommand("SELECT Key_Tarefa FROM Tarefa WHERE Key_Tarefa = @KeyTarefa");
        SqlCommand queryIndexTempo = new SqlCommand("SELECT Key_Tempo FROM Tempo WHERE Key_Tempo = @KeyTempo");
        SqlCommand queryInserirTarefa = new SqlCommand("INSERT INTO Tarefa (Key_Tarefa, Key_Trabalho, Key_Servico, Desconto) VALUES (@KeyTarefa, @KeyTrabalho, @KeyServico, @Desconto)");
        SqlCommand queryInserirTempo = new SqlCommand("INSERT INTO Tempo (Key_Tempo, Key_Tarefa, DataInicio, DataFim) VALUES (@KeyTempo, @KeyTarefa, @DataInicio, @DataFim)");
        SqlCommand queryAtualizarTarefa = new SqlCommand("UPDATE Tarefa SET Key_Servico = @KeyServico, Desconto = @Desconto WHERE Key_Tarefa = @KeyTarefa");
        SqlCommand queryAtualizarTempo = new SqlCommand("UPDATE Tempo SET DataInicio = @DataInicio, DataFim = @DataFim WHERE Key_Tempo = @KeyTempo");
        SqlCommand queryApagarTarefa = new SqlCommand("DELETE FROM Tarefa WHERE Key_Tarefa = @KeyTarefa");
        SqlCommand queryApagarTodosTempos = new SqlCommand("DELETE FROM Tempo WHERE Key_Tarefa = @KeyTarefa");
        SqlCommand queryApagarTempo = new SqlCommand("DELETE FROM Tempo WHERE Key_Tempo = @KeyTempo");

        SqlDataReader Reader;

        List<Servico> servicos = new List<Servico>();
        List<Tarefa> tarefas = new List<Tarefa>();
        List<Tempo> tempos = new List<Tempo>();

        List<ListaTarefas> listaTarefa = new List<ListaTarefas>();
        List<ListaTempo> listaTempo = new List<ListaTempo>();

        List<Tuple<string, string>> temposAlterados = new List<Tuple<string, string>>();

        //Gerar chave aleatória 
        public static string RandomString(int length)
        {
            Random random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            return new string(Enumerable.Range(1, length).Select(_ => chars[random.Next(chars.Length)]).ToArray());
        }

        DateTime? DataInicio = null;
        DateTime? DataFim = null;

        decimal preco = 0;

        string ChaveTarefaServico = null;

        bool ServicoValido = false;
        bool TempoValido = false;
        bool DataInicioValido = false;
        bool DataFimValido = true;

        bool Adicionar = false;
        bool ServicoGuardado = false;

        //Iniciação
        public GerirTarefas()
        {
            InitializeComponent();

            this.Language = XmlLanguage.GetLanguage("pt-PT");

            if (InterPages.KeyTrabalho != null)
            {
                Lbl_Trabalho.Content = "Tarefas do trabalho " + InterPages.NomeTrabalho;

                LigarBaseDados();

                DataBase.conexao.Open();
                queryTodosTempos.Connection = DataBase.conexao;

                foreach (Tarefa item in tarefas)
                {
                    queryTodosTempos.Parameters.AddWithValue("@KeyTarefa", item.ChaveTarefa);
                    Reader = queryTodosTempos.ExecuteReader();
                    queryTodosTempos.Parameters.Clear();

                    TimeSpan tempoDecorrido = new TimeSpan(0, 0, 0);

                    while (Reader.Read())
                    {
                        try
                        {
                            tempoDecorrido += Convert.ToDateTime(Reader["DataFim"].ToString()) - Convert.ToDateTime(Reader["DataInicio"].ToString());
                        }
                        catch (Exception)
                        {
                            tempoDecorrido += new TimeSpan(0);
                        }
                    }

                    Reader.Close();

                    listaTarefa.Add(new ListaTarefas { ChaveTarefa = item.ChaveTarefa, Servico = servicos.Find(lst => lst.ChaveServico == item.ChaveServico).Nome, Tempo = TimeSpan.Parse(String.Format("{0:00}:{1:00}:{2:00}", tempoDecorrido.Hours, tempoDecorrido.Minutes, tempoDecorrido.Seconds)) });
                }

                queryTodosTempos.Connection.Close();
                DataBase.conexao.Close();

                Lst_Tarefas.ItemsSource = listaTarefa;
                Lst_Tarefas.Items.Refresh();

                Cb_Servico.ItemsSource = servicos;
                Cb_Servico.DisplayMemberPath = "Nome";
                Cb_Servico.Items.Refresh();
            }
            else
            {
                Btn_AdicionarTarefa.IsEnabled = false;
                Lbl_Erros.Text = "Não foi possivel receber o código do trabalho!\nReinicie o programa e volte a tentar!\nSe fechar o programa, as alterações no menu de gestão de trabalhos, vão ser perdidas.";
            }
        }

        //Funçoes de butoes e lista
        //Butao adicionar nova tarefa
        private void Btn_AdicionarTarefa_Click(object sender, RoutedEventArgs e)
        {
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

            Cb_Servico.Text = EditarTarefaCampos.Servico;

            tempos.Clear();
            listaTempo.Clear();

            preco = 0;
            decimal valor;

            try
            {
                valor = servicos.Find(lst => lst.Nome == EditarTarefaCampos.Servico).Preco;
            }
            catch (Exception)
            {
                valor = 0;
            }

            foreach (Tempo item in EditarTarefaCampos.tempos)
            {
                DateTime data;
                TimeSpan tempo;

                if (item.DataFim == Convert.ToDateTime("01/01/0001 00:00:00") || item.DataFim == Convert.ToDateTime(null))
                {
                    data = Convert.ToDateTime(null);
                    tempo = new TimeSpan(0, 0, 0);
                }
                else
                {
                    data = Convert.ToDateTime(item.DataFim as DateTime?);
                    tempo = Convert.ToDateTime(item.DataFim as DateTime?) - item.DataInicio;
                    preco += valor * Convert.ToDecimal(TimeSpan.Parse(Convert.ToString(tempo)).TotalHours);
                }

                tempos.Add(new Tempo { ChaveTempo = item.ChaveTempo, ChaveTarefa = EditarTarefaCampos.ChaveTarefa, DataInicio = item.DataInicio, DataFim = data });
                listaTempo.Add(new ListaTempo { ChaveTempo = item.ChaveTempo, DataInicio = item.DataInicio, DataFim = data, TempoDecorrido = String.Format("{0:00}:{1:00}:{2:00}", tempo.Hours, tempo.Minutes, tempo.Seconds) });
            }

            Dp_DataInicio.SelectedDate = EditarTarefaCampos.DataInicio;
            Dp_DataInicio.DisplayDate = Convert.ToDateTime(EditarTarefaCampos.DataInicio);
            Dp_DataFim.SelectedDate = EditarTarefaCampos.DataFim;
            Dp_DataFim.DisplayDate = Convert.ToDateTime(EditarTarefaCampos.DataFim);

            Lst_Tempo.ItemsSource = listaTempo;
            Lst_Tempo.Items.Refresh();

            Sld_Desconto.Value = EditarTarefaCampos.Desconto;
            Tb_Desconto.Text = String.Format("{0:##0.00}%", Math.Round(Sld_Desconto.Value, 2));
            Lbl_Preco.Content = String.Format("{0:###0.00}€", preco * (1 - Functions.Clamp(Convert.ToDecimal(Sld_Desconto.Value))));

            Lbl_Servico.Visibility = Visibility.Hidden;
            Cb_Servico.Visibility = Visibility.Visible;
            Lst_Tempo.IsEnabled = true;
            Btn_AdicionarTempo.IsEnabled = false;
            Btn_AdicionarTempo.Visibility = Visibility.Visible;
            Btn_ApagarTempo.Visibility = Visibility.Visible;
            Lbl_DataInicio.Visibility = Visibility.Hidden;
            Dp_DataInicio.Visibility = Visibility.Visible;
            Btn_LimparDataInicio.Visibility = Visibility.Visible;
            Btn_AtualDataInicio.Visibility = Visibility.Visible;
            Lbl_DataFim.Visibility = Visibility.Hidden;
            Dp_DataFim.Visibility = Visibility.Visible;
            Btn_LimparDataFim.Visibility = Visibility.Visible;
            Btn_AtualDataFim.Visibility = Visibility.Visible;
            Sld_Desconto.Visibility = Visibility.Visible;
            Sld_Desconto.IsEnabled = true;
            Tb_Desconto.IsReadOnly = false;
            Lst_Tarefas.IsEnabled = false;
            Btn_GuardarTarefa.Visibility = Visibility.Visible;
            Btn_CancelarTarefa.Visibility = Visibility.Visible;
            Btn_AdicionarTarefa.Visibility = Visibility.Hidden;
            Btn_AtualizarTarefa.Visibility = Visibility.Hidden;
            Btn_ApagarTarefa.Visibility = Visibility.Hidden;
        }

        //Botao alterar tarefa selecionada
        private void Btn_AtualizarTarefa_Click(object sender, RoutedEventArgs e)
        {
            Adicionar = false;

            ServicoValido = true;
            TempoValido = true;
            DataInicioValido = true;
            DataFimValido = true;

            Lbl_CodigoTarefa.Content = tarefas[Lst_Tarefas.SelectedIndex].ChaveTarefa;
            Cb_Servico.SelectedItem = servicos.Find(lista => lista.ChaveServico == tarefas[Lst_Tarefas.SelectedIndex].ChaveServico).Nome;

            listaTempo.Clear();
            temposAlterados.Clear();

            decimal valor = servicos.Find(lst => lst.ChaveServico == tarefas[Lst_Tarefas.SelectedIndex].ChaveServico).Preco;
            preco = 0;
            foreach (Tempo item in tempos)
            {
                if (item.ChaveTarefa == tarefas[Lst_Tarefas.SelectedIndex].ChaveTarefa)
                {
                    TimeSpan tempoDecorrido;

                    try
                    {
                        if (item.DataFim == Convert.ToDateTime("01/01/0001 00:00:00") || item.DataFim == Convert.ToDateTime(null))
                        {
                            tempoDecorrido = new TimeSpan(0);
                        }
                        else
                        {
                            tempoDecorrido = Convert.ToDateTime(item.DataFim as DateTime?) - item.DataInicio;
                        }

                        preco += valor * Convert.ToDecimal(TimeSpan.Parse(Convert.ToString(tempoDecorrido)).TotalHours);
                    }
                    catch (Exception)
                    {
                        tempoDecorrido = new TimeSpan(0, 0, 0);
                    }

                    listaTempo.Add(new ListaTempo { ChaveTempo = item.ChaveTempo, DataInicio = item.DataInicio, DataFim = Convert.ToDateTime(item.DataFim as DateTime?), TempoDecorrido = String.Format("{0:00}:{1:00}:{2:00}", tempoDecorrido.Hours, tempoDecorrido.Minutes, tempoDecorrido.Seconds) } );
                }
            }   

            Lst_Tempo.ItemsSource = listaTempo;
            Lst_Tempo.Items.Refresh();

            Sld_Desconto.Value = Convert.ToDouble(tarefas[Lst_Tarefas.SelectedIndex].Desconto * 100);
            Tb_Desconto.Text = String.Format("{0:##0.00}%", Math.Round(Sld_Desconto.Value, 2));

            Lbl_Preco.Content = String.Format("{0:###0.00}€", preco * (1 - Functions.Clamp(Convert.ToDecimal(Sld_Desconto.Value))));

            Lbl_Servico.Visibility = Visibility.Hidden;
            Cb_Servico.Visibility = Visibility.Visible;
            Lst_Tempo.IsEnabled = true;
            Btn_AdicionarTempo.IsEnabled = false;
            Btn_AdicionarTempo.Visibility = Visibility.Visible;
            Btn_ApagarTempo.Visibility = Visibility.Visible;
            Lbl_DataInicio.Visibility = Visibility.Hidden;
            Dp_DataInicio.Visibility = Visibility.Visible;
            Btn_LimparDataInicio.Visibility = Visibility.Visible;
            Btn_AtualDataInicio.Visibility = Visibility.Visible;
            Lbl_DataFim.Visibility = Visibility.Hidden;
            Dp_DataFim.Visibility = Visibility.Visible;
            Btn_LimparDataFim.Visibility = Visibility.Visible;
            Btn_AtualDataFim.Visibility = Visibility.Visible;
            Sld_Desconto.Visibility = Visibility.Visible;
            Sld_Desconto.IsEnabled = true;
            Tb_Desconto.IsReadOnly = false;
            Lst_Tarefas.IsEnabled = false;
            Btn_GuardarAlteracoes.Visibility = Visibility.Visible;
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
                queryApagarTodosTempos.Connection = DataBase.conexao;
                queryApagarTodosTempos.Parameters.AddWithValue("@KeyTarefa", tarefas[Lst_Tarefas.SelectedIndex].ChaveTarefa);

                Reader = queryApagarTodosTempos.ExecuteReader();
                queryApagarTodosTempos.Parameters.Clear();

                Reader.Close();

                tempos.Clear();
                listaTempo.Clear();
                Lst_Tempo.Items.Refresh();

                queryApagarTarefa.Connection = DataBase.conexao;
                queryApagarTarefa.Parameters.AddWithValue("@KeyTarefa", tarefas[Lst_Tarefas.SelectedIndex].ChaveTarefa);

                Reader = queryApagarTarefa.ExecuteReader();
                queryApagarTarefa.Parameters.Clear();

                Reader.Close();
                queryApagarTarefa.Connection.Close();
                DataBase.conexao.Close();

                tarefas.RemoveAt(Lst_Tarefas.SelectedIndex);
                listaTarefa.RemoveAt(Lst_Tarefas.SelectedIndex);
                Lst_Tarefas.Items.Refresh();

                Lbl_Erros.Text = "A tarefa foi apagada com sucesso!";
                LimparCampos();

                BloquearFundo.Visibility = Visibility.Hidden;
                Grd_ValidarApagar.Visibility = Visibility.Hidden;
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
        }

        //Lista tarefas
        private void Lst_Tarefas_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Lst_Tarefas.SelectedIndex >= 0)
            {
                tempos.Clear();
                listaTempo.Clear();

                DataBase.conexao.Open();

                queryTodosTempos.Connection = DataBase.conexao;
                queryTodosTempos.Parameters.AddWithValue("@KeyTarefa", tarefas[Lst_Tarefas.SelectedIndex].ChaveTarefa);
                Reader = queryTodosTempos.ExecuteReader();
                queryTodosTempos.Parameters.Clear();

                while (Reader.Read())
                {
                    tempos.Add(new Tempo { ChaveTempo = Convert.ToString(Reader["Key_Tempo"].ToString()), ChaveTarefa = Convert.ToString(Reader["Key_Tarefa"].ToString()), DataInicio = Convert.ToDateTime(Reader["DataInicio"] as DateTime?), DataFim = Convert.ToDateTime(Reader["DataFim"] as DateTime?) });
                }

                Reader.Close();
                queryTodosTempos.Connection.Close();

                DataBase.conexao.Close();

                Lbl_CodigoTarefa.Content = tarefas[Lst_Tarefas.SelectedIndex].ChaveTarefa;
                Cb_Servico.Text = servicos.Find(lst => lst.ChaveServico == tarefas[Lst_Tarefas.SelectedIndex].ChaveServico).Nome;
                Lbl_Servico.Content = servicos.Find(lst => lst.ChaveServico == tarefas[Lst_Tarefas.SelectedIndex].ChaveServico).Nome;

                decimal valor = servicos.Find(lst => lst.ChaveServico == tarefas[Lst_Tarefas.SelectedIndex].ChaveServico).Preco;
                preco = 0;

                DateTime inicio = tempos.First().DataInicio;
                DateTime? fim = tempos.First().DataFim;

                foreach (Tempo item in tempos)
                {
                    TimeSpan tempoDecorrido = new TimeSpan(0);

                    try
                    {
                        if (item.DataInicio < inicio)
                        {
                            inicio = item.DataInicio;
                        }

                        if (item.DataFim > fim)
                        {
                            fim = item.DataFim;
                        }

                        if (item.DataFim == Convert.ToDateTime("01/01/0001 00:00:00") || item.DataFim == Convert.ToDateTime(null))
                        {
                            tempoDecorrido = new TimeSpan(0);
                        }
                        else
                        {
                            tempoDecorrido = Convert.ToDateTime(item.DataFim as DateTime?) - item.DataInicio;
                        }

                        preco += valor * Convert.ToDecimal(TimeSpan.Parse(Convert.ToString(tempoDecorrido)).TotalHours);
                    }
                    catch (Exception)
                    {
                        tempoDecorrido = new TimeSpan(0, 0, 0);
                    }

                    listaTempo.Add(new ListaTempo { ChaveTempo = item.ChaveTempo, DataInicio = item.DataInicio, DataFim = Convert.ToDateTime(item.DataFim as DateTime?), TempoDecorrido = String.Format("{0:00}:{1:00}:{2:00}", tempoDecorrido.Hours, tempoDecorrido.Minutes, tempoDecorrido.Seconds) });
                }

                Lst_Tempo.ItemsSource = listaTempo;
                Lst_Tempo.Items.Refresh();

                Lbl_DataInicio.Content = inicio;
                Lbl_DataFim.Content = fim;

                Sld_Desconto.Visibility = Visibility.Visible;
                Sld_Desconto.Value = Convert.ToDouble(tarefas[Lst_Tarefas.SelectedIndex].Desconto * 100);
                Tb_Desconto.Text = String.Format("{0:##0.00}%", Math.Round(Sld_Desconto.Value, 2));
                Lbl_Preco.Content = String.Format("{0:###0.00}€", preco * (1 - Functions.Clamp(Convert.ToDecimal(Sld_Desconto.Value))));

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
            if (Lst_Tempo.SelectedIndex > -1)
            {
                Dp_DataInicio.SelectedDate = null;
                Dp_DataInicio.DisplayDate = DateTime.Today;
                Dp_DataFim.SelectedDate = null;
                Dp_DataFim.DisplayDate = DateTime.Today;

                DataInicio = listaTempo[Lst_Tempo.SelectedIndex].DataInicio.AddMilliseconds(-listaTempo[Lst_Tempo.SelectedIndex].DataInicio.Millisecond);
                DataFim = listaTempo[Lst_Tempo.SelectedIndex].DataFim.AddMilliseconds(-listaTempo[Lst_Tempo.SelectedIndex].DataFim.Millisecond + 1);

                if (!Lst_Tempo.HasItems)
                {
                    TempoValido = false;
                }
                else
                {
                    TempoValido = true;
                }

                Dp_DataInicio.SelectedDate = DataInicio;
                Dp_DataFim.SelectedDate = DataFim;

                Btn_EditarTempo.Visibility = Visibility.Visible;
                Btn_AdicionarTempo.Visibility = Visibility.Hidden;
                Btn_ApagarTempo.IsEnabled = true;
            }
            else
            {
                Btn_AdicionarTempo.Visibility = Visibility.Visible;
                Btn_EditarTempo.Visibility = Visibility.Hidden;
                Btn_ApagarTempo.IsEnabled = false;
            }

            AtualizarBotoes();
        }

        //Botao guardar tarefa
        private void Btn_GuardarTarefa_Click(object sender, RoutedEventArgs e)
        {
            string keyServico = servicos.Find(lst => lst.Nome == Cb_Servico.Text.ToString()).ChaveServico;
            decimal desconto = Convert.ToDecimal(Math.Round(Sld_Desconto.Value, 2)) / 100;
            TimeSpan time = new TimeSpan(0);

            if (ServicoGuardado)
            {
                try
                {
                    DataBase.conexao.Open();
                    queryAtualizarTarefa.Connection = DataBase.conexao;
                    queryAtualizarTarefa.Parameters.AddWithValue("@KeyServico", keyServico);
                    queryAtualizarTarefa.Parameters.AddWithValue("@Desconto", desconto);
                    queryAtualizarTarefa.Parameters.AddWithValue("@KeyTarefa", ChaveTarefaServico);

                    queryAtualizarTarefa.ExecuteNonQuery();
                    queryAtualizarTarefa.Parameters.Clear();
                    queryAtualizarTarefa.Connection.Close();

                    queryInserirTempo.Connection = DataBase.conexao;

                    foreach (ListaTempo item in listaTempo)
                    {
                        queryInserirTempo.Parameters.AddWithValue("@KeyTempo", item.ChaveTempo);
                        queryInserirTempo.Parameters.AddWithValue("@KeyTarefa", ChaveTarefaServico);
                        queryInserirTempo.Parameters.AddWithValue("@DataInicio", item.DataInicio);

                        if (item.DataFim != Convert.ToDateTime(null))
                        {
                            queryInserirTempo.Parameters.AddWithValue("@DataFim", item.DataFim);
                        }
                        else
                        {
                            queryInserirTempo.Parameters.AddWithValue("@DataFim", DBNull.Value);
                        }

                        queryInserirTempo.ExecuteNonQuery();
                        queryInserirTempo.Parameters.Clear();
                    }

                    queryInserirTempo.Connection.Close();

                    time = new TimeSpan(0);

                    queryTodosTempos.Connection = DataBase.conexao;
                    queryTodosTempos.Parameters.AddWithValue("@KeyTarefa", Lbl_CodigoTarefa.Content.ToString());
                    Reader = queryTodosTempos.ExecuteReader();
                    queryTodosTempos.Parameters.Clear();

                    while (Reader.Read())
                    {
                        if (Convert.ToDateTime(Reader["DataFim"] as DateTime?) != Convert.ToDateTime(null))
                        {
                            time += Convert.ToDateTime(Reader["DataFim"] as DateTime?) - Convert.ToDateTime(Reader["DataInicio"] as DateTime?);
                        }
                    }

                    Reader.Close();
                    queryTodosTempos.Connection.Close();

                    DataBase.conexao.Close();

                    int index = tarefas.FindIndex(lst => lst.ChaveTarefa == Lbl_CodigoTarefa.Content.ToString());

                    tarefas[index].Desconto = desconto;
                    listaTarefa[index].Tempo = TimeSpan.Parse(String.Format("{0:00}:{1:00}:{2:00}", time.Hours, time.Minutes, time.Seconds));
                }
                catch (Exception ex)
                {
                    Lbl_Erros.Text = "Erro Inesperado!\nVerifique a lista de erros conhecidos.\nErro: " + ex;
                }
            }
            else
            {
                try
                {
                    DataBase.conexao.Open();
                    queryInserirTarefa.Connection = DataBase.conexao;
                    queryInserirTarefa.Parameters.AddWithValue("@KeyTarefa", Lbl_CodigoTarefa.Content.ToString());
                    queryInserirTarefa.Parameters.AddWithValue("@KeyTrabalho", InterPages.KeyTrabalho);
                    queryInserirTarefa.Parameters.AddWithValue("@KeyServico", keyServico);
                    queryInserirTarefa.Parameters.AddWithValue("@Desconto", desconto);

                    queryInserirTarefa.ExecuteNonQuery();
                    queryInserirTarefa.Parameters.Clear();

                    queryInserirTempo.Connection = DataBase.conexao;

                    foreach (ListaTempo item in listaTempo)
                    {
                        queryInserirTempo.Parameters.AddWithValue("@KeyTempo", item.ChaveTempo);
                        queryInserirTempo.Parameters.AddWithValue("@KeyTarefa", Lbl_CodigoTarefa.Content.ToString());
                        queryInserirTempo.Parameters.AddWithValue("@DataInicio", item.DataInicio);

                        if (item.DataFim != Convert.ToDateTime(null))
                        {
                            time += item.DataFim - item.DataInicio;

                            queryInserirTempo.Parameters.AddWithValue("@DataFim", item.DataFim);
                        }
                        else
                        {
                            queryInserirTempo.Parameters.AddWithValue("@DataFim", DBNull.Value);
                        }

                        queryInserirTempo.ExecuteNonQuery();
                        queryInserirTempo.Parameters.Clear();
                    }

                    queryInserirTempo.Connection.Close();
                    DataBase.conexao.Close();

                    tarefas.Add(new Tarefa { ChaveTarefa = Lbl_CodigoTarefa.Content.ToString(), ChaveTrabalho = InterPages.KeyTrabalho, ChaveServico = keyServico, Desconto = desconto });

                    listaTarefa.Add(new ListaTarefas { ChaveTarefa = Lbl_CodigoTarefa.Content.ToString(), Servico = Cb_Servico.Text, Tempo = TimeSpan.Parse(String.Format("{0:00}:{1:00}:{2:00}", time.Hours, time.Minutes, time.Seconds)) });
                }
                catch (Exception ex)
                {
                    Lbl_Erros.Text = "Erro Inesperado!\nVerifique a lista de erros conhecidos.\nErro: " + ex;
                }
            }

            tempos.Clear();
            listaTempo.Clear();

            Lst_Tarefas.Items.Refresh();
            LimparCampos();

            Adicionar = false;
            Lbl_Servico.Visibility = Visibility.Visible;
            Cb_Servico.Visibility = Visibility.Hidden;
            Lst_Tempo.IsEnabled = false;
            Btn_AdicionarTempo.Visibility = Visibility.Hidden;
            Btn_ApagarTempo.Visibility = Visibility.Hidden;
            Lbl_DataInicio.Visibility = Visibility.Visible;
            Dp_DataInicio.Visibility = Visibility.Hidden;
            Btn_LimparDataInicio.Visibility = Visibility.Hidden;
            Btn_AtualDataInicio.Visibility = Visibility.Hidden;
            Lbl_DataFim.Visibility = Visibility.Visible;
            Dp_DataFim.Visibility = Visibility.Hidden;
            Btn_LimparDataFim.Visibility = Visibility.Hidden;
            Btn_AtualDataFim.Visibility = Visibility.Hidden;
            Sld_Desconto.Visibility = Visibility.Hidden;
            Sld_Desconto.IsEnabled = false;
            Tb_Desconto.IsReadOnly = true;
            Lst_Tarefas.IsEnabled = true;
            Btn_GuardarTarefa.Visibility = Visibility.Hidden;
            Btn_CancelarTarefa.Visibility = Visibility.Hidden;
            Btn_AdicionarTarefa.Visibility = Visibility.Visible;
            Btn_AtualizarTarefa.Visibility = Visibility.Visible;
            Btn_ApagarTarefa.Visibility = Visibility.Visible;
        }

        //Botao guardar alteraçoes na tarefa
        private void Btn_GuardarAlteracoes_Click(object sender, RoutedEventArgs e)
        {
            if (Lst_Tempo.HasItems)
            {
                string keyServico = servicos.Find(lst => lst.Nome == Cb_Servico.Text.ToString()).ChaveServico;
                decimal desconto = Convert.ToDecimal(Math.Round(Sld_Desconto.Value, 2)) / 100;
                TimeSpan time = new TimeSpan(0);

                try
                {
                    DataBase.conexao.Open();
                    queryAtualizarTarefa.Connection = DataBase.conexao;
                    queryAtualizarTarefa.Parameters.AddWithValue("@KeyServico", keyServico);
                    queryAtualizarTarefa.Parameters.AddWithValue("@Desconto", desconto);
                    queryAtualizarTarefa.Parameters.AddWithValue("@KeyTarefa", Lbl_CodigoTarefa.Content.ToString());

                    queryAtualizarTarefa.ExecuteNonQuery();
                    queryAtualizarTarefa.Parameters.Clear();

                    queryInserirTempo.Connection = DataBase.conexao;
                    queryAtualizarTempo.Connection = DataBase.conexao;
                    queryApagarTempo.Connection = DataBase.conexao;

                    foreach (Tuple<string, string> item in temposAlterados)
                    {
                        if (item.Item2 == "Add")
                        {
                            queryInserirTempo.Parameters.AddWithValue("@KeyTempo", item.Item1);
                            queryInserirTempo.Parameters.AddWithValue("@KeyTarefa", Lbl_CodigoTarefa.Content.ToString());
                            queryInserirTempo.Parameters.AddWithValue("@DataInicio", tempos.Find(lst => lst.ChaveTempo == item.Item1).DataInicio);

                            if (tempos.Find(lst => lst.ChaveTempo == item.Item1).DataFim != Convert.ToDateTime(null))
                            {
                                queryInserirTempo.Parameters.AddWithValue("@DataFim", tempos.Find(lst => lst.ChaveTempo == item.Item1).DataFim);
                            }
                            else
                            {
                                queryInserirTempo.Parameters.AddWithValue("@DataFim", DBNull.Value);
                            }

                            queryInserirTempo.ExecuteNonQuery();
                            queryInserirTempo.Parameters.Clear();
                        }
                        else if (item.Item2 == "Edit")
                        {
                            queryAtualizarTempo.Parameters.AddWithValue("@DataInicio", tempos.Find(lst => lst.ChaveTempo == item.Item1).DataInicio);
                            queryAtualizarTempo.Parameters.AddWithValue("@DataFim", tempos.Find(lst => lst.ChaveTempo == item.Item1).DataFim);
                            queryAtualizarTempo.Parameters.AddWithValue("@KeyTempo", item.Item1);

                            queryAtualizarTempo.ExecuteNonQuery();
                            queryAtualizarTempo.Parameters.Clear();
                        }
                        else if (item.Item2 == "Delete")
                        {
                            queryApagarTempo.Parameters.AddWithValue("@KeyTempo", item.Item1);

                            queryApagarTempo.ExecuteNonQuery();
                            queryApagarTempo.Parameters.Clear();
                        }
                    }

                    queryInserirTempo.Connection.Close();
                    queryAtualizarTempo.Connection.Close();
                    queryApagarTempo.Connection.Close();

                    DataBase.conexao.Close();

                    tarefas[Lst_Tarefas.SelectedIndex].ChaveServico = keyServico;
                    tarefas[Lst_Tarefas.SelectedIndex].Desconto = desconto;

                    listaTarefa[Lst_Tarefas.SelectedIndex].Servico = Cb_Servico.Text.ToString();

                    TimeSpan tempo = new TimeSpan(0);

                    foreach (ListaTempo item in listaTempo)
                    {
                        tempo += TimeSpan.Parse(item.TempoDecorrido);
                    }

                    listaTarefa[Lst_Tarefas.SelectedIndex].Tempo = tempo;

                    tempos.Clear();
                    listaTempo.Clear();

                    Lst_Tarefas.Items.Refresh();

                    LimparCampos();

                    Lbl_Servico.Visibility = Visibility.Visible;
                    Cb_Servico.Visibility = Visibility.Hidden;
                    Lst_Tempo.IsEnabled = false;
                    Btn_AdicionarTempo.Visibility = Visibility.Hidden;
                    Btn_ApagarTempo.Visibility = Visibility.Hidden;
                    Lbl_DataInicio.Visibility = Visibility.Visible;
                    Dp_DataInicio.Visibility = Visibility.Hidden;
                    Btn_LimparDataInicio.Visibility = Visibility.Hidden;
                    Btn_AtualDataInicio.Visibility = Visibility.Hidden;
                    Lbl_DataFim.Visibility = Visibility.Visible;
                    Dp_DataFim.Visibility = Visibility.Hidden;
                    Btn_LimparDataFim.Visibility = Visibility.Hidden;
                    Btn_AtualDataFim.Visibility = Visibility.Hidden;
                    Sld_Desconto.Visibility = Visibility.Visible;
                    Sld_Desconto.IsEnabled = false;
                    Tb_Desconto.IsReadOnly = true;
                    Lst_Tarefas.IsEnabled = true;
                    Btn_GuardarAlteracoes.Visibility = Visibility.Hidden;
                    Btn_CancelarTarefa.Visibility = Visibility.Hidden;
                    Btn_AdicionarTarefa.Visibility = Visibility.Visible;
                    Btn_AtualizarTarefa.Visibility = Visibility.Visible;
                    Btn_ApagarTarefa.Visibility = Visibility.Visible;
                }
                catch (Exception ex)
                {
                    Lbl_Erros.Text = "Erro Inesperado!\nVerifique a lista de erros conhecidos.\nErro: " + ex;
                }
            }
            else
            {
                Lbl_Erros.Text = "Não tem tempos inseridos!";
            }
        }

        //Botao voltar á data anterior ou limpar data
        private void Btn_LimparDataInicio_Click(object sender, RoutedEventArgs e)
        {
            if (DataInicio != null)
            {
                Dp_DataInicio.SelectedDate = Convert.ToDateTime(DataInicio);
                Dp_DataInicio.DisplayDate = Convert.ToDateTime(DataInicio);
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
            Dp_DataInicio.SelectedDate = DateTime.Now.AddMilliseconds(-DateTime.Now.Millisecond);
            Dp_DataInicio.DisplayDate = DateTime.Now.AddMilliseconds(-DateTime.Now.Millisecond);
        }

        //Botao voltar á data anterior ou limpar data
        private void Btn_LimparDataFim_Click(object sender, RoutedEventArgs e)
        {
            if (DataFim != null)
            {
                Dp_DataFim.SelectedDate = Convert.ToDateTime(DataFim);
                Dp_DataFim.DisplayDate = Convert.ToDateTime(DataFim);
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
            Dp_DataFim.SelectedDate = DateTime.Now.AddMilliseconds(-DateTime.Now.Millisecond + 1);
            Dp_DataFim.DisplayDate = DateTime.Now.AddMilliseconds(-DateTime.Now.Millisecond + 1);
        }

        //Botao adicionar tempo
        private void Btn_AdicionarTempo_Click(object sender, RoutedEventArgs e)
        {
            string key = ReservarChave("Tempo");
            DateTime data;
            TimeSpan tempo;

            try
            {
                data = Dp_DataFim.SelectedDate.Value;
                tempo = Dp_DataFim.SelectedDate.Value - Dp_DataInicio.SelectedDate.Value;
            }
            catch (Exception)
            {
                data = Convert.ToDateTime(null);
                tempo = new TimeSpan(0, 0, 0);
            }

            tempos.Add(new Tempo { ChaveTarefa = Lbl_CodigoTarefa.Content.ToString(), ChaveTempo = key, DataInicio = Dp_DataInicio.SelectedDate.Value, DataFim = data });
            listaTempo.Add(new ListaTempo { ChaveTempo = key, DataInicio = Dp_DataInicio.SelectedDate.Value, DataFim = data, TempoDecorrido = String.Format("{0:00}:{1:00}:{2:00}", tempo.Hours, tempo.Minutes, tempo.Seconds) });

            Lst_Tempo.Items.Refresh();

            decimal valor = 0;

            try
            {
                valor = servicos.Find(lst => lst.Nome == Cb_Servico.Text).Preco;
            }
            catch (Exception)
            {
            }

            preco = 0;
            foreach (Tempo item in tempos)
            {
                TimeSpan tempoDecorrido;

                if (item.DataFim == Convert.ToDateTime("01/01/0001 00:00:00") || item.DataFim == Convert.ToDateTime(null))
                {
                    tempoDecorrido = new TimeSpan(0);
                }
                else
                {
                    tempoDecorrido = Convert.ToDateTime(item.DataFim as DateTime?) - item.DataInicio;
                }

                preco += valor * Convert.ToDecimal(TimeSpan.Parse(Convert.ToString(tempoDecorrido)).TotalHours);
            }

            Lbl_Preco.Content = String.Format("{0:###0.00}€", preco * (1 - Functions.Clamp(Convert.ToDecimal(Sld_Desconto.Value))));

            bool existe = false;

            foreach (Tuple<string, string> item in temposAlterados)
            {
                if (key == item.Item1)
                {
                    existe = true;

                    break;
                }
            }

            if (!existe)
            {
                temposAlterados.Add(new Tuple<string, string>(key, "Add"));
            }

            Dp_DataInicio.SelectedDate = null;
            Dp_DataInicio.DisplayDate = DateTime.Today;
            Dp_DataFim.SelectedDate = null;
            Dp_DataFim.DisplayDate = DateTime.Today;

            DataInicioValido = false;
            DataFimValido = true;

            if (listaTempo.Count > 0)
            {
                TempoValido = true;
            }
            else
            {
                TempoValido = false;
            }

            AtualizarBotoes();
        }

        //Botao editar tempo
        private void Btn_EditarTempo_Click(object sender, RoutedEventArgs e)
        {
            DateTime data;
            TimeSpan tempo;

            try
            {
                data = Dp_DataFim.SelectedDate.Value;
                tempo = Dp_DataFim.SelectedDate.Value - Dp_DataInicio.SelectedDate.Value;
            }
            catch (Exception)
            {
                data = Convert.ToDateTime(null);
                tempo = new TimeSpan(0, 0, 0);
            }

            listaTempo[Lst_Tempo.SelectedIndex].DataInicio = Dp_DataInicio.SelectedDate.Value;
            listaTempo[Lst_Tempo.SelectedIndex].DataFim = data;
            listaTempo[Lst_Tempo.SelectedIndex].TempoDecorrido = String.Format("{0:00}:{1:00}:{2:00}", tempo.Hours, tempo.Minutes, tempo.Seconds);

            int index = tempos.FindIndex(lst => lst.ChaveTempo == listaTempo[Lst_Tempo.SelectedIndex].ChaveTempo);
            
            tempos[index].DataInicio = Dp_DataInicio.SelectedDate.Value;
            tempos[index].DataFim = data;

            bool existe = false;

            foreach (Tuple<string, string> item in temposAlterados)
            {
                if (listaTempo[Lst_Tempo.SelectedIndex].ChaveTempo == item.Item1)
                {
                    existe = true;

                    break;
                }
            }

            if (!existe)
            {
                temposAlterados.Add(new Tuple<string, string>(listaTempo[Lst_Tempo.SelectedIndex].ChaveTempo, "Edit"));
            }

            Lst_Tempo.Items.Refresh();
            Lst_Tempo.SelectedIndex = -1;
            Dp_DataInicio.SelectedDate = null;
            Dp_DataInicio.DisplayDate = DateTime.Today;
            Dp_DataFim.SelectedDate = null;
            Dp_DataFim.DisplayDate = DateTime.Today;

            DataInicioValido = false;
            DataFimValido = true;

            decimal valor = 0;

            try
            {
                valor = servicos.Find(lst => lst.Nome == Cb_Servico.Text).Preco;
            }
            catch (Exception)
            {
            }

            preco = 0;
            foreach (Tempo item in tempos)
            {
                TimeSpan tempoDecorrido;

                if (item.DataFim == Convert.ToDateTime("01/01/0001 00:00:00") || item.DataFim == Convert.ToDateTime(null))
                {
                    tempoDecorrido = new TimeSpan(0);
                }
                else
                {
                    tempoDecorrido = Convert.ToDateTime(item.DataFim as DateTime?) - item.DataInicio;
                }

                preco += valor * Convert.ToDecimal(TimeSpan.Parse(Convert.ToString(tempoDecorrido)).TotalHours);
            }

            Lbl_Preco.Content = String.Format("{0:###0.00}€", preco * (1 - Functions.Clamp(Convert.ToDecimal(Sld_Desconto.Value))));

            if (listaTempo.Count > 0)
            {
                TempoValido = true;
            }
            else
            {
                TempoValido = false;
            }

            AtualizarBotoes();
        }

        //Botao apagar tempo
        private void Btn_ApagarTempo_Click(object sender, RoutedEventArgs e) 
        {
            int index = tempos.FindIndex(lst => lst.ChaveTempo == listaTempo[Lst_Tempo.SelectedIndex].ChaveTempo);

            bool existe = false;

            foreach (Tuple<string, string> item in temposAlterados)
            {
                if (listaTempo[Lst_Tempo.SelectedIndex].ChaveTempo == item.Item1)
                {
                    existe = true;

                    break;
                }
            }

            if (!existe)
            {
                temposAlterados.Add(new Tuple<string, string>(listaTempo[Lst_Tempo.SelectedIndex].ChaveTempo, "Delete"));
            }

            tempos.RemoveAt(tempos.FindIndex(lst => lst.ChaveTempo == listaTempo[index].ChaveTempo));
            listaTempo.RemoveAt(Lst_Tempo.SelectedIndex);

            Lst_Tempo.Items.Refresh();
            Dp_DataInicio.SelectedDate = null;
            Dp_DataInicio.DisplayDate = DateTime.Today;
            Dp_DataFim.SelectedDate = null;
            Dp_DataFim.DisplayDate = DateTime.Today;

            DataInicioValido = false;
            DataFimValido = true;

            decimal valor = 0;

            try
            {
                valor = servicos.Find(lst => lst.Nome == Cb_Servico.Text).Preco;
            }
            catch (Exception)
            {
            }

            preco = 0;
            foreach (Tempo item in tempos)
            {
                TimeSpan tempoDecorrido;

                if (item.DataFim == Convert.ToDateTime("01/01/0001 00:00:00") || item.DataFim == Convert.ToDateTime(null))
                {
                    tempoDecorrido = new TimeSpan(0);
                }
                else
                {
                    tempoDecorrido = Convert.ToDateTime(item.DataFim as DateTime?) - item.DataInicio;
                }

                preco += valor * Convert.ToDecimal(TimeSpan.Parse(Convert.ToString(tempoDecorrido)).TotalHours);
            }

            Lbl_Preco.Content = String.Format("{0:###0.00}€", preco * (1 - Functions.Clamp(Convert.ToDecimal(Sld_Desconto.Value))));

            if (listaTempo.Count > 0)
            {
                TempoValido = true;
            }
            else
            {
                TempoValido = false;
            }

            AtualizarBotoes();
        }

        //Botao cancelar nova tarefa
        private void Btn_CancelarTarefa_Click(object sender, RoutedEventArgs e)
        {
            Adicionar = false;

            tempos.Clear();
            listaTempo.Clear();

            LimparCampos();

            ServicoValido = false;
            TempoValido = false;
            DataInicioValido = false;
            DataFimValido = true;
            Lbl_Servico.Visibility = Visibility.Visible;
            Cb_Servico.Visibility = Visibility.Hidden;
            Lst_Tempo.IsEnabled = false;
            Btn_AdicionarTempo.Visibility = Visibility.Hidden;
            Btn_ApagarTempo.Visibility = Visibility.Hidden;
            Lbl_DataInicio.Visibility = Visibility.Visible;
            Dp_DataInicio.Visibility = Visibility.Hidden;
            Btn_LimparDataInicio.Visibility = Visibility.Hidden;
            Btn_AtualDataInicio.Visibility = Visibility.Hidden;
            Lbl_DataFim.Visibility = Visibility.Visible;
            Dp_DataFim.Visibility = Visibility.Hidden;
            Btn_LimparDataFim.Visibility = Visibility.Hidden;
            Btn_AtualDataFim.Visibility = Visibility.Hidden;
            Sld_Desconto.Visibility = Visibility.Visible;
            Sld_Desconto.IsEnabled = false;
            Tb_Desconto.IsReadOnly = true;
            Lst_Tarefas.IsEnabled = true;
            Btn_GuardarTarefa.Visibility = Visibility.Hidden;
            Btn_GuardarAlteracoes.Visibility = Visibility.Hidden;
            Btn_CancelarTarefa.Visibility = Visibility.Hidden;
            Btn_AdicionarTarefa.Visibility = Visibility.Visible;
            Btn_AtualizarTarefa.Visibility = Visibility.Visible;
            Btn_ApagarTarefa.Visibility = Visibility.Visible;
        }

        //Botao voltar para o menu principal
        private void Btn_Voltar_Click(object sender, RoutedEventArgs e)
        {
            if (Adicionar)
            {
                
                DateTime? dataInicio = Dp_DataInicio.SelectedDate;
                DateTime? dataFim = Dp_DataFim.SelectedDate;              

                EditarTarefaCampos.ChaveTarefa = Lbl_CodigoTarefa.Content.ToString();
                EditarTarefaCampos.Servico = Cb_Servico.Text;

                EditarTarefaCampos.tempos.Clear();

                foreach (ListaTempo item in listaTempo)
                {
                    EditarTarefaCampos.tempos.Add(new Tempo { ChaveTempo = item.ChaveTempo, ChaveTarefa = Lbl_CodigoTarefa.Content.ToString(), DataInicio = item.DataInicio, DataFim = item.DataFim });
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
                string servico = servicos[Cb_Servico.SelectedIndex].Nome;
                ServicoGuardado = false;

                foreach (ListaTarefas item in listaTarefa)
                {
                    if (item.Servico == servico && item.ChaveTarefa != Lbl_CodigoTarefa.Content.ToString())
                    {
                        ServicoGuardado = true;
                        ChaveTarefaServico = item.ChaveTarefa;

                        break;
                    }
                }

                decimal valor = 0;

                try
                {
                    valor = servicos.Find(lst => lst.Nome == servicos[Cb_Servico.SelectedIndex].Nome).Preco;
                }
                catch (Exception)
                {
                }

                preco = 0;
                foreach (Tempo item in tempos)
                {
                    TimeSpan tempoDecorrido;

                    if (item.DataFim == Convert.ToDateTime("01/01/0001 00:00:00") || item.DataFim == Convert.ToDateTime(null))
                    {
                        tempoDecorrido = new TimeSpan(0);
                    }
                    else
                    {
                        tempoDecorrido = Convert.ToDateTime(item.DataFim as DateTime?) - item.DataInicio;
                    }

                    preco += valor * Convert.ToDecimal(TimeSpan.Parse(Convert.ToString(tempoDecorrido)).TotalHours);
                }

                Lbl_Preco.Content = String.Format("{0:###0.00}€", preco * (1 - Functions.Clamp(Convert.ToDecimal(Sld_Desconto.Value))));

                if (ServicoGuardado)
                {
                    Lbl_Erros.Text = "Serviço já adicionado!\nO tempo será adicionado e o valor do desconto atualizado!\nNenhuns dados serão perdidos!";
                }
                else
                {
                    Lbl_Erros.Text = null;
                }

                ServicoValido = true;
            }

            AtualizarBotoes();
        }

        //Validar escrita do servico
        private void Cb_Servico_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                Cb_Servico.Text = Cb_Servico.Text.TrimStart();

                int index = servicos.FindIndex(lst => lst.Nome == Cb_Servico.Text);

                if (index >= 0)
                {
                    Lbl_Erros.Text = null;
                    ServicoValido = true;
                }
                else if (index == -1)
                {
                    Lbl_Erros.Text = "Tal serviço ainda não existe.\nSelecione um serviço da lista existente, ou procure ao escrever o nome do serviço.";
                    ServicoValido = false;
                }
            }
            catch (Exception)
            {
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
            else if (Dp_DataFim.SelectedDate == Convert.ToDateTime(null))
            {
                Dp_DataFim.SelectedDate = null;
                Dp_DataFim.DisplayDate = DateTime.Now;
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

        //Atribuir valor do slider a textbox
        private void Sld_Desconto_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (Sld_Desconto.IsFocused || !Tb_Desconto.IsFocused)
            {
                Tb_Desconto.Text = String.Format("{0:##0.00}%", Math.Round(Sld_Desconto.Value, 2));
            }

            Lbl_Preco.Content = String.Format("{0:###0.00}€", preco * (1 - Functions.Clamp(Convert.ToDecimal(Sld_Desconto.Value))));
        }

        //Atribuir valor da textbox ao slider
        private void Tb_Desconto_TextChanged(object sender, TextChangedEventArgs e)
        {
            Tb_Desconto.Text = Tb_Desconto.Text.Trim();
            char[] desconto = Tb_Desconto.Text.ToCharArray();

            bool comma = false;
            byte pos = Convert.ToByte(Tb_Desconto.SelectionStart);

            for (int i = 0; i < desconto.Length; i++)
            {
                if (comma == false && (desconto[i] == ',' || desconto[i] == '.'))
                {
                    comma = true;
                    desconto[i] = ',';
                    Tb_Desconto.Text = new string(desconto);
                    Tb_Desconto.SelectionStart = pos;
                }
                else if (comma == true && (desconto[i] == ',' || desconto[i] == '.'))
                {
                    Tb_Desconto.Text = Tb_Desconto.Text.Remove(i, 1);
                    Array.Clear(desconto, 0, desconto.Length);
                    desconto = Tb_Desconto.Text.Trim().ToCharArray();
                    Tb_Desconto.SelectionStart = pos;
                }
            }

            for (int i = 0; i < desconto.Length; i++)
            {
                if (i < desconto.Length - 1 && desconto[i] == '%')
                {
                    Tb_Desconto.Text = Tb_Desconto.Text.Remove(i, 1);
                    Array.Clear(desconto, 0, desconto.Length);
                    desconto = Tb_Desconto.Text.Trim().ToCharArray();
                    Tb_Desconto.SelectionStart = i;
                }
            }

            for (int i = 0; i < desconto.Length; i++)
            {
                if (!char.IsDigit(desconto[i]) && desconto[i] != ',' && desconto[i] != '%')
                {
                    Tb_Desconto.Text = Tb_Desconto.Text.Remove(i, 1);
                    Array.Clear(desconto, 0, desconto.Length);
                    desconto = Tb_Desconto.Text.Trim().ToCharArray();
                    Tb_Desconto.SelectionStart = i;
                }
            }

            int passComma = 0;

            for (int i = 0; i < desconto.Length; i++)
            {
                if (desconto[i] == ',')
                {
                    passComma = i;
                }

                if (!char.IsDigit(desconto[i]) && desconto[i] != ',' && desconto[i] != '%')
                {
                    Tb_Desconto.Text = Tb_Desconto.Text.Remove(i, 1);
                    Array.Clear(desconto, 0, desconto.Length);
                    desconto = Tb_Desconto.Text.Trim().ToCharArray();
                    Tb_Desconto.SelectionStart = i;
                }
                else if (i >= passComma + 3 && desconto[i] != '%')
                {
                    Tb_Desconto.Text = Tb_Desconto.Text.Remove(i, 1);
                    Array.Clear(desconto, 0, desconto.Length);
                    desconto = Tb_Desconto.Text.Trim().ToCharArray();
                    Tb_Desconto.SelectionStart = i;
                }
            }

            double valor;

            if (Tb_Desconto.Text.Contains("%"))
            {
                double.TryParse(Tb_Desconto.Text.Remove(Tb_Desconto.Text.Length - 1), out valor);
            }
            else
            {
                double.TryParse(Tb_Desconto.Text, out valor);
            }


            if (valor > 100.0)
            {
                Tb_Desconto.Text = "100,00%";
            }
            else if (valor < 0.0)
            {
                Tb_Desconto.Text = "0,00%";
            }

            if (Tb_Desconto.IsFocused || !Sld_Desconto.IsFocused)
            {
                Sld_Desconto.Value = valor;
            }
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
                    tarefas.Add(new Tarefa { ChaveTarefa = Convert.ToString(Reader["Key_Tarefa"].ToString()), ChaveTrabalho = Convert.ToString(Reader["Key_Trabalho"].ToString()), ChaveServico = Convert.ToString(Reader["Servico"].ToString()), Desconto = Convert.ToDecimal(Reader["Desconto"].ToString()) });
                }

                Reader.Close();

                queryTodosServicos.Connection = DataBase.conexao;
                Reader = queryTodosServicos.ExecuteReader();

                while (Reader.Read())
                {
                    servicos.Add(new Servico { ChaveServico = Convert.ToString(Reader["Key_Servico"].ToString()), Nome = Convert.ToString(Reader["Nome"].ToString()), Preco = Convert.ToDecimal(Reader["Preco"].ToString()) });
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
            Lst_Tempo.Items.Refresh();
            Lst_Tempo.SelectedIndex = -1;
            Lbl_DataInicio.Content = null;
            Dp_DataInicio.SelectedDate = null;
            Dp_DataInicio.DisplayDate = DateTime.Now;
            Lbl_DataFim.Content = null;
            Dp_DataFim.SelectedDate = null;
            Dp_DataFim.DisplayDate = DateTime.Now;
            Sld_Desconto.Value = 0;
            Tb_Desconto.Text = null;
            Lbl_Preco.Content = null;
            Lst_Tarefas.SelectedIndex = -1;
            Lbl_Erros.Text = null;
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

            if (!ServicoValido || !TempoValido)
            {
                Btn_GuardarTarefa.IsEnabled = false;

            }
            else if (ServicoValido && TempoValido)
            {
                Btn_GuardarTarefa.IsEnabled = true;
            }
        }

        //Verificar se chave existe na base de dados (Tabela: Tarefa, Tempo)
        string ReservarChave(string Tabela)
        {
            string key;

            DataBase.conexao.Open();

            while (true)
            {
                if (Tabela == "Tarefa")
                {
                    key = RandomString(20);
                    queryIndexTarefa.Connection = DataBase.conexao;
                    queryIndexTarefa.Parameters.AddWithValue("@KeyTarefa", key);
                    Reader = queryIndexTarefa.ExecuteReader();

                    queryIndexTarefa.Parameters.Clear();

                    if (!Reader.HasRows)
                    {
                        break;
                    }

                    queryIndexTarefa.Connection.Close();
                }
                else if (Tabela == "Tempo")
                {
                    bool keyExiste = false;

                    key = RandomString(30);

                    foreach (Tempo item in tempos)
                    {
                        if (!keyExiste && key == item.ChaveTempo)
                        {
                            keyExiste = true;
                            break;
                        }
                    }

                    if (!keyExiste)
                    {
                        queryIndexTempo.Connection = DataBase.conexao;
                        queryIndexTempo.Parameters.AddWithValue("@KeyTempo", key);
                        Reader = queryIndexTempo.ExecuteReader();

                        queryIndexTempo.Parameters.Clear();

                        if (!Reader.HasRows)
                        {
                            break;
                        }
                    }

                    queryIndexTempo.Connection.Close();
                }
            }

            Reader.Close();
            DataBase.conexao.Close();

            return key;
        }
    }

    internal class ListaTarefas
    {
        public string ChaveTarefa { get; set; }
        public string Servico { get; set; }
        public TimeSpan Tempo { get; set; }
    }

    internal class ListaTempo
    {
        public string ChaveTempo { get; set; }
        public DateTime DataInicio { get; set; }
        public DateTime DataFim { get; set; }
        public string TempoDecorrido { get; set; }
    }
}
