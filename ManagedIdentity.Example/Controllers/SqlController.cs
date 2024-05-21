using ManagedIdentity.Example.Models;
using Microsoft.AspNetCore.Mvc;

namespace ManagedIdentity.Example.Controllers;

[ApiController]
[Route("[controller]")]
public class SqlController : ControllerBase
{

    private readonly ManagedIdentityContext _context;

    public SqlController(ManagedIdentityContext context)
    {
        _context = context;
    }

    [HttpGet(Name = "GetSqlValues")]
    public IList<Info> Get()
    {
        return _context.Infos.ToList();
    }

    [HttpPost(Name = "SetSqlValues")]
    public void Set([FromBody] Info info)
    {
        _context.Infos.Add(info);
        _context.SaveChanges();
    }
}
