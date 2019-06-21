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
using System.Windows.Threading;

namespace Trabalhos
{
    /// <summary>
    /// Interaction logic for GerirServicos.xaml
    /// </summary>
    public partial class GerirServicos : Page
    {
        SqlCommand queryTodosServicos = new SqlCommand("SELECT Key_Servico, Nome, Preco FROM Servico ORDER BY Nome, Preco");
        SqlCommand queryIndexServico = new SqlCommand("SELECT Key_Servico FROM Servico WHERE Key_Servico = @KeyServico");
        SqlCommand queryInserirServico = new SqlCommand("INSERT INTO Servico (Key_Servico, Nome, Preco) VALUES (@KeyServico, @Nome, @Preco)");
        SqlCommand queryAtualizarServico = new SqlCommand("UPDATE Servico SET Nome = @Nome, Preco = @Preco WHERE Key_Servico = @Key_Servico");
        SqlCommand queryProcurarServico = new SqlCommand("SELECT * FROM Servico WHERE Nome = @Nome");
        SqlCommand queryProcurarTrabalhosServico = new SqlCommand("SELECT COUNT(Key_Tarefa) AS 'Contagem' FROM Tarefa WHERE Key_Servico = @Key_Servico");
        SqlCommand queryApagarServico = new SqlCommand("DELETE FROM Servico WHERE Key_Servico = @Key_Servico");

        SqlDataReader Reader;

        DispatcherTimer temporizador = new DispatcherTimer();

        List<Servico> servicos = new List<Servico>();

        //Gerar chave aleatória 
        public static string RandomString(int length)
        {
            Random random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            return new string(Enumerable.Range(1, length).Select(_ => chars[random.Next(chars.Length)]).ToArray());
        }

        bool NomeValido = false;
        bool PrecoValido = false;

        bool Adicionar = false;

        //Iniciação
        public GerirServicos()
        {
            InitializeComponent();

            LigarBaseDados();

            temporizador.Interval = new TimeSpan(0, 0, 1);
            temporizador.Tick += new EventHandler(Timer_Tick);

            Lst_Servicos.ItemsSource = servicos;
            Lst_Servicos.Items.Refresh();
        }

        //Funçoes de butoes e lista
        //Butao adicionar novo servico
        private void Btn_AdicionarServico_Click(object sender, RoutedEventArgs e)
        {
            Adicionar = true;

            LimparCampos();

            if (EditarServicoCampos.ChaveServico == null)
            {
                Lbl_CodigoServico.Content = ReservarChave();
            }
            else
            {
                Lbl_CodigoServico.Content = EditarServicoCampos.ChaveServico;
            }

            Tb_Servico.Text = EditarServicoCampos.Nome;
            Tb_Preco.Text = EditarServicoCampos.Preco.ToString();

            Tb_Servico.IsReadOnly = false;
            Tb_Preco.IsReadOnly = false;
            Lst_Servicos.IsEnabled = false;
            Btn_GuardarServico.Visibility = Visibility.Visible;
            Btn_CancelarServico.Visibility = Visibility.Visible;
            Btn_AdicionarServico.Visibility = Visibility.Hidden;
            Btn_AtualizarServico.Visibility = Visibility.Hidden;
            Btn_ApagarServico.Visibility = Visibility.Hidden;
        }

        //Botao alterar servico selecionado
        private void Btn_AtualizarServico_Click(object sender, RoutedEventArgs e)
        {
            Adicionar = false;

            NomeValido = false;
            PrecoValido = false;

            temporizador.Start();

            Lbl_CodigoServico.Content = servicos[Lst_Servicos.SelectedIndex].ChaveServico;
            Tb_Servico.Text = servicos[Lst_Servicos.SelectedIndex].Nome;
            Tb_Preco.Text = servicos[Lst_Servicos.SelectedIndex].Preco.ToString();

            Tb_Servico.IsReadOnly = false;
            Tb_Preco.IsReadOnly = false;
            Lst_Servicos.IsEnabled = false;
            Btn_GuardarAlteracoes.Visibility = Visibility.Visible;
            Btn_CancelarServico.Visibility = Visibility.Visible;
            Btn_AdicionarServico.Visibility = Visibility.Hidden;
            Btn_AtualizarServico.Visibility = Visibility.Hidden;
            Btn_ApagarServico.Visibility = Visibility.Hidden;
        }

