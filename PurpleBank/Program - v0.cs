using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using PurpleBank.Models;
using PurpleBank.Controllers;
using Microsoft.EntityFrameworkCore;
using PurpleBank.Data;

var builder = WebApplication.CreateBuilder(args);


// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddTransient<BankingController>();
builder.Services.AddDbContext<BankingDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("BankingDbConnection")));

// Add Swagger services for development environment only
if (builder.Environment.IsDevelopment()) {
 builder.Services.AddSwaggerGen(c =>
 {
  c.SwaggerDoc("v1", new OpenApiInfo { Title = "Banking API", Version = "v1" });
 });
}

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()) {
 app.UseDeveloperExceptionPage();

 // Use Swagger and Swagger UI middleware in development environment only
 Microsoft.AspNetCore.Builder.SwaggerBuilderExtensions.UseSwagger(app);
 app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Banking API v1"));
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
