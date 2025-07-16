using Livestock.Cas.Infrastructure.Contracts.Messages.Animals.V1;
using Livestock.Cas.Infrastructure.Messaging.Publishers;
using Microsoft.AspNetCore.Mvc;

namespace Livestock.Cas.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AnimalController(IMessagePublisher<CreateAnimalMessage> messagePublisher) : ControllerBase
{
    private readonly IMessagePublisher<CreateAnimalMessage> _messagePublisher = messagePublisher;

    [HttpPost]
    [Route("{name}")]
    public async Task CreateAsync(string name, CancellationToken cancellationToken)
    {
        var createAnimalMessage = new CreateAnimalMessage { Cph = "XXXX", Species = name };

        await _messagePublisher.PublishAsync(createAnimalMessage, cancellationToken);
    }
}
