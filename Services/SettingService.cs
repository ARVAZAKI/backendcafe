using backendcafe.Data;
using backendcafe.DTO;
using backendcafe.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace backendcafe.Services{
      public class SettingService : ISettingService{
      private readonly ApplicationDbContext _context;

        public SettingService(ApplicationDbContext context)
        {
            _context = context;
        }

       public async Task<List<SettingReadDTO>> GetSettingsByBranch(int branchId)
        {
            var query = _context.Settings
                .Where(p => p.BranchId == branchId);
                
            return await query.Select(p => new SettingReadDTO
            {
                OpeningTime = p.OpeningTime,
                ClosingTime = p.ClosingTime,
                WifiPassword = p.WifiPassword
            }).ToListAsync();
        }


        public async Task<SettingReadDTO> CreateSetting(SettingCreateDTO createSettingDto){
            var branchExists = await _context.Branches.AnyAsync(b => b.Id == createSettingDto.BranchId);
            if (!branchExists)
                throw new ArgumentException("Branch not found");

            var existingSetting = await _context.Settings.FirstOrDefaultAsync(s => s.BranchId == createSettingDto.BranchId);
            if (existingSetting != null)
                  throw new InvalidOperationException("This branch already has a setting. Use update instead.");

            var setting = new Setting{
                  BranchId = createSettingDto.BranchId,
                  OpeningTime = createSettingDto.OpeningTime,
                  ClosingTime = createSettingDto.ClosingTime,
                  WifiPassword = createSettingDto.WifiPassword
            };
            _context.Settings.Add(setting);
            await _context.SaveChangesAsync();

            return new SettingReadDTO{
                  Id = setting.Id,
                  OpeningTime = createSettingDto.OpeningTime,     
                  ClosingTime = createSettingDto.ClosingTime, 
                  WifiPassword = createSettingDto.WifiPassword    
            };
        }

        public async Task<SettingReadDTO> UpdateSetting(int SettingId, SettingUpdateDTO updateSettingDto) {
            var setting = await _context.Settings.FindAsync(SettingId);
            if (setting == null)
                throw new Exception("Setting not found");
            
            setting.OpeningTime = updateSettingDto.OpeningTime;
            setting.ClosingTime = updateSettingDto.ClosingTime;
            setting.WifiPassword = updateSettingDto.WifiPassword;

            await _context.SaveChangesAsync();

            return new SettingReadDTO{
                  Id = setting.Id,
                  ClosingTime = updateSettingDto.ClosingTime,
                  OpeningTime = updateSettingDto.OpeningTime,
                  WifiPassword = updateSettingDto.WifiPassword
            };
        }


            
      }


}