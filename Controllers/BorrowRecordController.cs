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

        public BorrowRecordController(IBorrowRecordService borrowRecordService)
        {
            _borrowRecordService = borrowRecordService;
        }

        // GET: api/BorrowRecord
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BorrowRecordDTO>>> GetMyBorrowRecords()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            var userRecords = await _borrowRecordService.GetBorrowRecordsByUserIdAsync(userId);
            return Ok(userRecords);
        }

        // GET: api/BorrowRecord/admin/all
        [HttpGet("admin/all")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<BorrowRecordDTO>>> GetAllBorrowRecords()
        {
            var allRecords = await _borrowRecordService.GetAllAsync();
            return Ok(allRecords);
        }

        // GET: api/BorrowRecord/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<BorrowRecordDTO>> GetBorrowRecordById(int id)
        {
            var record = await _borrowRecordService.GetByIdAsync(id);
            if (record == null)
                return NotFound();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!User.IsInRole("Admin") && record.UserId != userId)
                return Forbid();

            return Ok(record);
        }

        // Post: api/BorrowRecord/borrow/{bookId}
        // Removed Role requirement to allow custom error message from Service
        [HttpPost("borrow/{bookId}")]
        public async Task<ActionResult<BorrowRecordDTO>> BorrowBook(int bookId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            // The Service will check for Subscription and throw an error message if invalid
            var record = await _borrowRecordService.BorrowBookAsync(userId, bookId);
            return Ok(record);
        }

        // Return: api/BorrowRecord/return/5
        // Allow BOTH Admin and the Owner to return the book
        [HttpDelete("return/{borrowRecordId}")]
        public async Task<ActionResult<BorrowRecordDTO>> ReturnBook(int borrowRecordId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            var record = await _borrowRecordService.GetByIdAsync(borrowRecordId);
            if (record == null)
                return NotFound();

            // Permission Check: Must be Admin OR the person who borrowed it
            if (!User.IsInRole("Admin") && record.UserId != userId)
                return Forbid();

            var updatedRecord = await _borrowRecordService.ReturnBookAsync(borrowRecordId);
            return Ok(updatedRecord);
        }
    }
}
