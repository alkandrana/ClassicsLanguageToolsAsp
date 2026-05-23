using System.ComponentModel.DataAnnotations;

namespace ClassicsLanguageToolsAsp.Models;

public class Vocab
{
    public int Id { get; set; }
    [Required, StringLength(255)] public string Lemma { get; set; } = "";
    [Required, StringLength(255)] public string Definition { get; set; } = "";
    [Required] public Language Language { get; set; } = new Language();
    [Required, StringLength(255)] public string PartOfSpeech { get; set; } = String.Empty;
    [Required] public User Creator { get; set; } = new User();
    
}