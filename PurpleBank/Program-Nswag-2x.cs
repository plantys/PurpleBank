using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using PurpleBank.Controllers;
using PurpleBank.Data;
using PurpleBank.Models;
using Swashbuckle.AspNetCore.Swagger;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddScoped<IBankingController, BankingController>();

builder.Services.AddDbContext<BankingDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddSwaggerGen(c => {
 c.SwaggerDoc("v1", new OpenApiInfo { Title = "PurpleBank API", Version = "v1" });
});

var app = builder.Build();

// Register Swagger services
builder.Services.AddSwaggerGen(c => {
 c.SwaggerDoc("v1", new OpenApiInfo { Title = "PurpleBank API", Version = "v1" });
});

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
