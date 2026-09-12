#pragma warning disable CA1710 // Identifiers should have correct suffix
#pragma warning disable format // Formatting

namespace Common.Entities;
/// <summary>This is returned if a method must return an <see cref="Entity"/> but one could not be created.</summary>
public class ErrorEntity : Entity
{
  public required string Message { get; init; }
  public override BasicType Type => BasicType.Invalid;
  public override bool Equals (IEntity? other) => false; // Error entities are never equal to anything else, even other error entities.
  public override string Serialize () => $"Error: {Message}";
}
