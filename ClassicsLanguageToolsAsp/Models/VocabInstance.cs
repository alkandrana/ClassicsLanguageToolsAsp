using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace ClassicsLanguageToolsAsp.Models;

public class VocabInstance
{
    public int Id { get; set; }
    [Required, StringLength(50)] public string Instance { get; set; } = "";
    [Required, StringLength(255)] public string Form { get; set; } = "";
    [Required, StringLength(255)] public string Citation { get; set; } = "";
    [Required] public int VocabId { get; set; }
    public Vocab? Vocab { get; set; }
}