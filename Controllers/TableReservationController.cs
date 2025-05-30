using Microsoft.AspNetCore.Mvc;
using backendcafe.DTO;
using backendcafe.Services;
using System.ComponentModel.DataAnnotations;

namespace backendcafe.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TableReservationController : ControllerBase
    {
        private readonly ITableReservationService _reservationService;
        private readonly ILogger<TableReservationController> _logger;

        public TableReservationController(ITableReservationService reservationService, ILogger<TableReservationController> logger)
        {
            _reservationService = reservationService;
            _logger = logger;
        }

        /// <summary>
        /// Get all reservations
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TableReservationReadDTO>>> GetAllReservations()
        {
            try
            {
                var reservations = await _reservationService.GetAllReservationsAsync();
                return Ok(new { success = true, data = reservations });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all reservations");
                return StatusCode(500, new { success = false, message = "Internal server error" });
            }
        }

        /// <summary>
        /// Get reservations by branch
        /// </summary>
        [HttpGet("branch/{branchId}")]
        public async Task<ActionResult<IEnumerable<TableReservationReadDTO>>> GetReservationsByBranch(int branchId)
        {
            try
            {
                var reservations = await _reservationService.GetReservationsByBranchAsync(branchId);
                return Ok(new { success = true, data = reservations });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting reservations for branch {BranchId}", branchId);
                return StatusCode(500, new { success = false, message = "Internal server error" });
            }
        }

        /// <summary>
        /// Get reservations by customer
        /// </summary>
        [HttpGet("customer")]
        public async Task<ActionResult<IEnumerable<TableReservationReadDTO>>> GetReservationsByCustomer(
            [FromQuery] string customerName, 
            [FromQuery] string? customerPhone = null)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(customerName))
                    return BadRequest(new { success = false, message = "Customer name is required" });

                var reservations = await _reservationService.GetReservationsByCustomerAsync(customerName, customerPhone);
                return Ok(new { success = true, data = reservations });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting reservations for customer {CustomerName}", customerName);
                return StatusCode(500, new { success = false, message = "Internal server error" });
            }
        }

        /// <summary>
        /// Get reservation by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<TableReservationReadDTO>> GetReservationById(int id)
        {
            try
            {
                var reservation = await _reservationService.GetReservationByIdAsync(id);
                return Ok(new { success = true, data = reservation });
            }
            catch (Exception ex) when (ex.Message == "Reservation not found")
            {
                return NotFound(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting reservation {Id}", id);
                return StatusCode(500, new { success = false, message = "Internal server error" });
            }
        }

        /// <summary>
        /// Get reservation by code
        /// </summary>
        [HttpGet("code/{reservationCode}")]
        public async Task<ActionResult<TableReservationReadDTO>> GetReservationByCode(string reservationCode)
        {
            try
            {
                var reservation = await _reservationService.GetReservationByCodeAsync(reservationCode);
                return Ok(new { success = true, data = reservation });
            }
            catch (Exception ex) when (ex.Message == "Reservation not found")
            {
                return NotFound(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting reservation with code {Code}", reservationCode);
                return StatusCode(500, new { success = false, message = "Internal server error" });
            }
        }

        /// <summary>
        /// Create new reservation
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<TableReservationReadDTO>> CreateReservation([FromBody] TableReservationCreateDTO reservationDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(new { success = false, message = "Invalid input data", errors = ModelState });

                var reservation = await _reservationService.CreateReservationAsync(reservationDto);
                return CreatedAtAction(nameof(GetReservationById), new { id = reservation.Id }, 
                    new { success = true, data = reservation });
            }
            catch (Exception ex) when (ex.Message.Contains("not found") || ex.Message.Contains("not available") || 
                                     ex.Message.Contains("insufficient") || ex.Message.Contains("already reserved"))
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating reservation");
                return StatusCode(500, new { success = false, message = "Internal server error" });
            }
        }

        /// <summary>
        /// Update reservation
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult<TableReservationReadDTO>> UpdateReservation(int id, [FromBody] TableReservationUpdateDTO reservationDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(new { success = false, message = "Invalid input data", errors = ModelState });

                var reservation = await _reservationService.UpdateReservationAsync(id, reservationDto);
                return Ok(new { success = true, data = reservation });
            }
            catch (Exception ex) when (ex.Message == "Reservation not found")
            {
                return NotFound(new { success = false, message = ex.Message });
            }
            catch (Exception ex) when (ex.Message.Contains("Cannot update") || ex.Message.Contains("already reserved") || 
                                     ex.Message.Contains("insufficient"))
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating reservation {Id}", id);
                return StatusCode(500, new { success = false, message = "Internal server error" });
            }
        }

        /// <summary>
        /// Delete reservation
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteReservation(int id)
        {
            try
            {
                var deleteDto = new TableReservationDeleteDTO { Id = id };
                var deleted = await _reservationService.DeleteReservationAsync(deleteDto);
                
                if (!deleted)
                    return NotFound(new { success = false, message = "Reservation not found" });

                return Ok(new { success = true, message = "Reservation deleted successfully" });
            }
            catch (Exception ex) when (ex.Message.Contains("Cannot delete"))
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting reservation {Id}", id);
                return StatusCode(500, new { success = false, message = "Internal server error" });
            }
        }

        /// <summary>
        /// Check table availability
        /// </summary>
        [HttpPost("check-availability")]
        public async Task<ActionResult<IEnumerable<TableReadDTO>>> CheckTableAvailability([FromBody] TableAvailabilityCheckDTO checkDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(new { success = false, message = "Invalid input data", errors = ModelState });

                var availableTables = await _reservationService.CheckTableAvailabilityAsync(checkDto);
                return Ok(new { success = true, data = availableTables });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking table availability");
                return StatusCode(500, new { success = false, message = "Internal server error" });
            }
        }

        /// <summary>
        /// Confirm reservation
        /// </summary>
        [HttpPost("{id}/confirm")]
        public async Task<ActionResult<TableReservationReadDTO>> ConfirmReservation(int id)
        {
            try
            {
                var reservation = await _reservationService.ConfirmReservationAsync(id);
                return Ok(new { success = true, data = reservation, message = "Reservation confirmed successfully" });
            }
            catch (Exception ex) when (ex.Message == "Reservation not found")
            {
                return NotFound(new { success = false, message = ex.Message });
            }
            catch (Exception ex) when (ex.Message.Contains("Cannot change status"))
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error confirming reservation {Id}", id);
                return StatusCode(500, new { success = false, message = "Internal server error" });
            }
        }

        /// <summary>
        /// Check-in reservation
        /// </summary>
        [HttpPost("{id}/checkin")]
        public async Task<ActionResult<TableReservationReadDTO>> CheckInReservation(int id)
        {
            try
            {
                var reservation = await _reservationService.CheckInReservationAsync(id);
                return Ok(new { success = true, data = reservation, message = "Customer checked in successfully" });
            }
            catch (Exception ex) when (ex.Message == "Reservation not found")
            {
                return NotFound(new { success = false, message = ex.Message });
            }
            catch (Exception ex) when (ex.Message.Contains("Cannot change status"))
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking in reservation {Id}", id);
                return StatusCode(500, new { success = false, message = "Internal server error" });
            }
        }

        /// <summary>
        /// Complete reservation
        /// </summary>
        [HttpPost("{id}/complete")]
        public async Task<ActionResult<TableReservationReadDTO>> CompleteReservation(int id)
        {
            try
            {
                var reservation = await _reservationService.CompleteReservationAsync(id);
                return Ok(new { success = true, data = reservation, message = "Reservation completed successfully" });
            }
            catch (Exception ex) when (ex.Message == "Reservation not found")
            {
                return NotFound(new { success = false, message = ex.Message });
            }
            catch (Exception ex) when (ex.Message.Contains("Cannot change status"))
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error completing reservation {Id}", id);
                return StatusCode(500, new { success = false, message = "Internal server error" });
            }
        }

        /// <summary>
        /// Cancel reservation
        /// </summary>
        [HttpPost("{id}/cancel")]
        public async Task<ActionResult<TableReservationReadDTO>> CancelReservation(int id)
        {
            try
            {
                var reservation = await _reservationService.CancelReservationAsync(id);
                return Ok(new { success = true, data = reservation, message = "Reservation cancelled successfully" });
            }
            catch (Exception ex) when (ex.Message == "Reservation not found")
            {
                return NotFound(new { success = false, message = ex.Message });
            }
            catch (Exception ex) when (ex.Message.Contains("Cannot change status"))
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error cancelling reservation {Id}", id);
                return StatusCode(500, new { success = false, message = "Internal server error" });
            }
        }

        /// <summary>
        /// Get today's reservations for a branch
        /// </summary>
        [HttpGet("branch/{branchId}/today")]
        public async Task<ActionResult<IEnumerable<TableReservationReadDTO>>> GetTodayReservations(int branchId)
        {
            try
            {
                var reservations = await _reservationService.GetTodayReservationsAsync(branchId);
                return Ok(new { success = true, data = reservations });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting today's reservations for branch {BranchId}", branchId);
                return StatusCode(500, new { success = false, message = "Internal server error" });
            }
        }

        /// <summary>
        /// Get upcoming reservations for a branch
        /// </summary>
        [HttpGet("branch/{branchId}/upcoming")]
        public async Task<ActionResult<IEnumerable<TableReservationReadDTO>>> GetUpcomingReservations(int branchId, [FromQuery] int days = 7)
        {
            try
            {
                var reservations = await _reservationService.GetUpcomingReservationsAsync(branchId, days);
                return Ok(new { success = true, data = reservations });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting upcoming reservations for branch {BranchId}", branchId);
                return StatusCode(500, new { success = false, message = "Internal server error" });
            }
        }
    }
}