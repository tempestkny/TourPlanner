using Microsoft.EntityFrameworkCore;
using TourPlanner.Bll;
using TourPlanner.Bll.Auth;
using TourPlanner.Dal;

// Controller-based Web API
var builder = WebApplication.CreateBuilder(args);

// Add Services
builder.Services.AddControllers();

builder.Services.AddSwaggerGen();
// From get the Context with the connection string from the appsettings
builder.Services.AddDbContext<TourPlannerDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("TourPlannerDb")));
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ITourRepository, TourRepository>();
builder.Services.AddScoped<ITourService, TourService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IPasswordHasher, Pbkdf2PasswordHasher>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    using (var scope = app.Services.CreateScope()){
        var services = scope.ServiceProvider;
        var context = services.GetRequiredService<TourPlannerDbContext>();
        context.Database.EnsureCreated();
    }
}


// Add endpoints

app.MapControllers();

app.Run();
