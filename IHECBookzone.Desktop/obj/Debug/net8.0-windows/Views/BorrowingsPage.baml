   M S B A M L   `   `   `  ����O  KIHECBookzone.Desktop, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullG(clr-namespace:IHECBookzone.Desktop.ViewsIHECBookzone.Desktop.Views  5      Q-clr-namespace:IHECBookzone.Desktop.ConvertersIHECBookzone.Desktop.Converters  5      /    )IHECBookzone.Desktop.Views.BorrowingsPage   5      W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35H 9http://schemas.microsoft.com/winfx/2006/xaml/presentation      5      R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      :local(clr-namespace:IHECBookzone.Desktop.Views     5      D
converters-clr-namespace:IHECBookzone.Desktop.Converters     5        N� Title$  BorrowingsPage��5
      
 � Tag$ 
Borrowings��5       � 	Resources 5      @   :IHECBookzone.Desktop.Converters.CountToVisibilityConverter   CountToVisibilityConverter��%   &	          5   
   60   5      .��5      �  � RowDefinitions 5   
   �� 5      $
��Auto �6   �� 5      $
��Auto �6   �� 5      $��* �6   �� 5      $
��Auto �6   .+�5   
   �� $��0��6    �� Orientation$ 
Horizontal=�6"   $��
20,20,0,10q�6;   .+�5      �� $	����6   $��Segoe MDL2 Assets)�6(   $��24(�6G   $��Center=�6U   $��0,0,10,0q�6p   ���#3B50B26�   �� 5      $��My Borrowings��6   $��28(�6-   $��SemiBold"�6;   $��Center=�6Q   ���#3B50B26l   5      � 5"   
   $��1��6   $��
20,0,20,20q�6    � ColumnDefinitions 5#      �� 5$      $��* �6#   �� 5%      $
��Auto �6#   .+�5)      �� $ 
Horizontal=�6   $��0��62   .+�5*      �� $��
Filter by:��6   $��Center=�6.   $��0,0,10,0q�6I   �� 5+      -   5         ��Name$ FilterComboBox��5+      $	��180��63   $	��8,4q�6r   $��Center=�6�    5,       �� 
TargetType �� Property	 �� Value��%S   '��       �� 5-      " ��6    .�5.      ��  � ��$		 5,3q�68   5/      50      .@�51      �� $��	All Books��6#   
 �� 
IsSelected$

 True��67   �� 52      $��Active Borrowings��6#   �� 53      $��Returned Books��6#   �� 54      $��Overdue Books��6#   55      �� 58      -   5         ��Name$ StatusTextBlock��58   .   $��
Loading...��6   $��15,0,0,0q�6G   $y�	Collapsed=�6Y   $��Center=�6p   ��59      �� 5:      " ��6     �� Triggers 5;      P� 5<   "    P� Property � y� P� Value$ Visible=�6@    N� EnterActions 5=   &   �� 5>   *   .��5?   .   �� .��5@   2   W�  �� TargetProperty$ Opacity�6B    W� From 0.56f   	 W� To 16q    g� Duration$ 0:0:0.7D�6x    g� RepeatBehavior$ Forever��6�    g� AutoReverse$
 True��6�   5A   /   5B   +   5D   #   5F      5H      5I      �� 5L      -   5         ��Name$ RefreshButton��5L      $��1��6,   $	����6<   $��Segoe MDL2 Assets)�6O   $��40��6n   $��30��6y    � ToolTip$ Refresh borrowings list��5M   1   $��Center=�6S   ���#3B50B2��5N      �� 5O      " ��6   .�5P      ��  � ��
	 ���� 67   �� 5Q       � ��
	 ���� 68   �� 5R       � ��$	 16(�65   �� 5S       � ��	 5T      �� 5U   "    �� 
TargetType" ��62   .R�5V   &   �� #��z��6-   #��z��6W   #��z��5W   -    �� CornerRadius$ 3��6a   ._�5X   *   �� $��Center=�6;   $��Center=�6X   5Y   '   5Z   #   5\       5]      P� 5^       �@J�IsMouseOver$
 True��6=   . �5_   "   ��  � ��	 �#F0F0F06?   �� 5`   "    � ��	 �#CCCCCC6@   5a      5c      5e      5f      &   System.Windows.Controls.DataGrid  5i   
   -   5          Name$ BorrowingsDataGrid��5i       $��2��6   $��
20,0,20,10q�6<      AutoGenerateColumns$ False��5j         
IsReadOnly$
 True��6/      SelectionMode$ Single=�5k      $��1q�5m      ���#E0E0E0   RowHeaderWidth$ 0Q�5n         VerticalGridLinesBrush �#E0E0E0    HorizontalGridLinesBrush  �#E0E0E0!   AlternatingRowBackground! �#F8F8F8"   CanUserReorderColumns$" False��5r      #   CanUserResizeRows$# False��5s      $   CanUserSortColumns$
