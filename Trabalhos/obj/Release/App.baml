   M S B A M L   `   `   `  ���� F  BWorkPlanner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null%clr-namespace:Trabalhos	Trabalhos      Trabalhos.App   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     )localclr-namespace:Trabalhos       �� 	Resources   �� 
TargetType �� Property �� Value �� Triggers P� Property P� Value   	TextBlock �� 
TargetType ��Name	 J� SnapsToDevicePixels
 ��Name �� ContentStringFormat �� ContentStringFormat �� Triggers �� 
TargetName  ComboBoxNormalBorderBrush �� Color ! ComboBoxNormalBackgroundBrush   ComboBoxToggleButtonTemplate � ColumnDefinitions �� CornerRadius �� 	ClickMode d� 	IsChecked �� Path �� RelativeSource �� Mode J� IsHitTestVisible �� SelectionBoxItem �� SelectionBoxItemTemplate �� 
IsReadOnly � AllowsTransparency �� IsDropDownOpen � ActualWidth �� MaxDropDownHeight-  'System.Windows.Input.KeyboardNavigation   DirectionalNavigation  �� Padding  ScrollBarTrackThumb! �Name" � RowDefinitions# ]�Name$ ]� Thumb% n�Name& ]� IncreaseRepeatButton' ��Name( J� Opacity) ]� DecreaseRepeatButton* P� 
SourceName+ �� Angle��%�  '��       '�� +     &	  @    '�� f    &	 D    &	 Y    &	 n    7  7  7  '�� �    7  7  '�� �
    '��     &	 �    '�� D    �� " ��.���  � �� �	#FFEEEEEE�� " ��.���  � �� �	#FFEEEEEE P�  �@��
IsReadOnly . . ���  � �� �	#FF1B1B1B��  � ��$ 0q���  � �� �	#FF1B1B1BP�  �@��
IsReadOnly .  . ���  � �� �	#FF494949��  � ��$ 0q���  � �� �	#FF494949�� .���  � �� �	#FFEEEEEE�� " ��.���  � �� �	#FFEEEEEE��  � �� �	#FF494949��  � ��$ 0q���  � �� �� " ��.R��� $ button��#��z��#��z��#��z��	 . ._��� $
 contentPresenter��#��z��#��z��# z |�.  #��z��#��z����. #	 z	 #��z�� P�  �@J�IsMouseOver . . ��� $ button�� � �� �	#FF333333P�  � �� . . ��� $ button�� � �� �	#FF222222P�  � {� .  . ��� $ button�� � �� �	#FFAAAAAA�� $ 	#FF494949���� $ 	#FF494949���� " d�.R��  �� �� $��20 �.+��� $��Border��$��2��8��  $ 0��$��0q�8�� �� $��ButtonBorder��$��1��$��1,1,1,1q����	#FF494949$ 0��$��0q�8�� F� $��Arrow��$��1��=����        �    �� � 	= ��[ � z �� � z     � 	=  	= 	$��Center=����	#FFEEEEEE$��Center=� P�  �@J�IsMouseOver . . ��� $ ButtonBorder�� � �� �	#FF333333P�  �@d�	IsChecked$
 TrueY�. ��� $ ButtonBorder�� � �� �	#FF222222�� $ Arrow�� � �� �	#FFEEEEEEP�  � {� .  . ��� $ Arrow�� � �� �	#FFEEEEEE�� " ��.���  �@�OverridesDefaultStyle . ��  � ��$ Disabled=���  � ��$ Disabled=���  � �� . ��  � �� �	#FFEEEEEE��  �@�FocusVisualStyle Z� ��  �@��
IsEditable . ��  � �� �� " ��.R�� .+�d� $��ToggleButton��$��2��$ Press=�|�.  8��   �� $ IsDropDownOpen� �� *TemplatedParent+$ TwoWay=��� $��ContentSite��$��3,3,23,3q� .  $
��Left=�$��Center=�#��z #��z #��z���� $��PART_EditableTextBox��$��3,3,23,3q� .  $y�Hidden=�
������ $
��Left=�$��Center=�|�. .������ " ��.R��� $��PART_ContentHost��|�.  .��� $��Popup��$��Bottom=�|�.   . #��z $��Slide=�.��� $��DropDown��	 . #��z #��z .+��� $��DropDownBorder�����	#FF494949$��0,1,0,0q�$ 0��$��0q�8�� �� $��4q�	 . .���� $ 	Contained=� P�  �@��HasItems .  . ��� $ DropDownBorder�� � ��$ 95��P�  � {� .  . ���  � �� �	#FFEEEEEEP�  �@��
IsGrouping . . ���  � �� .  P�  �@��
IsEditable . . ���  �@ 	IsTabStop .  �� $ PART_EditableTextBox�� � y�$ Visible=��� $ ContentSite�� � y�$ Hidden=��� " ��.���  � �� �	#FFEEEEEE��  � �� �	#FF494949��  � ��$ 0q���  � ��$ 31����  � ��$ Center=��� " �� P�  � |� . . ���  � �� �	#FF494949��  � ��$ 0q���  � �� �	#FF494949P�  � |� .  . ���  � �� �	#FF494949��  � ��$ 0q���  � �� �	#FF494949.���  � �� �� " ��.R��� $ Bd��#��z��#��z��#��z��$  1q�	 . ._��� |�.  #��z����.  .���� #	 z	  P�  � {� .  . ��� $ Bd�� � �� �	#FFAAAAAA�� " n�.���  � �� �� " n�.R�� $
! Grid��.+��� $��Stretch=�$��Stretch=�$
��Auto��$
��Auto��
������ 	 . �� $ CornerScrollBarRectangle��$ 3��$��Stretch=�$��Stretch=�$
��Auto��$
��Auto��$��0q�#��z�� P�  �@�Tag$ 
Horizontal��. ��� $ CornerScrollBarRectangle�� � ��$
 Auto���� $ CornerScrollBarRectangle�� � ��$ 6���� " ��.���  �@��IsFlicksEnabled .  ��  � �� �#ABABAB��  � ��
 ���� ��  � ��$ 6����  � �� �� " ��.R�� $! GridRoot��$��6��#��z��" �� $��0.0000000001* �.+�]� $# 
PART_Track��$��0����. |�.  $ n� $% Thumb��#��z��#���  & �� $' PageUp��$��ScrollBar.PageDownCommand��$( 0Q�|�.  ) �� $' PageDown��$��ScrollBar.PageUpCommand��$( 0Q�|�.   P� $* Thumb�� �@J�IsMouseOver . . ��� $ Thumb�� � �� �	#FF878787P� $* Thumb�� �@n�
IsDragging . . ��� $ Thumb�� � �� �	#FF686868P�  � {� .  . ��� $ Thumb�� � y�$ 	Collapsed=�P�  �@��Orientation$ 
Horizontal=�. ��� $ GridRoot�� �@�LayoutTransform �� $	+ -90Q��� $ 
PART_Track�� �@�LayoutTransform �� $	+ -90Q���  � ��$
 Auto����  � ��$
 Auto���� $ Thumb�� �@�Tag$ 
Horizontal���� $ PageDown�� � ��$ ScrollBar.PageLeftCommand���� $ PageUp�� � ��$  ScrollBar.PageRightCommand��