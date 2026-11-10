using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using ClassicsLanguageToolsAsp.Data;
using ClassicsLanguageToolsAsp.Models;
using ClassicsLanguageToolsAsp.Data.Contracts;

namespace ClassicsLanguageToolsAsp;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowSpecificOrigins", policy =>
            {
                policy.WithOrigins("http://localhost:5173").AllowAnyMethod().AllowAnyHeader();
            });
        });

        builder.Services.AddAuthorization();
        // Add services to the container.
        // builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        //      .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAdB2C"));

        builder.Services.AddControllers().AddNewtonsoftJson(options =>
        {
            options.SerializerSettings.ReferenceLoopHandling = 
                ReferenceLoopHandling.Ignore;
        });

        // Add repository services
        builder.Services.AddScoped(typeof(IRecordRepo<>), typeof(RecordRepo<>));
        
        // Register your SQLite database connection service
        var connectionString = builder.Configuration
            .GetConnectionString("SqliteConnection");
        builder.Services.AddDbContext<AppDbContext>(options => 
            options.UseSqlite(connectionString));
        
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Services.AddIdentityApiEndpoints<ClassUser>()
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();

        builder.Services.AddAuthorization();
        

        var app = builder.Build();

        // Configure the HTTP request pipeline.

        

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }
        
        app.UseHttpsRedirection();
        app.UseCors("AllowSpecificOrigins");
        app.UseAuthentication();
        app.UseAuthorization();
        
        app.MapControllers();
        
        app.MapIdentityApi<ClassUser>();
        using (var scope = app.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider
                .GetRequiredService<AppDbContext>();
            SeedData.Seed(dbContext);
        }
        app.Run();
    }
}