using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ClassicsLanguageToolsAsp.Controllers;
[ApiController]
[Route("[controller]")]
public class HomeController : Controller
{
    private readonly UserManager<IdentityUser> _userManager;

    public HomeController(UserManager<IdentityUser> userManager)
    {
        _userManager = userManager;
    }
    // GET
    [HttpGet]
    public IActionResult Index()
    {
        string message = "<h1>Welcome to the Classics Language Tools API</h1>" +
                         "<h2>Unprotected Routes: </h2>" +
                         "<p> /languages</p>" +
                         "<p>You can view the existing list of languages or add your own without an account.</p>" +
                         "<p>/register</p>" +
                         "<p>Create a new account here.</p>" +
                         "<p>/login</p>" +
                         "<p>Login to your account here. </p>" +
                         "<p>/refresh</p>" +
                         "Get a new access token when your current token expires.</p>" +
                         "<h2>Protected routes: </h2>" +
                         "You must have an account to access these routes." +
                         "<p>/vocab</p>" +
                         "<p>Add your vocab lists here.</p>" +
                         "<p>/instances</p>" +
                         "<p>Record of occurrences of a given vocab entry, associated with their corresponding work citation.</p>" +
                         "<p>/comments </p>" +
                         "<p>Commentary on a given line of a work.</p>";
        return Content(message, "text/html");
    }

    [Authorize]
    [HttpGet]
    [Route("/profile")]
    public async Task<IActionResult> GetCurrentUser()
    {
        IdentityUser? currentUser = await _userManager.GetUserAsync(User);
        if (currentUser == null)
        {
            return Unauthorized();
        }
        return Ok(currentUser);
    }
}