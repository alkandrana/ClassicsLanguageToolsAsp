using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace ClassicsLanguageToolsAsp.Models;

[Index(nameof(Name), IsUnique = true)]
public class Language
{
    public int Id { get; set; }
    [Required] public string Name { get; set; }
}