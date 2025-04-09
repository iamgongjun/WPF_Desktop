using Microsoft.EntityFrameworkCore;
using DRF_WPF.Models;
using System;
using System.IO;

namespace DRF_WPF.Data
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Program> Programs { get; set; } = null!;
        public DbSet<ProgramStep> ProgramSteps { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Use the local MYDB.db file in the project directory
            string dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "MYDB.db");
            
            optionsBuilder.UseSqlite($"Data Source={dbPath}");
            
            // Enable detailed errors for development
            #if DEBUG
            optionsBuilder.EnableSensitiveDataLogging();
            #endif
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 配置用户实体
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Username).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Password).IsRequired().HasMaxLength(100);
                entity.Property(e => e.DisplayName).HasMaxLength(100);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
                
                // Create unique index on Username
                entity.HasIndex(e => e.Username).IsUnique();
            });
            
            // 配置程序实体
            modelBuilder.Entity<Program>(entity => 
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.DisplayName).HasMaxLength(100);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
                
                // 程序与步骤的一对多关系
                entity.HasMany(e => e.Steps)
                      .WithOne(e => e.Program)
                      .HasForeignKey(e => e.ProgramId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
            
            // 配置程序步骤实体
            modelBuilder.Entity<ProgramStep>(entity => 
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.ReagentName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Priority).HasMaxLength(50);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            });
            
            // 添加默认用户
            modelBuilder.Entity<User>().HasData(
                new User 
                { 
                    Id = 1, 
                    Username = "admin", 
                    Password = "admin123", 
                    DisplayName = "管理员", 
                    IsAdmin = true,
                    CreatedAt = DateTime.Now 
                }
            );
            
            // 添加默认染色程序
            modelBuilder.Entity<Program>().HasData(
                new Program
                {
                    Id = 1,
                    Name = "HE",
                    DisplayName = "HE (07:55)",
                    TotalTimeInSeconds = 475,
                    CreatedAt = DateTime.Now
                },
                new Program
                {
                    Id = 2,
                    Name = "常规HE",
                    DisplayName = "常规HE (40:25)",
                    TotalTimeInSeconds = 2425,
                    CreatedAt = DateTime.Now
                },
                new Program
                {
                    Id = 3,
                    Name = "胃镜",
                    DisplayName = "胃镜 (36:50)",
                    TotalTimeInSeconds = 2210,
                    CreatedAt = DateTime.Now
                },
                new Program
                {
                    Id = 4,
                    Name = "通道",
                    DisplayName = "通道 (01:40)",
                    TotalTimeInSeconds = 100,
                    CreatedAt = DateTime.Now
                },
                new Program
                {
                    Id = 5,
                    Name = "111",
                    DisplayName = "111 (01:07)",
                    TotalTimeInSeconds = 67,
                    CreatedAt = DateTime.Now
                },
                new Program
                {
                    Id = 6,
                    Name = "12",
                    DisplayName = "12 (01:18)",
                    TotalTimeInSeconds = 78,
                    CreatedAt = DateTime.Now
                },
                new Program
                {
                    Id = 7,
                    Name = "PAS染色",
                    DisplayName = "PAS染色 (35:40)",
                    TotalTimeInSeconds = 2140,
                    CreatedAt = DateTime.Now
                },
                new Program
                {
                    Id = 8,
                    Name = "VG染色",
                    DisplayName = "VG染色 (28:15)",
                    TotalTimeInSeconds = 1695,
                    CreatedAt = DateTime.Now
                },
                new Program
                {
                    Id = 9,
                    Name = "Giemsa染色",
                    DisplayName = "Giemsa染色 (22:30)",
                    TotalTimeInSeconds = 1350,
                    CreatedAt = DateTime.Now
                },
                new Program
                {
                    Id = 10,
                    Name = "AF染色",
                    DisplayName = "AF染色 (18:45)",
                    TotalTimeInSeconds = 1125,
                    CreatedAt = DateTime.Now
                }
            );
            
            // 添加默认程序步骤
            modelBuilder.Entity<ProgramStep>().HasData(
                // HE程序的步骤
                new ProgramStep
                {
                    Id = 1,
                    ProgramId = 1,
                    StepNumber = 1,
                    ReagentName = "脱蜡剂",
                    TimeInSeconds = 120,
                    Priority = "高",
                    BlowCount = 2,
                    IsBlow = true,
                    CreatedAt = DateTime.Now
                },
                new ProgramStep
                {
                    Id = 2,
                    ProgramId = 1,
                    StepNumber = 2,
                    ReagentName = "100%酒精",
                    TimeInSeconds = 60,
                    Priority = "中",
                    BlowCount = 1,
                    IsBlow = true,
                    CreatedAt = DateTime.Now
                },
                new ProgramStep
                {
                    Id = 3,
                    ProgramId = 1,
                    StepNumber = 3,
                    ReagentName = "95%酒精",
                    TimeInSeconds = 60,
                    Priority = "中",
                    BlowCount = 1,
                    IsBlow = true,
                    CreatedAt = DateTime.Now
                },
                new ProgramStep
                {
                    Id = 4,
                    ProgramId = 1,
                    StepNumber = 4,
                    ReagentName = "70%酒精",
                    TimeInSeconds = 60,
                    Priority = "中",
                    BlowCount = 1,
                    IsBlow = true,
                    CreatedAt = DateTime.Now
                },
                new ProgramStep
                {
                    Id = 5,
                    ProgramId = 1,
                    StepNumber = 5,
                    ReagentName = "苏木素",
                    TimeInSeconds = 150,
                    Priority = "高",
                    BlowCount = 0,
                    IsBlow = false,
                    CreatedAt = DateTime.Now
                },
                new ProgramStep
                {
                    Id = 6,
                    ProgramId = 1,
                    StepNumber = 6,
                    ReagentName = "伊红",
                    TimeInSeconds = 25,
                    Priority = "高",
                    BlowCount = 0,
                    IsBlow = false,
                    CreatedAt = DateTime.Now
                },
                
                // 常规HE程序的步骤
                new ProgramStep
                {
                    Id = 7,
                    ProgramId = 2,
                    StepNumber = 1,
                    ReagentName = "脱蜡剂1",
                    TimeInSeconds = 300,
                    Priority = "高",
                    BlowCount = 3,
                    IsBlow = true,
                    CreatedAt = DateTime.Now
                },
                new ProgramStep
                {
                    Id = 8,
                    ProgramId = 2,
                    StepNumber = 2,
                    ReagentName = "脱蜡剂2",
                    TimeInSeconds = 300,
                    Priority = "高",
                    BlowCount = 3,
                    IsBlow = true,
                    CreatedAt = DateTime.Now
                },
                new ProgramStep
                {
                    Id = 9,
                    ProgramId = 2,
                    StepNumber = 3,
                    ReagentName = "100%酒精1",
                    TimeInSeconds = 180,
                    Priority = "中",
                    BlowCount = 2,
                    IsBlow = true,
                    CreatedAt = DateTime.Now
                },
                new ProgramStep
                {
                    Id = 10,
                    ProgramId = 2,
                    StepNumber = 4,
                    ReagentName = "100%酒精2",
                    TimeInSeconds = 180,
                    Priority = "中",
                    BlowCount = 2,
                    IsBlow = true,
                    CreatedAt = DateTime.Now
                },
                new ProgramStep
                {
                    Id = 11,
                    ProgramId = 2,
                    StepNumber = 5,
                    ReagentName = "95%酒精",
                    TimeInSeconds = 120,
                    Priority = "中",
                    BlowCount = 2,
                    IsBlow = true,
                    CreatedAt = DateTime.Now
                },
                new ProgramStep
                {
                    Id = 12,
                    ProgramId = 2,
                    StepNumber = 6,
                    ReagentName = "80%酒精",
                    TimeInSeconds = 120,
                    Priority = "中",
                    BlowCount = 2,
                    IsBlow = true,
                    CreatedAt = DateTime.Now
                },
                new ProgramStep
                {
                    Id = 13,
                    ProgramId = 2,
                    StepNumber = 7,
                    ReagentName = "70%酒精",
                    TimeInSeconds = 120,
                    Priority = "中",
                    BlowCount = 2,
                    IsBlow = true,
                    CreatedAt = DateTime.Now
                },
                new ProgramStep
                {
                    Id = 14,
                    ProgramId = 2,
                    StepNumber = 8,
                    ReagentName = "水洗",
                    TimeInSeconds = 60,
                    Priority = "低",
                    BlowCount = 0,
                    IsBlow = false,
                    CreatedAt = DateTime.Now
                },
                new ProgramStep
                {
                    Id = 15,
                    ProgramId = 2,
                    StepNumber = 9,
                    ReagentName = "苏木素",
                    TimeInSeconds = 600,
                    Priority = "高",
                    BlowCount = 0,
                    IsBlow = false,
                    CreatedAt = DateTime.Now
                },
                new ProgramStep
                {
                    Id = 16,
                    ProgramId = 2,
                    StepNumber = 10,
                    ReagentName = "水洗",
                    TimeInSeconds = 60,
                    Priority = "低",
                    BlowCount = 0,
                    IsBlow = false,
                    CreatedAt = DateTime.Now
                },
                new ProgramStep
                {
                    Id = 17,
                    ProgramId = 2,
                    StepNumber = 11,
                    ReagentName = "盐酸酒精",
                    TimeInSeconds = 20,
                    Priority = "高",
                    BlowCount = 0,
                    IsBlow = false,
                    CreatedAt = DateTime.Now
                },
                new ProgramStep
                {
                    Id = 18,
                    ProgramId = 2,
                    StepNumber = 12,
                    ReagentName = "水洗",
                    TimeInSeconds = 60,
                    Priority = "低",
                    BlowCount = 0,
                    IsBlow = false,
                    CreatedAt = DateTime.Now
                },
                new ProgramStep
                {
                    Id = 19,
                    ProgramId = 2,
                    StepNumber = 13,
                    ReagentName = "伊红",
                    TimeInSeconds = 60,
                    Priority = "高",
                    BlowCount = 0,
                    IsBlow = false,
                    CreatedAt = DateTime.Now
                },
                new ProgramStep
                {
                    Id = 20,
                    ProgramId = 2,
                    StepNumber = 14,
                    ReagentName = "水洗",
                    TimeInSeconds = 60,
                    Priority = "低",
                    BlowCount = 0,
                    IsBlow = false,
                    CreatedAt = DateTime.Now
                },
                
                // 胃镜程序的步骤
                new ProgramStep
                {
                    Id = 21,
                    ProgramId = 3,
                    StepNumber = 1,
                    ReagentName = "福尔马林",
                    TimeInSeconds = 300,
                    Priority = "高",
                    BlowCount = 2,
                    IsBlow = true,
                    CreatedAt = DateTime.Now
                },
                new ProgramStep
                {
                    Id = 22,
                    ProgramId = 3,
                    StepNumber = 2,
                    ReagentName = "水洗",
                    TimeInSeconds = 120,
                    Priority = "低",
                    BlowCount = 0,
                    IsBlow = false,
                    CreatedAt = DateTime.Now
                },
                new ProgramStep
                {
                    Id = 23,
                    ProgramId = 3,
                    StepNumber = 3,
                    ReagentName = "特殊染色液A",
                    TimeInSeconds = 600,
                    Priority = "高",
                    BlowCount = 0,
                    IsBlow = false,
                    CreatedAt = DateTime.Now
                },
                new ProgramStep
                {
                    Id = 24,
                    ProgramId = 3,
                    StepNumber = 4,
                    ReagentName = "特殊染色液B",
                    TimeInSeconds = 450,
                    Priority = "高",
                    BlowCount = 0,
                    IsBlow = false,
                    CreatedAt = DateTime.Now
                },
                
                // 通道程序的步骤
                new ProgramStep
                {
                    Id = 25,
                    ProgramId = 4,
                    StepNumber = 1,
                    ReagentName = "二甲苯",
                    TimeInSeconds = 20,
                    Priority = "高",
                    BlowCount = 1,
                    IsBlow = true,
                    CreatedAt = DateTime.Now
                },
                new ProgramStep
                {
                    Id = 26,
                    ProgramId = 4,
                    StepNumber = 2,
                    ReagentName = "二甲苯",
                    TimeInSeconds = 20,
                    Priority = "高",
                    BlowCount = 1,
                    IsBlow = true,
                    CreatedAt = DateTime.Now
                },
                new ProgramStep
                {
                    Id = 27,
                    ProgramId = 4,
                    StepNumber = 3,
                    ReagentName = "100%酒精",
                    TimeInSeconds = 20,
                    Priority = "中",
                    BlowCount = 1,
                    IsBlow = true,
                    CreatedAt = DateTime.Now
                },
                new ProgramStep
                {
                    Id = 28,
                    ProgramId = 4,
                    StepNumber = 4,
                    ReagentName = "95%酒精",
                    TimeInSeconds = 20,
                    Priority = "中",
                    BlowCount = 1,
                    IsBlow = true,
                    CreatedAt = DateTime.Now
                },
                new ProgramStep
                {
                    Id = 29,
                    ProgramId = 4,
                    StepNumber = 5,
                    ReagentName = "80%酒精",
                    TimeInSeconds = 20,
                    Priority = "中",
                    BlowCount = 1,
                    IsBlow = true,
                    CreatedAt = DateTime.Now
                },
                
                // 111程序的步骤 
                new ProgramStep
                {
                    Id = 30,
                    ProgramId = 5,
                    StepNumber = 1,
                    ReagentName = "福尔马林",
                    TimeInSeconds = 30,
                    Priority = "高",
                    BlowCount = 3,
                    IsBlow = true,
                    CreatedAt = DateTime.Now
                },
                new ProgramStep
                {
                    Id = 31,
                    ProgramId = 5,
                    StepNumber = 2,
                    ReagentName = "70%酒精",
                    TimeInSeconds = 37,
                    Priority = "中",
                    BlowCount = 2,
                    IsBlow = true,
                    CreatedAt = DateTime.Now
                },
                
                // 12程序的步骤
                new ProgramStep
                {
                    Id = 32,
                    ProgramId = 6, 
                    StepNumber = 1,
                    ReagentName = "福尔马林",
                    TimeInSeconds = 30,
                    Priority = "高",
                    BlowCount = 2,
                    IsBlow = true,
                    CreatedAt = DateTime.Now
                },
                new ProgramStep
                {
                    Id = 33,
                    ProgramId = 6,
                    StepNumber = 2, 
                    ReagentName = "脱蜡剂",
                    TimeInSeconds = 48,
                    Priority = "高", 
                    BlowCount = 1,
                    IsBlow = true,
                    CreatedAt = DateTime.Now
                },
                
                // PAS染色程序步骤
                new ProgramStep
                {
                    Id = 34,
                    ProgramId = 7,
                    StepNumber = 1,
                    ReagentName = "二甲苯",
                    TimeInSeconds = 180,
                    Priority = "高",
                    BlowCount = 2,
                    IsBlow = true,
                    CreatedAt = DateTime.Now
                },
                new ProgramStep
                {
                    Id = 35,
                    ProgramId = 7,
                    StepNumber = 2,
                    ReagentName = "高碘酸",
                    TimeInSeconds = 300,
                    Priority = "高",
                    BlowCount = 0,
                    IsBlow = false,
                    CreatedAt = DateTime.Now
                },
                new ProgramStep
                {
                    Id = 36,
                    ProgramId = 7,
                    StepNumber = 3,
                    ReagentName = "水洗",
                    TimeInSeconds = 120,
                    Priority = "低",
                    BlowCount = 0,
                    IsBlow = false,
                    CreatedAt = DateTime.Now
                },
                new ProgramStep
                {
                    Id = 37,
                    ProgramId = 7,
                    StepNumber = 4,
                    ReagentName = "雪夫试剂",
                    TimeInSeconds = 600,
                    Priority = "高",
                    BlowCount = 0,
                    IsBlow = false,
                    CreatedAt = DateTime.Now
                },
                new ProgramStep
                {
                    Id = 38,
                    ProgramId = 7,
                    StepNumber = 5,
                    ReagentName = "水洗",
                    TimeInSeconds = 120,
                    Priority = "低",
                    BlowCount = 0,
                    IsBlow = false,
                    CreatedAt = DateTime.Now
                },
                
                // VG染色程序步骤
                new ProgramStep
                {
                    Id = 39,
                    ProgramId = 8,
                    StepNumber = 1,
                    ReagentName = "核苏木红",
                    TimeInSeconds = 300,
                    Priority = "高",
                    BlowCount = 0,
                    IsBlow = false,
                    CreatedAt = DateTime.Now
                },
                new ProgramStep
                {
                    Id = 40,
                    ProgramId = 8,
                    StepNumber = 2,
                    ReagentName = "水洗",
                    TimeInSeconds = 60,
                    Priority = "低",
                    BlowCount = 0,
                    IsBlow = false,
                    CreatedAt = DateTime.Now
                },
                new ProgramStep
                {
                    Id = 41,
                    ProgramId = 8,
                    StepNumber = 3,
                    ReagentName = "维多利亚蓝",
                    TimeInSeconds = 180,
                    Priority = "高",
                    BlowCount = 0,
                    IsBlow = false,
                    CreatedAt = DateTime.Now
                },
                new ProgramStep
                {
                    Id = 42,
                    ProgramId = 8,
                    StepNumber = 4,
                    ReagentName = "水洗",
                    TimeInSeconds = 60,
                    Priority = "低",
                    BlowCount = 0,
                    IsBlow = false,
                    CreatedAt = DateTime.Now
                },
                new ProgramStep
                {
                    Id = 43,
                    ProgramId = 8,
                    StepNumber = 5,
                    ReagentName = "番红",
                    TimeInSeconds = 240,
                    Priority = "高",
                    BlowCount = 0,
                    IsBlow = false,
                    CreatedAt = DateTime.Now
                },
                new ProgramStep
                {
                    Id = 44,
                    ProgramId = 8,
                    StepNumber = 6,
                    ReagentName = "水洗",
                    TimeInSeconds = 60,
                    Priority = "低",
                    BlowCount = 0,
                    IsBlow = false,
                    CreatedAt = DateTime.Now
                },
                
                // Giemsa染色程序步骤
                new ProgramStep
                {
                    Id = 45,
                    ProgramId = 9,
                    StepNumber = 1,
                    ReagentName = "甲醇固定",
                    TimeInSeconds = 180,
                    Priority = "高",
                    BlowCount = 0,
                    IsBlow = false,
                    CreatedAt = DateTime.Now
                },
                new ProgramStep
                {
                    Id = 46,
                    ProgramId = 9,
                    StepNumber = 2,
                    ReagentName = "Giemsa工作液",
                    TimeInSeconds = 600,
                    Priority = "高",
                    BlowCount = 0,
                    IsBlow = false,
                    CreatedAt = DateTime.Now
                },
                new ProgramStep
                {
                    Id = 47,
                    ProgramId = 9,
                    StepNumber = 3,
                    ReagentName = "水洗",
                    TimeInSeconds = 60,
                    Priority = "低",
                    BlowCount = 0,
                    IsBlow = false,
                    CreatedAt = DateTime.Now
                },
                new ProgramStep
                {
                    Id = 48,
                    ProgramId = 9,
                    StepNumber = 4,
                    ReagentName = "95%酒精",
                    TimeInSeconds = 30,
                    Priority = "中",
                    BlowCount = 1,
                    IsBlow = true,
                    CreatedAt = DateTime.Now
                },
                
                // AF染色程序步骤
                new ProgramStep
                {
                    Id = 49,
                    ProgramId = 10,
                    StepNumber = 1,
                    ReagentName = "酸性茜素",
                    TimeInSeconds = 240,
                    Priority = "高",
                    BlowCount = 0,
                    IsBlow = false,
                    CreatedAt = DateTime.Now
                },
                new ProgramStep
                {
                    Id = 50,
                    ProgramId = 10,
                    StepNumber = 2,
                    ReagentName = "水洗",
                    TimeInSeconds = 60,
                    Priority = "低",
                    BlowCount = 0,
                    IsBlow = false,
                    CreatedAt = DateTime.Now
                },
                new ProgramStep
                {
                    Id = 51,
                    ProgramId = 10,
                    StepNumber = 3,
                    ReagentName = "媒染剂",
                    TimeInSeconds = 180,
                    Priority = "高",
                    BlowCount = 0,
                    IsBlow = false,
                    CreatedAt = DateTime.Now
                },
                new ProgramStep
                {
                    Id = 52,
                    ProgramId = 10,
                    StepNumber = 4,
                    ReagentName = "水洗",
                    TimeInSeconds = 60,
                    Priority = "低",
                    BlowCount = 0,
                    IsBlow = false,
                    CreatedAt = DateTime.Now
                },
                new ProgramStep
                {
                    Id = 53,
                    ProgramId = 10,
                    StepNumber = 5,
                    ReagentName = "苯胺蓝",
                    TimeInSeconds = 300,
                    Priority = "高",
                    BlowCount = 0,
                    IsBlow = false,
                    CreatedAt = DateTime.Now
                }
            );
        }
    }
} 