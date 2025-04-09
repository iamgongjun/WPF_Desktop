using System;
using System.Windows;

namespace DRF_WPF.Views.Controls
{
    /// <summary>
    /// 消息框辅助类，提供便捷的消息框显示方法
    /// </summary>
    public static class MessageBoxHelper
    {
        /// <summary>
        /// 显示信息消息框
        /// </summary>
        /// <param name="message">消息内容</param>
        /// <param name="title">标题</param>
        /// <param name="details">详细信息</param>
        /// <param name="owner">父窗口</param>
        /// <param name="callback">回调函数</param>
        /// <returns>用户选择结果</returns>
        public static bool ShowInfo(string message, string title = "提示", string details = null, Window owner = null, Action<bool> callback = null)
        {
            return CustomMessageBox.ShowInfo(message, title, details, owner, callback);
        }

        /// <summary>
        /// 显示警告消息框
        /// </summary>
        /// <param name="message">消息内容</param>
        /// <param name="title">标题</param>
        /// <param name="details">详细信息</param>
        /// <param name="buttons">按钮类型</param>
        /// <param name="owner">父窗口</param>
        /// <param name="callback">回调函数</param>
        /// <returns>用户选择结果</returns>
        public static bool ShowWarning(string message, string title = "警告", string details = null, MessageBoxButton buttons = MessageBoxButton.OK, Window owner = null, Action<bool> callback = null)
        {
            return CustomMessageBox.ShowWarning(message, title, details, buttons, owner, callback);
        }

        /// <summary>
        /// 显示错误消息框
        /// </summary>
        /// <param name="message">消息内容</param>
        /// <param name="title">标题</param>
        /// <param name="details">详细信息</param>
        /// <param name="owner">父窗口</param>
        /// <param name="callback">回调函数</param>
        /// <returns>用户选择结果</returns>
        public static bool ShowError(string message, string title = "错误", string details = null, Window owner = null, Action<bool> callback = null)
        {
            return CustomMessageBox.ShowError(message, title, details, owner, callback);
        }

        /// <summary>
        /// 显示确认消息框
        /// </summary>
        /// <param name="message">消息内容</param>
        /// <param name="title">标题</param>
        /// <param name="details">详细信息</param>
        /// <param name="owner">父窗口</param>
        /// <param name="callback">回调函数</param>
        /// <returns>用户选择结果</returns>
        public static bool ShowConfirm(string message, string title = "确认", string details = null, Window owner = null, Action<bool> callback = null)
        {
            return CustomMessageBox.ShowConfirm(message, title, details, owner, callback);
        }

        /// <summary>
        /// 显示成功消息框
        /// </summary>
        /// <param name="message">消息内容</param>
        /// <param name="title">标题</param>
        /// <param name="details">详细信息</param>
        /// <param name="owner">父窗口</param>
        /// <param name="callback">回调函数</param>
        /// <returns>用户选择结果</returns>
        public static bool ShowSuccess(string message, string title = "成功", string details = null, Window owner = null, Action<bool> callback = null)
        {
            return CustomMessageBox.ShowSuccess(message, title, details, owner, callback);
        }

        /// <summary>
        /// 显示异常消息框
        /// </summary>
        /// <param name="ex">异常</param>
        /// <param name="title">标题</param>
        /// <param name="message">附加消息</param>
        /// <param name="owner">父窗口</param>
        /// <param name="callback">回调函数</param>
        /// <returns>用户选择结果</returns>
        public static bool ShowException(Exception ex, string title = "异常", string message = null, Window owner = null, Action<bool> callback = null)
        {
            string exMessage = string.IsNullOrEmpty(message) 
                ? $"发生异常: {ex.Message}" 
                : $"{message}\n\n异常: {ex.Message}";
            
            string details = ex.StackTrace;
            
            return CustomMessageBox.ShowError(exMessage, title, details, owner, callback);
        }
    }
} 