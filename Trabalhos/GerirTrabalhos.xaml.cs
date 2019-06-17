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
        SqlCommand queryTodosTrabalhos = new SqlCommand("SELECT Key_Trabalho, Key_Cliente, Trabalho.Nome AS 'Trabalho', Descricao FROM Trabalho INNER JOIN Cliente ON Trabalho.Key_Cliente = Cliente.Key_Cliente ORDER BY Trabalho.Nome, Cliente.Nome");
        SqlCommand queryTodosClientes = new SqlCommand("SELECT Key_Cliente, Nome, Email, Telemovel, Telefone FROM Cliente ORDER BY Nome");
        SqlCommand queryTodosServicos = new SqlCommand("SELECT Key_Servico, Nome, Preco FROM Servico ORDER BY Nome");
        SqlCommand queryIndexTrabalho = new SqlCommand("SELECT Key_Trabalho FROM Trabalho WHERE Key_Trabalho = @KeyTrabalho");
        SqlCommand queryIndexTempo = new SqlCommand("SELECT Key_Tempo FROM Tempo WHERE Key_Tempo = @KeyTempo");
        SqlCommand queryInserirTarefa = new SqlCommand("INSERT INTO Tarefa (Key_Tarefa, Key_Trabalho, Key_Servico, Desconto) VALUES (@KeyTarefa, @KeyTrabalho, @KeyServico, @Desconto)");
        SqlCommand queryInserirTempo = new SqlCommand("INSERT INTO Tempo (Key_Tempo, Key_Tarefa, DataInicio, DataFim) VALUES (@KeyTempo, @KeyTarefa, @DataInicio, @DataFim)");
        SqlCommand queryAtualizarTarefa = new SqlCommand("UPDATE Tarefa SET Key_Servico = @KeyServico, Desconto = @Desconto WHERE Key_Tarefa = @KeyTarefa");
        SqlCommand queryAtualizarTempo = new SqlCommand("UPDATE Tempo SET DataInicio = @DataInicio, DataFim = @DataFim WHERE Key_Tempo = @KeyTempo");
        SqlCommand queryApagarTarefa = new SqlCommand("DELETE FROM Tarefa WHERE Key_Tarefa = @KeyTarefa");
        SqlCommand queryApagarTodosTempos = new SqlCommand("DELETE FROM Tempo WHERE Key_Tarefa = @KeyTarefa");
        SqlCommand queryApagarTempo = new SqlCommand("DELETE FROM Tempo WHERE Key_Tempo = @KeyTempo");

        SqlDataReader Reader;

        List<Trabalho> trabalhos = new List<Trabalho>();
        List<Cliente> clientes = new List<Cliente>();

        //Gerar chave aleatória 
        public static string RandomString(int length)
        {
            Random random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            return new string(Enumerable.Range(1, length).Select(_ => chars[random.Next(chars.Length)]).ToArray());
        }

        bool KeyValido = false;
        bool TrabalhoValido = false;
        bool TarefaValido = false;

        bool Adicionar = false;

        public GerirTrabalhos()
        {
            InitializeComponent();

            LigarBaseDados();
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
                    trabalhos.Add(new Trabalho { ChaveTrabalho = Convert.ToString(Reader["Key_Trabalho"].ToString()), ChaveCliente = Convert.ToString(Reader["Key_Cliente"].ToString()), Nome = Convert.ToString(Reader["Trabalho"].ToString()), Descricao = Convert.ToString(Reader["Descricao"].ToString()) });
                }

                Reader.Close();

                queryTodosClientes.Connection = DataBase.conexao;
                Reader = queryTodosClientes.ExecuteReader();

                while (Reader.Read())
                {
                    string contacto = ContactoVisivel(Convert.ToString(Reader["Email"].ToString()), Convert.ToInt64(Reader["Telemovel"].ToString()), Convert.ToInt64(Reader["Telefone"].ToString()));

                    clientes.Add(new Cliente { ChaveCliente = Convert.ToString(Reader["Key_Cliente"].ToString()), Nome = Convert.ToString(Reader["Nome"].ToString()), DataNascimento = Convert.ToDateTime(null), Sexo = null, Morada = null, CodigoPostal = null, Localidade = null, Email = Convert.ToString(Reader["Email"].ToString()), Telemovel = Convert.ToInt64(Reader["Telemovel"].ToString()), Telefone = Convert.ToInt64(Reader["Telefone"].ToString()), Contacto = contacto });
                }

                Reader.Close();
                DataBase.conexao.Close();
            }
            catch (Exception ex)
            {
                Lbl_Erros.Text = "Erro Inesperado!\nVerifique a lista de erros conhecidos.\nErro: " + ex;
                Btn_AdicionarTrabalho.IsEnabled = false;
                Btn_AtualizarTrabalho.IsEnabled = false;
            }
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
            Lst_Trabalhos.SelectedIndex = -1;
            Lbl_Erros.Text = null;
        }

        //Atualiza os botoes caso os campos estejam incorretos ou corretos
        void AtualizarBotoes()
        {
            if (!KeyValido || !TrabalhoValido || !TarefaValido)
            {
                Btn_GuardarTrabalho.IsEnabled = false;

            }
            else if (KeyValido && TrabalhoValido && TarefaValido)
            {
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
}
