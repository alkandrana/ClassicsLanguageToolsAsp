using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace ClassicsLanguageToolsAsp.Models;

public class Vocab
{
    public int Id { get; set; }
    [Required, StringLength(255)] public string Lemma { get; set; } = "";
    [Required, StringLength(255)] public string Definition { get; set; } = "";
    [Required] public int LanguageId { get; set; } 
    public Language? Language { get; set; }
    [Required, StringLength(255)] public string PartOfSpeech { get; set; } = String.Empty;
    public IdentityUser? Creator { get; set; }
    
    public void PrintVocab()
    {
        Console.WriteLine("Vocab: ");
        Console.WriteLine(Id);
        Console.WriteLine(Lemma);
        Console.WriteLine(Definition);
        Console.WriteLine(PartOfSpeech);
        Console.WriteLine(Language);
        Console.WriteLine(Creator);
    }
}