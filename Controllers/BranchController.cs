using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using backendcafe.Services;
using backendcafe.DTO;

namespace backendcafe.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BranchController : ControllerBase
    {
        private readonly IBranchService _branchService;

        public BranchController(IBranchService branchService)
        {
            _branchService = branchService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllBranches()
        {
            try
            {
                var branches = await _branchService.GetAllBranchesAsync();
                return Ok(branches);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetBranchById(int id)
        {
            try
            {
                var branch = await _branchService.GetBranchByIdAsync(id);
                return Ok(branch);
            }
            catch (Exception ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<IActionResult> CreateBranch([FromBody] BranchCreateDTO branchDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var createdBranch = await _branchService.CreateBranchAsync(branchDto);
                return CreatedAtAction(nameof(GetBranchById), new { id = createdBranch.Id }, createdBranch);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<IActionResult> UpdateBranch(int id, [FromBody] BranchUpdateDTO branchDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var updatedBranch = await _branchService.UpdateBranchAsync(id, branchDto); 
                return Ok(updatedBranch);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete]
        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<IActionResult> DeleteBranch([FromBody] BranchDeleteDTO branchDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var result = await _branchService.DeleteBranchAsync(branchDto);
                if (!result)
                    return NotFound(new { message = "Branch not found" });
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}