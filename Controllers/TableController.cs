using Microsoft.AspNetCore.Mvc;
using backendcafe.DTO;
using backendcafe.Services;

namespace backendcafe.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TableController : ControllerBase
    {
        private readonly ITableService _tableService;
        private readonly ILogger<TableController> _logger;

        public TableController(ITableService tableService, ILogger<TableController> logger)
        {
            _tableService = tableService;
            _logger = logger;
        }

        /// <summary>
        /// Get all tables
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TableReadDTO>>> GetAllTables()
        {
            try
            {
                var tables = await _tableService.GetAllTablesAsync();
                return Ok(new { success = true, data = tables });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all tables");
                return StatusCode(500, new { success = false, message = "Internal server error" });
            }
        }

        /// <summary>
        /// Get tables by branch
        /// </summary>
        [HttpGet("branch/{branchId}")]
        public async Task<ActionResult<IEnumerable<TableReadDTO>>> GetTablesByBranch(int branchId)
        {
            try
            {
                var tables = await _tableService.GetTablesByBranchAsync(branchId);
                return Ok(new { success = true, data = tables });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting tables for branch {BranchId}", branchId);
                return StatusCode(500, new { success = false, message = "Internal server error" });
            }
        }

        /// <summary>
        /// Get table by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<TableReadDTO>> GetTableById(int id)
        {
            try
            {
                var table = await _tableService.GetTableByIdAsync(id);
                return Ok(new { success = true, data = table });
            }
            catch (Exception ex) when (ex.Message == "Table not found")
            {
                return NotFound(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting table {Id}", id);
                return StatusCode(500, new { success = false, message = "Internal server error" });
            }
        }

        /// <summary>
        /// Create new table
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<TableReadDTO>> CreateTable([FromBody] TableCreateDTO tableDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(new { success = false, message = "Invalid input data", errors = ModelState });

                var table = await _tableService.CreateTableAsync(tableDto);
                return CreatedAtAction(nameof(GetTableById), new { id = table.Id }, 
                    new { success = true, data = table });
            }
            catch (Exception ex) when (ex.Message.Contains("not found") || ex.Message.Contains("already exists"))
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating table");
                return StatusCode(500, new { success = false, message = "Internal server error" });
            }
        }

        /// <summary>
        /// Update table
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult<TableReadDTO>> UpdateTable(int id, [FromBody] TableUpdateDTO tableDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(new { success = false, message = "Invalid input data", errors = ModelState });

                var table = await _tableService.UpdateTableAsync(id, tableDto);
                return Ok(new { success = true, data = table });
            }
            catch (Exception ex) when (ex.Message == "Table not found")
            {
                return NotFound(new { success = false, message = ex.Message });
            }
            catch (Exception ex) when (ex.Message.Contains("already exists"))
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating table {Id}", id);
                return StatusCode(500, new { success = false, message = "Internal server error" });
            }
        }

        /// <summary>
        /// Delete table
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteTable(int id)
        {
            try
            {
                var deleteDto = new TableDeleteDTO { Id = id };
                var deleted = await _tableService.DeleteTableAsync(deleteDto);
                
                if (!deleted)
                    return NotFound(new { success = false, message = "Table not found" });

                return Ok(new { success = true, message = "Table deleted successfully" });
            }
            catch (Exception ex) when (ex.Message.Contains("Cannot delete"))
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting table {Id}", id);
                return StatusCode(500, new { success = false, message = "Internal server error" });
            }
        }

        /// <summary>
        /// Check table availability
        /// </summary>
        [HttpPost("check-availability")]
        public async Task<ActionResult<IEnumerable<TableReadDTO>>> CheckAvailability([FromBody] TableAvailabilityCheckDTO checkDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(new { success = false, message = "Invalid input data", errors = ModelState });

                var availableTables = await _tableService.GetAvailableTablesAsync(checkDto);
                return Ok(new { success = true, data = availableTables });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking table availability");
                return StatusCode(500, new { success = false, message = "Internal server error" });
            }
        }
    }
}