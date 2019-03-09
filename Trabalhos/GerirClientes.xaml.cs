using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
using System.Reflection;
using System.Windows.Media.Effects;
using System.Data.SqlClient;
using System.Net.Mail;
using System.Windows.Controls.Primitives;

namespace Trabalhos
{
    /// <summary>
    /// Interaction logic for GerirClientes.xaml
    /// </summary>
    public partial class GerirClientes : Page
    {
        SqlConnection conexao;
        string stringConexao = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|Trabalhos.mdf; Integrated Security=True";
        SqlCommand queryTodosClientes = new SqlCommand("SELECT Key_Cliente, Nome, DataNascimento, Sexo, Morada, CodigoPostal.CodPostal , Cliente.Localidade, Email, Telemovel, Telefone FROM Cliente LEFT JOIN CodigoPostal ON CodigoPostal.Key_CodPostal=Cliente.Key_CodPostal");
        SqlCommand queryIndexCliente = new SqlCommand("SELECT IDENT_CURRENT('Cliente') AS 'Index'");
        SqlCommand queryInserirCliente = new SqlCommand("INSERT INTO Cliente (Nome, DataNascimento, Sexo, Morada, Key_CodPostal, Localidade, Email, Telemovel, Telefone) VALUES (@Nome, @DataNascimento, @Sexo, @Morada, @CodPostal, @Localidade, @Email, @Telemovel, @Telefone)");
        SqlCommand queryAtualizarCliente = new SqlCommand("UPDATE Cliente SET Nome = @Nome, DataNascimento = @DataNascimento, Sexo = @Sexo, Morada = @Morada, Key_CodPostal = @CodPostal, Localidade = @Localidade, Email = @Email, Telemovel = @Telemovel, Telefone = @Telefone WHERE Key_Cliente = @Key_Cliente");
        SqlCommand queryProcurarCodigoPostal = new SqlCommand("SELECT * FROM CodigoPostal WHERE CodPostal=@CodPostal GROUP BY Key_CodPostal, CodPostal, Localidade, Rua");
        SqlCommand queryProcurarCliente = new SqlCommand("SELECT * FROM Cliente WHERE Nome = @Nome");
        SqlCommand queryProcurarTrabalhosCliente = new SqlCommand("SELECT COUNT(Key_Trabalho) AS 'Contagem' FROM Trabalho WHERE Key_Cliente = @Key_Cliente");
        SqlCommand queryApagarCliente = new SqlCommand("DELETE FROM Cliente WHERE Key_Cliente = @Key_Cliente");

        SqlDataReader Reader;

        Regex ValidarNome = new Regex("^[a-záàâãäåæçèéêëìíîïðñòóôõøùúûüýþÿı ]*$", RegexOptions.IgnoreCase);

        DispatcherTimer temporizador = new DispatcherTimer();

        List<Cliente> clientes = new List<Cliente>();

        string keyCodigo;
        bool NomeValido = false;
        bool CodigoValido = true;
        bool EmailValido = true;
        bool TelemovelValido = true;
        bool TelefoneValido = true;
        bool CPDirNulo = false;

        bool Adicionar = false;

        //Iniciação
        public GerirClientes()
        {
            InitializeComponent();

            LigarBaseDados();

            temporizador.Interval = new TimeSpan(0, 0, 1);
            temporizador.Tick += new EventHandler(Timer_Tick);

            Lst_Clientes.ItemsSource = clientes;
            Lst_Clientes.Items.Refresh();
        }

        //Funçoes de butoes e lista
        //Butao adicionar novo cliente
        private void Btn_AdicionarCliente_Click(object sender, RoutedEventArgs e)
        {
            Adicionar = true;

            string[] codigopostal = { null, null };

            LimparCampos();

            Lbl_CodigoCliente.Content = EditarClienteCampos.ChaveCliente;
            Tb_NomeCliente.Text = EditarClienteCampos.Nome;

            if (EditarClienteCampos.DataNascimento.ToString() != "01/01/0001 00:00:00")
            {
                Dp_Nascimento.SelectedDate = EditarClienteCampos.DataNascimento;
            }

            if (EditarClienteCampos.Sexo == "F")
            {
                RdB_Feminino.IsChecked = true;
            }
            else if (EditarClienteCampos.Sexo == "M")
            {
                RdB_Masculino.IsChecked = true;
            }
            else
            {
                RdB_Indefinido.IsChecked = true;
            }

            Tb_Morada.Text = EditarClienteCampos.Morada;

            if (string.IsNullOrEmpty(EditarClienteCampos.CodigoPostal))
            {
                codigopostal[0] = null;
                codigopostal[1] = null;
            }
            else
            {
                codigopostal = EditarClienteCampos.CodigoPostal.Split('-').ToArray();

                Tb_CodPostalEsquerda.Text = codigopostal[0];
                Tb_CodPostalDireita.Text = codigopostal[1];
            }

            Tb_Localidade.Text = EditarClienteCampos.Localidade;
            Tb_Email.Text = EditarClienteCampos.Email;

            if (EditarClienteCampos.Telemovel != 0)
            {
                Tb_Telemovel.Text = EditarClienteCampos.Telemovel.ToString();
            }

            if (EditarClienteCampos.Telefone != 0)
            {
                Tb_Telefone.Text = EditarClienteCampos.Telefone.ToString();
            }

            try
            {
                conexao.Open();
                queryIndexCliente.Connection = conexao;

                Reader = queryIndexCliente.ExecuteReader();

                Reader.Read();

                Lbl_CodigoCliente.Content = Convert.ToInt64(Reader["Index"].ToString()) + 1;

                Reader.Close();
                conexao.Close();
            }
            catch (Exception ex)
            {
                Lbl_Erros.Text = "Erro Inesperado!\nVerifique a lista de erros conhecidos.\nErro: " + ex;
            }

            Tb_NomeCliente.IsReadOnly = false;
            Lbl_Nascimento.Visibility = Visibility.Hidden;
            Dp_Nascimento.Visibility = Visibility.Visible;
            Btn_LimparData.Visibility = Visibility.Visible;
            Lbl_Sexo.Visibility = Visibility.Hidden;
            RdB_Feminino.Visibility = Visibility.Visible;
            RdB_Indefinido.Visibility = Visibility.Visible;
            RdB_Masculino.Visibility = Visibility.Visible;
            Tb_Morada.IsReadOnly = false;
            Tb_CodPostalEsquerda.IsReadOnly = false;
            Tb_CodPostalDireita.IsReadOnly = false;
            Lbl_CodPostalDiv.Visibility = Visibility.Visible;
            Btn_ProcurarCodigoPostal.Visibility = Visibility.Visible;
            Tb_Email.IsReadOnly = false;
            Tb_Telemovel.IsReadOnly = false;
            Tb_Telefone.IsReadOnly = false;
            Lst_Clientes.IsEnabled = false;
            Btn_GuardarCliente.Visibility = Visibility.Visible;
            Btn_CancelarCliente.Visibility = Visibility.Visible;
            Btn_AdicionarCliente.Visibility = Visibility.Hidden;
            Btn_AtualizarCliente.Visibility = Visibility.Hidden;
            Btn_ApagarCliente.Visibility = Visibility.Hidden;
        }

