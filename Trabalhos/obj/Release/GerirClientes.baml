   M S B A M L   `   `   `  ���� F  BWorkPlanner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null%clr-namespace:Trabalhos	Trabalhos      Trabalhos.GerirClientes   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     @d2http://schemas.microsoft.com/expression/blend/2008     )localclr-namespace:Trabalhos       N� Title$  GerirClientes�� N� 
FontFamily$ Segoe UI Light)� N� FontSize$ 16(�.��� ���	#FF1B1B1B � ColumnDefinitions �� $
��221* ��� $
��Auto ��� $
��318* � � RowDefinitions �� $��7* ��� $��4* �.+��� $��0��$��0��$��Clientes atuais��$��Stretch=�$	��Top=�$��Center=�$��Center=�$��
10,10,10,0q�$��31���� -    ��Name$ Lst_Clientes��$��0��$��0��$��2��$��Stretch=�$��Stretch=�$��10,49,10,10q�$��Stretch=����� .R��  �� $��* ��� $
��Auto �.+��� $��0��$��Center=�$��Right=�$��5q����� *Nome+�� $��1��$��1��$
��Left=�����  �� EndPoint$ 0.5,1&� � MappingMode$ RelativeToBoundingBox=� �� 
StartPoint$ 0.5,0&�.��� 	 � Color$	 	#00000000��
 � Offset$
 0Q�� $	 	#00000000��$
 1Q�� $	 White��$	
 0.4Q�� $	 White��$	
 0.6Q��� $��1��$��Center=�$
��Left=����� *
Contacto+�� $��0��$��1��$��2��$��1��$
��Left=����� $ 0.5,1&�$ RelativeToBoundingBox=�$ 0.5,0&�.��� $	 	#00000000��$
 0Q�� $	 	#00000000��$
 1Q�� $	 White��$	
 0.4Q�� $	 White��$	
 0.6Q��� $��0��$��1��$��Código cliente:��$��Stretch=�$	��Top=�$��Right=�$��Center=�$��
10,10,10,0q�$��31���� -    ��Name$ Lbl_CodigoCliente��$��0��$��2��$��Stretch=�$	��Top=�$
��Left=�$��Center=�$��
10,10,10,0q�$��31���� $��0��$��1��$��Nome cliente:��$��Stretch=�$	��Top=�$��Right=�$��Center=�$��
10,46,10,0q�$��31���� -    ��Name$ Tb_NomeCliente��$��0��$��2��$��Stretch=�$	��Top=�$
��Left=�$��Center=�$��
10,50,10,0q�$��21��$��16(� �� TextWrapping$ WrapWithOverflow=� �� 	MaxLength$ 50�� �� 
IsReadOnly . �� $��0��$��1��$��Data de nascimento:��$��Stretch=�$	��Top=�$��Right=�$��Center=�$��
10,82,10,0q�$��31���� -   $ Lbl_Nascimento��$��0��$��2��$��Stretch=�$	��Top=�$
��Left=�$��Center=�$��
10,82,10,0q�$��31��(  "System.Windows.Controls.DatePicker  -     Name$ Dp_Nascimento��$��0��$��2��$
��Left=�$	��Top=�$��	10,88,0,0q�$��16(�$��21��$	��230��   IsTodayHighlighted .     SelectedDateFormat$ Short=�$��0q�$��0q���. $y�Hidden=���Z�  � 	Resources :  4System.Windows.Controls.Primitives.DatePickerTextBox �� 
TargetType �� Property �� Value �� Path �� RelativeSource �� AncestorType �� StringFormat��%�   '        �� "  .���  � �� �� .R��� $ PART_TextBox��-   ���	#FF494949���	#FFEEEEEE$��0q� . ���� $ SelectedDate� �� "  $ 
dd/MM/yyyy���� -    ��Name$ Btn_LimparData��$��0��$��2��$��Limpar Data de Nascimento��$
��Left=�$	��Top=�$��Center=�$��Center=�$��
245,86,0,0q�$	��200��$y�Hidden=��� $��0��$��1��$��Género:��$��Stretch=�$	��Top=�$��Right=�$��Center=�$��10,118,10,0q�$��31���� -   $ Lbl_Sexo��$��0��$��2��$��Stretch=�$	��Top=�$
��Left=�$��Center=�$��10,118,10,0q�$��31��	� -	    	�Name$ RdB_Feminino��$��0��$��2��   	TextBlock#��[  $��Feminino�� 	� 	GroupName$
 Sexo��$
��Left=�$	��Top=�$��Center=�$��
10,124,0,0q�$��20��$��80��$y�Hidden=�	� -
   $ RdB_Indefinido��$��0��$��2��#��[  $��
Indefinido��$
 Sexo��$
��Left=�$	��Top=�$��Center=�$��
95,124,0,0q� d� 	IsChecked$
 TrueY�$��20��$��86��$y�Hidden=�	� -   $ RdB_Masculino��$��0��$��2��#��[  $��	Masculino��$
 Sexo��$
��Left=�$	��Top=�$��Center=�$��186,124,0,0q�$��20��$��86��$y�Hidden=��� $��0��$��1��$��Morada:��$��Stretch=�$	��Top=�$��Right=�$��Center=�$��10,154,10,0q�$��31���� -   $ 	Tb_Morada��$��0��$��2��$��Stretch=�$	��Top=�$
��Left=�$��Center=�$��10,159,10,0q�$��21��$��16(�$	 100�� . �� $��0��$��1��$��Código postal:��$��Stretch=�$	��Top=�$��Right=�$��Center=�$��10,190,10,0q�$��31���� -   $ Tb_CodPostalEsquerda��$��0��$��2��$
��Left=�$	��Top=�$
��Left=�$��Center=�$��
10,195,0,0q�$��21��$��16(�$ WrapWithOverflow=�$ 4��$��37�� . �� -   $ Lbl_CodPostalDiv��$��0��$��2��$��-��$
��Left=�$	��Top=�$��Right=�$��Center=�$��
47,190,0,0q�$��31��$y�Hidden=��� -   $ Tb_CodPostalDireita��$��0��$��2��$
��Left=�$	��Top=�$
��Left=�$��Center=�$��
63,195,0,0q�$��21��$��16(�$ WrapWithOverflow=�$ 3��$��30�� . �� -   $ Btn_ProcurarCodigoPostal��$��0��$��2��$��Procurar Código Postal��$
��Left=�$	��Top=�$��Center=�$��Center=�$��
98,194,0,0q�$	��162��$y�Hidden=��� -   $ Lbl_PesquisaCodigo��$��0��$��2��$	��Top=�$
��Left=�$��Center=�$��265,190,10,0q�$��31���� $��0��$��1��$��Localidade:��$��Stretch=�$	��Top=�$��Right=�$��Center=�$��10,226,10,0q�$��31���� -   $ Tb_Localidade��$��0��$��2��$
��Left=�$	��Top=�$
��Left=�$��Center=�$��
10,231,0,0q�$��21��$��16(�$ WrapWithOverflow=�$ 50��$	��230�� . �� $��0��$��1��$��Email:��$��Stretch=�$	��Top=�$��Right=�$��Center=�$��10,262,10,0q�$��31���� -   $ Tb_Email��$��0��$��2��$��Stretch=�$	��Top=�$
��Left=�$��Center=�$��10,266,10,0q�$��21��$��16(�$	 256�� . �� $��0��$��1��$��Telemóvel:��$��Stretch=�$	��Top=�$��Right=�$��Center=�$��10,298,10,0q�$��31���� -   $ Tb_Telemovel��$��0��$��2��$
��Left=�$	��Top=�$
��Left=�$��Center=�$��
10,302,0,0q�$��21��$��16(�$ WrapWithOverflow=�$ 9��$	��230�� . �� $��0��$��1��$��	Telefone:��$��Stretch=�$	��Top=�$��Right=�$��Center=�$��10,334,10,0q�$��31���� -   $ Tb_Telefone��$��0��$��2��$
��Left=�$	��Top=�$
��Left=�$��Center=�$��
10,338,0,0q�$��21��$��16(�$ WrapWithOverflow=�$ 9��$	��230�� . �� -   $ Lbl_ErroContacto��$��0��$��2��$	��Top=�$
��Left=�$��Center=�$��245,292,10,0q�$��69���� $��0��$��1��$��2��$��1��$��Bottom=����� $ 0,0.5&�$ 1,0.5&�.��� $	 	#00000000��$
 0Q�� $	 	#00000000��$
 1Q�� $	 White��$	
 0.4Q�� $	 White��$	
 0.6Q��� -   $ Btn_AdicionarCliente��$��1��$��1��$��Adicionar Novo Cliente��$��	10,10,0,0q�$	��250��$
��Left=�$��73��$	��Top=�$��2���� -   $ Btn_AtualizarCliente��$��1��$��1��$ ��Editar Cliente Selecionado��$��
10,89,0,89q�$	��250��$��73��$
��Left=�{�.  $��2���� -   $ Btn_ApagarCliente��$��1��$��1��$ ��Apagar Cliente Selecionado��$��	10,0,0,10q�$	��250��$��73��$
��Left=�{�.  $��Bottom=�$��2���� -   $ Btn_CancelarCliente��$��1��$��1��$��Cancelar��$��	10,0,0,10q�$	��250��$
��Left=�$��73��$��Bottom=�$��2��$y�Hidden=��� -   $ Btn_GuardarCliente��$��1��$��1��$��Guardar Cliente��$��	10,10,0,0q�$��73��$	��250��$	��Top=�$
��Left=�{�.  $��2��$y�Hidden=��� -   $ Btn_GuardarAlteracoes��$��1��$��1��$��Guardar Alterações��$��	10,10,0,0q�$��73��$	��250��$	��Top=�$
��Left=�{�. $��2��$y�Hidden=��� -   $ 
Btn_Voltar��$��1��$��2��$��Voltar��$��	0,0,10,10q�$��Right=�$	��100��$��50��$��Bottom=��� $��1��$��2��$��98,10,10,65q�$��Stretch=�$��Stretch=�$
��Auto=�.���� -    ��Name$ 	Lbl_Erros��#��[  $��WrapWithOverflow=�  �� TextAlignment$
  Left=��� -   ! ��Name$! BloquearFundo��$��3��$��2�����	#99000000$y�Hidden=�� -    " �Name$" Grd_ValidarApagar��$��0��$��0��$��2��$��3��$	��500��$
��1083�����	#FFAAAAAA$y�Hidden=�.+��� #��[  $	��Top=�$��Stretch=�$��
10,10,10,0q�$��WrapWithOverflow=�$  Center=��� $��40(�$7�1Tem a certeza que pretende eliminar este cliente?���� �� $��30(�$O�IEsta ação só é possivel pois o cliente não tem trabalhos associados!���� #��[  $��Center=�$��Stretch=�$��10,10,10,10q�$��WrapWithOverflow=�$  Center=��� $��Segoe UI Semibold)�$��70(�$"�Esta ação é irreversivel!���� -!   $ Btn_ConfirmarApagar��$��Apagar Cliente��$��
200,0,0,50q�$��Center=�$	��175��$��75��$��Bottom=��� -"   $ Btn_CancelarApagar��$��Cancelar��$��-200,0,0,50q�$��Center=�$	��175��$��75��$��Bottom=