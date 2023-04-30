using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using PurpleBank.Controllers;
using PurpleBank.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddScoped<BankingController>();


// Register the BankingDbContext for dependency injection and configure it to use SQL Server with the specified connection string.
builder.Services.AddDbContext<BankingDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register Swagger services
builder.Services.AddSwaggerGen(c => {
 c.SwaggerDoc("v1", new OpenApiInfo { Title = "PurpleBank API", Version = "v1" });
});

var app = builder.Build();// Build the application.

// Register Swagger services
builder.Services.AddSwaggerGen(c => {
 c.SwaggerDoc("v1", new OpenApiInfo { Title = "PurpleBank API", Version = "v1" });
});

app.UseHttpsRedirection();// Redirect HTTP requests to HTTPS to ensure secure communication.
app.UseRouting();// Enable endpoint routing
app.UseAuthorization();// Apply authorization to the request pipeline.
app.UseEndpoints(endpoints => {
 endpoints.MapControllers();// Map the controller routes to the request pipeline.
});

app.Run();// Run the application.
