using ManagedIdentity.Example.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var appConfigurationConnString = builder.Configuration.GetConnectionString("appConfiguration");
builder.Configuration.AddAzureAppConfiguration(appConfigurationConnString);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var sqlConnString = builder.Configuration.GetConnectionString("sqlServer");

builder.Services.AddDbContext<ManagedIdentityContext>(options =>
    options.UseSqlServer(sqlConnString));

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
