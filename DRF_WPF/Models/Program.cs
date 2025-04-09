using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DRF_WPF.Models
{
    public class Program
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;
        
        /// <summary>
        /// 程序显示名称，例如 "111 (01:07)"
        /// </summary>
        [MaxLength(100)]
        public string DisplayName { get; set; } = string.Empty;
        
        /// <summary>
        /// 程序总时间(秒)
        /// </summary>
        public int TotalTimeInSeconds { get; set; }
        
        /// <summary>
        /// 程序创建时间
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        
        /// <summary>
        /// 程序最后修改时间
        /// </summary>
        public DateTime? LastModified { get; set; }
        
        /// <summary>
        /// 程序包含的步骤列表
        /// </summary>
        public virtual ICollection<ProgramStep> Steps { get; set; } = new List<ProgramStep>();
    }
} 