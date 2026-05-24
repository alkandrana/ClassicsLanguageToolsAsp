using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ClassicsLanguageToolsAsp.Data;
using ClassicsLanguageToolsAsp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.JsonPatch;

namespace ClassicsLanguageToolsAsp.Controllers
{
    [Route("comments")]
    [ApiController]
    public class CommentController : ControllerBase
    {
        private readonly AppDbContext _ctx;
        private readonly UserManager<IdentityUser> _userManager;

        public CommentController(AppDbContext ctx, UserManager<IdentityUser> userMng)
        {
            _ctx = ctx;
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
            List<Comment> comments = await _ctx.Comments
                .Where(c => c.Creator.Id == currentUserId)
                .ToListAsync();
            return Ok(comments);
        }

        // GET: comments/5
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetComment(int id)
        {
            Comment? comment = await _ctx.Comments
                .Include(c => c.Creator)
                .SingleOrDefaultAsync(c => c.Id == id);
            if (comment == null)
            {
                return NotFound();
            }
            string? currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (currentUserId == null || comment.Creator?.Id != currentUserId)
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
            IdentityUser? currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return Unauthorized();
            }
            comment.Creator = currentUser;
            _ctx.Comments.Add(comment);
            await _ctx.SaveChangesAsync();

            return CreatedAtAction("GetComment", new { id = comment.Id }, comment);
        }
        
        // PATCH: comments/{id}
        [HttpPatch("{id:int}")]
        public async Task<IActionResult> EditComment([FromRoute] int id, [FromBody] JsonPatchDocument<Comment> commentData)
        {
            if (commentData == null)
            {
                return BadRequest();
            }
            Comment? comment = await _ctx.Comments
                .Include(c => c.Creator)
                .SingleOrDefaultAsync(i => i.Id == id);
            if (comment == null)
            {
                return NotFound();
            }
            string? currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (currentUserId == null || comment.Creator?.Id != currentUserId)
            {
                return Unauthorized();
            }
            commentData.ApplyTo(comment, ModelState);
            if (!TryValidateModel(comment) || !ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _ctx.SaveChangesAsync();
            return Ok(comment);
        }

        // DELETE: comments/5
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteComment(int id)
        {
            Comment? comment = await _ctx.Comments
                .Include(c => c.Creator)
                .SingleOrDefaultAsync(c => c.Id == id);
            if (comment == null)
            {
                return NotFound();
            }
            string? currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (currentUserId == null || comment.Creator?.Id != currentUserId)
            {
                return Unauthorized();
            }

            _ctx.Comments.Remove(comment);
            await _ctx.SaveChangesAsync();

            return NoContent();
        }
    }
}
