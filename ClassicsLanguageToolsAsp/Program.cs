using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Identity.Web;
using Microsoft.Identity.Abstractions;
using Microsoft.Identity.Web.Resource;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using ClassicsLanguageToolsAsp.Data;

namespace ClassicsLanguageToolsAsp;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddIdentityApiEndpoints<IdentityUser>()
            .AddEntityFrameworkStores<AppDbContext>();

        builder.Services.AddAuthorization();
        // Add services to the container.
        // builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        //      .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAdB2C"));

        builder.Services.AddControllers().AddNewtonsoftJson();
        
        // Register your SQLite database connection service
        var connectionString = builder.Configuration
            .GetConnectionString("SqliteConnection");
        builder.Services.AddDbContext<AppDbContext>(options => 
            options.UseSqlite(connectionString));
        
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        // builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true).AddEntityFrameworkStores<AppDbContext>();

        var app = builder.Build();

        // Configure the HTTP request pipeline.

        

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }
        
        app.UseHttpsRedirection();
        app.UseAuthentication();
        app.UseAuthorization();
        
        app.MapControllers();
        
        app.MapIdentityApi<IdentityUser>();

        app.Run();
    }
}