$ True��5t       5u      =  7System.Windows.Controls.Primitives.DataGridColumnHeader)  #System.Windows.Controls.DataGridRow*  $System.Windows.Controls.DataGridCell% �� Padding��%  '        '  �     '  �     �� 5v      "  6   .�5w      ��  � ��	 �#F5F5F563   �� 5x       � ��$
	 10,8q�60   �� 5y       � ��$	 SemiBold"�63   5z      �� 5{      "  6   .�5|      ��  � ��$	 40��6/   5}      �� 5~      "  6   .�5      ��  � ��$
	 10,5q�60   �� 5�       � ��	 5�      �� 5�      "  6.   .R�5�   "   �� #% z��6)   #��z��5�   )   #��z��5�   )   #��z��5�   )   ._�5�   &   �� $��Center=�67   5�   #   5�      5�      5�      5�      &   Columns& 5�      0  *System.Windows.Controls.DataGridTextColumn  5�      ,  &System.Windows.Controls.DataGridColumn'   Header$' 
Book Title��6%   (   Width5  /System.Windows.Controls.DataGridLengthConverter$( 2* 6X   1	  +System.Windows.Controls.DataGridBoundColumn) 	  Binding) 69   �� *
Book.Title+6X     5�      $' Author��6%   $( * 6U   ) 65   �� *Book.Author+6U     5�      $' Borrow Date��6%   $( * 6b   ) 6:   �� *FormattedBorrowDate+6b     5�      $' Due Date��6%   $( * 6\   ) 67   �� *FormattedDueDate+6\     5�      $' Return Date��6%   $( * 6b   ) 6:   �� *FormattedReturnDate+6b   4
  .System.Windows.Controls.DataGridTemplateColumn
  5�      $' Status��6)   $( * 69   * 
  CellTemplate* 5�      �� 5�      .R�5�      �� $ 12��6%   $	% 8,3q�67   $
��Left=�6E   ��5�   "   �� 5�   &   " ��6,    5�   *   �� 5�   .   + �� Binding+ 6:   �� *	DueStatus+, �� Value
, Active6X   .a�5�   2   ��  � ��	 �#E6F4EA6O   5�   /   �� 5�   .   + 6:   �� *	DueStatus+, Overdue6X   .a�5�   2   ��  � ��	 �#FEEAE66O   5�   /   �� 5�   .   + 6:   �� *	DueStatus+, Returned6X   .a�5�   2   ��  � ��	 �#EFEFEF6O   5�   /   5�   '   ._�5�   "   �� ��5�   &   �� 5�   *   " ��60    5�   .   �� 5�   2   + 6>   �� *	DueStatus+
, Active6\   .a�5�   6   ��  � ��$	 Active��6M   �� 5�   6    � ��	 �#1E8E3E6S   5�   3   �� 5�   2   + 6>   �� *	DueStatus+, Overdue6\   .a�5�   6   ��  � ��$	 Overdue��6M   �� 5�   6    � ��	 �#D930256S   5�   3   �� 5�   2   + 6>   �� *	DueStatus+, Returned6\   .a�5�   6   ��  � ��$	 Returned��6M   �� 5�   6    � ��	 �#5F63686S   5�   3   5�   +   5�   #   5�      5�      5�      -   RowStyle- 5�      �� 5�      "  6    5�      P� 5�       �@J�IsMouseOver$
 True��69   . �5�      ��  � ��	 �#F0F4FF6;   5�      P� 5�       �@ 
IsSelected$
 True��68   . �5�      ��  � ��	 �#E1E8FF6;   5�      5�      5�      �� 5�   
   $��2��6   $��No borrowings to display��6!   $��Center=�5�      $��Center=�61   ���#666666$��16(�5�   )   y�5�      �� . �� ElementName$. BorrowingsDataGrid��/ �� Path$/ Items.Count�0 �� 	Converter#0 [  �� 5�   
   $��3��6   $��0,1,0,0q�6   ���#E0E0E0$% 20,15q�6N   ._�5�      �� -   5        $ ReturnButton��5�      $��Return Book��6+   $	��140��6A   $��36��6M   ${�False��5�   0   $��Right=�6B   ���#3B50B2
�������$��0q�5�   =    5�      ��%^   '��       �� 5�      " ��6   .�5�      ��  �@��CornerRadius$	 4��69   5�      5�      ��5�      �� 5�      " ��6    5�      P� 5�       � {�$ False��6;   . �5�   "   ��  � ��	 �#CCCCCC6?   5�      P� 5�       �@J�IsMouseOver$
 True��6=   . �5�   "   ��  � ��	 �#2C3E8C6?   5�      5�      5�      5�      5�      5�      