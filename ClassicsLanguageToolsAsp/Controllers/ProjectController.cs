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
    [Route("projects")]
    [ApiController]
    public class ProjectController : ControllerBase
    {
        private readonly AppDbContext _ctx;
        private readonly UserManager<IdentityUser> _userManager;

        public ProjectController(AppDbContext ctx, UserManager<IdentityUser> userManager)
        {
            _ctx = ctx;
            _userManager = userManager;
        }

        // GET: projects
        [HttpGet]
        public async Task<IActionResult> GetProjects()
        {
            string? currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (currentUserId == null)
            {
                return Unauthorized();
            }
            List<Project> projects = await _ctx.Projects
                .Where(p => p.Creator.Id == currentUserId)
                .ToListAsync();
            return Ok(projects);
        }

        // GET: projects/5
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetProject(int id)
        {
            Project? project = await _ctx.Projects
                .Include(p => p.VocabList).ThenInclude(i => i.Vocab)
                .SingleOrDefaultAsync(p => p.Id == id);
            if (project == null)
            {
                return NotFound();
            }
            string projectUserId = await _ctx.Projects.Where(p => p.Id == id).Select(p => p.Creator.Id).SingleOrDefaultAsync();
            string? currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (currentUserId == null || currentUserId != projectUserId)
            {
                return Unauthorized();
            }

            return Ok(project);
        }

        // POST: projects
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<IActionResult> AddProject(Project project)
        {
            IdentityUser? currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return Unauthorized();
            }
            project.Creator = currentUser;
            project.StartDate = DateTime.Now;
            _ctx.Projects.Add(project);
            await _ctx.SaveChangesAsync();

            return CreatedAtAction("GetProject", new { id = project.Id }, project);
        }
        
        // PATCH: projects/{id}
        [HttpPatch("{id:int}")]
        public async Task<IActionResult> EditVocab([FromRoute] int id, [FromBody] JsonPatchDocument<Project> projectData)
        {
            if (projectData == null)
            {
                return BadRequest();
            }
            Project? project = await _ctx.Projects
                .Include(p => p.Creator)
                .SingleOrDefaultAsync(p => p.Id == id);
            if (project == null)
            {
                return NotFound();
            }
            string? currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (currentUserId == null)
            {
                return Unauthorized();
            }
            if (currentUserId != project.Creator?.Id)
            {
                return Forbid();
            }
            projectData.ApplyTo(project, ModelState);
            if (!TryValidateModel(project) || !ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _ctx.SaveChangesAsync();
            return Ok(project);
        }

        // DELETE: projects/5
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteProject(int id)
        {
            var project = await _ctx.Projects
                .Include(p => p.Creator)
                .SingleOrDefaultAsync(p => p.Id == id);
            if (project == null)
            {
                return NotFound();
            }
            string? currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (currentUserId == null)
            {
                return Unauthorized();
            }

            if (currentUserId != project.Creator?.Id)
            {
                return Forbid();
            }
            _ctx.Projects.Remove(project);
            await _ctx.SaveChangesAsync();

            return NoContent();
        }
    }
}
