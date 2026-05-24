using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace ClassicsLanguageToolsAsp.Models;

public class Comment
{
    public int Id { get; set; }
    [Required, StringLength(255)] public string Citation { get; set; } = "";
    [Required, StringLength(255)] public string Note { get; set; } = "";
    [Required, StringLength(255)] public string Reference { get; set; } = "";
    public IdentityUser? Creator { get; set; }
}