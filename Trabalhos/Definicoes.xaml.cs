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
using System.IO;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace Trabalhos
{
    /// <summary>
    /// Interaction logic for Definicoes.xaml
    /// </summary>
    public partial class Definicoes : Page
    {
        List<string> configs = new List<string>(); //Index tempo cópia, Local cópia, Idade minima, Index contacto preferivel

        bool TempoCopiaValido = false;
        bool LocalCopiaValido = false;
        bool IdadeMinimaValido = false;
        bool ContactoValido = false;

        public Definicoes()
        {
            InitializeComponent();

            LerConfigs();

            try
            {
                Cmb_TempoCopia.SelectedIndex = Convert.ToInt32(configs[0]);
                Tb_Localbackup.Text = configs[1];
                Tb_IdadeMinima.Text = configs[2];
                Cmb_Contacto.SelectedIndex = Convert.ToByte(configs[3]);
            }
            catch (Exception)
            {
                Cmb_TempoCopia.SelectedIndex = -1;
                Tb_Localbackup.Text = null;
                Tb_IdadeMinima.Text = null;
                Cmb_Contacto.SelectedIndex = -1;
            }

            AtivarButao();
        }

        //Validar tempo de cópia
        private void Cmb_TempoCopia_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Cmb_TempoCopia.SelectedIndex == -1)
            {
                TempoCopiaValido = false;
            }
            else
            {
                TempoCopiaValido = true;
            }

            AtivarButao();
        }

        //Abrir Folder Dialog
        private void Btn_LocalBackup_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new CommonOpenFileDialog();
            dialog.IsFolderPicker = true;
            CommonFileDialogResult result = dialog.ShowDialog();
            Tb_Localbackup.Text = dialog.FileName.ToString();
        }

        //Validar local backup
        private void Tb_Localbackup_TextChanged(object sender, TextChangedEventArgs e)
        {
            Tb_Localbackup.Text = Tb_Localbackup.Text.TrimStart();

            if (!Directory.Exists(Tb_Localbackup.Text.TrimEnd()))
            {
                LocalCopiaValido = false;
            }
            else
            {
                LocalCopiaValido = true;
            }

            AtivarButao();
        }

        //Validar idade minima
        private void Tb_IdadeMinima_TextChanged(object sender, TextChangedEventArgs e)
        {
            Tb_IdadeMinima.Text = Tb_IdadeMinima.Text.Trim();
            char[] idade = Tb_IdadeMinima.Text.ToCharArray();

            for (int i = 0; i < idade.Length; i++)
            {
                if (char.IsWhiteSpace(idade[i]) || !char.IsDigit(idade[i]))
                {
                    Tb_IdadeMinima.Text = Tb_IdadeMinima.Text.Remove(i, 1);
                    Array.Clear(idade, 0, idade.Length);
                    idade = Tb_IdadeMinima.Text.Trim().ToCharArray();
                    Tb_IdadeMinima.SelectionStart = i;
                }
            }

            if (string.IsNullOrWhiteSpace(Tb_IdadeMinima.Text))
            {
                IdadeMinimaValido = false;
            }
            else
            {
                IdadeMinimaValido = true;
            }

            AtivarButao();
        }

        //Validar contacto preferivel
        private void Cmb_Contacto_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Cmb_Contacto.SelectedIndex == -1)
            {
                ContactoValido = false;
            }
            else
            {
                ContactoValido = true;
            }

            AtivarButao();
        }

        //Botão guardar e voltar ao menu anterior
        private void Btn_Guardar_Click(object sender, RoutedEventArgs e)
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

            config.AppSettings.Settings.Remove("CopiaSeguranca");
            config.AppSettings.Settings.Remove("LocalCopiaSeguranca");
            config.AppSettings.Settings.Remove("IdadeMinima");
            config.AppSettings.Settings.Remove("ContactoPreferivel");

            config.AppSettings.Settings.Add("CopiaSeguranca", Cmb_TempoCopia.SelectedIndex.ToString());
            config.AppSettings.Settings.Add("LocalCopiaSeguranca", Tb_Localbackup.Text.Trim());
            config.AppSettings.Settings.Add("IdadeMinima", Tb_IdadeMinima.Text);
            config.AppSettings.Settings.Add("ContactoPreferivel", Cmb_Contacto.SelectedIndex.ToString());

            config.Save(ConfigurationSaveMode.Minimal);

            ConfigurationManager.RefreshSection("appSettings");

            LerConfigs();

            ((MainWindow)Application.Current.MainWindow).Frm_Principal.GoBack();
        }

        //Botão voltar menu anterior
        private void Btn_Voltar_Click(object sender, RoutedEventArgs e)
        {
            ((MainWindow)Application.Current.MainWindow).Frm_Principal.GoBack();
        }

        //Função ler as configurações guardadas
        void LerConfigs()
        {
            foreach (string chave in ConfigurationManager.AppSettings)
            {
                string valor = ConfigurationManager.AppSettings[chave];
                configs.Add(valor);
            }
        }

        //Função ativar butão
        void AtivarButao()
        {
            Console.WriteLine(TempoCopiaValido + " " + LocalCopiaValido + " " + IdadeMinimaValido + " " + ContactoValido);
            if (TempoCopiaValido && LocalCopiaValido && IdadeMinimaValido && ContactoValido)
            {
                Btn_Guardar.IsEnabled = true;
            }
            else
            {
                Btn_Guardar.IsEnabled = false;
            }
        }
    }
}
