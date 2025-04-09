using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using System.Diagnostics;
using DRF_WPF.Models;
using DRF_WPF.Services;
using System.Linq;
using DRF_WPF.Data;
using DRF_WPF.Helpers;
using Microsoft.EntityFrameworkCore;
using System.Windows.Controls.Primitives;
using System.Windows.Media.Animation;
using System.Threading.Tasks;
using DRF_WPF.Views.Controls;

namespace DRF_WPF;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private DispatcherTimer? timer;
    private ObservableCollection<ProgramStep> programSteps = new ObservableCollection<ProgramStep>();
    private ProgramService? programService;
    private Program? currentProgram;
    private ProgramStep? currentStep;
    
    // 当前登录用户
    public User? CurrentUser { get; set; }
    
    // 用户管理相关
    private readonly ApplicationDbContext _context;
    private readonly AuthService _authService;
    private User? _selectedUser;
    private bool _isEditMode = false;

    public MainWindow()
    {
        try
        {
            InitializeComponent();
            _context = new ApplicationDbContext();
            _authService = new AuthService();
            programService = new ProgramService();
            InitializeTimer();
            SetupNavigationButtons();
            LoadProgramsAsync();
            
            // 设置保存按钮点击事件
            var saveButton = FindName("SaveButton") as Button;
            if (saveButton != null)
            {
                saveButton.Click += SaveButton_Click;
            }
            
            // 当窗口完全加载后更新用户信息
            this.Loaded += (s, e) => 
            {
                UpdateUserInfo();
            };
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"MainWindow初始化错误: {ex}");
            MessageBox.Show($"初始化主界面时出错: {ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    #region 通用UI方法

    private void InitializeTimer()
    {
        try
        {
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Timer_Tick;
            timer.Start();
            UpdateTimeDisplay();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"初始化计时器时出错: {ex}");
        }
    }

    private void SetupNavigationButtons()
    {
        try
        {
            // 默认选中程序页面
            if (btnProgram != null)
            {
                btnProgram.IsChecked = true;
            }

            if (ProgramPage != null)
            {
                ProgramPage.Visibility = Visibility.Visible;
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"设置导航按钮时出错: {ex}");
        }
    }

    private void Timer_Tick(object? sender, EventArgs e)
    {
        try
        {
            UpdateTimeDisplay();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"更新时间显示时出错: {ex}");
        }
    }

    private void UpdateTimeDisplay()
    {
        if (TimeDisplay != null)
        {
            TimeDisplay.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        }
    }

    // 添加更新用户信息的方法
    private void UpdateUserInfo()
    {
        try
        {
            if (CurrentUser != null && UserDisplay != null)
            {
                string displayName = !string.IsNullOrEmpty(CurrentUser.DisplayName) 
                    ? CurrentUser.DisplayName 
                    : CurrentUser.Username;
                    
                UserDisplay.Text = $"欢迎, {displayName}";
                
                // 只有管理员可以看到用户管理按钮
                if (UserManagementButton != null)
                {
                    UserManagementButton.Visibility = CurrentUser.IsAdmin 
                        ? Visibility.Visible 
                        : Visibility.Collapsed;
                }
                
                // 只有管理员可以看到用户管理导航按钮
                if (btnUsers != null)
                {
                    btnUsers.Visibility = CurrentUser.IsAdmin
                        ? Visibility.Visible
                        : Visibility.Collapsed;
                }
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"更新用户信息时出错: {ex}");
        }
    }

    private void MinimizeButton_Click(object sender, RoutedEventArgs e)
    {
        WindowState = WindowState.Minimized;
    }

    private void MaximizeButton_Click(object sender, RoutedEventArgs e)
    {
        if (WindowState == WindowState.Maximized)
        {
            WindowState = WindowState.Normal;
        }
        else
        {
            WindowState = WindowState.Maximized;
        }
    }

    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }

    private void TopBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        DragMove();
    }

    private void NavRadioButton_Checked(object sender, RoutedEventArgs e)
    {
        try
        {
            // 隐藏所有页面
            if (ProgramPage != null) ProgramPage.Visibility = Visibility.Collapsed;
            if (MonitorPage != null) MonitorPage.Visibility = Visibility.Collapsed;
            if (RunPage != null) RunPage.Visibility = Visibility.Collapsed;
            if (CommonPage != null) CommonPage.Visibility = Visibility.Collapsed;
            if (SettingsPage != null) SettingsPage.Visibility = Visibility.Collapsed;
            if (HistoryPage != null) HistoryPage.Visibility = Visibility.Collapsed;
            if (UsersPage != null) UsersPage.Visibility = Visibility.Collapsed;

            // 显示选中的页面
            if (sender is RadioButton radioButton)
            {
                switch (radioButton.Name)
                {
                    case "btnProgram":
                        if (ProgramPage != null) ProgramPage.Visibility = Visibility.Visible;
                        break;
                    case "btnMonitor":
                        if (MonitorPage != null) MonitorPage.Visibility = Visibility.Visible;
                        break;
                    case "btnRun":
                        if (RunPage != null) RunPage.Visibility = Visibility.Visible;
                        break;
                    case "btnCommon":
                        if (CommonPage != null) CommonPage.Visibility = Visibility.Visible;
                        break;
                    case "btnSettings":
                        if (SettingsPage != null) SettingsPage.Visibility = Visibility.Visible;
                        break;
                    case "btnHistory":
                        if (HistoryPage != null) HistoryPage.Visibility = Visibility.Visible;
                        break;
                    case "btnUsers":
                        if (UsersPage != null)
                        {
                            UsersPage.Visibility = Visibility.Visible;
                            // 加载用户数据
                            LoadUsersAsync();
                        }
                        break;
                }
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"导航切换时出错: {ex}");
        }
    }

    // 添加打开用户管理窗口的方法
    private void UserManagementButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            UserManagementWindow userManagementWindow = new UserManagementWindow();
            userManagementWindow.Owner = this;
            userManagementWindow.ShowDialog();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"打开用户管理界面时出错: {ex}");
            MessageBox.Show($"无法打开用户管理: {ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
    
    // 添加注销方法
    private void LogoutButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            var result = MessageBoxHelper.ShowConfirm("确定要注销吗？", "确认", null, this, isConfirmed => {
                if (isConfirmed)
                {
                    // 获取应用程序路径
                    string appPath = System.Reflection.Assembly.GetExecutingAssembly().Location;
                    
                    // 创建一个进程启动信息对象
                    var startInfo = new ProcessStartInfo
                    {
                        FileName = appPath,
                        UseShellExecute = true
                    };
                    
                    // 尝试启动新进程
                    try
                    {
                        // 启动新进程
                        Process.Start(startInfo);
                        
                        // 关闭当前应用程序
                        Application.Current.Shutdown();
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"重启应用程序时出错: {ex}");
                        MessageBoxHelper.ShowError($"重启应用程序失败: {ex.Message}", "错误");
                        
                        // 如果重启失败，仍然退出到登录界面
                        LoginWindow loginWindow = new LoginWindow();
                        loginWindow.Show();
                        this.Close();
                    }
                }
            });
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"注销时出错: {ex}");
            MessageBoxHelper.ShowError($"注销失败: {ex.Message}", "错误");
        }
    }

    #endregion

    #region 程序和步骤管理

    /// <summary>
    /// 加载所有程序到程序列表
    /// </summary>
    private async void LoadProgramsAsync()
    {
        try
        {
            var programs = await programService.GetAllProgramsAsync();
            
            if (ProgramList != null)
            {
                ProgramList.Items.Clear();
                
                foreach (var program in programs)
                {
                    ProgramList.Items.Add(new ListBoxItem { Content = program.DisplayName, Tag = program.Id });
                }
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"加载程序列表时出错: {ex}");
            MessageBox.Show($"加载程序列表失败: {ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    /// <summary>
    /// 程序列表选择变更事件
    /// </summary>
    private async void ProgramList_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        try
        {
            if (ProgramList.SelectedItem is ListBoxItem selectedItem && selectedItem.Tag is int programId)
            {
                // 加载选中程序
                currentProgram = await programService.GetProgramByIdAsync(programId);
                
                if (currentProgram != null)
                {
                    // 更新程序步骤列表
                    var steps = await programService.GetProgramStepsAsync(programId);
                    
                    // 先清空集合
                programSteps.Clear();
                    foreach (var step in steps)
                    {
                        programSteps.Add(step);
                    }
                    
                    // 绑定到DataGrid - 确保数据源重新绑定
                    if (StepsDataGrid != null)
                    {
                        // 强制刷新UI
                        StepsDataGrid.ItemsSource = null;
                        StepsDataGrid.Items.Clear();
                        StepsDataGrid.ItemsSource = programSteps;
                        StepsDataGrid.Items.Refresh();
                    }
                    
                    // 更新程序名称
                    if (TxtProgramName != null)
                    {
                        TxtProgramName.Text = currentProgram.DisplayName;
                    }
                    
                    // 清空步骤详情
                    ClearStepDetails();
                }
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"程序选择变更时出错: {ex}");
            MessageBox.Show($"加载程序步骤时出错: {ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    /// <summary>
    /// 步骤列表选择变更事件
    /// </summary>
    private void StepsDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        try
        {
            if (StepsDataGrid != null && StepsDataGrid.SelectedItem is ProgramStep selectedStep)
            {
                // 更新当前步骤
                currentStep = selectedStep;
                
                // 确保currentProgram已设置
                if (currentProgram == null && selectedStep.ProgramId > 0)
                {
                    // 尝试查找所属的程序
                    foreach (ListBoxItem item in ProgramList.Items)
                    {
                        if (item.Tag is int programId && programId == selectedStep.ProgramId)
                        {
                            ProgramList.SelectedItem = item;
                        break;
                        }
                    }
                }
                
                // 显示步骤详情
                DisplayStepDetails(selectedStep);
            }
            else
            {
                ClearStepDetails();
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"步骤选择变更时出错: {ex}");
        }
    }

    /// <summary>
    /// 显示步骤详情
    /// </summary>
    private void DisplayStepDetails(ProgramStep step)
    {
        try
        {
            // 确保显示正确的程序名称
            if (currentProgram != null && TxtProgramName != null)
            {
                TxtProgramName.Text = currentProgram.DisplayName ?? currentProgram.Name;
            }
            
            // 填充步骤详情表单
            if (TxtStepNumber != null)
                TxtStepNumber.Text = step.StepNumber.ToString();
                
            if (CbReagent != null)
            {
                // 直接设置文本值而不依赖于下拉列表项
                CbReagent.Text = step.ReagentName ?? string.Empty;
                
                // 记录下拉列表选择状态，用于UI显示
                CbReagent.SelectedIndex = -1;
                foreach (ComboBoxItem item in CbReagent.Items)
                {
                    if (item.Content != null && 
                        item.Content.ToString().Equals(step.ReagentName, StringComparison.OrdinalIgnoreCase))
                    {
                        CbReagent.SelectedItem = item;
                        break;
                    }
                }
            }
                
            if (TxtTime != null)
                TxtTime.Text = step.TimeInSeconds.ToString();
                
            if (CbPriority != null)
            {
                // 设置优先级文本
                CbPriority.Text = step.Priority ?? "高";
                
                // 同时更新下拉列表选择
                CbPriority.SelectedIndex = -1;
                if (step.Priority == "高")
                    CbPriority.SelectedIndex = 0;
                else if (step.Priority == "中")
                    CbPriority.SelectedIndex = 1;
                else if (step.Priority == "低")
                    CbPriority.SelectedIndex = 2;
            }
                
            if (TxtBlowCount != null)
                TxtBlowCount.Text = step.BlowCount.ToString();
                
            if (CbIsBlow != null)
                CbIsBlow.SelectedIndex = step.IsBlow ? 0 : 1; // "是"在索引0，"否"在索引1
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"显示步骤详情时出错: {ex.Message}\n{ex.StackTrace}");
        }
    }

    /// <summary>
    /// 清空步骤详情
    /// </summary>
    private void ClearStepDetails()
    {
        try
        {
            if (TxtStepNumber != null)
                TxtStepNumber.Text = string.Empty;
                
            if (CbReagent != null)
                CbReagent.SelectedIndex = -1;
                
            if (TxtTime != null)
                TxtTime.Text = string.Empty;
                
            if (CbPriority != null)
                CbPriority.SelectedIndex = -1;
                
            if (TxtBlowCount != null)
                TxtBlowCount.Text = string.Empty;
                
            if (CbIsBlow != null)
                CbIsBlow.SelectedIndex = -1;
                
            currentStep = null;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"清空步骤详情时出错: {ex}");
        }
    }

    /// <summary>
    /// 保存按钮点击事件
    /// </summary>
    private async void SaveButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            if (currentStep == null || currentProgram == null)
            {
                MessageBox.Show("请先选择一个步骤", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            // 获取表单数据
            if (!int.TryParse(TxtStepNumber.Text, out int stepNumber))
            {
                MessageBox.Show("步骤序号必须是数字", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!int.TryParse(TxtTime.Text, out int timeInSeconds))
            {
                MessageBox.Show("时间必须是数字", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!int.TryParse(TxtBlowCount.Text, out int blowCount))
            {
                MessageBox.Show("吹风次数必须是数字", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            string reagentName = CbReagent.Text;
            if (string.IsNullOrWhiteSpace(reagentName))
            {
                MessageBox.Show("请选择试剂名称", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            string priority = CbPriority.Text;
            bool isBlow = CbIsBlow.SelectedIndex == 0; // "是"在索引0，"否"在索引1

            // 更新步骤数据
            currentStep.StepNumber = stepNumber;
            currentStep.ReagentName = reagentName;
            currentStep.TimeInSeconds = timeInSeconds;
            currentStep.Priority = priority;
            currentStep.BlowCount = blowCount;
            currentStep.IsBlow = isBlow;

            // 保存到数据库
            bool success = await programService.UpdateStepAsync(currentStep);
            if (success)
            {
                MessageBox.Show("保存成功", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                
                // 刷新步骤列表
                var steps = await programService.GetProgramStepsAsync(currentProgram.Id);
                programSteps.Clear();
                foreach (var step in steps)
                {
                    programSteps.Add(step);
                }
            }
            else
            {
                MessageBox.Show("保存失败", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"保存步骤时出错: {ex}");
            MessageBox.Show($"保存失败: {ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    /// <summary>
    /// 上移按钮点击事件 - 确保一次只上移一位
    /// </summary>
    private async void BtnMoveUp_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            // 检查选择状态
            if (StepsDataGrid?.SelectedItem == null || currentStep == null || currentProgram == null)
            {
                MessageBox.Show("请先选择一个步骤", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            // 获取当前步骤的序号和ID
            int currentStepId = currentStep.Id;
            int currentStepNumber = currentStep.StepNumber;
            
            // 检查是否是第一个步骤，第一个步骤不能上移
            if (currentStepNumber <= 1)
            {
                MessageBox.Show("已经是第一个步骤，不能上移", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            
            // 调用服务上移步骤
            bool success = await programService.MoveStepAsync(currentStepId, true);
            if (success)
            {
                // 完全重新加载步骤列表，但保留选中的步骤ID
                await RefreshStepsListAndSelectItem(currentProgram.Id, currentStepId);
            }
            else
            {
                MessageBox.Show("上移步骤失败", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"上移步骤时出错: {ex}");
            MessageBox.Show($"上移步骤失败: {ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    /// <summary>
    /// 下移按钮点击事件 - 确保一次只下移一位
    /// </summary>
    private async void BtnMoveDown_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            // 检查选择状态
            if (StepsDataGrid?.SelectedItem == null || currentStep == null || currentProgram == null)
            {
                MessageBox.Show("请先选择一个步骤", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            // 获取当前步骤的序号和ID
            int currentStepId = currentStep.Id;
            int currentStepNumber = currentStep.StepNumber;
            
            // 计算当前程序的步骤数量
            int totalSteps = programSteps.Count;
            
            // 检查是否是最后一个步骤，最后一个步骤不能下移
            if (currentStepNumber >= totalSteps)
            {
                MessageBox.Show("已经是最后一个步骤，不能下移", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            
            // 调用服务下移步骤
            bool success = await programService.MoveStepAsync(currentStepId, false);
            if (success)
            {
                // 完全重新加载步骤列表，但保留选中的步骤ID
                await RefreshStepsListAndSelectItem(currentProgram.Id, currentStepId);
            }
            else
            {
                MessageBox.Show("下移步骤失败", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"下移步骤时出错: {ex}");
            MessageBox.Show($"下移步骤失败: {ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
    
    /// <summary>
    /// 刷新步骤列表并选中指定ID的步骤
    /// </summary>
    private async Task RefreshStepsListAndSelectItem(int programId, int stepIdToSelect)
    {
        // 重新加载步骤列表
        var freshSteps = await programService.GetProgramStepsAsync(programId);
        
        // 清空当前列表
        programSteps.Clear();
                
        // 添加刷新的步骤
        foreach (var step in freshSteps)
        {
            programSteps.Add(step);
        }
        
        // 找到要选中的步骤
        ProgramStep stepToSelect = programSteps.FirstOrDefault(s => s.Id == stepIdToSelect);
        if (stepToSelect != null)
        {
            // 暂时移除事件处理器，避免触发选择变更事件
            StepsDataGrid.SelectionChanged -= StepsDataGrid_SelectionChanged;
            
            try
            {
                // 设置选中项
                StepsDataGrid.SelectedItem = stepToSelect;
                
                // 确保选中的行可见
                StepsDataGrid.ScrollIntoView(stepToSelect);
                
                // 直接更新当前步骤引用和UI
                currentStep = stepToSelect;
                DisplayStepDetails(stepToSelect);
                
                // 使用后台UI线程更新，确保选中效果生效
                await Dispatcher.InvokeAsync(() => 
                {
                    StepsDataGrid.UpdateLayout();
                    
                    // 下面这段代码强制刷新选中项的视觉效果
                    StepsDataGrid.SelectedItem = null; // 先清除选中
                    StepsDataGrid.UpdateLayout();      // 更新布局
                    StepsDataGrid.SelectedItem = stepToSelect; // 重新选中
                    
                    // 获取行容器并设置焦点
                    if (StepsDataGrid.ItemContainerGenerator.ContainerFromItem(stepToSelect) is DataGridRow row)
                    {
                        row.IsSelected = true;
                        row.Focus();
                    }
                }, DispatcherPriority.Background);
            }
            finally
            {
                // 重新添加事件处理器
                StepsDataGrid.SelectionChanged += StepsDataGrid_SelectionChanged;
            }
        }
    }

    #endregion

    #region 用户管理功能

    /// <summary>
    /// 加载所有用户到用户列表
    /// </summary>
    private async void LoadUsersAsync()
    {
        try
        {
            var users = await _context.Users.ToListAsync();
            
            if (UsersDataGrid != null)
            {
                UsersDataGrid.ItemsSource = users;
            }
            
            // 在第一次加载时清空表单
            ClearUserForm();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"加载用户列表时出错: {ex}");
            MessageBox.Show($"加载用户列表失败: {ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    /// <summary>
    /// 用户列表选择变更事件
    /// </summary>
    private void UsersDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        try
        {
            if (UsersDataGrid?.SelectedItem is User user)
            {
                _selectedUser = user;
                _isEditMode = true;
                
                // 填充表单数据
                TxtUsername.Text = user.Username;
                TxtDisplayName.Text = user.DisplayName;
                ChkIsAdmin.IsChecked = user.IsAdmin;
                
                // 在编辑模式下，用户名不可编辑
                TxtUsername.IsEnabled = false;
                
                // 清空密码框
                if (PwdPassword != null) PwdPassword.Password = string.Empty;
                if (PwdConfirmPassword != null) PwdConfirmPassword.Password = string.Empty;
                
                // 启用删除按钮
                if (BtnDeleteUser != null) BtnDeleteUser.IsEnabled = true;
                
                // 隐藏错误提示
                if (TxtUserError != null) TxtUserError.Visibility = Visibility.Collapsed;
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"用户选择变更时出错: {ex}");
        }
    }

    /// <summary>
    /// 清空用户表单
    /// </summary>
    private void ClearUserForm()
    {
        try
        {
            if (TxtUsername != null) 
            {
                TxtUsername.Text = string.Empty;
                TxtUsername.IsEnabled = true; // 新建模式下允许编辑用户名
            }
            
            if (TxtDisplayName != null) TxtDisplayName.Text = string.Empty;
            if (PwdPassword != null) PwdPassword.Password = string.Empty;
            if (PwdConfirmPassword != null) PwdConfirmPassword.Password = string.Empty;
            if (ChkIsAdmin != null) ChkIsAdmin.IsChecked = false;
            if (TxtUserError != null) TxtUserError.Visibility = Visibility.Collapsed;
            
            // 禁用删除按钮，因为没有选中的用户
            if (BtnDeleteUser != null) BtnDeleteUser.IsEnabled = false;
            
            _selectedUser = null;
            _isEditMode = false;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"清空用户表单时出错: {ex}");
        }
    }

    /// <summary>
    /// 刷新用户列表按钮点击事件
    /// </summary>
    private void BtnRefreshUsers_Click(object sender, RoutedEventArgs e)
    {
        LoadUsersAsync();
    }

    /// <summary>
    /// 新用户按钮点击事件
    /// </summary>
    private void BtnNewUser_Click(object sender, RoutedEventArgs e)
    {
        ClearUserForm();
    }

    /// <summary>
    /// 保存用户按钮点击事件
    /// </summary>
    private async void BtnSaveUser_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            if (TxtUserError != null) TxtUserError.Visibility = Visibility.Collapsed;
            
            // 验证用户名
            if (string.IsNullOrWhiteSpace(TxtUsername?.Text))
            {
                ShowUserError("用户名不能为空");
                return;
            }
            
            // 如果是编辑模式且密码为空，则保留原密码
            if (_isEditMode && string.IsNullOrEmpty(PwdPassword?.Password) && 
                string.IsNullOrEmpty(PwdConfirmPassword?.Password))
            {
                // 仅更新其他信息
                if (_selectedUser != null)
                {
                    _selectedUser.DisplayName = TxtDisplayName?.Text;
                    _selectedUser.IsAdmin = ChkIsAdmin?.IsChecked ?? false;
                    
                    _context.Update(_selectedUser);
                    await _context.SaveChangesAsync();
                    
                    MessageBox.Show("用户信息已更新", "成功", MessageBoxButton.OK, MessageBoxImage.Information);
                    LoadUsersAsync();
                }
                return;
            }
            
            // 校验密码
            if (PwdPassword?.Password != PwdConfirmPassword?.Password)
            {
                ShowUserError("两次输入的密码不匹配");
                return;
            }
            
            if ((PwdPassword?.Password?.Length ?? 0) < 6)
            {
                ShowUserError("密码长度至少需要6个字符");
                return;
            }
            
            // 保存或更新用户
            if (_isEditMode && _selectedUser != null)
            {
                // 更新用户
                _selectedUser.DisplayName = TxtDisplayName?.Text;
                _selectedUser.IsAdmin = ChkIsAdmin?.IsChecked ?? false;
                
                // 更新密码
                string password = PwdPassword?.Password ?? string.Empty;
                await _authService.ChangePasswordAsync(_selectedUser.Id, password);
                
                _context.Update(_selectedUser);
                await _context.SaveChangesAsync();
                
                MessageBox.Show("用户信息已更新", "成功", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                // 检查用户名是否已存在
                string username = TxtUsername?.Text ?? string.Empty;
                if (await _context.Users.AnyAsync(u => u.Username == username))
                {
                    ShowUserError("用户名已存在");
                    return;
                }
                
                // 新建用户
                var newUser = new User
                {
                    Username = username,
                    DisplayName = TxtDisplayName?.Text,
                    IsAdmin = ChkIsAdmin?.IsChecked ?? false,
                    Password = PwdPassword?.Password ?? string.Empty
                };
                
                bool result = await _authService.RegisterUserAsync(newUser);
                if (result)
                {
                    MessageBox.Show("用户已创建", "成功", MessageBoxButton.OK, MessageBoxImage.Information);
                    ClearUserForm();
                }
                else
                {
                    ShowUserError("创建用户失败");
                    return;
                }
            }
            
            // 刷新用户列表
            LoadUsersAsync();
        }
        catch (Exception ex)
        {
            ShowUserError($"操作失败: {ex.Message}");
        }
    }

    /// <summary>
    /// 删除用户按钮点击事件
    /// </summary>
    private async void BtnDeleteUser_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            if (_selectedUser == null) return;
            
            // 防止删除自己
            if (_selectedUser.Id == CurrentUser?.Id)
            {
                MessageBox.Show("不能删除当前登录的用户", "警告", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            
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
                    ClearUserForm();
                    LoadUsersAsync();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"删除用户失败: {ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"删除用户时出错: {ex}");
            MessageBox.Show($"删除用户失败: {ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    /// <summary>
    /// 显示用户表单错误信息
    /// </summary>
    private void ShowUserError(string message)
    {
        if (TxtUserError != null)
        {
            TxtUserError.Text = message;
            TxtUserError.Visibility = Visibility.Visible;
        }
    }

    #endregion

    /// <summary>
    /// A列载玻片全选按钮点击事件
    /// </summary>
    private void SlideSelectAllA_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            // 获取A列所有的CheckBox并全选
            StackPanel columnA = GetColumnStackPanel(0);
            SelectAllCheckBoxesInColumn(columnA);
            
            Logger.Info("已全选A列载玻片");
        }
        catch (Exception ex)
        {
            Logger.Error($"A列全选出错: {ex.Message}");
        }
    }

    /// <summary>
    /// B列载玻片全选按钮点击事件
    /// </summary>
    private void SlideSelectAllB_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            // 获取B列所有的CheckBox并全选
            StackPanel columnB = GetColumnStackPanel(1);
            SelectAllCheckBoxesInColumn(columnB);
            
            Logger.Info("已全选B列载玻片");
        }
        catch (Exception ex)
        {
            Logger.Error($"B列全选出错: {ex.Message}");
        }
    }

    /// <summary>
    /// C列载玻片全选按钮点击事件
    /// </summary>
    private void SlideSelectAllC_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            // 获取C列所有的CheckBox并全选
            StackPanel columnC = GetColumnStackPanel(2);
            SelectAllCheckBoxesInColumn(columnC);
            
            Logger.Info("已全选C列载玻片");
        }
        catch (Exception ex)
        {
            Logger.Error($"C列全选出错: {ex.Message}");
        }
    }

    /// <summary>
    /// D列载玻片全选按钮点击事件
    /// </summary>
    private void SlideSelectAllD_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            // 获取D列所有的CheckBox并全选
            StackPanel columnD = GetColumnStackPanel(3);
            SelectAllCheckBoxesInColumn(columnD);
            
            Logger.Info("已全选D列载玻片");
        }
        catch (Exception ex)
        {
            Logger.Error($"D列全选出错: {ex.Message}");
        }
    }

    /// <summary>
    /// E列载玻片全选按钮点击事件
    /// </summary>
    private void SlideSelectAllE_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            // 获取E列所有的CheckBox并全选
            StackPanel columnE = GetColumnStackPanel(4);
            SelectAllCheckBoxesInColumn(columnE);
            
            Logger.Info("已全选E列载玻片");
        }
        catch (Exception ex)
        {
            Logger.Error($"E列全选出错: {ex.Message}");
        }
    }

    /// <summary>
    /// F列载玻片全选按钮点击事件
    /// </summary>
    private void SlideSelectAllF_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            // 获取F列所有的CheckBox并全选
            StackPanel columnF = GetColumnStackPanel(5);
            SelectAllCheckBoxesInColumn(columnF);
            
            Logger.Info("已全选F列载玻片");
        }
        catch (Exception ex)
        {
            Logger.Error($"F列全选出错: {ex.Message}");
        }
    }

    /// <summary>
    /// 获取指定列索引的StackPanel
    /// </summary>
    private StackPanel GetColumnStackPanel(int columnIndex)
    {
        // 获取RunPage的主Grid
        var runPageGrid = RunPage.Children.OfType<Grid>().FirstOrDefault();
        if (runPageGrid == null) return null;
        
        // 获取载玻片列表区域的Grid
        var slidesGrid = runPageGrid.Children.OfType<Grid>().ElementAtOrDefault(1);
        if (slidesGrid == null) return null;
        
        // 获取指定列的StackPanel
        return slidesGrid.Children.OfType<StackPanel>().ElementAtOrDefault(columnIndex);
    }

    /// <summary>
    /// 全选指定列中的所有CheckBox
    /// </summary>
    private void SelectAllCheckBoxesInColumn(StackPanel columnPanel)
    {
        if (columnPanel == null) return;
        
        // 获取所有包含CheckBox的Grid
        var grids = columnPanel.Children.OfType<Grid>().Where(g => 
            g.ColumnDefinitions.Count >= 2 && 
            g.Children.OfType<CheckBox>().Any());
            
        // 选中所有CheckBox
        foreach (var grid in grids)
        {
            var checkbox = grid.Children.OfType<CheckBox>().FirstOrDefault();
            if (checkbox != null)
            {
                checkbox.IsChecked = true;
            }
        }
    }

    private void ShowCustomMessageBoxExample()
    {
        try
        {
            // 显示一个信息消息框
            MessageBoxHelper.ShowInfo("操作已完成，数据已更新。", "提示", owner: this);
            
            // 显示一个确认消息框
            bool result = MessageBoxHelper.ShowConfirm("确定要执行此操作吗？操作执行后将无法撤销。", "确认", owner: this);
            if (result)
            {
                // 用户点击了"是"
                MessageBoxHelper.ShowSuccess("操作已成功执行！", "成功", owner: this);
            }
            else
            {
                // 用户点击了"否"
                MessageBoxHelper.ShowInfo("操作已取消。", "提示", owner: this);
            }
        }
        catch (Exception ex)
        {
            // 显示异常信息
            MessageBoxHelper.ShowException(ex, "错误", "在执行操作时发生错误。", this);
        }
    }
    
    // 添加一个测试按钮的点击处理程序
    private void TestMessageBox_Click(object sender, RoutedEventArgs e)
    {
        ShowCustomMessageBoxExample();
    }
}