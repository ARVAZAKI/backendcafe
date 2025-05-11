using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using backendcafe.Services;
using backendcafe.DTO;

namespace backendcafe.Controllers{
      [Route("api/[controller]")]
    [ApiController]
    public class SettingController : ControllerBase{
      private readonly ISettingService _settingService;

        public SettingController(ISettingService settingService)
        {
            _settingService = settingService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetSetting(int branchId){
         try{
           var setting = await _settingService.GetSettingsByBranch(branchId);
          return Ok(setting);
         }catch(Exception ex){
           return BadRequest(ex.Message);
         }
        }

        [HttpPost]
        public async Task<IActionResult> CreateSetting([FromBody] SettingCreateDTO createSettingDTO){
          try{
            var createdSetting = await _settingService.CreateSetting(createSettingDTO);
            return Ok(createdSetting);

          }catch(Exception ex){
            return BadRequest(ex.Message);
          }
        }

        [HttpPut("{settingId}")]
        public async Task<ActionResult<SettingReadDTO>> UpdateSetting(
            [FromRoute] int settingId, 
            [FromBody] SettingUpdateDTO updateSettingDTO)
        {
            return await _settingService.UpdateSetting(settingId, updateSettingDTO);
        }

        // ATAU Solusi 2: Membuat action baru dengan parameter branch ID
        [HttpPut("branch/{branchId}")]
        public async Task<ActionResult<SettingReadDTO>> UpdateSettingByBranch(
            [FromRoute] int branchId,
            [FromBody] SettingUpdateDTO updateSettingDTO)
        {
            // Cari setting ID berdasarkan branch ID
            var settings = await _settingService.GetSettingsByBranch(branchId);
            if (settings.Count == 0)
                return NotFound($"No setting found for branch with ID {branchId}");
            
            var settingId = settings[0].Id;
            return await _settingService.UpdateSetting(settingId, updateSettingDTO);
        }
    }

    }
