   M S B A M L   `   `   `  ����B  >DRF_WPF, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null!clr-namespace:DRF_WPFDRF_WPF  5   	   "    DRF_WPF.UserManagementWindow   5   	   W SPresentationCore, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   'localclr-namespace:DRF_WPF     5   	     � Title$  用户管理��5   	   $	��500��6   $	��700��6#    � WindowStartupLocation$ CenterOwner=�6/   .��5	      � $��20q�6    � RowDefinitions 5
   
   �� 5      $
��Auto �6   �� 5      $��* �6   �� 5      $
��Auto �6    � ColumnDefinitions 5   
   �� 5      $��* �6   �� 5      $	��300 �6   .+�5   
   �� $��0��6   $��0��6"    �� Orientation$ 
Horizontal=�62   $��0,0,0,10q�6K   .+�5      �� $��用户列表��6   $��18(�6$   $��SemiBold"�62   $��Center=�6H   �� 5      -   5         ��Name$ 
BtnRefresh��5      $��刷新��6)   $��20,0,0,0q�66   $
��10,5q�6H   6W   5      &   System.Windows.Controls.DataGrid  5   
   -   5          Name$ UsersDataGrid��5      $��1��6*   $��0��67   $��0,0,20,0q�6G      AutoGenerateColumns$ False��5         
IsReadOnly$
 True��6/   	   SelectionMode$	 Single=�6A   
   Columns
 5      0  *System.Windows.Controls.DataGridTextColumn  5      ,  &System.Windows.Controls.DataGridColumn   Header$ ID��6%      Width5  /System.Windows.Controls.DataGridLengthConverter$ 50 6H   1  +System.Windows.Controls.DataGridBoundColumn   Binding 61   �� *Id+6H     5       $ 	用户名��6%   $	 100 6O    62   �� *
Username+6O     5!      $ 显示名称��6%   $	 120 6S    63   �� *DisplayName+6S   4  .System.Windows.Controls.DataGridCheckBoxColumn  5"      $ 	管理员��6)   $ 60 6R    66   �� *	IsAdmin+6R     5#      $ 创建时间��6%   $	 150 6v    63   �� *	CreatedAt+ �� StringFormat$ {0:yyyy-MM-dd HH:mm}��6v     5$      $ 最后登录��6%   $	 150 6z    63   �� *LastLoginTime+$ {0:yyyy-MM-dd HH:mm}��6z   5&      �� 5)   
   $��0��6   $��1��6!   $��用户信息��61   $��18(�6=   $��SemiBold"�6K   $��0,0,0,10q�6a   � 5+   
   $��1��6   $��1��6   .+�5,      �� .+�5-      �� $��
用户名:��6   $��0,10,0,5q�6(   �� 5.      -   5         ��Name$ TxtUsername��5.      $��0,0,0,10q�6/   $��5q�6A   �� 50      $��显示名称:��6   $��0,10,0,5q�6)   �� 51      -   5        $ TxtDisplayName��51      $��0,0,0,10q�62   $��5q�6D   �� 53      $��密码:��6   $��0,10,0,5q�6'   G� 54      -   5         G�Name$ PwdPassword��54      $��0,0,0,10q�63   $��5q�6E   �� 56      $��确认密码:��6   $��0,10,0,5q�6)   G� 57      -   5        $ PwdConfirmPassword��57      $��0,0,0,10q�6:   $��5q�6L   �� 59      -   5         ��Name$ 
ChkIsAdmin��59      $��管理员权限��6/   $��	0,10,0,20q�6?   �� 5;      -   5         ��Name$ TxtError��5;      
���  ��$
��Wrap=�6?   $��0,0,0,10q�6S   $y�	Collapsed=�6e   �� 5=      $ 
Horizontal=�6   $��0,10,0,0q�66   .+�5>      �� -	   5        $ BtnSave��5>      $��保存��6.   $��80��6;   $��5q�6F   6R   �� 5?      -
   5        $ BtnNew��5?      $��	新用户��6-   $��80��6;   $��10,0,0,0q�6F   $��5q�6X   6d   �� 5@      -   5        $ 	BtnDelete��5@      $��删除��60   $��80��6=   $��10,0,0,0q�6H   $��5q�6Z   6f   5A      5B      5C      �� 5E   
   -   5        $��2��5E      $��1��6   $��关闭��6.   $��Right=�6;   $��0,20,0,0q�6W   $
��10,5q�6i   6x   5F      5G      