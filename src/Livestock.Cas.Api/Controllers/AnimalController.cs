using Livestock.Cas.Infrastructure.Messaging;
using Microsoft.AspNetCore.Mvc;

namespace Livestock.Cas.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AnimalController : ControllerBase
{
    [HttpPost]
    [Route("{name}")]
    public async Task CreateAsync(string name)
    {
        var messagePublisher = new MessagePublisher();

        await messagePublisher.SendAsync(name);

        await Task.FromResult(true);
    }
}
