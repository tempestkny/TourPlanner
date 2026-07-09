using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using TourPlanner.Api.Middleware;
using TourPlanner.Bll;
using TourPlanner.Bll.Auth;
using TourPlanner.Dal;

// Controller-based Web API
var builder = WebApplication.CreateBuilder(args);

// Add Services
builder.Services.AddControllers(); 
builder.Services.AddProblemDetails();
builder.Services.AddHealthChecks();
builder.Services.AddCors(options => // Add CORS policy for Angular development
{
    options.AddPolicy("AngularDevelopment", policy =>
    {
        policy
            .WithOrigins("http://localhost:4200")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.AddSwaggerGen(options => // configures Swagger to use JWT authentication
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter the JWT bearer token."
    });

    options.AddSecurityRequirement(openApiDocument => new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecuritySchemeReference("Bearer", openApiDocument, null),
            new List<string>()
        }
    });
});
builder.Services.Configure<JwtOptions>( // loads JWT options from the configuration file
    builder.Configuration.GetSection(JwtOptions.SectionName));
var jwtOptions = builder.Configuration
    .GetSection(JwtOptions.SectionName)
    .Get<JwtOptions>()!;
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme) // backend JWT authentication configuration
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtOptions.Issuer,
            ValidAudience = jwtOptions.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Secret))
        };
    });
builder.Services.AddAuthorization(); // enables Authorization
// From get the Context with the connection string from the appsettings
builder.Services.AddDbContext<TourPlannerDbContext>(options => // configures the EF Core DbContext to use PostgreSQL 
    options.UseNpgsql(builder.Configuration.GetConnectionString("TourPlannerDb")));

// Add services and repositories to the DI container
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ITourRepository, TourRepository>();
builder.Services.AddScoped<ITourLogRepository, TourLogRepository>();
builder.Services.AddScoped<ITourService, TourService>();
builder.Services.AddScoped<ITourLogService, TourLogService>();
builder.Services.AddScoped<IImportExportService, ImportExportService>();
builder.Services.AddScoped<IStatisticsService, StatisticsService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IPasswordHasher, Pbkdf2PasswordHasher>();
builder.Services.AddScoped<ITokenService, TokenService>();

builder.Services.Configure<OpenRouteServiceOptions>(
    builder.Configuration.GetSection(OpenRouteServiceOptions.SectionName));
builder.Services.AddHttpClient<IOpenRouteService, OpenRouteService>(client =>
{
    client.BaseAddress = new Uri("https://api.openrouteservice.org/");
});

var app = builder.Build();

app.UseMiddleware<CorrelationIdMiddleware>(); // correlation runs early so that all logs have correlation id
app.UseMiddleware<GlobalExceptionMiddleware>();

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

app.UseCors("AngularDevelopment"); // CORS first
app.UseAuthentication(); // reads and validates JWT
app.UseAuthorization(); // checks [Authorize] 

app.MapHealthChecks("/health"); // maps endpoints
app.MapControllers();

app.Run();
