namespace Livestock.Cas.Infrastructure.Contracts.Messages.Animals.V1;

public record CreateAnimalMessage
{
    public string Species { get; init; } = "";
    public string Cph { get; init; } = "";
}
