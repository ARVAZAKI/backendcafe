using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using backendcafe.Services;
using backendcafe.DTO;

namespace backendcafe.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet("branch/{branchId}")]
        public async Task<IActionResult> GetCategoriesByBranch(int branchId)
        {
            try
            {
                var categories = await _categoryService.GetCategoriesByBranchAsync(branchId);
                return Ok(categories);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategoryById(int id)
        {
            try
            {
                var category = await _categoryService.GetCategoryByIdAsync(id);
                return Ok(category);
            }
            catch (Exception ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<IActionResult> CreateCategory([FromBody] CategoryCreateDTO categoryDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var createdCategory = await _categoryService.CreateCategoryAsync(categoryDto);
                return CreatedAtAction(nameof(GetCategoryById), new { id = createdCategory.Id }, createdCategory);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut]
        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<IActionResult> UpdateCategory([FromBody] CategoryUpdateDTO categoryDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var updatedCategory = await _categoryService.UpdateCategoryAsync(categoryDto);
                return Ok(updatedCategory);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete]
        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<IActionResult> DeleteCategory([FromBody] CategoryDeleteDTO categoryDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var result = await _categoryService.DeleteCategoryAsync(categoryDto);
                if (!result)
                    return NotFound(new { message = "Category not found" });
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}