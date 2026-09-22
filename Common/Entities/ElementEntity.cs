#pragma warning disable CA1710 // Identifiers should have correct suffix
#pragma warning disable format // Formatting

using System.Data;

namespace Common.Entities;

public class ElementEntity : Entity
{
  public bool IsHeader
  {
    get => (bool) DataValues["IsHeader"]!;
    init => DataValues["IsHeader"] = value;
  }
  public bool IsSingle {
    get => (bool) DataValues["IsSingle"]!;
    init => DataValues["IsSingle"] = value;
  }
  public required string Name
  {
    get => (string) DataValues["Name"]!;
    init => DataValues["Name"] = value;
  }
  public string? Namespace
  {
    get => (string?) DataValues["Namespace"];
    set => DataValues["Namespace"] = value;
  }
  public Collection<AttributeEntity> Attributes
  {
    get
    {
      if (!DataValues.ContainsKey("Attributes"))
        DataValues["Attributes"] = new Collection<AttributeEntity>();

      return (Collection<AttributeEntity>) DataValues["Attributes"]!;
    }
    init => AddAttributes(value);
  }

  public override string Serialize ()
  {
    string attrs = Attributes.Select(static child => child.ToString()).TextJoin(" ");
    string children = Children.Select(static child => child.ToString()).TextJoin(Chars.LFs);

    if (IsHeader)
      return $"<?xml {attrs}?>";

    string elem = $"<{Name} {attrs}";

    return Children.OfType<ElementEntity>().ICount == 0
    ? elem + " />"
    : elem + ">" + children + $"</{Name}>";
  }
  public override bool Equals (IEntity? other) =>
    other is ElementEntity ee &&
    Attributes.SequenceEqual(ee.Attributes) &&
    Children.SequenceEqual(ee.Children) &&
    Name.Equals(ee.Name, SCO);
  public void AddAttribute (IEntity attribute)
  {
    if (attribute is AttributeEntity ae)
    {
      ae.SetParent(this);
      Attributes.Add(ae);
    }
  }
  public void AddAttributes (IEnumerable<IEntity> attributes) => attributes.Foreach(AddAttribute);

}
