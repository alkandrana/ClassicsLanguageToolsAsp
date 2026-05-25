using ClassicsLanguageToolsAsp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace ClassicsLanguageToolsAsp.Data;

// 2. Define your Database Context connection bridge
public class AppDbContext : IdentityDbContext<IdentityUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    public DbSet<Vocab> Vocab => Set<Vocab>();
    public DbSet<Language> Languages => Set<Language>();
    public DbSet<VocabInstance> Instances => Set<VocabInstance>();
    public DbSet<Comment> Comments => Set<Comment>();
    public DbSet<Project> Projects => Set<Project>();
}