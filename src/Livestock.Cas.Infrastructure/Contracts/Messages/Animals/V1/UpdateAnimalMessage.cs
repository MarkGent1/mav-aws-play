namespace Livestock.Cas.Infrastructure.Contracts.Messages.Animals.V1;

public class UpdateAnimalMessage : MessageType
{
    public string Species { get; init; } = "";
    public string Cph { get; init; } = "";
}
