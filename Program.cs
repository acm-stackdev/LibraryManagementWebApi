using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using LibraryManagementSystem.Models;
using DotNetEnv;
using BackendApi.Services;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;

Env.Load();

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRateLimiter(options =>
{
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 10,              // Limiting to 10 requests
                Window = TimeSpan.FromMinutes(1), // for 1 minute
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                QueueLimit = 2
            }
        )
    );
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();
builder.Services.AddScoped<IBookService, BookService>();
builder.Services.AddScoped<IAuthorService, AuthorService>();
builder.Services.AddScoped<IBookAuthorService, BookAuthorService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IBorrowRecordService, BorrowRecordService>();
builder.Services.AddScoped<IWishlistService, WishlistService>();
builder.Services.AddScoped<ISubscriptionService, SubscriptionService>();
builder.Services.Configure<EmailSettings>(options =>
{
    options.SmtpServer = builder.Configuration["EmailSettings:SmtpServer"];
    options.SmtpPort = int.Parse(builder.Configuration["EmailSettings:SmtpPort"]);
    options.SmtpUsername = Environment.GetEnvironmentVariable("SMTP_USERNAME") ?? "";
    options.SmtpPassword = Environment.GetEnvironmentVariable("SMTP_PASSWORD") ?? "";
});
builder.Services.AddScoped<EmailService>();



var connectionString = builder.Configuration.GetConnectionString("Connection");

if (Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") == "true")
{
    connectionString = "Data Source=/app/library.db";
}


builder.Services.AddDbContext<LibraryContext>(options =>
    options.UseSqlite(connectionString));


builder.Services.AddIdentity<AppUser, IdentityRole>()
    .AddEntityFrameworkStores<LibraryContext>()
    .AddDefaultTokenProviders();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["JWT:Issuer"],
        ValidAudience = builder.Configuration["JWT:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["JWT:Key"]))
    };
});

var app = builder.Build();


// Create roles before running the app
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();

    string[] roles = { "Admin", "User", "Member"};


    //creating roles for the first time
    foreach (var role in roles)
    {
        if(!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole(role));
        }
    }

    //creating admin user for the first run
    var adminEmail = "admin@library.com";
    var adminUser = await userManager.FindByEmailAsync(adminEmail);
    if(adminUser == null)
    {
      var newAdmin = new AppUser
      {
        UserName = adminEmail,
        Email = adminEmail,
        Name = "Admin",
        EmailConfirmed = true,
      };

      var result = await userManager.CreateAsync(newAdmin, "Admin@1234");
      if(result.Succeeded)
      {
        await userManager.AddToRoleAsync(newAdmin, "Admin");
      }   
    }
}

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<LibraryContext>();

string[] categories = 
{ 
    "Fiction", 
    "Science", 
    "History", 
    "Technology", 
    "Philosophy" 
};

// creating categories for the first run
foreach (var category in categories)
{
    if (!context.Categories.Any(c => c.Name == category))
    {
        context.Categories.Add(new Category
        {
            Name = category
        });
    }
}

await context.SaveChangesAsync();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseMiddleware<GlobalExceptionHandlerMiddleware>();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();
