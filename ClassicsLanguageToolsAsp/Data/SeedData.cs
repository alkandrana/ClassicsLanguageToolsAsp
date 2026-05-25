using Azure.Core;
using ClassicsLanguageToolsAsp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;

namespace ClassicsLanguageToolsAsp.Data;

public class SeedData
{
    public static void Seed(AppDbContext ctx)
    {
        if (!ctx.Projects.Any())
        {
            Language latin = new Language { Name = "Latin" };
            ctx.Languages.Add(latin);
            Language english = new Language { Name = "English" };
            ctx.Languages.Add(english);
            Language greek = new Language { Name = "Greek" };
            ctx.Languages.Add(greek);
            Language hebrew = new Language { Name = "Hebrew" };
            ctx.Languages.Add(hebrew);
            ctx.SaveChanges();
        }
    }
}