        //Botao alterar cliente selecionado
        private void Btn_AtualizarCliente_Click(object sender, RoutedEventArgs e)
        {
            NomeValido = false;
            CodigoValido = true;
            EmailValido = true;
            TelemovelValido = true;
            TelefoneValido = true;

            temporizador.Start();

            Lbl_CodigoCliente.Content = clientes[Lst_Clientes.SelectedIndex].ChaveCliente;
            Dp_Nascimento.SelectedDate = clientes[Lst_Clientes.SelectedIndex].DataNascimento;

            if (clientes[Lst_Clientes.SelectedIndex].Sexo == "F")
            {
                RdB_Feminino.IsChecked = true;
                RdB_Indefinido.IsChecked = false;
                RdB_Masculino.IsChecked = false;
            }
            else if (clientes[Lst_Clientes.SelectedIndex].Sexo == "I")
            {
                RdB_Feminino.IsChecked = false;
                RdB_Indefinido.IsChecked = true;
                RdB_Masculino.IsChecked = false;
            }
            else if (clientes[Lst_Clientes.SelectedIndex].Sexo == "M")
            {
                RdB_Feminino.IsChecked = false;
                RdB_Indefinido.IsChecked = false;
                RdB_Masculino.IsChecked = true;
            }

            Tb_NomeCliente.IsReadOnly = false;
            Lbl_Nascimento.Visibility = Visibility.Hidden;
            Dp_Nascimento.Visibility = Visibility.Visible;
            Btn_LimparData.Visibility = Visibility.Visible;
            Lbl_Sexo.Visibility = Visibility.Hidden;
            RdB_Feminino.Visibility = Visibility.Visible;
            RdB_Indefinido.Visibility = Visibility.Visible;
            RdB_Masculino.Visibility = Visibility.Visible;
            Tb_Morada.IsReadOnly = false;
            Tb_CodPostalEsquerda.IsReadOnly = false;
            Tb_CodPostalDireita.IsReadOnly = false;
            Lbl_CodPostalDiv.Visibility = Visibility.Visible;
            Btn_ProcurarCodigoPostal.Visibility = Visibility.Visible;
            Tb_Email.IsReadOnly = false;
            Tb_Telemovel.IsReadOnly = false;
            Tb_Telefone.IsReadOnly = false;
            Lst_Clientes.IsEnabled = false;
            Btn_GuardarAlteracoes.Visibility = Visibility.Visible;
            Btn_CancelarCliente.Visibility = Visibility.Visible;
            Btn_AdicionarCliente.Visibility = Visibility.Hidden;
            Btn_AtualizarCliente.Visibility = Visibility.Hidden;
            Btn_ApagarCliente.Visibility = Visibility.Hidden;
        }

        //Botao apagar cliente selecionado
        private void Btn_ApagarCliente_Click(object sender, RoutedEventArgs e)
        {
            BloquearFundo.Visibility = Visibility.Visible;
            Grd_ValidarApagar.Visibility = Visibility.Visible;           
        }

        //Botao confirmar apagar cliente
        private void Btn_ConfirmarApagar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                conexao.Open();
                queryApagarCliente.Connection = conexao;
                queryApagarCliente.Parameters.AddWithValue("@Key_Cliente", clientes[Lst_Clientes.SelectedIndex].ChaveCliente);

                Reader = queryApagarCliente.ExecuteReader();
                queryApagarCliente.Parameters.Clear();

                Reader.Close();
                conexao.Close();

                clientes.RemoveAt(Lst_Clientes.SelectedIndex);
                Lst_Clientes.Items.Refresh();

