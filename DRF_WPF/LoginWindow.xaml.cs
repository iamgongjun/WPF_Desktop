using System;
using System.Windows;
using System.Windows.Input;
using System.Diagnostics;
using DRF_WPF.Services;
using System.Windows.Controls;

namespace DRF_WPF
{
    public partial class LoginWindow : Window
    {
        private readonly AuthService _authService;

        public LoginWindow()
        {
            InitializeComponent();
            // 设置窗口在屏幕中央显示
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            
            // 初始化认证服务
            _authService = new AuthService();
            
            // 设置默认用户名和密码
            tbUsername.Text = "admin";
            pbPasswordBox.Password = "admin123";
        }

        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // 禁用登录按钮并显示加载状态
                LoginButton.IsEnabled = false;
                LoginButton.Content = "登录中...";
                ErrorMessageText.Visibility = Visibility.Collapsed;

                string username = tbUsername.Text?.Trim() ?? "";
                string password = pbPasswordBox.Password?.Trim() ?? "";

                if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                {
                    ErrorMessageText.Text = "请输入用户名和密码";
                    ErrorMessageText.Visibility = Visibility.Visible;
                    return;
                }

                // 使用认证服务验证用户
                var user = await _authService.AuthenticateAsync(username, password);
                if (user == null)
                {
                    ErrorMessageText.Text = "用户名或密码不正确";
                    ErrorMessageText.Visibility = Visibility.Visible;
                    return;
                }

                // 登录成功，显示主窗口
                try 
                {
                    MainWindow mainWindow = new MainWindow();
                    // 传递用户信息到主窗口
                    mainWindow.CurrentUser = user;
                    mainWindow.Show();
                    this.Close();
                }
                catch (Exception mainEx)
                {
                    Debug.WriteLine($"创建主窗口时出错: {mainEx}");
                    MessageBox.Show($"打开主界面失败: {mainEx.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"登录错误: {ex}");
                ErrorMessageText.Text = "登录失败: " + ex.Message;
                ErrorMessageText.Visibility = Visibility.Visible;
                MessageBox.Show("登录失败: " + ex.Message, "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                // 无论成功或失败，恢复登录按钮状态
                LoginButton.IsEnabled = true;
                LoginButton.Content = "登 录";
            }
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
    }
} 