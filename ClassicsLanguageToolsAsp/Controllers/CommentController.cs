using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using ClassicsLanguageToolsAsp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.EntityFrameworkCore;
using WritingSessionsAspApi.Data.Contracts;

namespace ClassicsLanguageToolsAsp.Controllers
{
    [Route("comments")]
    [ApiController]
    public class CommentController : ControllerBase
    { 
        private readonly IRecordRepo<Comment> _commentRepo;
        private readonly UserManager<ClassUser> _userManager;

        public CommentController(IRecordRepo<Comment> comRepo, UserManager<ClassUser> userMng)
        {
            _commentRepo = comRepo;
            _userManager = userMng;
        }

        // GET: comments
        [HttpGet]
        public async Task<IActionResult> GetComments()
        {
            string? currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (currentUserId == null)
            {
                return Unauthorized();
            }

            List<Comment> comments = await _commentRepo.
                GetSelectRecordsAsync(
                    c => c.CreatorId == currentUserId);
            return Ok(comments);
        }

        // GET: comments/5
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetComment(int id)
        {
            Comment? comment = await _commentRepo
                .GetRecordByIdAsync(id, qu => qu.Include(c => c.Creator));
            if (comment == null)
            {
                return NotFound();
            }
            ClassUser? currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null || comment.CreatorId != currentUser.Id)
            {
                return Unauthorized();
            }
            
            return Ok(comment);
        }

        // POST: api/Comment
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<IActionResult> AddComment(Comment comment)
        {
            ClassUser? currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return Unauthorized();
            }
            comment.Creator = currentUser;
            int rowsAffected = await _commentRepo.CreateRecordAsync(comment);
            if (rowsAffected == 0)
            {
                return Problem("Failed to create comment.");
            }
            return CreatedAtAction("GetComment", new { id = comment.Id }, comment);
        }
        
        // PATCH: comments/{id}
        [HttpPatch("{id:int}")]
        public async Task<IActionResult> EditComment([FromRoute] int id,
            [FromBody] JsonPatchDocument<Comment> commentData)
        {
            Comment? commentToUpdate = await _commentRepo.GetRecordByIdAsync(id);
            if (commentToUpdate == null)
            {
                return NotFound();
            }

            ClassUser? currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null || commentToUpdate.CreatorId != currentUser.Id)
            {
                return Unauthorized();
            }

            commentData.ApplyTo(commentToUpdate, ModelState);
            if (!TryValidateModel(commentToUpdate) || !ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            int rowsAffected = await _commentRepo.UpdateRecordAsync(commentToUpdate);
            if (rowsAffected == 0)
            {
                return Problem("Failed to update comment.");
            }
        return NoContent();
        }

        // DELETE: comments/5
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteComment(int id)
        {
            Comment? comment = await _commentRepo.GetRecordByIdAsync(id);
            if (comment == null)
            {
                return NotFound();
            }
            ClassUser? currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null || comment.CreatorId != currentUser.Id)
            {
                return Unauthorized();
            }

            int rowsAffected = await _commentRepo.DeleteRecordAsync(comment);
            if (rowsAffected == 0)
            {
                return Problem("Failed to delete comment.");
            }

            return NoContent();
        }
    }
}
