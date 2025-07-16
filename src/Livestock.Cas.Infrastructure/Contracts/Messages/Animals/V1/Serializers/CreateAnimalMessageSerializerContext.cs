using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace Livestock.Cas.Infrastructure.Contracts.Messages.Animals.V1.Serializers;

[ExcludeFromCodeCoverage]
[JsonSourceGenerationOptions(
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
    Converters = []
)]
[JsonSerializable(typeof(CreateAnimalMessage))]
public partial class CreateAnimalMessageSerializerContext : JsonSerializerContext
{
}
