using System;
using System.Diagnostics;
using System.IO;

namespace DRF_WPF.Helpers
{
    /// <summary>
    /// 简单的日志记录器类
    /// </summary>
    public static class Logger
    {
        private static readonly string LogFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "application.log");

        /// <summary>
        /// 记录信息级别的日志
        /// </summary>
        /// <param name="message">日志消息</param>
        public static void Info(string message)
        {
            LogMessage("INFO", message);
        }

        /// <summary>
        /// 记录错误级别的日志
        /// </summary>
        /// <param name="message">日志消息</param>
        public static void Error(string message)
        {
            LogMessage("ERROR", message);
        }

        /// <summary>
        /// 记录日志消息
        /// </summary>
        /// <param name="level">日志级别</param>
        /// <param name="message">日志消息</param>
        private static void LogMessage(string level, string message)
        {
            string logEntry = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [{level}] {message}";
            
            // 输出到调试窗口
            Debug.WriteLine(logEntry);
            
            try
            {
                // 输出到日志文件
                File.AppendAllText(LogFilePath, logEntry + Environment.NewLine);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"写入日志文件时出错: {ex.Message}");
            }
        }
    }
} 