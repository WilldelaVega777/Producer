   M S B A M L   `   `   `  ����  ProducerAlpha    ProducerAlpha.Welcome   5      R NWindowsBase, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    8x,http://schemas.microsoft.com/winfx/2006/xaml    5      >d2http://schemas.microsoft.com/expression/blend/2006    5      Hmc;http://schemas.openxmlformats.org/markup-compatibility/2006    5        � 	Resources  5	         RecentFilesDataTemplate  WelcomeButtonStyle �� 
TargetType �� Property �� Value �� 
TargetType ��Name �� CornerRadius �� Padding J� SnapsToDevicePixels	 �� Triggers
 P� Property P� Value �� 
TargetName  RecentFileButtonStyle  WelcomeBackgroundBrush �� Color  WelcomeHeaderFontColor  InputBackgroundBrush �� EndPoint �� 
StartPoint � Color � Offset��%�  &	        &	 4     &	 �    &	 {    &	 �    &	 �    �� 5
      .R�5      �� #��z��6   5      �� 5      " ��6'   .�5      ��  � �� �#FFFFFF6%   �� 5       � ��$
 Auto��6    �� 5       � ��$ 16(�6#   �� 5       � ��$ Calibri)�6%   �� 5       � ��$
 Bold"�6%   �� 5       � �� 5   
   �� 5      " ��6   .R�5      �� $ bd��6   $ 3,3,3,3��6!   $ 5,0,0,0q�68   ._�5      �� # z 6"   #��z��6^   ��. $��5,1,0,1q�6�   5      	 5      P� 5      
 �@J�IsKeyboardFocused . 65   P� 5      
 �@J�IsMouseOver . . �5      ��  �@�Cursor$
 Hand��6+   �� 5      $ bd��6/    � �� �	#FF2222226?   5      5!      5#   	   5$      �� 5&      " ��6*   .�5'      ��  � �� �	#FFD5E8F86%   �� 5(       � ��$
 Auto��6    �� 5)       � ��$ 12(�6#   �� 5*       � ��$ Calibri)�6%   �� 5+       � ��$
 Bold"�6%   �� 5,       � �� 5-   
   �� 5.      " ��6   .R�5/      �� $ bd��6   $ 3,3,3,3��6!   $ 20,0,0,0q�68   ._�50      �� # z 6!   #��z��6]   ��. $��5,1,0,1q�6�   51      	 52      P� 53      
 �@J�IsKeyboardFocused . 65   P� 54      
 �@J�IsMouseOver . . �55      ��  �@�Cursor$
 Hand��6+   �� 56      $ bd��6/    � �� �	#FF29367B6?   57      59      5;   	   5<      �� 5?      $ 	#FF202020��65   �� 5@      $ 	#FFE6E6E6��65   �� 5D      $ 0.504,0.351&�67   $ 0.504,-0.319&�6N   .��5E   
   � $ 	#FF000000��6   $ 0.416Q�6)   � 5F   
   $ 	#FF4A5790��6   $
 0.54Q�6)   5G      5J      .��5L      �� .+�5O      �� -   5        $ Header��5O   '   $ 0q�6    J� Opacity$	 0.8Q�6   
�������$��0.3,0.3,0.3,0q�6K   $ 5,5,0,0��6k   $��Center=�6�   ��5P      ��  �� ElementName$ brush�� �� Path$ ImageSource.Width���5Q      �� $ brush��$ ImageSource.Height���5R   
   �� 5S      -   5        	brush�� �� ImageSource$/ )/Assets/Bitmaps/Welcome/WelcomeHeader.png��5S   (   5U      �� 5Y      $��7��6   $
��Auto��6   $��Stretch=�6(   $	 0.7Q�6F   ��5Z   
   �� 5[      $ 0.504,0.351&�6"   $ 0.504,-0.319&�69   .��5\      � $ Yellow��6   $ 0.416Q�6.   � 5]      $ 	#FF4A5790��6   $
 2.84Q�61   5^      5`      �� 5a      ���	#ffdddddd$��0.3,0.3,0.3,0.3q�6%   $	��240��6G   ._�5b      � -   5         �Name$ ContentGrid��5b      $��0,0,0,0q�6   ���	#ff444444 � RowDefinitions 5d   
   �� 5e      $
��Auto �6   �� 5f      $��* �6   .+�5h   
   �� $��Stretch=�6   $��	5,20,5,10q�63   .+�5i      �� -   5         ��Name$ 	NewButton��5i   $   $��0,0,0,5q�6   $	��New��67    �� 	IsDefault . $��Stretch=�6n   #���  6�   $
��Auto��6�   �� 5j      -   5        $ 
OpenButton��5j   $   $��0,0,0,5q�6   $��Open...��68   $��Stretch=�6c   #���  6�   $
��Auto��6�   �� 5k      -   5        $ ImportButton��5k   $   $��0,0,0,5q�6   $��	Import...��6:   $��Stretch=�6i   #���  6�   $
��Auto��6�   5l      �� 5n   
   {�.  $��Stretch=�6)   $��Bottom=�6G   $��1��6b   
������ $��2,0,2,0q�6�   �� 5p   
   $��Open Recent��6   $
��Left=�6&   $	��Top=�6A   $��1��6Y   $��10,0,0,0q�6f   ���	#FFB5C8D86x   �� 5r   
   -   5         ��Name$ RecentFilesStackPanel��5r   \   $��Stretch=�6   $��Stretch=�63   $��1��6O   $��5,30.87,5,10q�6{   5t   	   5u      �� 5x      -   5        $ Footer��5x   0   ���	#ff222222$��35��6$   $	 0.9Q�6@   ���	#ff222222$��
.3,0,.3,.3q�6f   $ 0,0,5,5��6�   ._�5y      �� -	   5         ��Name$ VersionLabel��5y      $��10,7,0,0q�6$   ���	#FFB5C8D8$��Version ��6M   5z      5|      5}      