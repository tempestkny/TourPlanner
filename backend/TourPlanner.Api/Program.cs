var builder = WebApplication.CreateBuilder(args);

// Add Services
builder.Services.AddControllers();

builder.Services.AddSwaggerGen();
// From get the Context with the connection string from the appsettings
builder.Services.AddDbContext<TourPlannerDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("TourPlannerDb")));

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