using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LibraryManagementSystem.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using LibraryManagementSystem.DTOs;
using Microsoft.Extensions.Logging;

namespace BackendApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
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
        public async Task<ActionResult<IEnumerable<BorrowRecordDTO>>> GetBorrowRecords()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if(User.IsInRole("Admin"))
                {
                    var allRecords = await _borrowRecordService.GetAllAsync();
                    return Ok(allRecords);
                }else
                {
                    var userRecords = await _borrowRecordService.GetBorrowRecordsByUserIdAsync(userId);
                    return Ok(userRecords);
                }
            }
            catch(Exception ex)
            {
                _logger.LogError($"Error getting borrow records: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        // GET: api/BorrowRecord/user/{userId}
        [HttpGet("user/{userId}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<BorrowRecordDTO>>> GetUserBorrowRecords(string userId)
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

        // GET: api/BorrowRecord/id
        [HttpGet("id")]
        public async Task<ActionResult<BorrowRecordDTO>> GetBorrowRecordById(int id)
        {
            try
            {
                var record = await _borrowRecordService.GetByIdAsync(id);
                if (record == null)
                    return NotFound();

                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (!User.IsInRole("Admin") && record.UserId != userId)
                    return Forbid();

                return Ok(record);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving borrow record {id}");
                return StatusCode(500, "Internal server error");
            }
        }

        //Post: api/BorrowRecord/borrow/{bookId}
        [HttpPost("borrow/{bookId}")]
        [Authorize(Roles = "Member")]
        public async Task<ActionResult<BorrowRecordDTO>> BorrowBook(int bookId)
        {
           var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            try
            {
                var record = await _borrowRecordService.BorrowBookAsync(userId, bookId);
                _logger.LogInformation($"User {userId} borrowed book {bookId}");
                return Ok(record);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error borrowing book {bookId} for user {userId}");
                return BadRequest(new { message = ex.Message });
            }
        }

        // Return: api/BorrowRecord/return/5
        [HttpDelete("return/{borrowRecordId}")]
        public async Task<ActionResult<BorrowRecordDTO>> ReturnBook(int borrowRecordId)
        {
           var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            try
            {
                var record = await _borrowRecordService.GetByIdAsync(borrowRecordId);
                if (record == null)
                    return NotFound();

                if (!User.IsInRole("Admin") && record.UserId != userId)
                    return Forbid();

                var updatedRecord = await _borrowRecordService.ReturnBookAsync(borrowRecordId);
                _logger.LogInformation($"User {userId} returned book {updatedRecord.BookId} (BorrowRecordId: {borrowRecordId})");

                return Ok(updatedRecord);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error returning book for BorrowRecordId {borrowRecordId}");
                return BadRequest(new { message = ex.Message });
            }
        }

    }
}
