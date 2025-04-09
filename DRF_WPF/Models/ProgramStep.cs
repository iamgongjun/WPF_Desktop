using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DRF_WPF.Models
{
    public class ProgramStep
    {
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// 步骤序号
        /// </summary>
        public int StepNumber { get; set; }

        /// <summary>
        /// 试剂名称
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string ReagentName { get; set; } = string.Empty;

        /// <summary>
        /// 步骤执行时间(秒)
        /// </summary>
        public int TimeInSeconds { get; set; }

        /// <summary>
        /// 优先级
        /// </summary>
        [MaxLength(50)]
        public string Priority { get; set; } = "高";

        /// <summary>
        /// 吹风次数
        /// </summary>
        public int BlowCount { get; set; }

        /// <summary>
        /// 是否吹风
        /// </summary>
        public bool IsBlow { get; set; } = true;

        /// <summary>
        /// 所属程序ID
        /// </summary>
        public int ProgramId { get; set; }

        /// <summary>
        /// 所属程序
        /// </summary>
        [ForeignKey("ProgramId")]
        public virtual Program Program { get; set; } = null!;

        /// <summary>
        /// 步骤创建时间
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        /// <summary>
        /// 步骤最后修改时间
        /// </summary>
        public DateTime? LastModified { get; set; }
        
        #region UI显示属性，不存储到数据库
        
        /// <summary>
        /// 是否吹风的文本表示（"是"或"否"）
        /// </summary>
        [NotMapped]
        public string BlowText => IsBlow ? "是" : "否";
        
        #endregion
    }
} 