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
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.JsonPatch;

namespace ClassicsLanguageToolsAsp.Controllers
{
    [Route("instances")]
    [ApiController]
    [Authorize]
    public class InstanceController : ControllerBase
    {
        private readonly AppDbContext _ctx;
        private readonly UserManager<IdentityUser> _userManager;

        public InstanceController(AppDbContext ctx, UserManager<IdentityUser> userMng)
        {
            _ctx = ctx;
            _userManager = userMng;
        }

        // GET: instances
        [HttpGet]
        public async Task<IActionResult> GetInstances()
        {
            string? currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (currentUserId == null)
            {
                return Unauthorized();
            }
            List<VocabInstance> instances =  await _ctx.Instances
                .Where(v => v.Vocab.Creator.Id == currentUserId)
                .ToListAsync();
            return Ok(instances);
        }
        
        [HttpGet]
        [Route("/instances/project/{id:int}")]
        public async Task<IActionResult> GetInstancesByProject(int id)
        {
            string? currentId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (currentId == null)
            {
                return Unauthorized();
            }

            List<VocabInstance> vocab = await _ctx.Instances
                .Where(i => i.Vocab.Creator.Id == currentId && i.ProjectId == id)
                .Include(i => i.Vocab).ToListAsync();
            return Ok(vocab);
        }

        // GET: instances/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetInstance(int id)
        {
            string? currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (currentUserId == null)
            {
                return Unauthorized();
            }
            VocabInstance? instance = await _ctx.Instances
                .Include(i => i.Vocab)
                .ThenInclude(v => v.Creator)
                .SingleOrDefaultAsync(v => v.Id == id);
            if (instance == null)
            {
                return NotFound();
            }

            if (instance.Vocab?.Creator?.Id != currentUserId)
            {
                return Unauthorized();
            }

            return Ok(instance);
        }

        // POST: instances
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<IActionResult> AddInstance(VocabInstance instance)
        {
            string? currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (currentUserId == null)
            {
                return Unauthorized();
            }
            Vocab? parent = await _ctx.Vocab.Include(v => v.Creator).SingleOrDefaultAsync(v => v.Id == instance.VocabId);
            if (parent == null)
            {
                return NotFound();
            }

            if (parent.Creator.Id != currentUserId)
            {
                return Unauthorized();
            }
            _ctx.Instances.Add(instance);
            await _ctx.SaveChangesAsync();

            return CreatedAtAction("GetInstance", new { id = instance.Id }, instance);
        }
        
        // PATCH: instances/{id}
        [HttpPatch("{id}")]
        public async Task<IActionResult> EditInstance([FromRoute] int id, [FromBody] JsonPatchDocument<VocabInstance> instanceData)
        {
            if (instanceData == null)
            {
                return BadRequest();
            }
            VocabInstance? instance = await _ctx.Instances
                .Include(i => i.Vocab)
                .ThenInclude(v => v.Creator)
                .SingleOrDefaultAsync(i => i.Id == id);
            if (instance == null)
            {
                return NotFound();
            }
            string? currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (currentUserId == null || instance.Vocab?.Creator?.Id != currentUserId)
            {
                return Unauthorized();
            }
            instanceData.ApplyTo(instance, ModelState);
            if (!TryValidateModel(instance) || !ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _ctx.SaveChangesAsync();
            return Ok(instance);
        }

        // DELETE: instances/5
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteInstance(int id)
        {
            VocabInstance? instance = await _ctx.Instances
                .Include(v => v.Vocab)
                .ThenInclude(v => v.Creator)
                .SingleOrDefaultAsync(v => v.Id == id);
            if (instance == null)
            {
                return NotFound();
            }
            string? currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (currentUserId == null || instance.Vocab?.Creator?.Id != currentUserId)
            {
                return Unauthorized();
            }
            
            _ctx.Instances.Remove(instance);
            await _ctx.SaveChangesAsync();

            return NoContent();
        }
    }
}
