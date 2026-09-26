#pragma warning disable CA1710 // Identifiers should have correct suffix
#pragma warning disable format // Formatting

using System.Data;

namespace Common.Entities;

public class ElementEntity : Entity
{
  public bool IsHeader
  {
    get => (bool) DataValues["IsHeader"]!;
    set => DataValues["IsHeader"] = value;
  }
  public bool IsSingle {
    get => (bool) DataValues["IsSingle"]!;
    set => DataValues["IsSingle"] = value;
  }
  public required string Name
  {
    get => (string) DataValues["Name"]!;
    set => DataValues["Name"] = value;
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
    set => AddAttributes(value);
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
  protected override void Assign (Match match)
  {
    IsHeader = match.HasValidGroup("header");
    Origin = match.Value;
    Name = match.Groups["name"].Value;
    //Attributes = ParseAttributes(match);
    //TODO: Figure out what to do about ParseAttributes.
  }
}
