#pragma warning disable CA1710 // Identifiers should have correct suffix
#pragma warning disable format // Formatting

using BT = Common.Entities.BasicType;

namespace Common.Entities;

public class ElementOpenPlaceholder : Entity
{
  public required string Name
  {
    get => (string) DataValues["Name"]!;
    init => DataValues["Name"] = value;
  }

  public Collection<AttributeEntity> Attributes
  {
    get => (Collection<AttributeEntity>) PropertyCollections["Attributes"];
    init => AddAttributes(value);
  }
  public string? Namespace
  {
    get => (string?) DataValues["Namespace"];
    set => DataValues["Namespace"] = value;
  }
  public override BT Type => BT.Placeholder;
  public void AddAttribute (IEntity attribute)
  {
    if (attribute is AttributeEntity ae)
    {
      ae.SetParent(this);
      Attributes.Add(ae);
    }
  }
  public void AddAttributes (IEnumerable<IEntity> attributes) => attributes.Foreach(AddAttribute);
  public override bool Equals (IEntity? other) =>
    other is ElementOpenPlaceholder eop &&
    Name.Is(eop.Name) &&
    Attributes.SequenceEqual(eop.Attributes) &&
    ((Namespace is null && eop.Namespace is null) || Namespace.Is(eop.Namespace));
  public override string Serialize () => $"<{Name} {Attributes}>";

}
