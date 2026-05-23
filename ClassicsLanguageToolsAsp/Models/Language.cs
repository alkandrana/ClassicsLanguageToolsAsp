using System.ComponentModel.DataAnnotations;

namespace ClassicsLanguageToolsAsp.Models;

public class Language
{
    public int Id { get; set; }
    [Required, StringLength(50)] public string Name { get; set; } = "";
}