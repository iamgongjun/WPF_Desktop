using DRF_WPF.Data;
using DRF_WPF.Models;
using DRF_WPF.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace DRF_WPF
{
    public partial class UserManagementWindow : Window
    {
        private readonly ApplicationDbContext _context;
        private readonly AuthService _authService;
        private User? _selectedUser;
        private bool _isEditMode = false;

        public UserManagementWindow()
        {
            InitializeComponent();
            _context = new ApplicationDbContext();
            _authService = new AuthService();
            
            LoadUsers();
            ClearForm();
        }

        private async void LoadUsers()
        {
            try
            {
                var users = await _context.Users.ToListAsync();
                UsersDataGrid.ItemsSource = users;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"加载用户数据失败: {ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ClearForm()
        {
            TxtUsername.Text = string.Empty;
            TxtDisplayName.Text = string.Empty;
            PwdPassword.Password = string.Empty;
            PwdConfirmPassword.Password = string.Empty;
            ChkIsAdmin.IsChecked = false;
            TxtError.Visibility = Visibility.Collapsed;

            _selectedUser = null;
            _isEditMode = false;
            
            // 在新建模式下，用户名可编辑
            TxtUsername.IsEnabled = true;
            BtnDelete.IsEnabled = false;
        }

        private void UsersDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (UsersDataGrid.SelectedItem is User user)
            {
                _selectedUser = user;
                _isEditMode = true;
                
                TxtUsername.Text = user.Username;
                TxtDisplayName.Text = user.DisplayName;
                ChkIsAdmin.IsChecked = user.IsAdmin;
                
                // 在编辑模式下，用户名不可编辑
                TxtUsername.IsEnabled = false;
                BtnDelete.IsEnabled = true;
                
                // 清空密码框，因为我们不显示哈希后的密码
                PwdPassword.Password = string.Empty;
                PwdConfirmPassword.Password = string.Empty;
            }
        }

        private async void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                TxtError.Visibility = Visibility.Collapsed;
                
                // 验证输入
                if (string.IsNullOrWhiteSpace(TxtUsername.Text))
                {
                    TxtError.Text = "用户名不能为空";
                    TxtError.Visibility = Visibility.Visible;
                    return;
                }

                // 如果是编辑模式且密码为空，则保留原密码
                if (_isEditMode && string.IsNullOrEmpty(PwdPassword.Password) && 
                    string.IsNullOrEmpty(PwdConfirmPassword.Password))
                {
                    // 仅更新其他信息
                    if (_selectedUser != null)
                    {
                        _selectedUser.DisplayName = TxtDisplayName.Text;
                        _selectedUser.IsAdmin = ChkIsAdmin.IsChecked ?? false;
                        
                        _context.Update(_selectedUser);
                        await _context.SaveChangesAsync();
                        
                        MessageBox.Show("用户信息已更新", "成功", MessageBoxButton.OK, MessageBoxImage.Information);
                        LoadUsers();
                    }
                    return;
                }

                // 密码验证
                if (PwdPassword.Password != PwdConfirmPassword.Password)
                {
                    TxtError.Text = "两次输入的密码不匹配";
                    TxtError.Visibility = Visibility.Visible;
                    return;
                }

                if (PwdPassword.Password.Length < 6)
                {
                    TxtError.Text = "密码长度至少需要6个字符";
                    TxtError.Visibility = Visibility.Visible;
                    return;
                }

                // 保存或更新用户
                if (_isEditMode && _selectedUser != null)
                {
                    // 更新用户
                    _selectedUser.DisplayName = TxtDisplayName.Text;
                    _selectedUser.IsAdmin = ChkIsAdmin.IsChecked ?? false;
                    
                    // 更新密码
                    await _authService.ChangePasswordAsync(_selectedUser.Id, PwdPassword.Password);
                    
                    _context.Update(_selectedUser);
                    await _context.SaveChangesAsync();
                    
                    MessageBox.Show("用户信息已更新", "成功", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    // 新建用户
                    var newUser = new User
                    {
                        Username = TxtUsername.Text,
                        DisplayName = TxtDisplayName.Text,
                        IsAdmin = ChkIsAdmin.IsChecked ?? false,
                        Password = PwdPassword.Password // 将在注册过程中哈希处理
                    };

                    bool result = await _authService.RegisterUserAsync(newUser);
                    if (result)
                    {
                        MessageBox.Show("用户已创建", "成功", MessageBoxButton.OK, MessageBoxImage.Information);
                        ClearForm();
                    }
                    else
                    {
                        TxtError.Text = "创建用户失败，可能用户名已存在";
                        TxtError.Visibility = Visibility.Visible;
                        return;
                    }
                }

                LoadUsers();
            }
            catch (Exception ex)
            {
                TxtError.Text = $"操作失败: {ex.Message}";
                TxtError.Visibility = Visibility.Visible;
            }
        }

        private async void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedUser == null) return;

            // 防止删除唯一的管理员
            int adminCount = await _context.Users.CountAsync(u => u.IsAdmin);
            if (_selectedUser.IsAdmin && adminCount <= 1)
            {
                MessageBox.Show("不能删除系统中唯一的管理员用户", "警告", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show($"确定要删除用户 '{_selectedUser.Username}' 吗？", 
                "确认删除", MessageBoxButton.YesNo, MessageBoxImage.Question);
                
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    _context.Users.Remove(_selectedUser);
                    await _context.SaveChangesAsync();
                    
                    MessageBox.Show("用户已删除", "成功", MessageBoxButton.OK, MessageBoxImage.Information);
                    ClearForm();
                    LoadUsers();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"删除用户失败: {ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void BtnNew_Click(object sender, RoutedEventArgs e)
        {
            ClearForm();
        }

        private void BtnRefresh_Click(object sender, RoutedEventArgs e)
        {
            LoadUsers();
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
} 