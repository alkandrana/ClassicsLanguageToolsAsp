using System.Security.Claims;
using ClassicsLanguageToolsAsp.Data;
using ClassicsLanguageToolsAsp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Web.Resource;

namespace ClassicsLanguageToolsAsp.Controllers;

[Authorize]
[ApiController]
[Route("vocab")]
public class VocabController : ControllerBase
{

    private readonly ILogger<VocabController> _logger;
    private AppDbContext _ctx;
    private UserManager<IdentityUser> _userManager;
    
    public VocabController(ILogger<VocabController> logger, AppDbContext ctx, UserManager<IdentityUser> userMng)
    {
        _logger = logger;
        _ctx = ctx;
        _userManager = userMng;
    }

    [HttpGet]
    public async Task<IActionResult> GetVocab()
    {
        Console.WriteLine("Fetching vocab lists: ");
        // IdentityUser? currentUser = await _userManager.GetUserAsync(User);
        string? currentId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        // Console.WriteLine("Current user: " + currentId + " " + currentUser.UserName);
        if (currentId == null)
        {
            return Unauthorized();
        }
        List<Vocab> vocab = await _ctx.Vocab.Where(v => v.Creator.Id == currentId).ToListAsync();
        return Ok(vocab);
    }

    [HttpGet]
    [Route("/vocab/language/{languageId}")]
    public async Task<IActionResult> GetVocabByLanguage(int languageId)
    {
        string? currentId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (currentId == null)
        {
            return Unauthorized();
        }
        List<Vocab> vocab = await _ctx.Vocab
            .Where(v => v.Language.Id == languageId && 
                        v.Creator.Id == currentId)
            .ToListAsync();
        return Ok(vocab);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetOneVocab(int id)
    {
        string? currentId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (currentId == null)
        {
            return NotFound();
        }
        Vocab? vocab = await _ctx.Vocab.FindAsync(id);
        if (vocab == null)
        {
            return NotFound();
        } 
        
        if (vocab.Creator.Id != currentId)
        {
            return Unauthorized();
        }
        return Ok(vocab);
    }

    [HttpPost]
    public async Task<IActionResult> AddVocab([FromBody] Vocab newVocab)
    {
        IdentityUser? currentUser = await _userManager.GetUserAsync(User);
        //ClassUser? currentUser = (ClassUser?)currentIdentUser;
        if (currentUser == null)
        {
            return NotFound();
        }
        newVocab.Creator = currentUser;
        _ctx.Vocab.Add(newVocab);
        await _ctx.SaveChangesAsync();
        return CreatedAtAction(nameof(GetOneVocab), 
            new { id = newVocab.Id }, newVocab);
    }

    [HttpPatch("{id}")]
    public async Task<IActionResult> EditVocab([FromRoute] int id, [FromBody] JsonPatchDocument<Vocab> vocabData)
    {
        if (vocabData == null)
        {
            return BadRequest();
        }
        Vocab vocab = await _ctx.Vocab.FindAsync(id);
        if (vocab == null)
        {
            return NotFound();
        }
        vocabData.ApplyTo(vocab, ModelState);
        if (!TryValidateModel(vocab) || !ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        await _ctx.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteVocab(int id)
    {
        string? currentUser = User.Identity.Name;
        if (currentUser == null)
        {
            return NotFound();
        }
        
        Vocab vocab = await _ctx.Vocab.FindAsync(id);
        if (vocab == null)
        {
            return NotFound();
        }

        // if (vocab.Creator.Id != currentUser.Id)
        // {
        //     return Unauthorized();
        // }
        
        _ctx.Vocab.Remove(vocab);
        await _ctx.SaveChangesAsync();
        
        return NoContent();
    }
}

