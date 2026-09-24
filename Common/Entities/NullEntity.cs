#pragma warning disable CA1710 // Identifiers should have correct suffix
#pragma warning disable format // Formatting

namespace Common.Entities;

/// <summary>An entity representing a null value.</summary>
public class NullEntity : Entity
{
  private const string NullString = "null";
  public override bool Equals (IEntity? other) => other is NullEntity;
  public override string Serialize () => NullString;
}
