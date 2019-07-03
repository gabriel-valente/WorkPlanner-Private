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

namespace Trabalhos
{
    /// <summary>
    /// Interaction logic for GerirTrabalhos.xaml
    /// </summary>
    public partial class GerirTrabalhos : Page
    {
        SqlCommand queryTodosTrabalhos = new SqlCommand("SELECT Key_Trabalho, Trabalho.Key_Cliente AS 'Cliente', Trabalho.Nome AS 'Trabalho', Descricao, Pago FROM Trabalho INNER JOIN Cliente ON Trabalho.Key_Cliente = Cliente.Key_Cliente ORDER BY Trabalho.Nome, Cliente.Nome");
        SqlCommand queryTodosClientes = new SqlCommand("SELECT Key_Cliente, Nome, Email, Telemovel, Telefone FROM Cliente ORDER BY Nome");
        SqlCommand queryTodosServicos = new SqlCommand("SELECT Key_Servico, Nome, Preco FROM Servico ORDER BY Nome");
        SqlCommand queryTodasTarefas = new SqlCommand("SELECT Key_Tarefa, Key_Servico, Desconto FROM Tarefa WHERE Key_Trabalho = @KeyTrabalho");
        SqlCommand queryTodosTempos = new SqlCommand("SELECT Key_Tempo, DataInicio, DataFim FROM Tempo WHERE Key_Tarefa = @KeyTarefa ORDER BY DataInicio, DataFim");
        SqlCommand queryIndexTrabalho = new SqlCommand("SELECT Key_Trabalho FROM Trabalho WHERE Key_Trabalho = @KeyTrabalho");
        SqlCommand queryInserirTrabalho = new SqlCommand("INSERT INTO Trabalho (Key_Trabalho, Key_Cliente, Nome, Descricao, Pago) VALUES (@KeyTrabalho, @KeyCliente, @Nome, @Descricao, @Pago)");
        SqlCommand queryAtualizarTrabalho = new SqlCommand("UPDATE Trabalho SET Key_Cliente = @KeyCliente, Nome = @Nome, Descricao = @Descricao, Pago = @Pago WHERE Key_Trabalho = @KeyTrabalho");
        SqlCommand queryApagarTempo = new SqlCommand("DELETE FROM Tempo WHERE Key_Tarefa = @KeyTarefa");
        SqlCommand queryApagarTarefa = new SqlCommand("DELETE FROM Tarefa WHERE Key_Tarefa = @KeyTarefa");
        SqlCommand queryApagarTrabalho = new SqlCommand("DELETE FROM Trabalho WHERE Key_Trabalho = @KeyTrabalho");

        SqlDataReader Reader;

        List<Trabalho> trabalhos = new List<Trabalho>();
        List<ListaTrabalho> listaTrabalhos = new List<ListaTrabalho>();
        List<Cliente> clientes = new List<Cliente>();
        List<Servico> servicos = new List<Servico>();
        List<TrabalhoTarefas> tarefas = new List<TrabalhoTarefas>(); 

        //Gerar chave aleatória 
        public static string RandomString(int length)
        {
            Random random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            return new string(Enumerable.Range(1, length).Select(_ => chars[random.Next(chars.Length)]).ToArray());
        }

        decimal preco = 0;

        bool ClienteValido = false;
        bool TrabalhoValido = false;
        bool PrecoValido = true;

        bool Adicionar = false;

        //Iniciação
        public GerirTrabalhos()
        {
            InitializeComponent();

            //Carregarmento inicial mudado para Page_Loaded => CarrgarPagina();
        }

        //Recarregar dados da página
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            CarregarPagina();
        }

        //Funçoes de butoes e lista
        //Butao adicionar novo trabalho
        private void Btn_AdicionarTrabalho_Click(object sender, RoutedEventArgs e)
        {
            Adicionar = true;

            LimparCampos();

            Lbl_Erros.Text = "Para ser possivel gerir as tarefas é necessário adicionar o trabalho.\nApós todos os campos serem válidos se carregar em \"Gerir Tarefas\" este trabalho será guardado!";

            Lbl_CodigoTrabalho.Content = ReservarChave();

            tarefas.Clear();

            preco = 0;

            Lst_Tarefas.ItemsSource = tarefas;
            Lst_Tarefas.Items.Refresh();

            Lbl_Preco.Content = String.Format("{0:###0.00}€", preco);

            Tb_Trabalho.Text = null;
            Tb_Descricao.Text = null;
            Lbl_Cliente.Visibility = Visibility.Hidden;
            Cb_Cliente.Visibility = Visibility.Visible;
            Tb_Trabalho.IsReadOnly = false;
            Tb_Descricao.IsReadOnly = false;
            Lst_Tarefas.IsEnabled = true;
            Tb_ValorPago.IsReadOnly = false;
            Tb_ValorPago.Text = "0,00€";
            Lst_Trabalhos.IsEnabled = false;
            Btn_EditarTarefas.IsEnabled = false;
            Btn_EditarTarefas.Visibility = Visibility.Visible;
            Btn_GuardarTrabalho.Visibility = Visibility.Visible;
            Btn_CancelarTrabalho.Visibility = Visibility.Visible;
            Btn_AdicionarTrabalho.Visibility = Visibility.Hidden;
            Btn_AtualizarTrabalho.Visibility = Visibility.Hidden;
            Btn_ApagarTrabalho.Visibility = Visibility.Hidden;
        }

