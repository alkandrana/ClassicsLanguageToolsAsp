using Microsoft.AspNetCore.Identity;

namespace ClassicsLanguageToolsAsp.Models;

public class Comment
{
    public int Id { get; set; }
    public string Citation { get; set; } = "";
    public string Note { get; set; } = "";
    public IdentityUser? Creator { get; set; }
}