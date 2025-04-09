using DRF_WPF.Data;
using DRF_WPF.Helpers;
using DRF_WPF.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace DRF_WPF.Services
{
    public class AuthService
    {
        private readonly ApplicationDbContext _context;

        public AuthService()
        {
            _context = new ApplicationDbContext();
            // 确保数据库初始化，并添加默认管理员用户
            EnsureAdminUserExists();
        }
        
        private void EnsureAdminUserExists()
        {
            try
            {
                // 确保数据库已创建
                _context.Database.EnsureCreated();
                
                // 检查是否有任何用户
                if (!_context.Users.Any())
                {
                    // 创建默认管理员用户 - 使用明文密码，不再哈希处理
                    var admin = new User
                    {
                        Username = "admin",
                        Password = "admin123",  // 直接使用明文密码
                        DisplayName = "管理员",
                        IsAdmin = true,
                        CreatedAt = DateTime.Now
                    };

                    _context.Users.Add(admin);
                    _context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"确保管理员用户存在时出错: {ex}");
            }
        }

        public async Task<User?> AuthenticateAsync(string username, string password)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
            if (user == null)
                return null;

            // 直接比较密码，不再使用哈希验证
            if (user.Password != password)
                return null;

            // Update last login time
            user.LastLoginTime = DateTime.Now;
            await _context.SaveChangesAsync();
            
            return user;
        }

        public async Task<bool> RegisterUserAsync(User user)
        {
            try
            {
                // Check if user already exists
                if (await _context.Users.AnyAsync(u => u.Username == user.Username))
                    return false;

                // 不再哈希密码
                user.CreatedAt = DateTime.Now;

                _context.Users.Add(user);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> ChangePasswordAsync(int userId, string newPassword)
        {
            try
            {
                var user = await _context.Users.FindAsync(userId);
                if (user == null)
                    return false;

                // 直接保存明文密码，不再哈希
                user.Password = newPassword;
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
} 