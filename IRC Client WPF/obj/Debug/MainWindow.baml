   M S B A M L   `   `   `  ����O  Kmscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e0891&clr-namespace:System;assembly=mscorlibSystem  5   	    IRC Client WPF   IRC_Client_WPF.MainWindow   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   9System&clr-namespace:System;assembly=mscorlib     5   	   -   5          � Title$  
MainWindow��5   	   $	��500��6   $	��700��6)   $	��500��6N   $	��300��6]   .��5      � .+�5      R� -   5         R�Name$ UIServerList��5      $
��Left=�6#   $��
10,10,0,67q�6>   $	��118��6R   �� 5      -   5         ��Name$ 	UIChatBox��5      $��133,33,133,35q�6#    �� 
IsReadOnly . $
��Auto=�6L   ��.  � 	Resources 5	       �� 
TargetType �� Property �� Value��%X   'J�       �� 5
      " J�6   .�5      ��  �@��Margin$ 0q�6    5      5      . �5      /� .T�5      J� 5      5      �� 5      -   5         ��Name$ 
UINickList��5      $��
0,10,10,35q�6!   $��Right=�65   $	��118��6Q   5      �� 5      -   5        $ 
UIInputBox��5      $��
10,0,10,10q�6$   $��20��6O   $��Bottom=�6[   $��Center=�6v   . �5      /� .T�5      J� .*�6   .*�5      �� $� ��6   5      5      5      �� 5      -   5        	 ��Name$	 UIAddServer��5      $��	10,0,0,35q�6    $
��Bold"�63   $��18(�6E   $
��Left=�6S   $��Bottom=�6n   $��27��6�   $��27��6�   .��5      �� $��+��6   $��0,-3,0,0q�6   5      �� 5      -   5        
 ��Name$
 UIServerName��5      $��	42,0,0,35q�6"    �� TextWrapping$ NoWrap=�65   $�� ��6K    J� RenderTransformOrigin$ -0.133,-0.609&�6S   $��27��6y   $��Bottom=�6�   $
��Left=�6�   $��86��6�   $��Center=�6�   6�   �� 5      -   5        $
 UIChanTopic��5      $��133,10,133,0q�6!   $
 Wrap=�67   $�� ��6K   $��23��6S   $	��Top=�6_   $��Center=�6w   6�   5       5!      