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
        private readonly LibraryContext _context;

        public BorrowRecordController(LibraryContext context)
        {
            _context = context;
        }

        // GET: api/BorrowRecord
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BorrowRecord>>> GetBorrowRecords()
        {
            return await _context.BorrowRecords.ToListAsync();
        }

        // GET: api/BorrowRecord/5
        [HttpGet("{id}")]
        public async Task<ActionResult<BorrowRecord>> GetBorrowRecord(int id)
        {
            var borrowRecord = await _context.BorrowRecords.FindAsync(id);

            if (borrowRecord == null)
            {
                return NotFound();
            }

            return borrowRecord;
        }

        // PUT: api/BorrowRecord/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBorrowRecord(int id, BorrowRecord borrowRecord)
        {
            if (id != borrowRecord.BorrowRecordId)
            {
                return BadRequest();
            }

            _context.Entry(borrowRecord).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BorrowRecordExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/BorrowRecord
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<BorrowRecord>> PostBorrowRecord(BorrowRecord borrowRecord)
        {
            _context.BorrowRecords.Add(borrowRecord);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetBorrowRecord", new { id = borrowRecord.BorrowRecordId }, borrowRecord);
        }

        // DELETE: api/BorrowRecord/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBorrowRecord(int id)
        {
            var borrowRecord = await _context.BorrowRecords.FindAsync(id);
            if (borrowRecord == null)
            {
                return NotFound();
            }

            _context.BorrowRecords.Remove(borrowRecord);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool BorrowRecordExists(int id)
        {
            return _context.BorrowRecords.Any(e => e.BorrowRecordId == id);
        }
    }
}
