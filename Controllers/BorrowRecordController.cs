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
        // Returns ONLY the logged-in user's borrow history (User or Admin)
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BorrowRecordDTO>>> GetMyBorrowRecords()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            var userRecords = await _borrowRecordService.GetBorrowRecordsByUserIdAsync(userId);
            return Ok(userRecords);
        }

        // GET: api/BorrowRecord/admin/all
        // Admin only: View all borrowing history in the system
        [HttpGet("admin/all")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<BorrowRecordDTO>>> GetAllBorrowRecords()
        {
            var allRecords = await _borrowRecordService.GetAllAsync();
            return Ok(allRecords);
        }

        // GET: api/BorrowRecord/user/{userId}
        [HttpGet("user/{userId}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<BorrowRecordDTO>>> GetUserBorrowRecords(string userId)
        {
            var records = await _borrowRecordService.GetBorrowRecordsByUserIdAsync(userId);
            return Ok(records);
        }

        // GET: api/BorrowRecord/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<BorrowRecordDTO>> GetBorrowRecordById(int id)
        {
            var record = await _borrowRecordService.GetByIdAsync(id);
            if (record == null)
                return NotFound();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Users can only see their own specific record; Admins see any.
            if (!User.IsInRole("Admin") && record.UserId != userId)
                return Forbid();

            return Ok(record);
        }

        // Post: api/BorrowRecord/borrow/{bookId}
        [HttpPost("borrow/{bookId}")]
        [Authorize(Roles = "Member")]
        public async Task<ActionResult<BorrowRecordDTO>> BorrowBook(int bookId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            var record = await _borrowRecordService.BorrowBookAsync(userId, bookId);
            return Ok(record);
        }

        // Return: api/BorrowRecord/return/5
        [HttpDelete("return/{borrowRecordId}")]
        public async Task<ActionResult<BorrowRecordDTO>> ReturnBook(int borrowRecordId)
        {
            var updatedRecord = await _borrowRecordService.ReturnBookAsync(borrowRecordId);
            return Ok(updatedRecord);
        }
    }
}
