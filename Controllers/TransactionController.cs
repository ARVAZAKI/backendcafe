using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using backendcafe.Services;
using backendcafe.DTO;
using System;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;
namespace backendcafe.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionController : ControllerBase
    {
        private readonly ITransactionService _transactionService;

        public TransactionController(ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllTransactions()
        {
            try
            {
                var transactions = await _transactionService.GetAllTransactionsAsync();
                return Ok(transactions);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTransactionById(int id)
        {
            try
            {
                var transaction = await _transactionService.GetTransactionByIdAsync(id);
                return Ok(transaction);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("order/{orderId}")]
        public async Task<IActionResult> GetTransactionByOrderId(string orderId)
        {
            try
            {
                var transaction = await _transactionService.GetTransactionByOrderIdAsync(orderId);
                return Ok(transaction);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateTransaction([FromBody] TransactionCreateDTO transactionDto)
        {
            try
            {
                var result = await _transactionService.CreateTransactionWithCartItemsAsync(transactionDto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("payment")]
        public async Task<IActionResult> CreatePayment([FromBody] CreatePaymentRequestDTO paymentRequest)
        {
            try
            {
                var result = await _transactionService.CreatePaymentAsync(paymentRequest);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("notification")]
        public async Task<IActionResult> HandlePaymentNotification()
    {
        try
        {
            // Read raw request body
            string requestBody;
            using (var reader = new StreamReader(Request.Body))
            {
                requestBody = await reader.ReadToEndAsync();
            }

            // Deserialize manually to handle potential issues
            var notification = JsonSerializer.Deserialize<MidtransNotificationDTO>(requestBody, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (notification == null)
            {
                return BadRequest("Invalid notification format");
            }

            var result = await _transactionService.HandlePaymentNotificationAsync(notification);
            
            return Ok(new { message = "Notification processed successfully", transaction = result });
        }
        catch (JsonException jsonEx)
        {
            return Ok(new { message = "Notification received but format invalid" });
        }
        catch (Exception ex)
        {
            // Return OK to prevent Midtrans from retrying
            return Ok(new { message = "Notification received but processing failed" });
        }
    }

        [HttpPut("status/{id}")]
        public async Task<IActionResult> UpdateTransactionStatus(int id, [FromBody] TransactionStatusUpdateDTO statusUpdateDto)
        {
            try
            {
                var result = await _transactionService.UpdateTransactionStatusAsync(id, statusUpdateDto.Status);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}