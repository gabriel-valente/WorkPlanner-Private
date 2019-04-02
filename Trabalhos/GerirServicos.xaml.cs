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
        SqlCommand queryTodosServicos = new SqlCommand("SELECT Key_Servico, Nome, Preco FROM Servico");
        SqlCommand queryIndexServico = new SqlCommand("SELECT IDENT_CURRENT('Servico') AS 'Index'");
        SqlCommand queryInserirServico = new SqlCommand("INSERT INTO Servico (Nome, Preco) VALUES (@Nome, @Preco)");
        SqlCommand queryAtualizarServico = new SqlCommand("UPDATE Servico SET Nome = @Nome, Preco = @Preco WHERE Key_Servico = @Key_Servico");
        SqlCommand queryProcurarServico = new SqlCommand("SELECT * FROM Servico WHERE Nome = @Nome");
        SqlCommand queryApagarServico = new SqlCommand("DELETE FROM Servico WHERE Key_Servico = @Key_Servico");

        SqlDataReader Reader;

        DispatcherTimer temporizador = new DispatcherTimer();

        List<Servico> servicos = new List<Servico>();

        bool NomeValido = true;
        bool PrecoValido = true;

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

            Lbl_CodigoServico.Content = EditarServicoCampos.ChaveServico;
            Tb_Servico.Text = EditarServicoCampos.Nome;
            Tb_Preco.Text = EditarServicoCampos.Preco.ToString();

            try
            {
                DataBase.conexao.Open();
                queryIndexServico.Connection = DataBase.conexao;

                Reader = queryIndexServico.ExecuteReader();

                Reader.Read();

                Lbl_CodigoServico.Content = Convert.ToInt64(Reader["Index"].ToString()) + 1;

                Reader.Close();
                DataBase.conexao.Close();
            }
            catch (Exception ex)
            {
                Lbl_Erros.Text = "Erro Inesperado!\nVerifique a lista de erros conhecidos.\nErro: " + ex;
            }

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
            NomeValido = false;
            PrecoValido = false;

            temporizador.Start();

            Lbl_CodigoServico.Content = servicos[Lst_Servicos.SelectedIndex].ChaveServico;
            Tb_Servico.Text = servicos[Lst_Servicos.SelectedIndex].Nome;
            Tb_Preco.Text = servicos[Lst_Servicos.SelectedIndex].Preco.ToString();

            Tb_Servico.IsReadOnly = false;
            Tb_Preco.IsReadOnly = false;
            Lst_Servicos.IsEnabled = false;
            Btn_GuardarServico.Visibility = Visibility.Visible;
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

                Lbl_Erros.Text = "O serviço foi apagado com sucesso!";
                LimparCampos();
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

        //Botao voltar para o menu principal
        private void Btn_Voltar_Click(object sender, RoutedEventArgs e)
        {
            if (Adicionar)
            {
                Decimal? preco = Convert.ToDecimal(Tb_Preco.Text);

                EditarServicoCampos.ChaveServico = Convert.ToInt64(Lbl_CodigoServico.Content);
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

            for (int i = 0; i < preco.Length; i++)
            {
                if (comma == false && (preco[i] == ',' || preco[i] == '.'))
                {
                    comma = true;
                    preco[i] = ',';
                    Tb_Preco.Text = preco.ToString();
                }
                else if (comma == true && (preco[i] == ',' || preco[i] == '.'))
                {
                    Tb_Preco.Text = Tb_Preco.Text.Remove(i, 1);
                    Array.Clear(preco, 0, preco.Length);
                    preco = Tb_Preco.Text.TrimStart().ToCharArray();
                    Tb_Preco.SelectionStart = i;
                }
            }

            for (int i = 0; i < preco.Length; i++)
            {
                if (!char.IsDigit(preco[i]) && preco[i] != ',')
                {
                    Tb_Preco.Text = Tb_Preco.Text.Remove(i, 1);
                    Array.Clear(preco, 0, preco.Length);
                    preco = Tb_Preco.Text.TrimStart().ToCharArray();
                    Tb_Preco.SelectionStart = i;
                }
            }

            if (Tb_Preco.Text.Length > 0 && Convert.ToDecimal(Tb_Preco.Text) < Configuracoes.ServicoPrecoMinimo)
            {
                Lbl_AvisoPreco.Visibility = Visibility.Visible;
            }
            else if (Tb_Preco.Text.Length > 0 && Convert.ToDecimal(Tb_Preco.Text) >= Configuracoes.ServicoPrecoMinimo)
            {
                Lbl_AvisoPreco.Visibility = Visibility.Hidden;
            }
            else
            {
                Lbl_AvisoPreco.Visibility = Visibility.Visible;
            }
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
                    servicos.Add(new Servico { ChaveServico = Convert.ToInt32(Reader["Key_Servico"].ToString()), Nome = Convert.ToString(Reader["Nome"].ToString()), Preco = Convert.ToDecimal(Reader["Preco"].ToString()) });
                }

                Reader.Close();
                DataBase.conexao.Close();
            }
            catch (Exception ex)
            {
                Lbl_Erros.Text = "Erro Inesperado!\nVerifique a lista de erros conhecidos.\nErro: " + ex;
                Btn_AdicionarServico.IsEnabled = false;
                Btn_AtualizarServico.IsEnabled = false;
            }
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

                    Reader.Close();
                    DataBase.conexao.Close();
                }
                catch (Exception ex)
                {
                    Lbl_Erros.Text = "Erro Inesperado!\nVerifique a lista de erros conhecidos.\nErro: " + ex;
                    NomeValido = false;
                }
            }
            else
            {
                NomeValido = false;
            }

            AtualizarBotoes();
        }
    }
}
