using Azure.Identity;
using ManagedIdentity.Example.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

//App Configuration connection string
//var appConfigurationConnString = builder.Configuration.GetConnectionString("appConfiguration");
//builder.Configuration.AddAzureAppConfiguration(appConfigurationConnString);

// App Configuration managed identity
var appConfigurationUri = builder.Configuration.GetValue<string>("appConfigurationUri");
builder.Configuration.AddAzureAppConfiguration(options
    => options.Connect(
        new Uri(appConfigurationUri!),
        new DefaultAzureCredential()));

// Key Vault managed identity
builder.Configuration.AddAzureKeyVault(
    new Uri(builder.Configuration["keyVaultUri"]!),
    new DefaultAzureCredential());

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// SQL Username password
//var sqlConnString = builder.Configuration.GetConnectionString("sqlServer");

// SQL Managed Identity
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