        //Botao apagar servico selecionado
        private void Btn_ApagarServico_Click(object sender, RoutedEventArgs e)
        {
            BloquearFundo.Visibility = Visibility.Visible;
            Grd_ValidarApagar.Visibility = Visibility.Visible;
        }

        //Botao confirmar apagar servico
        private void Btn_ConfirmarApagar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DataBase.conexao.Open();
                queryApagarServico.Connection = DataBase.conexao;
                queryApagarServico.Parameters.AddWithValue("@Key_Servico", servicos[Lst_Servicos.SelectedIndex].ChaveServico);

                Reader = queryApagarServico.ExecuteReader();
                queryApagarServico.Parameters.Clear();

                Reader.Close();
                DataBase.conexao.Close();

                servicos.RemoveAt(Lst_Servicos.SelectedIndex);
                Lst_Servicos.Items.Refresh();

                LimparCampos();
                Lbl_Erros.Text = "O serviço foi apagado com sucesso!";

                BloquearFundo.Visibility = Visibility.Hidden;
                Grd_ValidarApagar.Visibility = Visibility.Hidden;
            }
            catch (Exception ex)
            {
                Lbl_Erros.Text = "Erro Inesperado!\nVerifique a lista de erros conhecidos.\nErro: " + ex;
            }
        }

        //Botao cancelar apagar servico
        private void Btn_CancelarApagar_Click(object sender, RoutedEventArgs e)
        {
            Grd_ValidarApagar.Visibility = Visibility.Hidden;
            BloquearFundo.Visibility = Visibility.Hidden;
            LimparCampos();
        }

        //Botao guardar novo servico
        private void Btn_GuardarServico_Click(object sender, RoutedEventArgs e)
        {
            if (NomeValido && PrecoValido)
            {
                decimal valor;

                if (Tb_Preco.Text.Contains("€"))
                {
                    decimal.TryParse(Tb_Preco.Text.Remove(Tb_Preco.Text.Length - 1), out valor);
                }
                else
                {
                    decimal.TryParse(Tb_Preco.Text, out valor);
                }

                try
                {
                    DataBase.conexao.Open();
                    queryInserirServico.Connection = DataBase.conexao;
                    queryInserirServico.Parameters.AddWithValue("@Nome", Tb_Servico.Text.Trim());
                    queryInserirServico.Parameters.AddWithValue("@Preco", valor);
                    queryInserirServico.Parameters.AddWithValue("@KeyServico", Lbl_CodigoServico.Content);

                    queryInserirServico.ExecuteNonQuery();
                    queryInserirServico.Parameters.Clear();

                    DataBase.conexao.Close();

                    servicos.Add(new Servico { ChaveServico = Convert.ToString(Lbl_CodigoServico.Content), Nome = Convert.ToString(Tb_Servico.Text.Trim()), Preco = valor });

                    Lst_Servicos.Items.Refresh();

                    LimparCampos();

                    Tb_Servico.IsReadOnly = true;
                    Tb_Preco.IsReadOnly = true;
                    Lst_Servicos.IsEnabled = true;
                    Lbl_AvisoPreco.Visibility = Visibility.Hidden;
                    Btn_GuardarServico.Visibility = Visibility.Hidden;
                    Btn_CancelarServico.Visibility = Visibility.Hidden;
                    Btn_AdicionarServico.Visibility = Visibility.Visible;
                    Btn_AtualizarServico.Visibility = Visibility.Visible;
                    Btn_ApagarServico.Visibility = Visibility.Visible;
                }
                catch (Exception ex)
                {
                    Lbl_Erros.Text = "Erro Inesperado!\nVerifique a lista de erros conhecidos.\nErro: " + ex;
                }
            }

            Adicionar = false;
        }

        //Botao guardar alteraçoes no servico
        private void Btn_GuardarAlteracoes_Click(object sender, RoutedEventArgs e)
        {
            if (NomeValido && PrecoValido)
            {
                decimal valor;

                if (Tb_Preco.Text.Contains("€"))
                {
                    decimal.TryParse(Tb_Preco.Text.Remove(Tb_Preco.Text.Length - 1), out valor);
                }
                else
                {
                    decimal.TryParse(Tb_Preco.Text, out valor);
                }

                try
                {
                    DataBase.conexao.Open();
                    queryAtualizarServico.Connection = DataBase.conexao;
                    queryAtualizarServico.Parameters.AddWithValue("@Nome", Tb_Servico.Text.Trim());
                    queryAtualizarServico.Parameters.AddWithValue("@Preco", valor);
                    queryAtualizarServico.Parameters.AddWithValue("@Key_Servico", Lbl_CodigoServico.Content.ToString());

                    queryAtualizarServico.ExecuteNonQuery();
                    queryAtualizarServico.Parameters.Clear();

                    DataBase.conexao.Close();

                    servicos[Lst_Servicos.SelectedIndex].Nome = Convert.ToString(Tb_Servico.Text.Trim());
                    servicos[Lst_Servicos.SelectedIndex].Preco = valor;

                    Lst_Servicos.Items.Refresh();

                    LimparCampos();

                    Tb_Servico.IsReadOnly = true;
                    Tb_Preco.IsReadOnly = true;
                    Lst_Servicos.IsEnabled = true;
                    Lbl_AvisoPreco.Visibility = Visibility.Hidden;
                    Btn_GuardarServico.Visibility = Visibility.Hidden;
                    Btn_CancelarServico.Visibility = Visibility.Hidden;
                    Btn_AdicionarServico.Visibility = Visibility.Visible;
                    Btn_AtualizarServico.Visibility = Visibility.Visible;
                    Btn_ApagarServico.Visibility = Visibility.Visible;
                }
                catch (Exception ex)
                {
                    Lbl_Erros.Text = "Erro Inesperado!\nVerifique a lista de erros conhecidos.\nErro: " + ex;
                }
            }
        }

        //Lista servicos
        private void Lst_Servicos_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            long trabalhosServico;

            if (Lst_Servicos.SelectedIndex >= 0)
            {
                Lbl_CodigoServico.Content = servicos[Lst_Servicos.SelectedIndex].ChaveServico;
                Tb_Servico.Text = servicos[Lst_Servicos.SelectedIndex].Nome;
                Tb_Preco.Text = servicos[Lst_Servicos.SelectedIndex].Preco.ToString() + " €";

                Btn_AtualizarServico.IsEnabled = true;

                try
                {
                    DataBase.conexao.Open();
                    queryProcurarTrabalhosServico.Connection = DataBase.conexao;
                    queryProcurarTrabalhosServico.Parameters.AddWithValue("@Key_Servico", servicos[Lst_Servicos.SelectedIndex].ChaveServico);

                    Reader = queryProcurarTrabalhosServico.ExecuteReader();
                    queryProcurarTrabalhosServico.Parameters.Clear();

                    if (Reader.HasRows)
                    {
                        Reader.Read();

                        trabalhosServico = Convert.ToInt64(Reader["Contagem"].ToString());

                        if (trabalhosServico > 0)
                        {
                            Btn_ApagarServico.IsEnabled = false;
                        }
                        else if (trabalhosServico == 0)
                        {
                            Btn_ApagarServico.IsEnabled = true;
                        }
                    }
                    else
                    {
                        Btn_ApagarServico.IsEnabled = true;
                    }
                }
                catch (Exception ex)
                {
                    Lbl_Erros.Text = "Erro Inesperado!\nVerifique a lista de erros conhecidos.\nErro: " + ex;
                }

                Reader.Close();
                DataBase.conexao.Close();
            }
            else if (Lst_Servicos.SelectedIndex == -1)
            {
                Btn_AtualizarServico.IsEnabled = false;
                Btn_ApagarServico.IsEnabled = false;
            }
        }

        //Botao cancelar novo servico
        private void Btn_CancelarServico_Click(object sender, RoutedEventArgs e)
        {
            Adicionar = false;

            LimparCampos();

            NomeValido = false;
            PrecoValido = false;
            Tb_Servico.IsReadOnly = true;
            Tb_Preco.IsReadOnly = true;
            Lst_Servicos.IsEnabled = true;
            Btn_GuardarServico.Visibility = Visibility.Hidden;
            Btn_GuardarAlteracoes.Visibility = Visibility.Hidden;
            Btn_AdicionarServico.Visibility = Visibility.Visible;
            Btn_AtualizarServico.Visibility = Visibility.Visible;
            Btn_ApagarServico.Visibility = Visibility.Visible;
            Btn_CancelarServico.Visibility = Visibility.Hidden;
        }

        //Botao voltar para o menu principal
        private void Btn_Voltar_Click(object sender, RoutedEventArgs e)
        {
            if (Adicionar)
            {
                decimal? preco;

                try
                {
                    preco = Convert.ToDecimal(Tb_Preco.Text);
                }
                catch (Exception)
                {
                    preco = null;
                }

                EditarServicoCampos.ChaveServico = Lbl_CodigoServico.Content.ToString();
                EditarServicoCampos.Nome = Tb_Servico.Text;
                EditarServicoCampos.Preco = preco;
            }

            ((MainWindow)Application.Current.MainWindow).Frm_Principal.GoBack();
        }

        //Funçoes de validação
        //Validar servico
        private void Tb_Servico_TextChanged(object sender, TextChangedEventArgs e)
        {
            temporizador.Stop();

            Tb_Servico.Text = Tb_Servico.Text.TrimStart();
            char[] servico = Tb_Servico.Text.ToCharArray();

            if (!string.IsNullOrEmpty(Tb_Servico.Text.TrimStart()) && !char.IsUpper(servico[0]))
            {
                servico[0] = char.ToUpper(servico[0]);
                Tb_Servico.Text = new string(servico);
                Tb_Servico.SelectionStart = Tb_Servico.Text.Length;
            }

            for (int i = 0; i < servico.Length; i++)
            {

                if (i >= 1 && char.IsWhiteSpace(servico[i]) & char.IsWhiteSpace(servico[i - 1]))
                {
                    Tb_Servico.Text = Tb_Servico.Text.Remove(i, 1);
                    Array.Clear(servico, 0, servico.Length);
                    servico = Tb_Servico.Text.TrimStart().ToCharArray();
                    Tb_Servico.SelectionStart = i;
                }
            }

            NomeValido = false;

            temporizador.Start();

            AtualizarBotoes();
        }

        //Chama função para validar o servico quando sai do TextBox
        private void Tb_Servico_LostFocus(object sender, RoutedEventArgs e)
        {
            temporizador.Stop();

            VerificarServico();
        }

        //Validar Preco
        private void Tb_Preco_TextChanged(object sender, TextChangedEventArgs e)
        {
            Tb_Preco.Text = Tb_Preco.Text.TrimStart();
            char[] preco = Tb_Preco.Text.ToCharArray();

            bool comma = false;
            byte pos = Convert.ToByte(Tb_Preco.SelectionStart);

            for (int i = 0; i < preco.Length; i++)
            {
                if (comma == false && (preco[i] == ',' || preco[i] == '.'))
                {
                    comma = true;
                    preco[i] = ',';
                    Tb_Preco.Text = new string(preco);
                    Tb_Preco.SelectionStart = pos;
                }
                else if (comma == true && (preco[i] == ',' || preco[i] == '.'))
                {
                    Tb_Preco.Text = Tb_Preco.Text.Remove(i, 1);
                    Array.Clear(preco, 0, preco.Length);
                    preco = Tb_Preco.Text.TrimStart().ToCharArray();
                    Tb_Preco.SelectionStart = pos;
                }
            }

            if (Tb_Preco.Text.Length > 0 && preco[0] == ',')
            {
                Tb_Preco.Text = Tb_Preco.Text.Remove(0, 1);
                Array.Clear(preco, 0, preco.Length);
                preco = Tb_Preco.Text.TrimStart().ToCharArray();
                Tb_Preco.SelectionStart = pos;
            }

            for (int i = 0; i < preco.Length; i++)
            {
                if (i < preco.Length - 1 && preco[i] == '€')
                {
                    Tb_Preco.Text = Tb_Preco.Text.Remove(i, 1);
                    Array.Clear(preco, 0, preco.Length);
                    preco = Tb_Preco.Text.Trim().ToCharArray();
                    Tb_Preco.SelectionStart = i;
                }
            }

            for (int i = 0; i < preco.Length; i++)
            {
                if (!char.IsDigit(preco[i]) && preco[i] != ',' && preco[i] != '€')
                {
                    Tb_Preco.Text = Tb_Preco.Text.Remove(i, 1);
                    Array.Clear(preco, 0, preco.Length);
                    preco = Tb_Preco.Text.Trim().ToCharArray();
                    Tb_Preco.SelectionStart = i;
                }
            }

            int passComma = 0;

            for (int i = 0; i < preco.Length; i++)
            {
                if (preco[i] == ',')
                {
                    passComma = i;
                }

                if (!char.IsDigit(preco[i]) && preco[i] != ',' && preco[i] != '€')
                {
                    Tb_Preco.Text = Tb_Preco.Text.Remove(i, 1);
                    Array.Clear(preco, 0, preco.Length);
                    preco = Tb_Preco.Text.Trim().ToCharArray();
                    Tb_Preco.SelectionStart = i;
                }
                else if (i >= passComma + 3 && preco[i] != '€')
                {
                    Tb_Preco.Text = Tb_Preco.Text.Remove(i, 1);
                    Array.Clear(preco, 0, preco.Length);
                    preco = Tb_Preco.Text.Trim().ToCharArray();
                    Tb_Preco.SelectionStart = i;
                }
            }

            decimal valor;

            if (Tb_Preco.Text.Contains("€"))
            {
                decimal.TryParse(Tb_Preco.Text.Remove(Tb_Preco.Text.Length - 1), out valor);
            }
            else
            {
                decimal.TryParse(Tb_Preco.Text, out valor);
            }

            if (valor.ToString().Length > 0)
            {
                if (!Tb_Preco.IsReadOnly && valor < Configuracoes.ServicoPrecoMinimo)
                {
                    Lbl_AvisoPreco.Visibility = Visibility.Visible;
                }
                else if (!Tb_Preco.IsReadOnly && valor >= Configuracoes.ServicoPrecoMinimo)
                {
                    Lbl_AvisoPreco.Visibility = Visibility.Hidden;
                }
                else if (Tb_Preco.IsReadOnly)
                {
                    Lbl_AvisoPreco.Visibility = Visibility.Hidden;
                }
            }
            else
            {
                Lbl_AvisoPreco.Visibility = Visibility.Hidden;
            }

            if (valor.ToString().Length <= 0 || valor == 0)
            {
                PrecoValido = false;
            }
            else
            {
                PrecoValido = true;
            }

            AtualizarBotoes();
        }

        //Funçoes gerais
        //Ligar com base de dados e ler todos os servicos
        void LigarBaseDados()
        {
            try
            {
                DataBase.conexao = new SqlConnection(DataBase.stringConexao);

                DataBase.conexao.Open();
                queryTodosServicos.Connection = DataBase.conexao;
                Reader = queryTodosServicos.ExecuteReader();

                while (Reader.Read())
                {
                    servicos.Add(new Servico { ChaveServico = Convert.ToString(Reader["Key_Servico"].ToString()), Nome = Convert.ToString(Reader["Nome"].ToString()), Preco = Convert.ToDecimal(Reader["Preco"].ToString()) });
                }
            }
            catch (Exception ex)
            {
                Lbl_Erros.Text = "Erro Inesperado!\nVerifique a lista de erros conhecidos.\nErro: " + ex;
                Btn_AdicionarServico.IsEnabled = false;
                Btn_AtualizarServico.IsEnabled = false;
            }

            Reader.Close();
            DataBase.conexao.Close();
        }

        //Limpar todos os campos que estao indroduzidos
        void LimparCampos()
        {
            Lbl_CodigoServico.Content = null;
            Tb_Servico.Text = null;
            Tb_Preco.Text = null;
            Lst_Servicos.SelectedIndex = -1;
            Lbl_Erros.Text = null;
            temporizador.Stop();
        }

        //Atualiza os botoes caso os campos estejam incorretos ou corretos
        void AtualizarBotoes()
        {
            if (!NomeValido || !PrecoValido)
            {
                Btn_GuardarServico.IsEnabled = false;
                Btn_GuardarAlteracoes.IsEnabled = false;
            }
            else if (NomeValido && PrecoValido)
            {
                Btn_GuardarServico.IsEnabled = true;
                Btn_GuardarAlteracoes.IsEnabled = true;
            }
        }

        //Verificar se chave existe na base de dados
        string ReservarChave()
        {
            string key;

            DataBase.conexao.Open();

            while (true)
            {
                key = RandomString(5);
                queryIndexServico.Connection = DataBase.conexao;
                queryIndexServico.Parameters.AddWithValue("@KeyServico", key);
                Reader = queryIndexServico.ExecuteReader();

                queryIndexServico.Parameters.Clear();

                if (!Reader.HasRows)
                {
                    break;
                }
            }

            Reader.Close();
            DataBase.conexao.Close(); 

            return key;
        }

        //Chama função para validar o nome aoo fim de 1 segundos
        private void Timer_Tick(object sender, EventArgs e)
        {
            temporizador.Stop();

            VerificarServico();
        }

        //Verificar se o nome nao existe na base de dados
        void VerificarServico()
        {
            string servico = Tb_Servico.Text.Trim();

            if (servico.Length > 0)
            {
                try
                {
                    DataBase.conexao.Open();
                    queryProcurarServico.Connection = DataBase.conexao;
                    queryProcurarServico.Parameters.AddWithValue("@Nome", servico);

                    Reader = queryProcurarServico.ExecuteReader();
                    queryProcurarServico.Parameters.Clear();

                    if (Reader.HasRows)
                    {
                        Reader.Read();

                        if (Lst_Servicos.SelectedIndex >= 0 && Lbl_CodigoServico.Content.ToString() == Convert.ToString(Reader["Key_Servico"].ToString()))
                        {
                            Lbl_Erros.Text = null;
                            NomeValido = true;
                        }
                        else
                        {
                            Lbl_Erros.Text = "Este nome de serviço já existe!\nCódigo Serviço: " + Convert.ToString(Reader["Key_Servico"].ToString());
                            NomeValido = false;
                        }
                    }
                    else
                    {
                        Lbl_Erros.Text = null;
                        NomeValido = true;
                    }
                }
                catch (Exception ex)
                {
                    Lbl_Erros.Text = "Erro Inesperado!\nVerifique a lista de erros conhecidos.\nErro: " + ex;
                    Console.WriteLine(ex.ToString());
                    NomeValido = false;
                }
                Reader.Close();
                DataBase.conexao.Close();
            }
            else
            {
                NomeValido = false;
            }

            AtualizarBotoes();
        }
    }
}
