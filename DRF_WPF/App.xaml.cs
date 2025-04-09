using System;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using DRF_WPF.Data;
using DRF_WPF.Helpers;
using DRF_WPF.Models;
using DRF_WPF.Services;
using DRF_WPF.Views.Controls;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace DRF_WPF;

/// <summary>
/// App.xaml 的交互逻辑
/// </summary>
public partial class App : Application
{
    private const string DatabaseFileName = "MYDB.db";
    private const string DatabaseBackupFileName = "MYDB_old.db";

    /// <summary>
    /// 应用程序启动时执行的方法
    /// </summary>
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        
        try
        {
            InitializeDatabase();
            // 预加载认证服务
            new AuthService();
        }
        catch (Exception ex)
        {
            MessageBoxHelper.ShowException(ex, "错误", "应用程序启动时出错");
            Shutdown();
        }
    }

    /// <summary>
    /// 初始化数据库连接和验证
    /// </summary>
    private void InitializeDatabase()
    {
        // 获取数据库文件路径
        string dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, DatabaseFileName);
        
        // 可选，强制重建数据库（取消注释以启用）
        // RecreateDatabase(dbPath);
        
        // 验证数据库连接和必要数据
        using (var context = new ApplicationDbContext())
        {
            // 检查是否可以连接到数据库
            bool canConnect = context.Database.CanConnect();
            if (!canConnect)
            {
                MessageBoxHelper.ShowError("数据库连接失败，请重启应用程序", "错误");
                Shutdown();
                return;
            }
            
            // 验证基本数据是否存在
            bool hasPrograms = context.Programs.Any();
            bool hasAdminUser = context.Users.Any(u => u.Username == "admin");
            
            // 记录日志信息（调试用）
            Debug.WriteLine($"数据库连接成功! 程序数: {context.Programs.Count()}, 用户数: {context.Users.Count()}");
            
            // 如果需要在启动时显示数据库信息，可以取消下面的注释
            /*
            MessageBoxHelper.ShowInfo(
                $"数据库已成功初始化！\n包含程序数: {context.Programs.Count()}\n包含用户数: {context.Users.Count()}", 
                "数据库初始化"
            );
            */
        }
    }
    
    /// <summary>
    /// 重建数据库（仅在需要时使用）
    /// </summary>
    /// <param name="dbPath">数据库文件路径</param>
    private void RecreateDatabase(string dbPath)
    {
        try
        {
            CloseExistingDatabaseConnections(dbPath);
            DeleteExistingDatabaseFile(dbPath);
            CreateNewDatabase();
        }
        catch (Exception ex)
        {
            MessageBoxHelper.ShowException(ex, "错误", "重建数据库时出错");
            Debug.WriteLine($"重建数据库时出错: {ex.Message}");
            throw;
        }
    }

    /// <summary>
    /// 关闭现有的数据库连接
    /// </summary>
    /// <param name="dbPath">数据库文件路径</param>
    private void CloseExistingDatabaseConnections(string dbPath)
    {
        using (var connection = new SqliteConnection($"Data Source={dbPath}"))
        {
            try
            {
                connection.Open();
                // 执行简单查询来验证连接
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT name FROM sqlite_master WHERE type='table'";
                    using (var reader = command.ExecuteReader())
                    {
                        // 读取所有结果以确保连接正常工作
                        while (reader.Read()) { }
                    }
                }
            }
            catch
            {
                // 忽略错误，可能是数据库文件不存在
            }
            finally
            {
                // 确保连接关闭
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
        }
    }

    /// <summary>
    /// 删除现有的数据库文件
    /// </summary>
    /// <param name="dbPath">数据库文件路径</param>
    private void DeleteExistingDatabaseFile(string dbPath)
    {
        if (!File.Exists(dbPath))
            return;
            
        try
        {
            File.Delete(dbPath);
        }
        catch (IOException)
        {
            // 文件可能被锁定，尝试重命名备份
            BackupLockedDatabaseFile(dbPath);
        }
    }

    /// <summary>
    /// 当数据库文件被锁定时，尝试创建备份
    /// </summary>
    /// <param name="dbPath">原数据库文件路径</param>
    private void BackupLockedDatabaseFile(string dbPath)
    {
        string directory = Path.GetDirectoryName(dbPath) ?? AppDomain.CurrentDomain.BaseDirectory;
        string tempPath = Path.Combine(directory, DatabaseBackupFileName);
        
        // 先删除已存在的备份文件
        if (File.Exists(tempPath))
        {
            try
            {
                File.Delete(tempPath);
            }
            catch
            {
                // 忽略备份文件删除错误
            }
        }
        
        try
        {
            File.Move(dbPath, tempPath);
        }
        catch
        {
            MessageBoxHelper.ShowWarning(
                "无法访问数据库文件，请确保没有其他程序正在使用数据库，然后重启应用程序", 
                "警告"
            );
            
            Debug.WriteLine("无法访问数据库文件，请确保没有其他程序正在使用数据库，然后重启应用程序");
        }
    }

    /// <summary>
    /// 创建新的数据库
    /// </summary>
    private void CreateNewDatabase()
    {
        using (var context = new ApplicationDbContext())
        {
            // 确保删除任何现有的数据库
            context.Database.EnsureDeleted();
            
            // 创建新数据库
            context.Database.EnsureCreated();
            
            Debug.WriteLine("数据库已重新创建");
        }
    }
}

