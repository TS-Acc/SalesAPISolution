using Microsoft.EntityFrameworkCore;
using SalesAPI.Models;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddDbContext<SalesDbContext>(dbContOptBuild => {
    dbContOptBuild.UseSqlServer(builder.Configuration.GetConnectionString("SalesDbContext"));
});

builder.Services.AddCors(); // Cors stands for "cross origin resource sharing"

WebApplication app = builder.Build();

// Configure the HTTP request pipeline.

app.UseCors(corsPolicyBuilder => corsPolicyBuilder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());

app.UseAuthorization();

app.MapControllers();

app.Run();