                Lbl_Erros.Text = "O cliente foi apagado com sucesso!";
                LimparCampos();
                Lbl_CodPostalDiv.Visibility = Visibility.Hidden;
                BloquearFundo.Visibility = Visibility.Hidden;
                Grd_ValidarApagar.Visibility = Visibility.Hidden;
            }
            catch (Exception ex)
            {
                Lbl_Erros.Text = "Erro Inesperado!\nVerifique a lista de erros conhecidos.\nErro: " + ex;
            }
        }

        //Botao cancelar apagar cliente
        private void Btn_CancelarApagar_Click(object sender, RoutedEventArgs e)
        {
            Grd_ValidarApagar.Visibility = Visibility.Hidden;
            BloquearFundo.Visibility = Visibility.Hidden;
            LimparCampos();
            Lbl_CodPostalDiv.Visibility = Visibility.Hidden;
        }

        //Botao guardar novo cliente
        private void Btn_GuardarCliente_Click(object sender, RoutedEventArgs e)
        {          
            if ((Tb_CodPostalEsquerda.Text.Length == 0 & Tb_CodPostalDireita.Text.Length == 0) || (Tb_CodPostalEsquerda.Text.Length == 4 & Tb_CodPostalDireita.Text.Length == 3))
            {
                char sexo = 'I';
                string codigo = Tb_CodPostalEsquerda.Text + "-" + Tb_CodPostalDireita.Text;
                long telemovel;
                long telefone;

                if (Tb_CodPostalEsquerda.Text.Length == 0 && Tb_CodPostalDireita.Text.Length == 0)
                {
                    CodigoValido = true;
                }

                if (NomeValido && CodigoValido)
                {
                    if (RdB_Feminino.IsChecked == true)
                    {
                        sexo = 'F';
                    }
                    else if (RdB_Indefinido.IsChecked == true)
                    {
                        sexo = 'I';
                    }
                    else if (RdB_Masculino.IsChecked == true)
                    {
                        sexo = 'M';
                    }

                    if (Tb_Telemovel.Text.Trim() == string.Empty)
                    {
                        telemovel = 0;
                    }
                    else
                    {
                        telemovel = Convert.ToInt64(Tb_Telemovel.Text);
                    }

                    if (Tb_Telefone.Text.Trim() == string.Empty)
                    {
                        telefone = 0;
                    }
                    else
                    {
                        telefone = Convert.ToInt64(Tb_Telefone.Text);
                    }

                    try
                    {
                        if (Tb_CodPostalEsquerda.Text.Length == 0 && Tb_CodPostalDireita.Text.Length == 0)
                        {
                            keyCodigo = null;
                        }
                        else
                        {
                            keyCodigo = ProcurarCodigoPostal(codigo);
                        }

                        conexao = new SqlConnection(stringConexao);

                        conexao.Open();
                        queryInserirCliente.Connection = conexao;
                        queryInserirCliente.Parameters.AddWithValue("@Nome", Tb_NomeCliente.Text.Trim());

                        if (Dp_Nascimento.SelectedDate.HasValue)
                        {
                            queryInserirCliente.Parameters.AddWithValue("@DataNascimento", Dp_Nascimento.SelectedDate.Value);
                        }
                        else if (!Dp_Nascimento.SelectedDate.HasValue)
                        {
                            queryInserirCliente.Parameters.AddWithValue("@DataNascimento", DBNull.Value);
                        }

                        queryInserirCliente.Parameters.AddWithValue("@Sexo", sexo);
                        queryInserirCliente.Parameters.AddWithValue("@Morada", Tb_Morada.Text.Trim());

                        if (keyCodigo == null)
                        {
                            queryInserirCliente.Parameters.AddWithValue("@CodPostal", DBNull.Value);
                        }
                        else
                        {
                            queryInserirCliente.Parameters.AddWithValue("@CodPostal", keyCodigo);
                        }

                        queryInserirCliente.Parameters.AddWithValue("@Localidade", Tb_Localidade.Text.Trim());
                        queryInserirCliente.Parameters.AddWithValue("@Email", Tb_Email.Text.Trim());
                        queryInserirCliente.Parameters.AddWithValue("@Telemovel", telemovel);
                        queryInserirCliente.Parameters.AddWithValue("@Telefone", telefone);

                        queryInserirCliente.ExecuteNonQuery();
                        queryInserirCliente.Parameters.Clear();

                        conexao.Close();

                        string contacto = ContactoVisivel(Tb_Email.Text.Trim(), telemovel, telefone);
                        DateTime? dataNascimento = Convert.ToDateTime(null);
                        string codigoPostal = null;

                        if (Dp_Nascimento.SelectedDate.HasValue)
                        {
                            dataNascimento = Convert.ToDateTime(Dp_Nascimento.SelectedDate.Value.ToString());
                        }

                        if (keyCodigo != null)
                        {
                            codigoPostal = Convert.ToString(Tb_CodPostalEsquerda.Text.Trim() + "-" + Tb_CodPostalDireita.Text.Trim());
                        }

                        clientes.Add(new Cliente { ChaveCliente = Convert.ToInt32(Lbl_CodigoCliente.Content), Nome = Convert.ToString(Tb_NomeCliente.Text.Trim()), DataNascimento = Convert.ToDateTime(dataNascimento.Value), Sexo = Convert.ToString(sexo), Morada = Convert.ToString(Tb_Morada.Text.Trim()), CodigoPostal = Convert.ToString(codigoPostal), Localidade = Convert.ToString(Tb_Localidade.Text.Trim()), Email = Convert.ToString(Tb_Email.Text.Trim()), Telemovel = Convert.ToInt64(telemovel), Telefone = Convert.ToInt64(telefone), Contacto = contacto });

                        Lst_Clientes.Items.Refresh();

                        LimparCampos();

                        Tb_NomeCliente.IsReadOnly = true;
                        Lbl_Nascimento.Visibility = Visibility.Visible;
                        Dp_Nascimento.Visibility = Visibility.Hidden;
                        Btn_LimparData.Visibility = Visibility.Hidden;
                        Lbl_Sexo.Visibility = Visibility.Visible;
                        RdB_Feminino.Visibility = Visibility.Hidden;
                        RdB_Indefinido.Visibility = Visibility.Hidden;
                        RdB_Masculino.Visibility = Visibility.Hidden;
                        Tb_Morada.IsReadOnly = true;
                        Tb_CodPostalEsquerda.IsReadOnly = true;
                        Tb_CodPostalDireita.IsReadOnly = true;
                        Lbl_CodPostalDiv.Visibility = Visibility.Hidden;
                        Btn_ProcurarCodigoPostal.Visibility = Visibility.Hidden;
                        Tb_Email.IsReadOnly = true;
                        Tb_Telemovel.IsReadOnly = true;
                        Tb_Telefone.IsReadOnly = true;
                        Lst_Clientes.IsEnabled = true;
                        Btn_GuardarCliente.Visibility = Visibility.Hidden;
                        Btn_CancelarCliente.Visibility = Visibility.Hidden;
                        Btn_AdicionarCliente.Visibility = Visibility.Visible;
                        Btn_AtualizarCliente.Visibility = Visibility.Visible;
                        Btn_ApagarCliente.Visibility = Visibility.Visible;
                    }
                    catch (Exception ex)
                    {
                        Lbl_Erros.Text = "Erro Inesperado!\nVerifique a lista de erros conhecidos.\nErro: " + ex;
                    }
                }               
            }
            else
            {
                Lbl_PesquisaCodigo.Content = "Codigo Postal com formato incorreto!";
            }

            Adicionar = false;
        }

        //Botao guardar alteraçoes no cliente
        private void Btn_GuardarAlteracoes_Click(object sender, RoutedEventArgs e)
        {
            if ((Tb_CodPostalEsquerda.Text.Length == 0 & Tb_CodPostalDireita.Text.Length == 0) || (Tb_CodPostalEsquerda.Text.Length == 4 & Tb_CodPostalDireita.Text.Length == 3))
            {
                long keyCliente = Convert.ToInt64(Lbl_CodigoCliente.Content.ToString());
                DateTime? nascimento = null;
                char sexo = 'I';
                string codigo = Tb_CodPostalEsquerda.Text + "-" + Tb_CodPostalDireita.Text;
                long telemovel;
                long telefone;

                if (Tb_CodPostalEsquerda.Text.Length == 0 && Tb_CodPostalDireita.Text.Length == 0)
                {
                    CodigoValido = true;
                    keyCodigo = null;
                }
                else
                {
                    keyCodigo = ProcurarCodigoPostal(codigo);
                }

                if (NomeValido && CodigoValido)
                {
                    if (Dp_Nascimento.SelectedDate.HasValue)
                    {
                        nascimento = Dp_Nascimento.SelectedDate.Value;
                    }
                    else if (!Dp_Nascimento.SelectedDate.HasValue)
                    {
                        nascimento = null;
                    }


                    if (RdB_Feminino.IsChecked == true)
                    {
                        sexo = 'F';
                    }
                    else if (RdB_Indefinido.IsChecked == true)
                    {
                        sexo = 'I';
                    }
                    else if (RdB_Masculino.IsChecked == true)
                    {
                        sexo = 'M';
                    }

                    if (Tb_Telemovel.Text.Trim() == string.Empty)
                    {
                        telemovel = 0;
                    }
                    else
                    {
                        telemovel = Convert.ToInt64(Tb_Telemovel.Text);
                    }

                    if (Tb_Telefone.Text.Trim() == string.Empty)
                    {
                        telefone = 0;
                    }
                    else
                    {
                        telefone = Convert.ToInt64(Tb_Telefone.Text);
                    }

                    try
                    {
                        conexao = new SqlConnection(stringConexao);

                        conexao.Open();
                        queryAtualizarCliente.Connection = conexao;
                        queryAtualizarCliente.Parameters.AddWithValue("@Nome", Tb_NomeCliente.Text.Trim());

                        if (Dp_Nascimento.SelectedDate.HasValue)
                        {
                            queryAtualizarCliente.Parameters.AddWithValue("@DataNascimento", nascimento);
                        }
                        else if (!Dp_Nascimento.SelectedDate.HasValue)
                        {
                            queryAtualizarCliente.Parameters.AddWithValue("@DataNascimento", DBNull.Value);
                        }

                        queryAtualizarCliente.Parameters.AddWithValue("@Sexo", sexo);
                        queryAtualizarCliente.Parameters.AddWithValue("@Morada", Tb_Morada.Text.Trim());

                        if (Tb_CodPostalEsquerda.Text.Length == 0 && Tb_CodPostalDireita.Text.Length == 0)
                        {
                            keyCodigo = null;
                            queryAtualizarCliente.Parameters.AddWithValue("@CodPostal", DBNull.Value);
                        }
                        else
                        {
                            keyCodigo = ProcurarCodigoPostal(codigo);
                            queryAtualizarCliente.Parameters.AddWithValue("@CodPostal", keyCodigo);
                        }

                        queryAtualizarCliente.Parameters.AddWithValue("@Localidade", Tb_Localidade.Text.Trim());
                        queryAtualizarCliente.Parameters.AddWithValue("@Email", Tb_Email.Text.Trim());
                        queryAtualizarCliente.Parameters.AddWithValue("@Telemovel", telemovel);
                        queryAtualizarCliente.Parameters.AddWithValue("@Telefone", telefone);
                        queryAtualizarCliente.Parameters.AddWithValue("@Key_Cliente", keyCliente);

                        queryAtualizarCliente.ExecuteNonQuery();
                        queryAtualizarCliente.Parameters.Clear();

                        conexao.Close();

                        clientes[Lst_Clientes.SelectedIndex].Nome = Convert.ToString(Tb_NomeCliente.Text.Trim());

                        if (Dp_Nascimento.SelectedDate.HasValue)
                        {
                            clientes[Lst_Clientes.SelectedIndex].DataNascimento = Convert.ToDateTime(Dp_Nascimento.SelectedDate.Value.ToString());
                        }
                        else if (!Dp_Nascimento.SelectedDate.HasValue)
                        {
                            clientes[Lst_Clientes.SelectedIndex].DataNascimento = Convert.ToDateTime(null);
                        }

                        clientes[Lst_Clientes.SelectedIndex].Sexo = Convert.ToString(sexo);
                        clientes[Lst_Clientes.SelectedIndex].Morada = Convert.ToString(Tb_Morada.Text.Trim());

                        if (keyCodigo == null)
                        {
                            clientes[Lst_Clientes.SelectedIndex].CodigoPostal = null;
                        }
                        else
                        {
                            clientes[Lst_Clientes.SelectedIndex].CodigoPostal = Convert.ToString(Tb_CodPostalEsquerda.Text.Trim() + "-" + Tb_CodPostalDireita.Text.Trim());
                        }

                        clientes[Lst_Clientes.SelectedIndex].Localidade = Convert.ToString(Tb_Localidade.Text.Trim());
                        clientes[Lst_Clientes.SelectedIndex].Email = Convert.ToString(Tb_Email.Text.Trim());
                        clientes[Lst_Clientes.SelectedIndex].Telemovel = Convert.ToInt64(telemovel);
                        clientes[Lst_Clientes.SelectedIndex].Telefone = Convert.ToInt64(telefone);

                        string contacto = ContactoVisivel(Tb_Email.Text.Trim(), telemovel, telefone);

                        clientes[Lst_Clientes.SelectedIndex].Contacto = contacto;

                        Lst_Clientes.Items.Refresh();

                        LimparCampos();

                        Tb_NomeCliente.IsReadOnly = true;
                        Lbl_Nascimento.Visibility = Visibility.Visible;
                        Dp_Nascimento.Visibility = Visibility.Hidden;
                        Btn_LimparData.Visibility = Visibility.Hidden;
                        Lbl_Sexo.Visibility = Visibility.Visible;
                        RdB_Feminino.Visibility = Visibility.Hidden;
                        RdB_Indefinido.Visibility = Visibility.Hidden;
                        RdB_Masculino.Visibility = Visibility.Hidden;
                        Tb_Morada.IsReadOnly = true;
                        Tb_CodPostalEsquerda.IsReadOnly = true;
                        Tb_CodPostalDireita.IsReadOnly = true;
                        Lbl_CodPostalDiv.Visibility = Visibility.Hidden;
                        Btn_ProcurarCodigoPostal.Visibility = Visibility.Hidden;
                        Tb_Email.IsReadOnly = true;
                        Tb_Telemovel.IsReadOnly = true;
                        Tb_Telefone.IsReadOnly = true;
                        Lst_Clientes.IsEnabled = true;
                        Btn_GuardarAlteracoes.Visibility = Visibility.Hidden;
                        Btn_CancelarCliente.Visibility = Visibility.Hidden;
                        Btn_AdicionarCliente.Visibility = Visibility.Visible;
                        Btn_AtualizarCliente.Visibility = Visibility.Visible;
                        Btn_ApagarCliente.Visibility = Visibility.Visible;
                    }
                    catch (Exception ex)
                    {
                        Lbl_Erros.Text = "Cena Erro Inesperado!\nVerifique a lista de erros conhecidos.\nErro: " + ex;
                    }
                }
            }
            else
            {
                Lbl_PesquisaCodigo.Content = "Codigo Postal com formato incorreto!";
            }
        }

        //Lista clientes
        private void Lst_Clientes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string dataNascimento;
            string genero = "Indefinido";
            string[] codigopostal = { null, null };
            string telemovel;
            string telefone;
            long trabalhosCliente;

            if (Lst_Clientes.SelectedIndex >= 0)
            {
                if (clientes[Lst_Clientes.SelectedIndex].DataNascimento.ToString() == "01/01/0001 00:00:00")
                {
                    dataNascimento = null;
                }
                else
                {
                    dataNascimento = clientes[Lst_Clientes.SelectedIndex].DataNascimento.ToShortDateString().ToString();
                }

                if (clientes[Lst_Clientes.SelectedIndex].Sexo == "F")
                {
                    genero = "Feminino";
                }
                else if (clientes[Lst_Clientes.SelectedIndex].Sexo == "I")
                {
                    genero = "Indefinido";
                }
                else if (clientes[Lst_Clientes.SelectedIndex].Sexo == "M")
                {
                    genero = "Masculino";
                }

                if (string.IsNullOrEmpty(clientes[Lst_Clientes.SelectedIndex].CodigoPostal))
                {
                    codigopostal[0] = null;
                    codigopostal[1] = null;
                    Lbl_CodPostalDiv.Visibility = Visibility.Hidden;
                }
                else
                {
                    codigopostal = clientes[Lst_Clientes.SelectedIndex].CodigoPostal.Split('-').ToArray();
                    Lbl_CodPostalDiv.Visibility = Visibility.Visible;
                }


                if (clientes[Lst_Clientes.SelectedIndex].Telemovel == 0)
                {
                    telemovel = "";
                }
                else
                {
                    telemovel = clientes[Lst_Clientes.SelectedIndex].Telemovel.ToString();
                }

                if (clientes[Lst_Clientes.SelectedIndex].Telefone == 0)
                {
                    telefone = "";
                }
                else
                {
                    telefone = clientes[Lst_Clientes.SelectedIndex].Telefone.ToString();
                }

                Lbl_CodigoCliente.Content = clientes[Lst_Clientes.SelectedIndex].ChaveCliente;
                Tb_NomeCliente.Text = clientes[Lst_Clientes.SelectedIndex].Nome;
                Lbl_Nascimento.Content = dataNascimento;
                Lbl_Sexo.Content = genero;
                Tb_Morada.Text = clientes[Lst_Clientes.SelectedIndex].Morada;
                Tb_CodPostalEsquerda.Text = codigopostal[0];
                Tb_CodPostalDireita.Text = codigopostal[1];
                Tb_Localidade.Text = clientes[Lst_Clientes.SelectedIndex].Localidade;
                Tb_Email.Text = clientes[Lst_Clientes.SelectedIndex].Email;
                Tb_Telemovel.Text = telemovel;
                Tb_Telefone.Text = telefone;

                Btn_AtualizarCliente.IsEnabled = true;

                try
                {
                    conexao.Open();
                    queryProcurarTrabalhosCliente.Connection = conexao;
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
                    conexao.Close();
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

        //Botao cancelar novo cliente
        private void Btn_CancelarCliente_Click(object sender, RoutedEventArgs e)
        {
            LimparCampos();

            NomeValido = false;
            CodigoValido = true;
            Tb_NomeCliente.IsReadOnly = true;
            Lbl_Nascimento.Visibility = Visibility.Visible;
            Dp_Nascimento.Visibility = Visibility.Hidden;
            Btn_LimparData.Visibility = Visibility.Hidden;
            Lbl_Sexo.Visibility = Visibility.Visible;
            RdB_Feminino.Visibility = Visibility.Hidden;
            RdB_Indefinido.Visibility = Visibility.Hidden;
            RdB_Masculino.Visibility = Visibility.Hidden;
            Tb_Morada.IsReadOnly = true;
            Tb_CodPostalEsquerda.IsReadOnly = true;
            Tb_CodPostalDireita.IsReadOnly = true;
            Lbl_CodPostalDiv.Visibility = Visibility.Hidden;
            Btn_ProcurarCodigoPostal.Visibility = Visibility.Hidden;
            Tb_Email.IsReadOnly = true;
            Tb_Telemovel.IsReadOnly = true;
            Tb_Telefone.IsReadOnly = true;
            Lst_Clientes.IsEnabled = true;
            Btn_GuardarCliente.Visibility = Visibility.Hidden;
            Btn_GuardarAlteracoes.Visibility = Visibility.Hidden;
            Btn_AdicionarCliente.Visibility = Visibility.Visible;
            Btn_AtualizarCliente.Visibility = Visibility.Visible;
            Btn_ApagarCliente.Visibility = Visibility.Visible;
            Btn_CancelarCliente.Visibility = Visibility.Hidden;
        }

        //Botao limpar data de nascimento
        private void Btn_LimparData_Click(object sender, RoutedEventArgs e)
        {
            Dp_Nascimento.SelectedDate = null;
            Dp_Nascimento.DisplayDate = DateTime.Today;
        }

        //Botao procurar codigo postal
        private void Btn_ProcurarCodigoPostal_Click(object sender, RoutedEventArgs e)
        {
            if (Tb_CodPostalEsquerda.Text.Length == 4 && Tb_CodPostalDireita.Text.Length == 3)
            {
                string codigo = Tb_CodPostalEsquerda.Text + "-" + Tb_CodPostalDireita.Text;

                keyCodigo = ProcurarCodigoPostal(codigo);
            }
            else if (Tb_CodPostalEsquerda.Text.Length == 0 && Tb_CodPostalDireita.Text.Length == 0)
            {
                Lbl_PesquisaCodigo.Content = "Código Postal vazio!";
                Tb_Localidade.Text = null;
                CodigoValido = true;
            }
            else
            {
                Lbl_PesquisaCodigo.Content = "Código Postal com formato incorreto!";
                Tb_Localidade.Text = null;
                CodigoValido = false;
            }

            AtualizarBotoes();
        }

        //Botao voltar para o menu principal
        private void Btn_Voltar_Click(object sender, RoutedEventArgs e)
        {
            if (Adicionar)
            {
                DateTime? data = Dp_Nascimento.SelectedDate;
                char sexo = 'I';
                long telemovel = 0;
                long telefone = 0;

                if (Dp_Nascimento.Text == null)
                {
                    data = Convert.ToDateTime("01/01/0001 00:00:00");
                }

                if (RdB_Feminino.IsChecked.Value)
                {
                    sexo = 'F';
                }
                else if (RdB_Masculino.IsChecked.Value)
                {
                    sexo = 'M';
                }

                EditarClienteCampos.ChaveCliente = Convert.ToInt64(Lbl_CodigoCliente.Content);
                EditarClienteCampos.Nome = Tb_NomeCliente.Text;
                EditarClienteCampos.DataNascimento = data;
                EditarClienteCampos.Sexo = sexo.ToString();
                EditarClienteCampos.Morada = Tb_Morada.Text;
                EditarClienteCampos.CodigoPostal = Tb_CodPostalEsquerda.Text + "-" + Tb_CodPostalDireita.Text;
                EditarClienteCampos.Localidade = Tb_Localidade.Text;
                EditarClienteCampos.Email = Tb_Email.Text;

                if (Tb_Telemovel.Text.Length > 0)
                {
                    telemovel = Convert.ToInt64(Tb_Telemovel.Text);
                }

                EditarClienteCampos.Telemovel = telemovel;

                if (Tb_Telefone.Text.Length > 0)
                {
                    telefone = Convert.ToInt64(Tb_Telefone.Text);
                }

                EditarClienteCampos.Telefone = telefone;
            }

            ((MainWindow)Application.Current.MainWindow).Frm_Principal.GoBack();
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

        //Chama função para validar o nome quando sai do TextBox
        private void Tb_NomeCliente_LostFocus(object sender, RoutedEventArgs e)
        {
            temporizador.Stop();

            VerificarNome();
        }

        //Validar data de nascimento
        private void Dp_Nascimento_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Dp_Nascimento.SelectedDate > DateTime.Today.AddYears(-Configuracoes.IdadeMinima))
            {
                Dp_Nascimento.SelectedDate = null;
                Dp_Nascimento.DisplayDate = DateTime.Today.AddYears(-Configuracoes.IdadeMinima);
                Lbl_Erros.Text = "O cliente tem de ter mais de " + Configuracoes.IdadeMinima + " anos.\nPode alterar este valor nas Definições!";
            }
            else if (Dp_Nascimento.SelectedDate == Convert.ToDateTime(null))
            {
                Dp_Nascimento.SelectedDate = null;
                Dp_Nascimento.DisplayDate = DateTime.Today.AddYears(-Configuracoes.IdadeMinima);
            }
            else if (Dp_Nascimento.SelectedDate <= DateTime.Today.AddYears(-125))
            {
                Dp_Nascimento.SelectedDate = null;
                Dp_Nascimento.DisplayDate = DateTime.Today.AddYears(-Configuracoes.IdadeMinima);
                Lbl_Erros.Text = "Não é possivel ter um cliente com mais de 125 anos!";
            }
            else
            {
                Lbl_Erros.Text = null;
            }
        }

        //Abrir DropDown ao clicar no TextBox
        private void PART_TextBox_GotMouseCapture(object sender, MouseEventArgs e)
        {
            Dp_Nascimento.IsDropDownOpen = true;
        }

        //Validar morada
        private void Tb_Morada_TextChanged(object sender, TextChangedEventArgs e)
        {
            Tb_Morada.Text = Tb_Morada.Text.TrimStart();
            char[] morada = Tb_Morada.Text.ToCharArray();

            if (!string.IsNullOrEmpty(Tb_Morada.Text.TrimStart()) && !char.IsUpper(morada[0]))
            {
                morada[0] = char.ToUpper(morada[0]);
                Tb_Morada.Text = new string(morada);
                Tb_Morada.SelectionStart = Tb_Morada.Text.Length;
            }

            for (int i = 0; i < morada.Length; i++)
            {

                if (i >= 1 && char.IsWhiteSpace(morada[i]) & char.IsWhiteSpace(morada[i - 1]))
                {
                    Tb_Morada.Text = Tb_Morada.Text.Remove(i, 1);
                    Array.Clear(morada, 0, morada.Length);
                    morada = Tb_Morada.Text.TrimStart().ToCharArray();
                    Tb_Morada.SelectionStart = i;
                }
            }
        }

        //Validar codigo postal (esquerda)
        private void Tb_CodPostalEsquerda_TextChanged(object sender, TextChangedEventArgs e)
        {
            Tb_CodPostalEsquerda.Text = Tb_CodPostalEsquerda.Text.Trim();
            char[] codPostal = Tb_CodPostalEsquerda.Text.ToCharArray();

            for (int i = 0; i < codPostal.Length; i++)
            {
                if (i == 0)
                {
                    if (!char.IsDigit(codPostal[i]) || codPostal[i] == '0')
                    {
                        Tb_CodPostalEsquerda.Text = Tb_CodPostalEsquerda.Text.Remove(i, 1);
                        Array.Clear(codPostal, 0, codPostal.Length);
                        codPostal = Tb_CodPostalEsquerda.Text.Trim().ToCharArray();
                        Tb_CodPostalEsquerda.SelectionStart = i;
                    }
                }
                else
                {
                    if (!char.IsDigit(codPostal[i]))
                    {
                        Tb_CodPostalEsquerda.Text = Tb_CodPostalEsquerda.Text.Remove(i, 1);
                        Array.Clear(codPostal, 0, codPostal.Length);
                        codPostal = Tb_CodPostalEsquerda.Text.Trim().ToCharArray();
                        Tb_CodPostalEsquerda.SelectionStart = i;
                    }
                }
            }

            if (codPostal.Length == 4)
            {
                Tb_CodPostalDireita.Focus();
                Tb_CodPostalDireita.SelectionStart = Tb_CodPostalDireita.Text.Length;
            }
        }

        //Inserir valor do lado direito do TextBox
        private void Tb_CodPostalEsquerda_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (Tb_CodPostalEsquerda.Text.Length == 4 && char.IsDigit(Convert.ToChar(e.Key.ToString().Substring(e.Key.ToString().Length - 1, 1))))
            {
                Tb_CodPostalDireita.Focus();
                Tb_CodPostalDireita.SelectionStart = 0;
            }
        }

        //Validar codigo postal (direita)
        private void Tb_CodPostalDireita_TextChanged(object sender, TextChangedEventArgs e)
        {
            Tb_CodPostalDireita.Text = Tb_CodPostalDireita.Text.Trim();
            char[] codPostal = Tb_CodPostalDireita.Text.ToCharArray();

            for (int i = 0; i < codPostal.Length; i++)
            {
                if (!char.IsDigit(codPostal[i]))
                {
                    Tb_CodPostalDireita.Text = Tb_CodPostalDireita.Text.Remove(i, 1);
                    Array.Clear(codPostal, 0, codPostal.Length);
                    
                    Tb_CodPostalDireita.SelectionStart = i;
                }
                else if (i == 2 && codPostal[i] == '0' && codPostal[i - 1] == '0' && codPostal[i - 2] == '0')
                {
                    Tb_CodPostalDireita.Text = "001";
                    Array.Clear(codPostal, 0, codPostal.Length);
                    codPostal = Tb_CodPostalDireita.Text.Trim().ToCharArray();
                    Tb_CodPostalDireita.SelectionStart = Tb_CodPostalDireita.Text.Length;
                }
            }
        }

        //Mudança de TextBox ao apagar
        private void Tb_CodPostalDireita_PreviewKeyDown(object sender, KeyEventArgs e)
        {            
            if (Tb_CodPostalDireita.Text.Length == 0)
            {
                CPDirNulo = true;
            }

            if (Tb_CodPostalDireita.Text.Length == 0 && CPDirNulo == true && Keyboard.IsKeyDown(Key.Back))
            {
                CPDirNulo = false;
                e.Handled = true;
                Tb_CodPostalEsquerda.Focus();
                Tb_CodPostalEsquerda.SelectionStart = Tb_CodPostalEsquerda.Text.Length;
            }
        }

        //Validar Email
        private void Tb_Email_TextChanged(object sender, TextChangedEventArgs e)
        {
            Tb_Email.Text = Tb_Email.Text.Trim();
            char[] email = Tb_Email.Text.ToCharArray();

            for (int i = 0; i < email.Length; i++)
            {
                if (char.IsWhiteSpace(email[i]))
                {
                    Tb_Email.Text = Tb_Email.Text.Remove(i, 1);
                    Array.Clear(email, 0, email.Length);
                    email = Tb_Email.Text.Trim().ToCharArray();
                    Tb_Email.SelectionStart = i;
                }
            }

            if (Tb_Email.Text.Length > 0)
            {
                try
                {
                    var em = new MailAddress(Tb_Email.Text);

                    Lbl_Erros.Text = null;
                    EmailValido = true;
                }
                catch (Exception)
                {
                    Lbl_Erros.Text = "Email com formato não reconhecido!";
                    EmailValido = false;
                }
            }
            else if (Tb_Email.Text.Length == 0)
            {
                Lbl_Erros.Text = null;
                EmailValido = true;
            }

            AtualizarBotoes();
        }

        //Validar telemovel
        private void Tb_Telemovel_TextChanged(object sender, TextChangedEventArgs e)
        {
            Tb_Telemovel.Text = Tb_Telemovel.Text.Trim();
            char[] numero = Tb_Telemovel.Text.ToCharArray();

            if (!string.IsNullOrEmpty(Tb_Telemovel.Text.Trim()) && numero[0] != '9')
            {
                numero[0] = '9';
                Tb_Telemovel.Text = new string(numero);
                Tb_Telemovel.SelectionStart = Tb_Telemovel.Text.Length;
            }

            for (int i = 0; i < numero.Length; i++)
            {
                if (!char.IsDigit(numero[i]))
                {
                    Tb_Telemovel.Text = Tb_Telemovel.Text.Remove(i, 1);
                    Array.Clear(numero, 0, numero.Length);
                    numero = Tb_Telemovel.Text.Trim().ToCharArray();
                    Tb_Telemovel.SelectionStart = i;
                }
            }

            if (numero.Length == 9 || numero.Length == 0)
            {
                TelemovelValido = true;
            }
            else
            {
                TelemovelValido = false;
            }

            AtualizarBotoes();
        }

        //Validar telefone
        private void Tb_Telefone_TextChanged(object sender, TextChangedEventArgs e)
        {
            Tb_Telefone.Text = Tb_Telefone.Text.Trim();
            char[] numero = Tb_Telefone.Text.ToCharArray();

            if (!string.IsNullOrEmpty(Tb_Telefone.Text.Trim()) && numero[0] != '2')
            {
                numero[0] = '2';
                Tb_Telefone.Text = new string(numero);
                Tb_Telefone.SelectionStart = Tb_Telefone.Text.Length;
            }

            for (int i = 0; i < numero.Length; i++)
            {
                if (!char.IsDigit(numero[i]))
                {
                    Tb_Telefone.Text = Tb_Telefone.Text.Remove(i, 1);
                    Array.Clear(numero, 0, numero.Length);
                    numero = Tb_Telefone.Text.Trim().ToCharArray();
                    Tb_Telefone.SelectionStart = i;
                }
            }

            if (numero.Length == 9 || numero.Length == 0)
            {
                TelefoneValido = true;
            }
            else
            {
                TelefoneValido = false;
            }

            AtualizarBotoes();
        }

        //Funçoes gerais
        //Ligar com base de dados e ler todos os clientes
        void LigarBaseDados()
        {
            try
            {
                conexao = new SqlConnection(stringConexao);

                conexao.Open();
                queryTodosClientes.Connection = conexao;
                Reader = queryTodosClientes.ExecuteReader();

                while (Reader.Read())
                {
                    string contacto = ContactoVisivel(Convert.ToString(Reader["Email"].ToString()), Convert.ToInt64(Reader["Telemovel"].ToString()), Convert.ToInt64(Reader["Telefone"].ToString()));

                    clientes.Add(new Cliente { ChaveCliente = Convert.ToInt32(Reader["Key_Cliente"].ToString()), Nome = Convert.ToString(Reader["Nome"].ToString()), DataNascimento = Convert.ToDateTime(Reader["DataNascimento"] as DateTime?), Sexo = Convert.ToString(Reader["Sexo"].ToString()), Morada = Convert.ToString(Reader["Morada"].ToString()), CodigoPostal = Convert.ToString(Reader["CodPostal"].ToString()), Localidade = Convert.ToString(Reader["Localidade"].ToString()), Email = Convert.ToString(Reader["Email"].ToString()), Telemovel = Convert.ToInt64(Reader["Telemovel"].ToString()), Telefone = Convert.ToInt64(Reader["Telefone"].ToString()), Contacto = contacto });
                }

                Reader.Close();
                conexao.Close();
            }
            catch (Exception ex)
            {
                Lbl_Erros.Text = "Erro Inesperado!\nVerifique a lista de erros conhecidos.\nErro: " + ex;
                Btn_AdicionarCliente.IsEnabled = false;
                Btn_AtualizarCliente.IsEnabled = false;
            }
        }

        //Limpar todos os campos que estao indroduzidos
        void LimparCampos()
        {
            Lbl_CodigoCliente.Content = null;
            Tb_NomeCliente.Text = null;
            Lbl_Nascimento.Content = null;
            Dp_Nascimento.SelectedDate = null;
            Dp_Nascimento.DisplayDate = DateTime.Today.AddYears(-Configuracoes.IdadeMinima);
            Lbl_Sexo.Content = null;
            RdB_Feminino.IsChecked = false;
            RdB_Indefinido.IsChecked = true;
            RdB_Masculino.IsChecked = false;
            Tb_Morada.Text = null;
            Tb_CodPostalEsquerda.Text = null;
            Tb_CodPostalDireita.Text = null;
            Lbl_PesquisaCodigo.Content = null;
            Tb_Localidade.Text = null;
            Tb_Email.Text = null;
            Tb_Telemovel.Text = null;
            Tb_Telefone.Text = null;
            Lbl_ErroContacto.Content = null;
            Lst_Clientes.SelectedIndex = -1;
            Lbl_Erros.Text = null;
            temporizador.Stop();
        }

        //Selecionar o contacto a mostrar
        string ContactoVisivel(string email, long telemovel, long telefone)
        {
            string contacto = null;

            if (Configuracoes.ContactoPreferivel == 0)
            {
                if (email != null)
                {
                    contacto = Convert.ToString(email);
                }
                else if (telemovel != 0)
                {
                    contacto = Convert.ToString(telemovel);
                }
                else if (telefone != 0)
                {
                    contacto = Convert.ToString(telefone);
                }
                else
                {
                    contacto = null;
                }
            }
            else if (Configuracoes.ContactoPreferivel == 1)
            {
                if (email != null)
                {
                    contacto = Convert.ToString(email);
                }
                else if (telemovel != 0)
                {
                    contacto = Convert.ToString(telemovel);
                }
                else if (telefone != 0)
                {
                    contacto = Convert.ToString(telefone);
                }
                else
                {
                    contacto = null;
                }
            }
            else if (Configuracoes.ContactoPreferivel == 2)
            {
                if (telemovel != 0)
                {
                    contacto = Convert.ToString(telemovel);
                }
                else if (email != null)
                {
                    contacto = Convert.ToString(email);
                }
                else if (telefone != 0)
                {
                    contacto = Convert.ToString(telefone);
                }
                else
                {
                    contacto = null;
                }
            }
            else if (Configuracoes.ContactoPreferivel == 3)
            {
                if (telefone != 0)
                {
                    contacto = Convert.ToString(telefone);
                }
                else if (email != null)
                {
                    contacto = Convert.ToString(email);
                }
                else if (telemovel != 0)
                {
                    contacto = Convert.ToString(telemovel);
                }
                else
                {
                    contacto = null;
                }
            }
            else
            {
                if (email != null)
                {
                    contacto = Convert.ToString(email);
                }
                else if (telemovel != 0)
                {
                    contacto = Convert.ToString(telemovel);
                }
                else if (telefone != 0)
                {
                    contacto = Convert.ToString(telefone);
                }
                else
                {
                    contacto = null;
                }
            }

            return contacto;
        }

        //Iniciar o POPUP como vista anual
        private void Dp_Nascimento_CalendarOpened(object sender, RoutedEventArgs e)
        {
            Popup popup = (Popup)Dp_Nascimento.Template.FindName("PART_Popup", Dp_Nascimento);
            Calendar cal = (Calendar)popup.Child;
            cal.DisplayMode = CalendarMode.Decade;
            cal.DisplayDate = DateTime.Today.AddYears(-Configuracoes.IdadeMinima);
        }

        //Procura codigo postal na base de dados
        string ProcurarCodigoPostal(string codigo)
        {
            string keyCodigo;

            try
            {
                conexao.Open();
                queryProcurarCodigoPostal.Connection = conexao;
                queryProcurarCodigoPostal.Parameters.AddWithValue("@CodPostal", codigo);

                Reader = queryProcurarCodigoPostal.ExecuteReader();
                queryProcurarCodigoPostal.Parameters.Clear();

                if (Reader.HasRows)
                {
                    Reader.Read();

                    Lbl_PesquisaCodigo.Content = null;
                    Tb_Localidade.Text = Convert.ToString(Reader["Localidade"].ToString());
                    keyCodigo = Convert.ToString(Reader["Key_CodPostal"].ToString());

                    if (string.IsNullOrWhiteSpace(Tb_Morada.Text))
                    {
                        Tb_Morada.Text = null;
                        Tb_Morada.Text = Convert.ToString(Reader["Rua"].ToString());
                        Lbl_Erros.Text = null;
                    }
                    else if (Tb_Morada.Text.Trim().Contains(Convert.ToString(Reader["Rua"].ToString())))
                    {
                        Lbl_Erros.Text = null;
                    }
                    else
                    {
                        Lbl_Erros.Text = "Localidade: " + Convert.ToString(Reader["Localidade"].ToString()) + "\nRua: " + Convert.ToString(Reader["Rua"].ToString()) + "\nPara inserir a Morada apague o campo e clique em \"Procurar Código Postal\"";
                    }

                    CodigoValido = true;
                }
                else
                {
                    Lbl_PesquisaCodigo.Content = "Não existe nenhum resultado!";
                    Lbl_Erros.Text = null;
                    keyCodigo = null;

                    CodigoValido = false;
                }

                Reader.Close();
                conexao.Close();
            }
            catch (Exception ex)
            {
                Lbl_Erros.Text = "Erro Inesperado!\nVerifique a lista de erros conhecidos.\nErro: " + ex;
                CodigoValido = false;
                keyCodigo = null;
            }

            AtualizarBotoes();

            return keyCodigo;      
        }

        //Atualiza os botoes caso os campos estejam incorretos ou corretos
        void AtualizarBotoes()
        {
            if (Tb_Email.Text.Length == 0 && Tb_Telemovel.Text.Length == 0 && Tb_Telefone.Text.Length == 0)
            {
                Btn_GuardarCliente.IsEnabled = false;
                Btn_GuardarAlteracoes.IsEnabled = false;
                Lbl_ErroContacto.Content = "Insira pelo menos uma forma de contacto!";
                Lbl_ErroContacto.Visibility = Visibility.Visible;
            }
            else if (!NomeValido || !CodigoValido || !TelemovelValido || !TelefoneValido || !EmailValido)
            {
                Btn_GuardarCliente.IsEnabled = false;
                Btn_GuardarAlteracoes.IsEnabled = false;
                Lbl_ErroContacto.Visibility = Visibility.Hidden;
            }
            else if (NomeValido && CodigoValido && TelemovelValido && TelefoneValido && EmailValido)
            {
                Btn_GuardarCliente.IsEnabled = true;
                Btn_GuardarAlteracoes.IsEnabled = true;
                Lbl_ErroContacto.Visibility = Visibility.Hidden;
            }
        }

        //Chama função para validar o nome aoo fim de 1 segundos
        private void Timer_Tick(object sender, EventArgs e)
        {
            temporizador.Stop();

            VerificarNome();
        }

        //Verificar se o nome nao existe na base de dados
        void VerificarNome()
        {
            string nome = Tb_NomeCliente.Text.Trim();

            if (nome.Length > 0)
            {
                try
                {
                    conexao.Open();
                    queryProcurarCliente.Connection = conexao;
                    queryProcurarCliente.Parameters.AddWithValue("@nome", nome);

                    Reader = queryProcurarCliente.ExecuteReader();
                    queryProcurarCliente.Parameters.Clear();

                    if (Reader.HasRows)
                    {
                        Reader.Read();

                        if (Lst_Clientes.SelectedIndex >= 0 && Lbl_CodigoCliente.Content.ToString() == Convert.ToString(Reader["Key_Cliente"].ToString()))
                        {
                            Lbl_Erros.Text = null;
                            NomeValido = true;
                        }
                        else
                        {
                            Lbl_Erros.Text = "Este nome de cliente já existe!\nCódigo Cliente: " + Convert.ToString(Reader["Key_Cliente"].ToString());
                            NomeValido = false;
                        }
                    }
                    else
                    {
                        Lbl_Erros.Text = null;
                        NomeValido = true;
                    }

                    Reader.Close();
                    conexao.Close();
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

        //Verificar se o nome já existe na base de dados (ao fim de 3 segundos)
        //private void Timer_Tick(object sender, EventArgs e)
        //{
        //    string nome = Tb_NomeCliente.Text;

        //    conexao.Open();
        //    queryProcurarCliente.Connection = conexao;
        //    queryProcurarCliente.Parameters.AddWithValue("@nome", "%" + nome + "%");

        //    Reader = queryProcurarCliente.ExecuteReader();
        //    queryProcurarCliente.Parameters.Clear();

        //    if (Reader.HasRows)
        //    {
        //        Reader.Read();
        //        Lbl_Erros.Text = "Este nome de Cliente já existe!\nCódigo Cliente: " + Convert.ToString(Reader["Key_Cliente"].ToString()) + "\nNome Cliente: " + Convert.ToString(Reader["Nome"].ToString());
        //        Btn_GuardarCliente.IsEnabled = false;
        //    }
        //    else
        //    {
        //        Lbl_Erros.Text = null;
        //        Btn_GuardarCliente.IsEnabled = true;
        //    }

        //    Reader.Close();
        //    conexao.Close();
        //}
    }
}
