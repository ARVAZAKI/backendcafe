using backendcafe.DTO;
namespace backendcafe.Services{
      public interface ISettingService {
            Task<List<SettingReadDTO>> GetSettingsByBranch(int branchId);
        Task<SettingReadDTO> CreateSetting(SettingCreateDTO createSettingDto);
        Task<SettingReadDTO> UpdateSetting(int settingId, SettingUpdateDTO updateSettingDto);
      }
}