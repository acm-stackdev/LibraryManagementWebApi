using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LibraryManagementSystem.Models;

namespace BackendApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BorrowRecordController : ControllerBase
    {
        private readonly IBorrowRecordService _borrowRecordService;
        private readonly ILogger<BorrowRecordController> _logger;

        public BorrowRecordController(IBorrowRecordService borrowRecordService, ILogger<BorrowRecordController> logger)
        {
            _borrowRecordService = borrowRecordService;
            _logger = logger;
        }

        // GET: api/BorrowRecord
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BorrowRecord>>> GetBorrowRecords()
        {
            try
            {
                var records = await _borrowRecordService.GetAllAsync();
                return Ok(records);
            }
            catch(Exception ex)
            {
                _logger.LogError($"Error getting borrow records: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        // GET: api/BorrowRecord/user/{userId}
        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<BorrowRecord>> >GetUserBorrowHistory(string userId)
        {
            try
            {
                var records = await _borrowRecordService.GetBorrowRecordsByUserIdAsync(userId);
                return Ok(records);
            }
            catch(Exception ex)
            {
                _logger.LogError($"Error getting borrow records for user {userId}: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        // Borrow: api/BorrowRecord/borrow/{userId}/{bookId}

        [HttpPost("borrow/{userId}/{bookId}")]
        public async Task<ActionResult<BorrowRecord>> BorrowBook(string userId, int bookId)
        {
           try{
            var record = await _borrowRecordService.BorrowBookAsync(userId, bookId);
            _logger.LogInformation($"User {userId} borrowed book {bookId} successfully.");
            return Ok(record);
           }
           catch(Exception ex){
            _logger.LogError($"Error borrowing book: {ex.Message}");
            return StatusCode(500, "Internal server error");
           }
        }

        // Return: api/BorrowRecord/return/5
        [HttpDelete("return/{id}")]
        public async Task<IActionResult> ReturnBook(int id)
        {
            try
            {
                await _borrowRecordService.ReturnBookAsync(id);
                _logger.LogInformation($"Book {id} returned successfully.");
                return NoContent();
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, $"Error returning book for record {id}");
                return StatusCode(500, "Internal server error");
            }
        }

    }
}
