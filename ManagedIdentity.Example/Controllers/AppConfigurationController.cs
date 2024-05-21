using Microsoft.AspNetCore.Mvc;

namespace ManagedIdentity.Example.Controllers;

[ApiController]
[Route("[controller]")]
public class AppConfigurationController : ControllerBase
{
    private readonly IConfiguration _configuration;

    public AppConfigurationController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    [HttpGet(Name = "GetAppConfigurationValue")]
    public string Get()
    {
        return _configuration.GetValue<string>("MyConfig");
    }
}
