   M S B A M L   `   `   `  ����O  KIHECBookzone.Desktop, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null;"clr-namespace:IHECBookzone.DesktopIHECBookzone.Desktop  5   	   %    IHECBookzone.Desktop.MainWindow   5   	   W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35H 9http://schemas.microsoft.com/winfx/2006/xaml/presentation      5   	   R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   4local"clr-namespace:IHECBookzone.Desktop     5   	     � Title$  IHEC Bookzone��5   	   $	��768��6   $
��1280��6,   $	��800��5	   	   $	��600��6    � WindowStartupLocation$ CenterScreen=�5
   	   .��5      �  � RowDefinitions 5   
   �� 5      $
��Auto �6   �� 5      $��* �6   .+�5   
   �� $��0��6   ���#3b50b2$��60��5      ._�5      �  � ColumnDefinitions 5      �� 5      $
��Auto �6'   �� 5      $��* �6'   �� 5      $
��Auto �6'   .+�5      �� $��0��6    �� Orientation$ 
Horizontal=�5      $��Center=�5       $
��20,0q�5!      .+�5"      �� ���#e5deff$��36��5#      $��36��5$       �� CornerRadius$ 18��5%      ._�5&      �� $��IB��6$   ���#3b50b2$
��Bold"�5(   $   $��16(�5)   $   $��Center=�5*   $   $��Center=�5+   $   5,      �� 5-      $��IHEC Bookzone��6    
�������$��SemiBold"�5/       $��18(�50       $��12,0,0,0q�51       $��Center=�52       53      �� 56      $��1��6   $ 
Horizontal=�57      $��Center=�58      $��Center=�59      .+�5:      	� -   5        $
��Home��5:   "      NavButtonStyle#��[  5;   "   
 � Tag$
 Home��5<   "    d� 	IsChecked 5>   "   �� *CurrentPageTag+  EqualityConverter �� 	Converter# [ 	 �� ConverterParameter$
	 Home��	� 5?      -   5        $��Library��5?   "   #��[  5@   "   $ Library��5A   "    5C   "   �� *CurrentPageTag+# [ $	 Library��	� 5D      -   5        $��Profile��5D   "   #��[  5E   "   $ Profile��5F   "    5H   "   �� *CurrentPageTag+# [ $	 Profile��y�5I   "   �� *IsAuthenticated+   BooleanToVisibilityConverter# [ 	� 5J      -   5        $��Admin��5J   "   #��[  5K   "   $ Admin��5L   "    5N   "   �� *CurrentPageTag+# [ $	 Admin��y�5O   "   �� *	IsAdmin+# [ 5P      �� 5S      $��2��6   $ 
Horizontal=�5T      $��Center=�5U      $��0,0,20,0q�5V      .+�5Y      �� -   5        $��Login��5Y      ���#e5deff���#3b50b2$��SemiBold"�5\      $
��15,5q�5]      $��0q�5^      y�5_      �� *IsAuthenticated+ $  InverseBoolToVisibilityConverter# [ 
 � 	Resources
 5a       �� 
TargetType �� Property �� Value��%^   '��       �� 5b      " ��6$   .�5c   "   ��  �@��CornerRadius$ 4��6A   5d      5e      5f      �� 5i      $ 
Horizontal=�6!   y�5j   !   �� *IsAuthenticated+# [ .+�5k      �� 
�������$��Center=�5m   $   $��0,0,10,0q�5n   $   ��5k   $   �� *CurrentUser.FullName+5n   $   �� 5q      $��36��6!   $��36��5r   !   ���#e5deff$ 18��5t   !   ._�5u      �� ���#3b50b2$��SemiBold"�5w   (   $��Center=�5x   (   $��Center=�5y   (   ��5u   (   �� *CurrentUser.FullName+  NameToInitialsConverter# [ 5y   (   5z      �� 5}      -   5        $��Logout��5}   !   
������ 
�������$��0q�5�   !   $��15,0,0,0q�5�   !   5�   !   5�      5�      5�      5�       � 5�   
   -   5          �Name$ 	MainFrame��5�      $��1��5�        � NavigationUIVisibility$ Hidden=�5�      5�      5�      