        //Botao atualizar trabalho
        private void Btn_AtualizarTrabalho_Click(object sender, RoutedEventArgs e)
        {
            Cb_Cliente.Text = Lbl_Cliente.Content.ToString();

            Lbl_Cliente.Visibility = Visibility.Hidden;
            Cb_Cliente.Visibility = Visibility.Visible;
            Tb_Trabalho.IsReadOnly = false;
            Tb_Descricao.IsReadOnly = false;
            Lst_Tarefas.IsEnabled = true;
            Tb_ValorPago.IsReadOnly = false;
            Lst_Trabalhos.IsEnabled = false;
            Btn_EditarTarefas.IsEnabled = true;
            Btn_EditarTarefas.Visibility = Visibility.Visible;
            Btn_GuardarAlteracoes.Visibility = Visibility.Visible;
            Btn_CancelarTrabalho.Visibility = Visibility.Visible;
            Btn_AdicionarTrabalho.Visibility = Visibility.Hidden;
            Btn_AtualizarTrabalho.Visibility = Visibility.Hidden;
            Btn_ApagarTrabalho.Visibility = Visibility.Hidden;
        }

        //Botao apagar tarefa selecionada
        private void Btn_ApagarTrabalho_Click(object sender, RoutedEventArgs e)
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
                queryApagarTarefa.Connection = DataBase.conexao;

                foreach (TrabalhoTarefas item in tarefas)
                {
                    queryApagarTempo.Parameters.AddWithValue("@KeyTarefa", item.ChaveTarefa);

                    Reader = queryApagarTempo.ExecuteReader();
                    queryApagarTempo.Parameters.Clear();

                    Reader.Close();

                    queryApagarTarefa.Parameters.AddWithValue("@KeyTarefa", item.ChaveTarefa);

                    Reader = queryApagarTarefa.ExecuteReader();
                    queryApagarTarefa.Parameters.Clear();

                    Reader.Close();
                }

                tarefas.Clear();

                queryApagarTrabalho.Connection = DataBase.conexao;
                queryApagarTrabalho.Parameters.AddWithValue("@KeyTrabalho", Lbl_CodigoTrabalho.Content.ToString());
                Reader = queryApagarTrabalho.ExecuteReader();
                queryApagarTrabalho.Parameters.Clear();

                Reader.Close();

                queryApagarTempo.Connection.Close();
                queryApagarTarefa.Connection.Close();
                queryApagarTrabalho.Connection.Close();
                DataBase.conexao.Close();

                listaTrabalhos.RemoveAt(Lst_Trabalhos.SelectedIndex);
                trabalhos.RemoveAt(Lst_Trabalhos.SelectedIndex);
                Lst_Trabalhos.Items.Refresh();

                LimparCampos();

                Btn_EditarTarefas.Visibility = Visibility.Hidden;
                Lbl_Receber.Content = null;
                Lbl_Erros.Text = "O trabalho foi apagado com sucesso!";

