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

        [HttpPut]
        public async Task<IActionResult> UpdateSetting([FromBody] int branchId, SettingUpdateDTO updateSettingDTO){
          try{
            var updatedSetting = await _settingService.UpdateSetting(branchId, updateSettingDTO);
            return Ok(updatedSetting);
          }catch(Exception ex){
            return BadRequest(ex.Message);
          }
        }

    }
}