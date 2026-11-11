using Microsoft.AspNetCore.Mvc;
using ClassicsLanguageToolsAsp.Models;
using Microsoft.AspNetCore.JsonPatch;
using ClassicsLanguageToolsAsp.Data.Contracts;

namespace ClassicsLanguageToolsAsp.Controllers
{
    [Route("lines")]
    [ApiController]
    public class LineController : ControllerBase
    {
        private readonly IRecordRepo<Line> _lineRepo;

        public LineController(IRecordRepo<Line> lineRepo)
        {
            _lineRepo = lineRepo;
        }

        // GET: lines
        [HttpGet]
        public async Task<IActionResult> GetLines()
        {
            List<Line> lines = await _lineRepo.GetAllRecordsAsync();
            return Ok(lines);
        }

        // GET: lines/5
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetLine(int id)
        {
            Line? line = await _lineRepo
                .GetRecordByIdAsync(id);
            if (line == null)
            {
                return NotFound();
            }

            return Ok(line);
        }

        // POST: lines
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<IActionResult> AddLine(Line line)
        {
            int rowsAffected = await _lineRepo.CreateRecordAsync(line);
            if (rowsAffected == 0)
            {
                return Problem("Failed to create line");
            }

            return CreatedAtAction("GetLine", new { id = line.Id }, line);
        }
        
        // PATCH: opera/{id}
        [HttpPatch("{id:int}")]
        public async Task<IActionResult> EditLine([FromRoute] int id, [FromBody] JsonPatchDocument<Line> lineData)
        {
            Line? lineToUpdate = await _lineRepo
                .GetRecordByIdAsync(id);
            if (lineToUpdate == null)
            {
                return NotFound();
            }
            lineData.ApplyTo(lineToUpdate, ModelState);
            if (!TryValidateModel(lineToUpdate) || !ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            int rowsAffected = await  _lineRepo.UpdateRecordAsync(lineToUpdate);
            if (rowsAffected == 0)
            {
                return Problem("Failed to update line");
            }
            return Ok(lineToUpdate);
        }

        // DELETE: opera/5
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteLine(int id)
        {
            Line? line = await _lineRepo.GetRecordByIdAsync(id);
            if (line == null)
            {
                return NotFound();
            }
            int rowsAffected = await _lineRepo.DeleteRecordAsync(line);
            if (rowsAffected == 0)
            {
                return Problem("Failed to delete line");
            }
            return NoContent();
        }
    }
}
