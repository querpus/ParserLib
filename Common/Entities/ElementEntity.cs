#pragma warning disable CA1710 // Identifiers should have correct suffix
#pragma warning disable format // Formatting

using System.Data;

namespace Common.Entities;

public class ElementEntity : Entity
{
  public bool IsHeader
  {
    get => DataValues.TryGetValue("IsHeader", out object? value) && (bool) value!;
    set => DataValues["IsHeader"] = value;
  }
  public bool IsSingle
  {
    get => DataValues.TryGetValue("IsSingle", out object? value) && (bool) value!;
    set => DataValues["IsSingle"] = value;
  }
  public string? Name
  {
    get => DataValues.TryGetValue("Name", out object? value) ? (string) value! : null;
    set => DataValues["Name"] = value;
  }
  public string? Namespace
  {
    get => DataValues.TryGetValue("Namespace", out object? value) ? (string?) value : null;
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
  public void AddAttribute (IEntity attribute)
  {
    if (attribute is AttributeEntity ae)
    {
      ae.SetParent(this);
      Attributes.Add(ae);
    }
  }
  public void AddAttributes (IEnumerable<IEntity> attributes) => attributes.Foreach(AddAttribute);
  public override void Assign (Match match)
  {
    IsHeader = match.HasValidGroup("header");
    IsSingle = match.HasValidGroup("single");
    Origin = match.Value;
    Name = match.Groups["name"].Value;
    Namespace = match.GetGroup("namespace");

    // Attributes
    string[]? namespaces = match.GetCaptures("a_ns");
    string[]? names = match.GetCaptures("a_name");
    string[]? values = match.GetCaptures("a_val");
    string[]? quotes = match.GetCaptures("a_qt");

    if (names is null || namespaces is null || values is null || quotes is null)
      return;

    var together = namespaces.Zip(names, values.Zip(quotes));
    foreach (var (ns, name, (val, qt)) in together)
    {
      AddAttribute(new AttributeEntity {Namespace=ns, Quote=qt, Value=val, Key=name});
    }
  }
}
