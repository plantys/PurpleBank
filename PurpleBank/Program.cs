using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using PurpleBank.Data;
using PurpleBank.Models;
using Swashbuckle.AspNetCore.Swagger;
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
// Register the BankingDbContext for dependency injection and configure it to use SQL Server with the specified connection string.
builder.Services.AddDbContext<BankingDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
// Register Swagger services
builder.Services.AddSwaggerGen(c => {
 c.SwaggerDoc("v1", new OpenApiInfo { Title = "PurpleBank API", Version = "v1" });
});
var app = builder.Build();// Build the application.
// Configure the Swagger middleware - using Use Swashbuckle's UseSwagger
if (app.Environment.IsDevelopment()) {
 Microsoft.AspNetCore.Builder.SwaggerBuilderExtensions.UseSwagger(app);
 app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "PurpleBank API v1"));
}
app.UseHttpsRedirection();// Redirect HTTP requests to HTTPS to ensure secure communication.
app.UseAuthorization();// Apply authorization to the request pipeline.
app.MapControllers();// Map the controller routes to the request pipeline.
app.Run();// Run the application.
 
