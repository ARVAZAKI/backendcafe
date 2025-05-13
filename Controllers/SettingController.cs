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

        [HttpGet("{branchId}")]
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

        [HttpPut("{branchId}")]
        public async Task<ActionResult<SettingReadDTO>> UpdateSetting(
            [FromRoute] int branchId, 
            [FromBody] SettingUpdateDTO updateSettingDTO)
        {
            return await _settingService.UpdateSetting(branchId, updateSettingDTO);
        }

    }

    }
