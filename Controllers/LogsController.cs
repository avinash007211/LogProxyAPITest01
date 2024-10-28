using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using LogProxyAPI.Data;
using LogProxyAPI.Models;

namespace LogProxyAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class LogsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly int GetLogById;

        public LogsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("messages")]
        public async Task<IActionResult> CreateLog([FromBody] LogRequest request)
        {
            var log = new Log
            {
                Title = request.Title,
                Text = request.Text,
                UserId = request.Properties.UserId,
                Module = request.Properties.Module,
                Severity = request.Properties.Severity,
                ReceivedAt = DateTime.UtcNow
            };

            _context.Logs.Add(log);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetLogById), new { id = log.Id }, log);
        }

        /// <summary>
        /// Get All messages 
        /// </summary>        
        /// <returns>All logs.</returns>
        [HttpGet("messages")]
        public async Task<IActionResult> GetLogs()
        {
            var logs = await _context.Logs.ToListAsync();
            return Ok(logs);
        }

        /// <summary>
        /// Deletes a log entry by ID.
        /// </summary>
        /// <param name="id">The ID of the log entry to delete.</param>
        /// <returns>NoContent if successful, NotFound if the log entry does not exist.</returns>
        [HttpDelete("messages/{id}")]
        public async Task<IActionResult> DeleteLog(int id)
        {
            var log = await _context.Logs.FindAsync(id);
            if (log == null)
            {
                return NotFound(new { Message = "Log entry not found" });
            }

            _context.Logs.Remove(log);
            await _context.SaveChangesAsync();

            return NoContent();
        }

       
    }
}
