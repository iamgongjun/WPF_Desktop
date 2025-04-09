using DRF_WPF.Data;
using DRF_WPF.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace DRF_WPF.Services
{
    public class ProgramService
    {
        private readonly ApplicationDbContext _context;

        public ProgramService()
        {
            _context = new ApplicationDbContext();
        }

        #region 程序相关方法

        /// <summary>
        /// 获取所有程序
        /// </summary>
        public async Task<List<Program>> GetAllProgramsAsync()
        {
            return await _context.Programs
                .Include(p => p.Steps)
                .OrderBy(p => p.Name)
                .ToListAsync();
        }

        /// <summary>
        /// 根据ID获取程序
        /// </summary>
        public async Task<Program?> GetProgramByIdAsync(int id)
        {
            return await _context.Programs
                .Include(p => p.Steps.OrderBy(s => s.StepNumber))
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        /// <summary>
        /// 添加新程序
        /// </summary>
        public async Task<bool> AddProgramAsync(Program program)
        {
            try
            {
                _context.Programs.Add(program);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"添加程序时出错: {ex}");
                return false;
            }
        }

        /// <summary>
        /// 更新程序
        /// </summary>
        public async Task<bool> UpdateProgramAsync(Program program)
        {
            try
            {
                program.LastModified = DateTime.Now;
                _context.Programs.Update(program);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"更新程序时出错: {ex}");
                return false;
            }
        }

        /// <summary>
        /// 删除程序
        /// </summary>
        public async Task<bool> DeleteProgramAsync(int id)
        {
            try
            {
                var program = await _context.Programs.FindAsync(id);
                if (program == null)
                    return false;

                _context.Programs.Remove(program);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"删除程序时出错: {ex}");
                return false;
            }
        }

        #endregion

        #region 程序步骤相关方法

        /// <summary>
        /// 获取程序的所有步骤
        /// </summary>
        public async Task<List<ProgramStep>> GetProgramStepsAsync(int programId)
        {
            return await _context.ProgramSteps
                .Where(s => s.ProgramId == programId)
                .OrderBy(s => s.StepNumber)
                .ToListAsync();
        }

        /// <summary>
        /// 获取程序步骤的ObservableCollection，便于绑定UI
        /// </summary>
        public async Task<ObservableCollection<ProgramStep>> GetProgramStepsObservableAsync(int programId)
        {
            var steps = await GetProgramStepsAsync(programId);
            return new ObservableCollection<ProgramStep>(steps);
        }

        /// <summary>
        /// 根据ID获取步骤
        /// </summary>
        public async Task<ProgramStep?> GetStepByIdAsync(int id)
        {
            return await _context.ProgramSteps.FindAsync(id);
        }

        /// <summary>
        /// 添加新步骤
        /// </summary>
        public async Task<bool> AddStepAsync(ProgramStep step)
        {
            try
            {
                // 确保步骤序号正确
                if (step.StepNumber <= 0)
                {
                    var maxStepNumber = await _context.ProgramSteps
                        .Where(s => s.ProgramId == step.ProgramId)
                        .MaxAsync(s => (int?)s.StepNumber) ?? 0;
                    
                    step.StepNumber = maxStepNumber + 1;
                }

                _context.ProgramSteps.Add(step);
                await _context.SaveChangesAsync();
                
                // 更新程序的最后修改时间
                await UpdateProgramLastModifiedAsync(step.ProgramId);
                
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"添加步骤时出错: {ex}");
                return false;
            }
        }

        /// <summary>
        /// 更新步骤
        /// </summary>
        public async Task<bool> UpdateStepAsync(ProgramStep step)
        {
            try
            {
                step.LastModified = DateTime.Now;
                _context.ProgramSteps.Update(step);
                await _context.SaveChangesAsync();
                
                // 更新程序的最后修改时间
                await UpdateProgramLastModifiedAsync(step.ProgramId);
                
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"更新步骤时出错: {ex}");
                return false;
            }
        }

        /// <summary>
        /// 删除步骤
        /// </summary>
        public async Task<bool> DeleteStepAsync(int id)
        {
            try
            {
                var step = await _context.ProgramSteps.FindAsync(id);
                if (step == null)
                    return false;

                int programId = step.ProgramId;
                _context.ProgramSteps.Remove(step);
                await _context.SaveChangesAsync();
                
                // 更新序号
                await ReorderStepsAsync(programId);
                
                // 更新程序的最后修改时间
                await UpdateProgramLastModifiedAsync(programId);
                
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"删除步骤时出错: {ex}");
                return false;
            }
        }

        /// <summary>
        /// 移动步骤（上移或下移）- 彻底重写确保每次只移动一个序号
        /// </summary>
        public async Task<bool> MoveStepAsync(int stepId, bool moveUp)
        {
            // 开启数据库事务，保证操作的原子性
            using var transaction = await _context.Database.BeginTransactionAsync();
            
            try
            {
                System.Diagnostics.Debug.WriteLine($"开始移动步骤，ID={stepId}，方向={(moveUp ? "上移" : "下移")}");
                
                // 查找当前步骤
                var currentStep = await _context.ProgramSteps.FindAsync(stepId);
                if (currentStep == null)
                {
                    System.Diagnostics.Debug.WriteLine($"找不到ID为{stepId}的步骤");
                    return false;
                }
                
                int currentNumber = currentStep.StepNumber;
                int programId = currentStep.ProgramId;
                
                // 确定目标步骤序号
                int targetNumber = moveUp ? currentNumber - 1 : currentNumber + 1;
                System.Diagnostics.Debug.WriteLine($"当前步骤序号：{currentNumber}，目标序号：{targetNumber}");
                
                // 查找目标步骤
                var targetStep = await _context.ProgramSteps
                    .Where(s => s.ProgramId == programId && s.StepNumber == targetNumber)
                    .FirstOrDefaultAsync();
                
                if (targetStep == null)
                {
                    System.Diagnostics.Debug.WriteLine($"找不到序号为{targetNumber}的目标步骤");
                    await transaction.RollbackAsync();
                    return false;
                }
                
                // 暂存ID，用于打印日志
                int targetId = targetStep.Id;
                
                System.Diagnostics.Debug.WriteLine($"找到目标步骤，ID={targetId}，序号={targetNumber}");
                
                // 更新步骤序号 - 使用临时序号避免唯一性冲突
                System.Diagnostics.Debug.WriteLine($"设置目标步骤临时序号 {targetStep.StepNumber} -> -999");
                targetStep.StepNumber = -999; // 临时序号
                await _context.SaveChangesAsync();
                
                // 更新当前步骤序号
                System.Diagnostics.Debug.WriteLine($"设置当前步骤序号 {currentStep.StepNumber} -> {targetNumber}");
                currentStep.StepNumber = targetNumber;
                await _context.SaveChangesAsync();
                
                // 更新目标步骤为原始序号
                System.Diagnostics.Debug.WriteLine($"设置目标步骤序号 -999 -> {currentNumber}");
                targetStep.StepNumber = currentNumber;
                await _context.SaveChangesAsync();
                
                // 更新步骤的最后修改时间
                currentStep.LastModified = DateTime.Now;
                targetStep.LastModified = DateTime.Now;
                await _context.SaveChangesAsync();
                
                // 提交事务
                System.Diagnostics.Debug.WriteLine("提交事务");
                await transaction.CommitAsync();
                
                // 更新程序的最后修改时间
                await UpdateProgramLastModifiedAsync(programId);
                
                System.Diagnostics.Debug.WriteLine($"移动成功：步骤{stepId}从序号{currentNumber}移到{targetNumber}，" +
                               $"步骤{targetId}从序号{targetNumber}移到{currentNumber}");
                return true;
            }
            catch (Exception ex)
            {
                // 回滚事务
                await transaction.RollbackAsync();
                System.Diagnostics.Debug.WriteLine($"移动步骤时出错: {ex}");
                Console.WriteLine($"移动步骤时出错: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// 重新排序程序的所有步骤
        /// </summary>
        private async Task ReorderStepsAsync(int programId)
        {
            try
            {
                var steps = await _context.ProgramSteps
                    .Where(s => s.ProgramId == programId)
                    .OrderBy(s => s.StepNumber)
                    .ToListAsync();

                for (int i = 0; i < steps.Count; i++)
                {
                    steps[i].StepNumber = i + 1;
                    _context.ProgramSteps.Update(steps[i]);
                }

                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"重新排序步骤时出错: {ex}");
            }
        }

        /// <summary>
        /// 更新程序的最后修改时间
        /// </summary>
        private async Task UpdateProgramLastModifiedAsync(int programId)
        {
            try
            {
                var program = await _context.Programs.FindAsync(programId);
                if (program != null)
                {
                    program.LastModified = DateTime.Now;
                    _context.Programs.Update(program);
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"更新程序最后修改时间时出错: {ex}");
            }
        }

        #endregion
    }
} 