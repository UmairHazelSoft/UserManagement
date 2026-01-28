using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Net;
using System.Text;
using UserManagementSystem.DbContext;
using UserManagementSystem.DTOs;
using UserManagementSystem.Filters;
using UserManagementSystem.Helpers;
using UserManagementSystem.Middleware;
using UserManagementSystem.Models.Identity;
using UserManagementSystem.Repositories;
using UserManagementSystem.Sender.Email;
using UserManagementSystem.Services.AuthService;
using UserManagementSystem.Services.JwtService;
using UserManagementSystem.Services.UserService;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

//builder.Services.AddControllers();

builder.Services.AddControllers(options =>
{
    options.Filters.Add<ValidateModelAttribute>(); // Apply globally
}).ConfigureApiBehaviorOptions(options =>
{
    // Prevent automatic 400 responses
    options.SuppressModelStateInvalidFilter = true;
}); ;


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
AppSettings.Init(builder.Configuration);
// Add DbContext
//builder.Services.AddDbContext<ApplicationDbContext>(options =>
//    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(AppSettings.DefaultConnection));

// Configure Identity
builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
{
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.User.RequireUniqueEmail = true; // ensure email is unique
})
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// Add Authentication for JWT (optional)
//builder.Services.AddAuthentication(options =>
//{
//    options.DefaultAuthenticateScheme = "Bearer";
//    options.DefaultChallengeScheme = "Bearer";
//}).AddJwtBearer("Bearer", options =>
//{
//    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
//    {
//        ValidateIssuer = false,
//        ValidateAudience = false,
//        ValidateLifetime = true,
//        ValidateIssuerSigningKey = false,
//    };
//});

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = "Bearer";
    options.DefaultChallengeScheme = "Bearer";
})
.AddJwtBearer("Bearer", options =>
{
    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
    {
        //ValidateIssuer = false,
        //ValidateAudience = false,
        //ValidateLifetime = true,
        //ValidateIssuerSigningKey = false,

        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = AppSettings.JwtIssuer,
        ValidAudience = AppSettings.JwtAudience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(AppSettings.JwtKey))
    };

    options.Events = new Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerEvents
    {
        OnChallenge = async context =>
        {
            // ?? VERY IMPORTANT
            // Stops default 401 + WWW-Authenticate header
            context.HandleResponse();

            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            context.Response.ContentType = "application/json";

            var response = new ApiResponse<object>(
                null,
                false,
                "Unauthorized - Authentication required",
                HttpStatusCode.Unauthorized
            );

            await context.Response.WriteAsJsonAsync(response);
        },

        OnForbidden = async context =>
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            context.Response.ContentType = "application/json";

            var response = new ApiResponse<object>(
                null,
                false,
                "Forbidden - You do not have permission to access this resource",
                HttpStatusCode.Forbidden
            );

            await context.Response.WriteAsJsonAsync(response);
        }
    };
});


builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IEmailSender, EmailSender>(); // Implement for demo

builder.Services.AddAutoMapper(typeof(Program));
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IUserService, UserService>();




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

app.UseMiddleware<ApiResponseMiddleware>();


app.MapControllers();

app.Run();
