using System;
using System.Security.Cryptography;
using System.Text;

namespace DRF_WPF.Helpers
{
    public static class PasswordHelper
    {
        // 不再使用哈希，改为直接存储
        public static string HashPassword(string password)
        {
            // 直接返回原始密码
            return password;
        }

        public static bool VerifyPassword(string password, string storedPassword)
        {
            // 直接比较原始密码
            return password == storedPassword;
        }
    }
} 