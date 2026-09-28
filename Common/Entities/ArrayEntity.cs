#pragma warning disable CA1710 // Identifiers should have correct suffix
#pragma warning disable format // Formatting

namespace Common.Entities;

/// <summary>Represents an entity that contains an ordered collection of child entities and serializes them as a comma-separated
/// list.</summary>
/// <remarks>Valid only when it has no properties or data values. Serialization joins the Children collection with
/// commas. Assignment from a regex match is ignored (no-op), <see cref="IEntity.AddChild"/> should be used to assign children.</remarks>
public class ArrayEntity : Entity
{
  public override bool IsValid => Properties.Count == 0 && DataValues.Count == 0;
  public override string Serialize () => Children.TextJoin(",");
  public override void Assign (Match match) { }
}
