using Microsoft.AspNetCore.Mvc;

namespace ManagedIdentity.Example.Controllers;

[ApiController]
[Route("[controller]")]
public class KeyVaultController : ControllerBase
{
    private readonly IConfiguration _configuration;

    public KeyVaultController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    [HttpGet(Name = "GetKeyVaultSecretValue")]
    public string Get()
    {
        return _configuration.GetValue<string>("MySecret");
    }
}
