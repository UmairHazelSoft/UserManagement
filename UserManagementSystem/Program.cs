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
using UserManagementSystem.Repositories.GenericRepository;
using UserManagementSystem.Sender.Email;
using UserManagementSystem.Services.AuthService;
using UserManagementSystem.Services.JwtService;
using UserManagementSystem.Services.UserService;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers(options =>
{
    options.Filters.Add<ValidateModelAttribute>(); // Apply globally
}).ConfigureApiBehaviorOptions(options =>
{
    
    options.SuppressModelStateInvalidFilter = true;
}); ;



builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configure Identity
builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
{
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.User.RequireUniqueEmail = true; // ensure email is unique
})
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();



builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = "Bearer";
    options.DefaultChallengeScheme = "Bearer";
})
.AddJwtBearer("Bearer", options =>
{
    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
    {

        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration[AppSettings.JwtIssuer],
        ValidAudience = builder.Configuration[AppSettings.JwtAudience],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration[AppSettings.JwtKey]))
    };

    options.Events = new Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerEvents
    {
        OnChallenge = async context =>
        {

            context.HandleResponse();

            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            context.Response.ContentType = "application/json";

            var response = new ApiResponse<object>(
                null,
                false,
                AppSettings.UnAuthoriized
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
                AppSettings.Forbidden
            );

            await context.Response.WriteAsJsonAsync(response);
        }
    };
});


builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IEmailSender, EmailSender>(); 

builder.Services.AddAutoMapper(typeof(Program));
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
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

app.UseMiddleware<ApiRouteNotFoundMiddleware>();


app.MapControllers();

app.Run();
