   M S B A M L   `   `   `  ����  ProducerAlpha!    ProducerAlpha.ErrorReporter   5      R NWindowsBase, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      8x,http://schemas.microsoft.com/winfx/2006/xaml    5      $	��576��5      $
��1024��6   .��5      � ���	#dd222222.+�5   
   � -   5          �Name$  LayoutContent��5      $	��600��6&   $	��350��62    � ColumnDefinitions 5      �� 5      $��80 �6#   �� 5	      $��* �6#   .+�5      � -   5        $  	IconPanel��5      $��0��6&   $��Stretch=�66   $��Stretch=�6R   .+�5      �� -   5         ��Name$ 	ErrorIcon��5      $4��...\..\Assets\Icons\ErrorReporter\XamlError.png��6+   $��48��6c   $��48��6n   5      � 5      -   5        $  MessagePanel��5      $��1��6)   $��Stretch=�69   $��Stretch=�6U    � RowDefinitions 5      �� 5      $��25 �6$   �� 5      $	��25* �6$   �� 5      $	��75* �6$   �� 5      $��33 �6$   .+�5      �� $��0��6   $��Tahoma)�5      $
��Bold"�5      $
��11pt(�5      
�������$
��Left=�5      $	��Top=�5      $��Xaml Error:��5      $��	10,5,10,5q�5      $
��Wrap=�5      �� 5"      -   5         ��Name$ txtErrorDetails��5"      $��1��5#      $��Tahoma)�5$      $	��8pt(�5%      $
��Bold"�5&      
�������$
��Left=�5(      $	��Top=�5)      $��Error Details Here...��5*      $��
10,5,10,10q�5+      $
��Wrap=�5,      �� 5.      -   5         ��Name$ StackTraceViewer��5.      $��2��5/      $��Visible=�50      $
��Auto=�51      .��53      �� -   5        $ txtErrorStackTrace��53       $��Tahoma)�54       $	��8pt(�55       $��Italic$�56       
�������$
��Left=�58       $	��Top=�59       $��Error StackTrace Here...��5:       $��
10,8,10,10q�5;       $
��Wrap=�5<       5>      � 5?      $��3��6    5@      �� 5A      $��* �6+   �� 5B      $��80 �6+   �� 5C      $��80 �6+   �� 5D      $��10 �6+   .+�5F      �� -   5         ��Name$ cmdSave��5F      $
��Save��5G      $��1��5H      
�������$��20��5J      $��70��5K       �� TextAlignment$ Center=�5L      $��Right=�5M      $��0,3,0,3q�5N      5O      �� 5R      -	   5        $ cmdClose��5R      $��Close��5S      $��2��5T      
�������$��20��5V      $��70��5W      $ Center=�5X      $��Right=�5Y      $��0,3,0,3q�5Z      5[      5]      5_      �� 5`      $��2��6   
���  ��$��Stretch=�6;   $��Stretch=�6W   $��1q�6u   5a      5b      5c      