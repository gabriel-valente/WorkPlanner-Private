   M S B A M L   `   `   `  ���� F  BWorkPlanner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null%clr-namespace:Trabalhos	Trabalhos      Trabalhos.GerirServicos   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     @d2http://schemas.microsoft.com/expression/blend/2008     )localclr-namespace:Trabalhos       N� Title$  GerirServicos�� N� 
FontFamily$ Segoe UI Light)� N� FontSize$ 16(�.��� ���	#FF1B1B1B � ColumnDefinitions �� $
��221* ��� $
��Auto ��� $
��318* � � RowDefinitions �� $��7* ��� $��4* �.+��� $��0��$��0��$��Serviços atuais��$��Stretch=�$	��Top=�$��Center=�$��Center=�$��
10,10,10,0q�$��31���� -    ��Name$ Lst_Servicos��$��0��$��0��$��2��$��Stretch=�$��Stretch=�$��10,49,10,10q�$��Stretch=����� .R��  �� $��3* ��� $��* �.+��� $��0��$��Center=�$��Right=�$��5q����� *Nome+�� $��1��$��1��$
��Left=�����  �� EndPoint$ 0.5,1&� � MappingMode$ RelativeToBoundingBox=� �� 
StartPoint$ 0.5,0&�.��� 	 � Color$	 	#00000000��
 � Offset$
 0Q�� $	 	#00000000��$
 1Q�� $	 White��$	
 0.4Q�� $	 White��$	
 0.6Q��� $��1��$��Center=�$
��Left=����� *Preco+�� $��0��$��1��$��2��$��1��$
��Left=����� $ 0.5,1&�$ RelativeToBoundingBox=�$ 0.5,0&�.��� $	 	#00000000��$
 0Q�� $	 	#00000000��$
 1Q�� $	 White��$	
 0.4Q�� $	 White��$	
 0.6Q��� $��0��$��1��$��Código serviço:��$��Stretch=�$	��Top=�$��Right=�$��Center=�$��9,10,9,0q�$��31���� -    ��Name$ Lbl_CodigoServico��$��0��$��2��$��Stretch=�$	��Top=�$
��Left=�$��Center=�$��
11,10,10,0q�$��31���� $��0��$��1��$��	Serviço:��$��Stretch=�$	��Top=�$��Right=�$��Center=�$��9,46,9,0q�$��31���� -    ��Name$ 
Tb_Servico��$��0��$��2��$��Stretch=�$	��Top=�$
��Left=�$��Center=�$��
11,50,10,0q�$��21��$��16(� �� TextWrapping$ WrapWithOverflow=� �� 	MaxLength$ 30�� �� 
IsReadOnly . �� $��0��$��1��$��Preço Hora:��$��Stretch=�$	��Top=�$��Right=�$��Center=�$��9,82,9,0q�$��31���� -   $ Tb_Preco��$��0��$��2��$
��Left=�$	��Top=�$
��Left=�$��Center=�$��	11,86,0,0q�$��21��$��16(�$ WrapWithOverflow=�$ 6�� . $��90���� -   $ Lbl_AvisoPreco��$��0��$��2��$%��Preço hora abaixo do definido!��$��Stretch=�$	��Top=�$
��Left=�$��Center=�$��
106,82,9,0q�$��31��$y�Hidden=��� $��0��$��1��$��2��$��1��$��Bottom=����� $ 0,0.5&�$ 1,0.5&�.��� $	 	#00000000��$
 0Q�� $	 	#00000000��$
 1Q�� $	 White��$	
 0.4Q�� $	 White��$	
 0.6Q��� -    ��Name$ Btn_AdicionarServico��$��1��$��1��$��Adicionar Novo Servico��$��	10,10,0,0q�$	��250��$
��Left=�$��73��$	��Top=�$��2���� -   $ Btn_AtualizarServico��$��1��$��1��$ ��Editar Servico Selecionado��$��
10,89,0,89q�$	��250��$��73��$
��Left=�{�.  $��2���� -   $ Btn_ApagarServico��$��1��$��1��$ ��Apagar Servico Selecionado��$��	10,0,0,10q�$	��250��$��73��$
��Left=�{�.  $��Bottom=�$��2���� -	   $ Btn_CancelarServico��$��1��$��1��$��Cancelar��$��	10,0,0,10q�$	��250��$
��Left=�$��73��$��Bottom=�$��2��$y�Hidden=��� -
   $ Btn_GuardarServico��$��1��$��1��$��Guardar Servico��$��	10,10,0,0q�$��73��$	��250��$	��Top=�$
��Left=�{�.  $��2��$y�Hidden=��� -   $ Btn_GuardarAlteracoes��$��1��$��1��$��Guardar Alterações��$��	10,10,0,0q�$��73��$	��250��$	��Top=�$
��Left=�{�. $��2��$y�Hidden=��� -   $ 
Btn_Voltar��$��1��$��2��$��Voltar��$��	0,0,10,10q�$��Right=�$	��100��$��50��$��Bottom=��� $��1��$��2��$��131,10,10,65q�$��Stretch=�$��Stretch=�$
��Auto=�.���� -    ��Name$ 	Lbl_Erros��   	TextBlock#��[  $��WrapWithOverflow=� �� TextAlignment$
 Left=��� -    ��Name$ BloquearFundo��$��3��$��2�����	#99000000$y�Hidden=�� -    �Name$ Grd_ValidarApagar��$��0��$��0��$��2��$��3��$	��500��$
��1083�����	#FFAAAAAA$y�Hidden=�.+��� #��[  $	��Top=�$��Stretch=�$��
10,10,10,0q�$��WrapWithOverflow=�$ Center=��� $��40(�$8�2Tem a certeza que pretende eliminar este serviço?���� �� $��30(�$P�JEsta ação só é possivel pois o serviço não tem trabalhos associados!���� #��[  $��Center=�$��Stretch=�$��10,10,10,10q�$��WrapWithOverflow=�$ Center=��� $��Segoe UI Semibold)�$��70(�$"�Esta ação é irreversivel!���� -   $ Btn_ConfirmarApagar��$��Apagar Serviço��$��
200,0,0,50q�$��Center=�$	��175��$��75��$��Bottom=��� -   $ Btn_CancelarApagar��$��Cancelar��$��-200,0,0,50q�$��Center=�$	��175��$��75��$��Bottom=