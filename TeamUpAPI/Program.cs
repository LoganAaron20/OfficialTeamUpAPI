using Microsoft.EntityFrameworkCore;
using TeamUpAPI.Models;
using TeamUpAPI.Services;
using TeamUpAPI.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<MovieContext>(options =>
     options.UseSqlServer(builder.Configuration.GetConnectionString("MovieContext")));

builder.Services.AddDbContext<TeamUpContext>(options =>
     options.UseSqlServer(builder.Configuration.GetConnectionString("TeamUpContext")));

builder.Services.AddScoped<IEmailService, EmailService>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{

    // Apply database migrations
    using (var scope = app.Services.CreateScope())
    {
        var serviceProvider = scope.ServiceProvider;

        // Apply migrations for TeamUp context
        var teamUpContext = serviceProvider.GetRequiredService<TeamUpContext>();
        teamUpContext.Database.Migrate();
    }

    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
