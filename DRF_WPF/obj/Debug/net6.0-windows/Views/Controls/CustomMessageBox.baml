   M S B A M L   `   `   `  ����  DRF_WPF-    'DRF_WPF.Views.Controls.CustomMessageBox   5      W SPresentationCore, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5      R NSystem.Xaml, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      $	��400��5      $
��Auto��5        � 	Resources  5	         PositiveButtonStyle �� 
TargetType �� Property �� Value �� 
TargetType �� CornerRadius �� Triggers P� Property P� Value  NegativeButtonStyle  InfoIcon  WarningIcon  	ErrorIcon  QuestionIcon  SuccessIcon��%0  &	        &	     &	 >    &	     &	 �    &	 �    &	 #
    �� 5   
   " ��6,   .�5      ��  � �� �#2196F36+   �� 5       � ��
 �����6+   �� 5       � ��$ 0q�60   �� 5       � ��$ 13(�6)   �� 5       �@�Cursor$
 Hand��6'   �� 5       � �� 5      �� 5      " ��6&   .R�5      �� #��z��6!   #��z��5   !   #��z��5   !   $ 4��5   !   ._�5      �� $��Center=�6/   $��Center=�5   /   5       5      P� 5       �@J�IsMouseOver$
 True��6=   . �5   "   ��  � �� �#1E88E56?   5      P� 5        � ��$
 True��6;   . �5!   "   ��  � �� �#1976D26?   5"      P� 5#       � {�$ False��6;   . �5$   "   ��  � �� �#BBDEFB6?   �� 5%   "    � �� �#78909C6?   5&      5(      5*      5+      �� 5.   
   " ��6,   .�5/      ��  � �� �#E0E0E06+   �� 50       � �� �#4242426+   �� 51       � ��$ 0q�60   �� 52       � ��$ 13(�6)   �� 53       �@�Cursor$
 Hand��6'   �� 54       � �� 55      �� 56      " ��6&   .R�57      �� #��z��6!   #��z��58   !   #��z��59   !   $ 4��5:   !   ._�5;      �� $��Center=�6/   $��Center=�5<   /   5=       5>      P� 5?       �@J�IsMouseOver$
 True��6=   . �5@   "   ��  � �� �#D5D5D56?   5A      P� 5B       � ��$
 True��6;   . �5C   "   ��  � �� �#BDBDBD6?   5D      P� 5E       � {�$ False��6;   . �5F   "   ��  � �� �#F5F5F56?   �� 5G   "    � �� �#9E9E9E6?   5H      5J      5L      5M      F� 5P   
   �����@]� @T� ��ا @T� ��ا ��j �@]� ��j �@]� @f��ا @f��ا �ا �@]� �ا � � �� ׀�  �  ��� ��� � � ��O ��� ��� ׀�O �  ��� ��� � � ��  ��� ��� 	���#2196F36�   F� 5Q   
   �����@]� @]� ��ا @]� ��ا ��j �@]� ��j �@]� @f��ا @f��ا ��� �@]� ��� � � �� ׀�  �  ��� ��� � � ��O ��� ��� ׀�O �  ��� ��� � � ��  ��� ��� 	���#FF98006�   F� 5R   
   �����@]� @]� ��ا @]� ��ا ��j �@]� ��j ��ا ��� �@]� ��� �@]� @f��ا @f� � �� ׀�  �  ��� ��� � � ��O ��� ��� ׀�O �  ��� ��� � � ��  ��� ��� 	���#F443366�   F� 5S   
   ����Ѐ�� ��!�@]� ��!�@]� ��Oр�� ��Oр�� ��!� � �� ׀� z ���[ ��[ �p�P]� Po�Ϣ  �� �� ��� 0T� ��� Pf� p�� �7� �@]� ��� @]�  $� @]� @fр�� @fӀ�� ��� ��� p�� �Ϣ 0T� ��� �� 0T� �� `�� �Ϣ Ӑ�� ��� ��� @T� ���  z � � @KL  ��- ��- �@T�  z  ��- ��- р�[  z � � �� ���[ ��[ 	���#2196F363  F� 5T   
   ����� � �� ׀�O � ���� ��� � � ��O���� ��� ׀�  � ���� ��� � � �� ���� ��� ��ا  �� р�`�� Ѱ$� �q{ ��ا p�� �p�x 0�� Ѡ.c  � ��ا  �� 	���#4CAF506�   5U      .��5W      �� -   5        	 ��Name$	 
MainBorder��5\      
����������#CCCCCC$��1q�5Y      $ 4��5Z      
 �� Padding$
 0q�5[       J� Effect 5]   
   3  -System.Windows.Media.Effects.DropShadowEffect  5^      -   5        shadowEffect��   
BlurRadius$ 15Q�5_         ShadowDepth$ 0Q�5`         	Direction$ 0Q�5a         Color$ 	#22000000��5b      ._�5d   
   �  � RowDefinitions 5e      �� 5f      $��40 �6    �� 5g      $
��Auto �6    �� 5h      $
��Auto �6    �� 5i      $��60 �6    .+�5m      � $��0��6   ���#F0F0F0 � ColumnDefinitions 5n      �� 5o      $��* �6'   �� 5p      $
��Auto �6'   .+�5s      �� -   5         ��Name$ TitleTextBlock��5s      $��0��5t      $��消息提示��5u      $��15,0,0,0q�5v      $��14(�5w      $��SemiBold"�5x      $��Center=�5y      �� 5{      -   5         ��Name$ CloseButton��5{      $��1��5|      $��30��5}      $��30��5~      $��×��5      ���#666666
������ $��0q�5�      $��16(�5�      ��5�      �� 5�      " ��6*   .R�5�      �� #��z��6%   ._�5�   "   �� $��Center=�63   $��Center=�5�   3   5�       5�      P� 5�   "    �@J�IsMouseOver$
 True��6A   . �5�   &   ��  � �� �#E0E0E06C   5�   #   P� 5�   "    � ��$
 True��6?   . �5�   &   ��  � �� �#CCCCCC6C   5�   #   5�      5�      5�      � 5�      $��1��6   $��20,20,20,10q�6     5�      �� 5�      $
��Auto �6'   �� 5�      $��* �6'   .+�5�      F� -   5         F�Name$ MessageIcon��5�      $��32��5�      $��32��5�      $��0,0,15,0q�5�       �� Stretch$ Uniform=�5�      $	��Top=�5�      �� 5�      -   5        $ MessageTextBlock��5�      $��1��5�      $��消息内容��5�      $
��Wrap=�5�      $��Center=�5�      $��14(�5�      $��0,0,0,0q�5�      5�      9� 5�      -   5         9�Name$ DetailsExpander��5�      $��2��5�      $��显示详情��5�      $��
20,0,20,10q�5�      $y�	Collapsed=�5�      .��5�      �� -   5         ��Name$ DetailsTextBox��5�       �� TextWrapping$
 Wrap=�5�      $	��120��5�       �� 
IsReadOnly$
 True��5�      " �� VerticalScrollBarVisibility$
 Auto=�5�      $��Consolas)�5�      $��12(�5�      ���#F5F5F5$��10q�5�      5�      � 5�      $��3��6   $��
20,0,20,15q�6     5�      �� 5�      $��* �6'   �� 5�      $
��Auto �6'   �� 5�      $
��Auto �6'   .+�5�      �� -	   5        $ NegativeButton��5�      $��1��5�      $��取消��5�      $	��100��5�      $��30��5�      $��0,0,10,0q�5�      $y�	Collapsed=�5�      #��[ 5�      �� 5�      -
   5        $ PositiveButton��5�      $��2��5�      $��确定��5�      $	��100��5�      $��30��5�      #��[  5�      5�      5�      5�      5�      