                BloquearFundo.Visibility = Visibility.Hidden;
                Grd_ValidarApagar.Visibility = Visibility.Hidden;
            }
            catch (Exception)
            {
                Lbl_Erros.Text = "Erro Inesperado!\nVá às definições e carregue em \"Ajuda\". Na página aberta descreva como aconteceu o problema.";
            }
        }

        //Botao cancelar apagar tarefa
        private void Btn_CancelarApagar_Click(object sender, RoutedEventArgs e)
        {
            Grd_ValidarApagar.Visibility = Visibility.Hidden;
            BloquearFundo.Visibility = Visibility.Hidden;
        }

        //Lista trabalhos
        private void Lst_Trabalhos_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CarregarPagina();
        }

        //Botao guardar trabalho
        private void Btn_GuardarTrabalho_Click(object sender, RoutedEventArgs e)
        {
            Adicionar = false;

            GuardarTrabalho();

            VoltarPaginaTrabalho.ChaveTrabalho = null;
            VoltarPaginaTrabalho.ChaveCliente = null;
            VoltarPaginaTrabalho.Trabalho = null;
            VoltarPaginaTrabalho.Descricao = null;
            VoltarPaginaTrabalho.Pago = null;

            InterPages.KeyTrabalho = null;
            InterPages.NomeTrabalho = null;
        }

        //Botao guardar alteracoes
        private void Btn_GuardarAlteracoes_Click(object sender, RoutedEventArgs e)
        {
            string keyCliente = clientes.Find(lst => lst.Nome == Cb_Cliente.Text).ChaveCliente;
            string pago = Tb_ValorPago.Text.Trim();

            if (pago.Contains("€"))
            {
                pago = pago.Remove(pago.Length - 1);
            }
            else if (pago.Length == 0)
            {
                pago = "0";
            }

            try
            {
                DataBase.conexao.Open();
                queryAtualizarTrabalho.Connection = DataBase.conexao;
                queryAtualizarTrabalho.Parameters.AddWithValue("@KeyCliente", clientes.Find(lst => lst.Nome == Cb_Cliente.Text).ChaveCliente);
                queryAtualizarTrabalho.Parameters.AddWithValue("@Nome", Tb_Trabalho.Text.Trim());
                queryAtualizarTrabalho.Parameters.AddWithValue("@Descricao", Tb_Descricao.Text.Trim());
                queryAtualizarTrabalho.Parameters.AddWithValue("@Pago", Convert.ToDecimal(pago));
                queryAtualizarTrabalho.Parameters.AddWithValue("@KeyTrabalho", Lbl_CodigoTrabalho.Content.ToString());

                queryAtualizarTrabalho.ExecuteNonQuery();
                queryAtualizarTrabalho.Parameters.Clear();
                queryAtualizarTrabalho.Connection.Close();

                trabalhos[Lst_Trabalhos.SelectedIndex].ChaveCliente = clientes.Find(lst => lst.Nome == Cb_Cliente.Text).ChaveCliente;
                trabalhos[Lst_Trabalhos.SelectedIndex].Nome = Tb_Trabalho.Text.Trim();
                trabalhos[Lst_Trabalhos.SelectedIndex].Descricao = Tb_Descricao.Text.Trim();
                trabalhos[Lst_Trabalhos.SelectedIndex].Pago = String.Format("{0:###0.00}€", pago);

                listaTrabalhos[Lst_Trabalhos.SelectedIndex].NomeCliente = Cb_Cliente.Text;
                listaTrabalhos[Lst_Trabalhos.SelectedIndex].NomeTrabalho = Tb_Trabalho.Text;

                tarefas.Clear();
                Lst_Tarefas.Items.Refresh();

                LimparCampos();

                ClienteValido = false;
                TrabalhoValido = false;
                PrecoValido = true;
                Lbl_Cliente.Visibility = Visibility.Visible;
                Cb_Cliente.Visibility = Visibility.Hidden;
                Tb_Trabalho.IsReadOnly = true;
                Tb_Descricao.IsReadOnly = true;
                Lst_Tarefas.IsEnabled = false;
                Btn_EditarTarefas.Visibility = Visibility.Hidden;
                Tb_ValorPago.IsReadOnly = true;
                Lbl_Receber.Content = null;
                Lst_Trabalhos.IsEnabled = true;
                Btn_GuardarTrabalho.Visibility = Visibility.Hidden;
                Btn_GuardarAlteracoes.Visibility = Visibility.Hidden;
                Btn_CancelarTrabalho.Visibility = Visibility.Hidden;
                Btn_AdicionarTrabalho.Visibility = Visibility.Visible;
                Btn_AtualizarTrabalho.Visibility = Visibility.Visible;
                Btn_ApagarTrabalho.Visibility = Visibility.Visible;
            }
            catch (Exception)
            {
                Lbl_Erros.Text = "Erro Inesperado!\nVá às definições e carregue em \"Ajuda\". Na página aberta descreva como aconteceu o problema.";
            }
        }

        //Pagina editar tarefas
        private void Btn_EditarTarefas_Click(object sender, RoutedEventArgs e)
        {
            int index = trabalhos.FindIndex(lst => lst.ChaveTrabalho == Lbl_CodigoTrabalho.Content.ToString());

            VoltarPaginaTrabalho.ChaveTrabalho = Lbl_CodigoTrabalho.Content.ToString();
            VoltarPaginaTrabalho.Trabalho = Tb_Trabalho.Text;
            VoltarPaginaTrabalho.Descricao = Tb_Descricao.Text;
            VoltarPaginaTrabalho.Pago = Tb_ValorPago.Text;
            if (index == -1)
            {
                VoltarPaginaTrabalho.ChaveCliente = clientes.Find(lst => lst.Nome == Cb_Cliente.Text).ChaveCliente;

                GuardarTrabalho();
            }
            else
            {
                VoltarPaginaTrabalho.ChaveCliente = clientes.Find(lst => lst.Nome == Lbl_Cliente.Content.ToString()).ChaveCliente;
            }

            InterPages.KeyTrabalho = VoltarPaginaTrabalho.ChaveTrabalho;
            InterPages.NomeTrabalho = VoltarPaginaTrabalho.Trabalho;
            ((MainWindow)Application.Current.MainWindow).Frm_Principal.Content = new GerirTarefas();
        }

        //Botao cancelar nova tarefa
        private void Btn_CancelarTrabalho_Click(object sender, RoutedEventArgs e)
        {
            Adicionar = false;

            tarefas.Clear();

            InterPages.KeyTrabalho = null;
            InterPages.NomeTrabalho = null;

            LimparCampos();

            ClienteValido = false;
            TrabalhoValido = false;
            Lbl_Cliente.Visibility = Visibility.Visible;
            Cb_Cliente.Visibility = Visibility.Hidden;
            Tb_Trabalho.IsReadOnly = true;
            Tb_Descricao.IsReadOnly = true;
            Lst_Tarefas.IsEnabled = false;
            Btn_EditarTarefas.Visibility = Visibility.Hidden;
            Tb_ValorPago.IsReadOnly = true;
            Lbl_Receber.Content = null;
            Lst_Trabalhos.IsEnabled = true;
            Btn_GuardarTrabalho.Visibility = Visibility.Hidden;
            Btn_GuardarAlteracoes.Visibility = Visibility.Hidden;
            Btn_CancelarTrabalho.Visibility = Visibility.Hidden;
            Btn_AdicionarTrabalho.Visibility = Visibility.Visible;
            Btn_AtualizarTrabalho.Visibility = Visibility.Visible;
            Btn_ApagarTrabalho.Visibility = Visibility.Visible;
        }

        //Botao voltar para o menu principal
        private void Btn_Voltar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                VoltarPaginaTrabalho.ChaveTrabalho = Lbl_CodigoTrabalho.Content.ToString();
                VoltarPaginaTrabalho.ChaveCliente = clientes.Find(lst => lst.Nome == Cb_Cliente.Text).ChaveCliente;
                VoltarPaginaTrabalho.Trabalho = Tb_Trabalho.Text;
                VoltarPaginaTrabalho.Descricao = Tb_Descricao.Text;
                VoltarPaginaTrabalho.Pago = Tb_ValorPago.Text;
            }
            catch (Exception)
            {
            }          

            InterPages.KeyTrabalho = null;
            InterPages.NomeTrabalho = null;
            ((MainWindow)Application.Current.MainWindow).Frm_Principal.GoBack();
        }

        //Funções de validação
        //Validar cliente
        private void Cb_Cliente_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Cb_Cliente.SelectedIndex == -1)
            {
                ClienteValido = false;
            }
            else
            {
                ClienteValido = true;
            }

            AtualizarBotoes();
        }

        //Validar escrita do cliente
        private void Cb_Cliente_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                Cb_Cliente.Text = Cb_Cliente.Text.TrimStart();

                int index = clientes.FindIndex(lst => lst.Nome == Cb_Cliente.Text);

                if (index >= 0)
                {
                    if (Adicionar)
                    {
                        Lbl_Erros.Text = "Para ser possivel gerir as tarefas é necessário adicionar o trabalho.\nApós todos os campos serem válidos se carregar em \"Gerir Tarefas\" este trabalho será guardado!";
                    }
                    else
                    {
                        Lbl_Erros.Text = null;
                    }

                    ClienteValido = true;
                }
                else if (index == -1)
                {
                    Lbl_Erros.Text = "Tal serviço ainda não existe.\nSelecione um serviço da lista existente, ou procure ao escrever o nome do serviço.";
                    ClienteValido = false;
                }
            }
            catch (Exception)
            {
            }

            AtualizarBotoes();
        }

        //Validar nome trabalho
        private void Tb_Trabalho_TextChanged(object sender, TextChangedEventArgs e)
        {
            Tb_Trabalho.Text = Tb_Trabalho.Text.TrimStart();
            char[] trabalho = Tb_Trabalho.Text.ToCharArray();

            try
            {
                trabalho[0] = char.ToUpper(trabalho[0]);
            }
            catch (Exception)
            {
            }

            Tb_Trabalho.Text = new string(trabalho);
            Tb_Trabalho.SelectionStart = Tb_Trabalho.Text.Length;

            if (Tb_Trabalho.Text.Length >= 3)
            {
                TrabalhoValido = true;
            }
            else
            {
                TrabalhoValido = false;
            }

            AtualizarBotoes();
        }

        //Validar descricao
        private void Tb_Descricao_TextChanged(object sender, TextChangedEventArgs e)
        {
            Tb_Descricao.Text = Tb_Descricao.Text.TrimStart();
            char[] descricao = Tb_Descricao.Text.ToCharArray();

            try
            {
                descricao[0] = char.ToUpper(descricao[0]);
            }
            catch (Exception)
            {
            }

            Tb_Descricao.Text = new string(descricao);
            Tb_Descricao.SelectionStart = Tb_Descricao.Text.Length;
        }

        //Validar valor pago
        private void Tb_ValorPago_TextChanged(object sender, TextChangedEventArgs e)
        {
            Tb_ValorPago.Text = Tb_ValorPago.Text.TrimStart();
            char[] preco = Tb_ValorPago.Text.ToCharArray();

            bool comma = false;
            byte pos = Convert.ToByte(Tb_ValorPago.SelectionStart);

            for (int i = 0; i < preco.Length; i++)
            {
                if (comma == false && (preco[i] == ',' || preco[i] == '.'))
                {
                    comma = true;
                    preco[i] = ',';
                    Tb_ValorPago.Text = new string(preco);
                    Tb_ValorPago.SelectionStart = pos;
                }
                else if (comma == true && (preco[i] == ',' || preco[i] == '.'))
                {
                    Tb_ValorPago.Text = Tb_ValorPago.Text.Remove(i, 1);
                    Array.Clear(preco, 0, preco.Length);
                    preco = Tb_ValorPago.Text.TrimStart().ToCharArray();
                    Tb_ValorPago.SelectionStart = pos;
                }
            }

            if (Tb_ValorPago.Text.Length > 0 && preco[0] == ',')
            {
                Tb_ValorPago.Text = Tb_ValorPago.Text.Remove(0, 1);
                Array.Clear(preco, 0, preco.Length);
                preco = Tb_ValorPago.Text.TrimStart().ToCharArray();
                Tb_ValorPago.SelectionStart = pos;
            }

            for (int i = 0; i < preco.Length; i++)
            {
                if (i < preco.Length - 1 && preco[i] == '€')
                {
                    Tb_ValorPago.Text = Tb_ValorPago.Text.Remove(i, 1);
                    Array.Clear(preco, 0, preco.Length);
                    preco = Tb_ValorPago.Text.Trim().ToCharArray();
                    Tb_ValorPago.SelectionStart = i;
                }
            }

            for (int i = 0; i < preco.Length; i++)
            {
                if (!char.IsDigit(preco[i]) && preco[i] != ',' && preco[i] != '€')
                {
                    Tb_ValorPago.Text = Tb_ValorPago.Text.Remove(i, 1);
                    Array.Clear(preco, 0, preco.Length);
                    preco = Tb_ValorPago.Text.Trim().ToCharArray();
                    Tb_ValorPago.SelectionStart = i;
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
                    Tb_ValorPago.Text = Tb_ValorPago.Text.Remove(i, 1);
                    Array.Clear(preco, 0, preco.Length);
                    preco = Tb_ValorPago.Text.Trim().ToCharArray();
                    Tb_ValorPago.SelectionStart = i;
                }
                else if (i >= passComma + 3 && preco[i] != '€')
                {
                    Tb_ValorPago.Text = Tb_ValorPago.Text.Remove(i, 1);
                    Array.Clear(preco, 0, preco.Length);
                    preco = Tb_ValorPago.Text.Trim().ToCharArray();
                    Tb_ValorPago.SelectionStart = i;
                }
            }

            decimal valor;

            if (Tb_ValorPago.Text.Contains("€"))
            {
                decimal.TryParse(Tb_ValorPago.Text.Remove(Tb_ValorPago.Text.Length - 1), out valor);
            }
            else
            {
                decimal.TryParse(Tb_ValorPago.Text, out valor);
            }

            decimal total;

            try
            {
                total = Convert.ToDecimal(Lbl_Preco.Content.ToString().Remove(Lbl_Preco.Content.ToString().Length - 1));
            }
            catch (Exception)
            {
                total = 0;
            }

            decimal resta = total - valor;

            Lbl_Receber.Content = String.Format("{0:###0.00}€", resta);

            if (valor.ToString().Length <= 0 || valor < 0)
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
        //Ligar com base de dados e ler todos os dados
        void LigarBaseDados()
        {
            try
            {
                DataBase.conexao = new SqlConnection(DataBase.stringConexao);

                DataBase.conexao.Open();
                queryTodosTrabalhos.Connection = DataBase.conexao;
                Reader = queryTodosTrabalhos.ExecuteReader();

                while (Reader.Read())
                {
                    trabalhos.Add(new Trabalho { ChaveTrabalho = Convert.ToString(Reader["Key_Trabalho"].ToString()), ChaveCliente = Convert.ToString(Reader["Cliente"].ToString()), Nome = Convert.ToString(Reader["Trabalho"].ToString()), Descricao = Convert.ToString(Reader["Descricao"].ToString()), Pago = Convert.ToString(Reader["Pago"].ToString()) });
                }

                Reader.Close();

                queryTodosClientes.Connection = DataBase.conexao;
                Reader = queryTodosClientes.ExecuteReader();

                while (Reader.Read())
                {
                    clientes.Add(new Cliente { ChaveCliente = Convert.ToString(Reader["Key_Cliente"].ToString()), Nome = Convert.ToString(Reader["Nome"].ToString()), DataNascimento = Convert.ToDateTime(null), Sexo = null, Morada = null, CodigoPostal = null, Localidade = null, Email = Convert.ToString(Reader["Email"].ToString()), Telemovel = Convert.ToInt64(Reader["Telemovel"].ToString()), Telefone = Convert.ToInt64(Reader["Telefone"].ToString()), Contacto = null });
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
            catch (Exception)
            {
                Lbl_Erros.Text = "Erro Inesperado!\nVá às definições e carregue em \"Ajuda\". Na página aberta descreva como aconteceu o problema.";
                Btn_AdicionarTrabalho.IsEnabled = false;
                Btn_AtualizarTrabalho.IsEnabled = false;
            }
        }

        //Guardar Trabalho
        void GuardarTrabalho()
        {
            string keyCliente = clientes.Find(lst => lst.Nome == Cb_Cliente.Text).ChaveCliente;
            string pago = Tb_ValorPago.Text.Trim();

            if (pago.Contains("€"))
            {
                pago = pago.Remove(pago.Length - 1);
            }
            else if (pago.Length == 0)
            {
                pago = "0";
            }

            try
            {
                DataBase.conexao.Open();
                queryInserirTrabalho.Connection = DataBase.conexao;
                queryInserirTrabalho.Parameters.AddWithValue("@KeyTrabalho", Lbl_CodigoTrabalho.Content.ToString());
                queryInserirTrabalho.Parameters.AddWithValue("@KeyCliente", keyCliente);
                queryInserirTrabalho.Parameters.AddWithValue("@Nome", Tb_Trabalho.Text.Trim());
                queryInserirTrabalho.Parameters.AddWithValue("@Descricao", Tb_Descricao.Text.Trim());
                queryInserirTrabalho.Parameters.AddWithValue("@Pago", Convert.ToDecimal(pago));

                queryInserirTrabalho.ExecuteNonQuery();
                queryInserirTrabalho.Parameters.Clear();

                queryInserirTrabalho.Connection.Close();
                DataBase.conexao.Close();

                trabalhos.Add(new Trabalho { ChaveTrabalho = Lbl_CodigoTrabalho.Content.ToString(), ChaveCliente = keyCliente, Nome = Tb_Trabalho.Text.Trim(), Descricao = Tb_Descricao.Text.Trim(), Pago = String.Format("{0:###0.00}€", pago) });
                listaTrabalhos.Add(new ListaTrabalho { ChaveTrabalho = Lbl_CodigoTrabalho.Content.ToString(), NomeCliente = Cb_Cliente.Text, NomeTrabalho = Tb_Trabalho.Text.Trim() });

                tarefas.Clear();
                Lst_Tarefas.Items.Refresh();

                Lst_Trabalhos.Items.Refresh();

                LimparCampos();

                ClienteValido = false;
                TrabalhoValido = false;
                PrecoValido = true;
                Lbl_Cliente.Visibility = Visibility.Visible;
                Cb_Cliente.Visibility = Visibility.Hidden;
                Tb_Trabalho.IsReadOnly = true;
                Tb_Descricao.IsReadOnly = true;
                Lst_Tarefas.IsEnabled = false;
                Btn_EditarTarefas.Visibility = Visibility.Hidden;
                Tb_ValorPago.IsReadOnly = true;
                Lbl_Receber.Content = null;
                Lst_Trabalhos.IsEnabled = true;
                Btn_GuardarTrabalho.Visibility = Visibility.Hidden;
                Btn_GuardarAlteracoes.Visibility = Visibility.Hidden;
                Btn_CancelarTrabalho.Visibility = Visibility.Hidden;
                Btn_AdicionarTrabalho.Visibility = Visibility.Visible;
                Btn_AtualizarTrabalho.Visibility = Visibility.Visible;
                Btn_ApagarTrabalho.Visibility = Visibility.Visible;
            }
            catch (Exception)
            {
                Lbl_Erros.Text = "Erro Inesperado!\nVá às definições e carregue em \"Ajuda\". Na página aberta descreva como aconteceu o problema.";
            }
        }

        //Carregar conteudo página
        void CarregarPagina()
        {
            if (Lst_Trabalhos.SelectedIndex >= 0)
            {
                tarefas.Clear();

                DataBase.conexao.Open();
                queryTodasTarefas.Connection = DataBase.conexao;
                queryTodasTarefas.Parameters.AddWithValue("@KeyTrabalho", listaTrabalhos[Lst_Trabalhos.SelectedIndex].ChaveTrabalho);
                Reader = queryTodasTarefas.ExecuteReader();

                while (Reader.Read())
                {
                    tarefas.Add(new TrabalhoTarefas
                    {
                        ChaveTarefa = Convert.ToString(Reader["Key_Tarefa"].ToString()),
                        Tarefa = servicos.Find(lst => lst.ChaveServico == Convert.ToString(Reader["Key_Servico"].ToString())).Nome,
                        Tempo = new TimeSpan(0),
                        Preco = Convert.ToString(Reader["Desconto"].ToString())
                    });
                }

                queryTodasTarefas.Parameters.Clear();
                Reader.Close();
                queryTodasTarefas.Connection.Close();
                DataBase.conexao.Close();

                decimal total = 0;

                foreach (TrabalhoTarefas item in tarefas)
                {
                    DataBase.conexao.Open();
                    queryTodosTempos.Connection = DataBase.conexao;

                    decimal desconto = Convert.ToDecimal(item.Preco);

                    queryTodosTempos.Parameters.AddWithValue("@KeyTarefa", item.ChaveTarefa);
                    Reader = queryTodosTempos.ExecuteReader();

                    TimeSpan time = new TimeSpan(0);

                    while (Reader.Read())
                    {
                        try
                        {
                            time += Convert.ToDateTime(Reader["DataFim"].ToString()) - Convert.ToDateTime(Reader["DataInicio"].ToString());
                        }
                        catch (Exception)
                        {
                            time += new TimeSpan(0);
                        }
                    }

                    queryTodosTempos.Parameters.Clear();
                    Reader.Close();
                    queryTodosTempos.Connection.Close();
                    DataBase.conexao.Close();

                    decimal valor = (servicos.Find(lst => lst.Nome == item.Tarefa).Preco * Convert.ToDecimal(time.TotalHours)) * (1 - desconto);

                    total += valor;

                    item.Tempo = time;
                    item.Preco = String.Format("{0:###0.00}€", valor);
                }

                Lbl_CodigoTrabalho.Content = trabalhos[Lst_Trabalhos.SelectedIndex].ChaveTrabalho;
                Lbl_Cliente.Content = clientes.Find(lst => lst.ChaveCliente == trabalhos[Lst_Trabalhos.SelectedIndex].ChaveCliente).Nome;
                Tb_Trabalho.Text = trabalhos[Lst_Trabalhos.SelectedIndex].Nome;
                Tb_Descricao.Text = trabalhos[Lst_Trabalhos.SelectedIndex].Descricao;
                Lbl_Preco.Content = String.Format("{0:###0.00}€", total);
                Tb_ValorPago.Text = null;
                Tb_ValorPago.Text = String.Format("{0:#####0,00}€", trabalhos[Lst_Trabalhos.SelectedIndex].Pago);

                Lst_Tarefas.ItemsSource = tarefas;
                Lst_Tarefas.Items.Refresh();

                InterPages.KeyTrabalho = trabalhos[Lst_Trabalhos.SelectedIndex].ChaveTrabalho;
                InterPages.NomeTrabalho = trabalhos[Lst_Trabalhos.SelectedIndex].Nome;

                Btn_EditarTarefas.IsEnabled = true;
                Btn_EditarTarefas.Visibility = Visibility.Visible;
                Btn_AtualizarTrabalho.IsEnabled = true;
                Btn_ApagarTrabalho.IsEnabled = true;
            }
            else if (Lst_Tarefas.SelectedIndex == -1)
            {
                trabalhos.Clear();
                listaTrabalhos.Clear();
                clientes.Clear();
                servicos.Clear();
                tarefas.Clear();

                LigarBaseDados();

                foreach (Trabalho item in trabalhos)
                {
                    listaTrabalhos.Add(new ListaTrabalho { ChaveTrabalho = item.ChaveTrabalho, NomeCliente = clientes.Find(lst => lst.ChaveCliente == item.ChaveCliente).Nome, NomeTrabalho = item.Nome });
                }

                Lst_Trabalhos.ItemsSource = listaTrabalhos;
                Lst_Trabalhos.Items.Refresh();

                Cb_Cliente.ItemsSource = clientes;
                Cb_Cliente.DisplayMemberPath = "Nome";
                Cb_Cliente.Items.Refresh();

                if (Adicionar)
                {
                    if (VoltarPaginaTrabalho.ChaveTrabalho != null)
                    {
                        Lst_Trabalhos.SelectedIndex = trabalhos.FindIndex(lst => lst.ChaveTrabalho == VoltarPaginaTrabalho.ChaveTrabalho);
                        Lbl_CodigoTrabalho.Content = VoltarPaginaTrabalho.ChaveTrabalho;
                        Lbl_Cliente.Content = clientes.Find(lst => lst.ChaveCliente == VoltarPaginaTrabalho.ChaveCliente).Nome;
                        Tb_Trabalho.Text = VoltarPaginaTrabalho.Trabalho;
                        Tb_Descricao.Text = VoltarPaginaTrabalho.Descricao;
                        Tb_ValorPago.Text = null;
                        Tb_ValorPago.Text = VoltarPaginaTrabalho.Pago;
                        tarefas.Clear();

                        DataBase.conexao.Open();
                        queryTodasTarefas.Connection = DataBase.conexao;
                        queryTodasTarefas.Parameters.AddWithValue("@KeyTrabalho", VoltarPaginaTrabalho.ChaveTrabalho);
                        Reader = queryTodasTarefas.ExecuteReader();

                        while (Reader.Read())
                        {
                            tarefas.Add(new TrabalhoTarefas { ChaveTarefa = Convert.ToString(Reader["Key_Tarefa"].ToString()), Tarefa = servicos.Find(lst => lst.ChaveServico == Convert.ToString(Reader["Key_Servico"].ToString())).Nome, Tempo = new TimeSpan(0), Preco = Convert.ToString(Reader["Desconto"].ToString()) });
                        }

                        queryTodasTarefas.Parameters.Clear();
                        Reader.Close();
                        queryTodasTarefas.Connection.Close();
                        DataBase.conexao.Close();

                        decimal total = 0;

                        foreach (TrabalhoTarefas item in tarefas)
                        {
                            DataBase.conexao.Open();
                            queryTodosTempos.Connection = DataBase.conexao;

                            decimal desconto = Convert.ToDecimal(item.Preco);

                            queryTodosTempos.Parameters.AddWithValue("@KeyTarefa", item.ChaveTarefa);
                            Reader = queryTodosTempos.ExecuteReader();

                            TimeSpan time = new TimeSpan(0);

                            while (Reader.Read())
                            {
                                if (Convert.ToString(Reader["DataFim"].ToString()) == "01/01/0001 00:00:00" || Convert.ToString(Reader["DataFim"].ToString()) == null)
                                {
                                    time += new TimeSpan(0);
                                }
                                else
                                {
                                    time += Convert.ToDateTime(Reader["DataFim"].ToString()) - Convert.ToDateTime(Reader["DataInicio"].ToString());
                                }
                            }

                            queryTodosTempos.Parameters.Clear();
                            Reader.Close();
                            queryTodosTempos.Connection.Close();
                            DataBase.conexao.Close();

                            decimal valor = (servicos.Find(lst => lst.Nome == item.Tarefa).Preco * Convert.ToDecimal(time.TotalHours)) * (1 - desconto);

                            total += valor;

                            item.Tempo = time;
                            item.Preco = String.Format("{0:###0.00}€", valor);

                            Lst_Tarefas.ItemsSource = tarefas;
                            Lst_Tarefas.Items.Refresh();

                            Lbl_Preco.Content = String.Format("{0:###0.00}€", total);
                        }
                    }

                    Btn_AtualizarTrabalho.IsEnabled = true;
                    Btn_ApagarTrabalho.IsEnabled = true;
                }
                else
                {
                    Btn_AtualizarTrabalho.IsEnabled = false;
                    Btn_ApagarTrabalho.IsEnabled = false;
                }
            }
        }

        //Limpar todos os campos que estao indroduzidos
        void LimparCampos()
        {
            Lbl_CodigoTrabalho.Content = null;
            Lbl_Cliente.Content = null;
            Cb_Cliente.Text = null;
            Cb_Cliente.SelectedIndex = -1;
            Tb_Trabalho.Text = null;
            Tb_Descricao.Text = null;
            Lst_Tarefas.Items.Refresh();
            Lst_Tarefas.SelectedIndex = -1;
            Lbl_Preco.Content = null;
            Tb_ValorPago.Text = null;
            Lst_Trabalhos.SelectedIndex = -1;
            Lbl_Erros.Text = null;
        }

        //Atualiza os botoes caso os campos estejam incorretos ou corretos
        void AtualizarBotoes()
        {
            if (!ClienteValido || !TrabalhoValido || !PrecoValido)
            {
                Btn_EditarTarefas.IsEnabled = false;
                Btn_GuardarTrabalho.IsEnabled = false;
            }
            else if (ClienteValido && TrabalhoValido && PrecoValido)
            {
                InterPages.KeyTrabalho = Lbl_CodigoTrabalho.Content.ToString();
                InterPages.NomeTrabalho = Tb_Trabalho.Text;

                Btn_EditarTarefas.IsEnabled = true;
                Btn_GuardarTrabalho.IsEnabled = true;
            }
        }

        //Verificar se chave existe na base de dados
        string ReservarChave()
        {
            string key;

            DataBase.conexao.Open();

            while (true)
            {
                key = RandomString(10);
                queryIndexTrabalho.Connection = DataBase.conexao;
                queryIndexTrabalho.Parameters.AddWithValue("@KeyTrabalho", key);
                Reader = queryIndexTrabalho.ExecuteReader();

                queryIndexTrabalho.Parameters.Clear();

                if (!Reader.HasRows)
                {
                    break;
                }
            }

            Reader.Close();
            DataBase.conexao.Close();

            return key;
        }
    }

    internal class ListaTrabalho
    {
        public string ChaveTrabalho { get; set; }
        public string NomeCliente { get; set; }
        public string NomeTrabalho { get; set; }
    }
}
