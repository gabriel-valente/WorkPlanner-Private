﻿#pragma checksum "..\..\Definicoes.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "6FC61DC8DD4DDFB4F0F0CFB463D6DA65EF346192AB044C31B2F963D46F63FDF3"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;
using Trabalhos;


namespace Trabalhos {
    
    
    /// <summary>
    /// Definicoes
    /// </summary>
    public partial class Definicoes : System.Windows.Controls.Page, System.Windows.Markup.IComponentConnector {
        
        
        #line 83 "..\..\Definicoes.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ComboBox Cmb_TempoCopia;
        
        #line default
        #line hidden
        
        
        #line 97 "..\..\Definicoes.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button Btn_LocalBackup;
        
        #line default
        #line hidden
        
        
        #line 98 "..\..\Definicoes.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox Tb_Localbackup;
        
        #line default
        #line hidden
        
        
        #line 105 "..\..\Definicoes.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox Tb_IdadeMinima;
        
        #line default
        #line hidden
        
        
        #line 108 "..\..\Definicoes.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ComboBox Cmb_Contacto;
        
        #line default
        #line hidden
        
        
        #line 126 "..\..\Definicoes.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox Tb_ServicoPrecoMinimo;
        
        #line default
        #line hidden
        
        
        #line 130 "..\..\Definicoes.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button Btn_Guardar;
        
        #line default
        #line hidden
        
        
        #line 131 "..\..\Definicoes.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button Btn_Voltar;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/WorkPlanner;component/definicoes.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\Definicoes.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            this.Cmb_TempoCopia = ((System.Windows.Controls.ComboBox)(target));
            
            #line 83 "..\..\Definicoes.xaml"
            this.Cmb_TempoCopia.SelectionChanged += new System.Windows.Controls.SelectionChangedEventHandler(this.Cmb_TempoCopia_SelectionChanged);
            
            #line default
            #line hidden
            return;
            case 2:
            this.Btn_LocalBackup = ((System.Windows.Controls.Button)(target));
            
            #line 97 "..\..\Definicoes.xaml"
            this.Btn_LocalBackup.Click += new System.Windows.RoutedEventHandler(this.Btn_LocalBackup_Click);
            
            #line default
            #line hidden
            return;
            case 3:
            this.Tb_Localbackup = ((System.Windows.Controls.TextBox)(target));
            
            #line 98 "..\..\Definicoes.xaml"
            this.Tb_Localbackup.TextChanged += new System.Windows.Controls.TextChangedEventHandler(this.Tb_Localbackup_TextChanged);
            
            #line default
            #line hidden
            return;
            case 4:
            this.Tb_IdadeMinima = ((System.Windows.Controls.TextBox)(target));
            
            #line 105 "..\..\Definicoes.xaml"
            this.Tb_IdadeMinima.TextChanged += new System.Windows.Controls.TextChangedEventHandler(this.Tb_IdadeMinima_TextChanged);
            
            #line default
            #line hidden
            return;
            case 5:
            this.Cmb_Contacto = ((System.Windows.Controls.ComboBox)(target));
            
            #line 108 "..\..\Definicoes.xaml"
            this.Cmb_Contacto.SelectionChanged += new System.Windows.Controls.SelectionChangedEventHandler(this.Cmb_Contacto_SelectionChanged);
            
            #line default
            #line hidden
            return;
            case 6:
            this.Tb_ServicoPrecoMinimo = ((System.Windows.Controls.TextBox)(target));
            
            #line 126 "..\..\Definicoes.xaml"
            this.Tb_ServicoPrecoMinimo.TextChanged += new System.Windows.Controls.TextChangedEventHandler(this.Tb_ServicoPrecoMinimo_TextChanged);
            
            #line default
            #line hidden
            return;
            case 7:
            this.Btn_Guardar = ((System.Windows.Controls.Button)(target));
            
            #line 130 "..\..\Definicoes.xaml"
            this.Btn_Guardar.Click += new System.Windows.RoutedEventHandler(this.Btn_Guardar_Click);
            
            #line default
            #line hidden
            return;
            case 8:
            this.Btn_Voltar = ((System.Windows.Controls.Button)(target));
            
            #line 131 "..\..\Definicoes.xaml"
            this.Btn_Voltar.Click += new System.Windows.RoutedEventHandler(this.Btn_Voltar_Click);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

