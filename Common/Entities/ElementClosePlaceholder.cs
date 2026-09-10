#pragma warning disable CA1710 // Identifiers should have correct suffix
#pragma warning disable format // Formatting

namespace Common.Entities;

public class ElementClosePlaceholder : Entity
{
  public required string Name { get; set; }
  public string? Namespace
  {
    get => (string?) DataValues["Namespace"];
    set => DataValues["Namespace"] = value;
  }
  public override BasicType Type => BasicType.Placeholder;

  public override bool Equals (IEntity? other) =>
    other is ElementClosePlaceholder ecp &&
    Name.Is(ecp.Name) &&
    ((Namespace is null && ecp.Namespace is null) || Namespace.Is(ecp.Namespace));
  public override string Serialize () => $"</{Name}>";
}
