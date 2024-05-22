using Azure.Identity;
using ManagedIdentity.Example.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
//var appConfigurationConnString = builder.Configuration.GetConnectionString("appConfiguration");
//builder.Configuration.AddAzureAppConfiguration(appConfigurationConnString);

var appConfigurationUri = builder.Configuration.GetValue<string>("appConfigurationUri");
builder.Configuration.AddAzureAppConfiguration(options
    => options.Connect(
        new Uri(appConfigurationUri!),
        new DefaultAzureCredential()));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var sqlConnString = builder.Configuration.GetConnectionString("sqlServerIdentity");

builder.Services.AddDbContext<ManagedIdentityContext>(options =>
    options.UseSqlServer(sqlConnString));

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
