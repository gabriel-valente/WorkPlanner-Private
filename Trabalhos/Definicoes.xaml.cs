﻿using System;
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

namespace Trabalhos
{
    /// <summary>
    /// Interaction logic for Definicoes.xaml
    /// </summary>
    public partial class Definicoes : Page
    {
        bool IdadeMinimaValido = false;
        bool ContactoValido = false;
        bool ServicoPrecoMinimoValido = false;

        public Definicoes()
        {
            InitializeComponent();

            LerConfigs();

            Tb_IdadeMinima.Text = Convert.ToString(Configuracoes.IdadeMinima);
            Cmb_Contacto.SelectedIndex = Configuracoes.ContactoPreferivel;
            Tb_ServicoPrecoMinimo.Text = Convert.ToString(Configuracoes.ServicoPrecoMinimo);

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

        //Validar aviso de preço minimo nos servicos
        private void Tb_ServicoPrecoMinimo_TextChanged(object sender, TextChangedEventArgs e)
        {
            Tb_ServicoPrecoMinimo.Text = Tb_ServicoPrecoMinimo.Text.TrimStart();
            char[] preco = Tb_ServicoPrecoMinimo.Text.ToCharArray();

            bool comma = false;

            for (int i = 0; i < preco.Length; i++)
            {
                if (comma == false && (preco[i] == ',' || preco[i] == '.'))
                {
                    comma = true;
                    preco[i] = ',';
                    Tb_ServicoPrecoMinimo.Text = preco.ToString();
                }
                else if (comma == true && (preco[i] == ',' || preco[i] == '.'))
                {
                    Tb_ServicoPrecoMinimo.Text = Tb_ServicoPrecoMinimo.Text.Remove(i, 1);
                    Array.Clear(preco, 0, preco.Length);
                    preco = Tb_ServicoPrecoMinimo.Text.TrimStart().ToCharArray();
                    Tb_ServicoPrecoMinimo.SelectionStart = i;
                }
            }

            for (int i = 0; i < preco.Length; i++)
            {
                if (!char.IsDigit(preco[i]) && preco[i] != ',')
                {
                    Tb_ServicoPrecoMinimo.Text = Tb_ServicoPrecoMinimo.Text.Remove(i, 1);
                    Array.Clear(preco, 0, preco.Length);
                    preco = Tb_ServicoPrecoMinimo.Text.TrimStart().ToCharArray();
                    Tb_ServicoPrecoMinimo.SelectionStart = i;
                }
            }

            if (string.IsNullOrWhiteSpace(Tb_ServicoPrecoMinimo.Text) || Convert.ToDecimal(Tb_ServicoPrecoMinimo.Text) < 0)
            {
                ServicoPrecoMinimoValido = false;
            }
            else
            {
                ServicoPrecoMinimoValido = true;
            }

            AtivarButao();
        }

        //Botão guardar e voltar ao menu anterior
        private void Btn_Guardar_Click(object sender, RoutedEventArgs e)
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

            config.AppSettings.Settings.Remove("IdadeMinima");
            config.AppSettings.Settings.Remove("ContactoPreferivel");
            config.AppSettings.Settings.Remove("ServicoPrecoMinimo");

            config.AppSettings.Settings.Add("IdadeMinima", Tb_IdadeMinima.Text);
            config.AppSettings.Settings.Add("ContactoPreferivel", Cmb_Contacto.SelectedIndex.ToString());
            config.AppSettings.Settings.Add("ServicoPrecoMinimo", Tb_ServicoPrecoMinimo.Text.Trim());

            config.Save(ConfigurationSaveMode.Minimal);

            ConfigurationManager.RefreshSection("appSettings");

            LerConfigs();

            try
            {
                ((MainWindow)Application.Current.MainWindow).Frm_Principal.GoBack();
            }
            catch (Exception)
            {
                ((MainWindow)Application.Current.MainWindow).Frm_Principal.Content = new PaginaPrincipal();
            }
        }

        //Botão voltar menu anterior
        private void Btn_Voltar_Click(object sender, RoutedEventArgs e)
        {
            ((MainWindow)Application.Current.MainWindow).Frm_Principal.Content = new PaginaPrincipal();
        }

        //Botão ajuda
        private void Btn_Ajuda_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/Muhdo/WorkPlanner/issues");
        }

        //Função ler as configurações guardadas
        void LerConfigs()
        {
            try
            {
                Configuracoes.IdadeMinima = Convert.ToInt32(ConfigurationManager.AppSettings["IdadeMinima"]);
                Configuracoes.ContactoPreferivel = Convert.ToInt32(ConfigurationManager.AppSettings["ContactoPreferivel"]);
                Configuracoes.ServicoPrecoMinimo = Convert.ToDecimal(ConfigurationManager.AppSettings["ServicoPrecoMinimo"]);
            }
            catch (Exception)
            {

            }
        }

        //Função ativar butão
        void AtivarButao()
        {
            if (IdadeMinimaValido && ContactoValido && ServicoPrecoMinimoValido)
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
