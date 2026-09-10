#pragma warning disable CA1710 // Identifiers should have correct suffix
#pragma warning disable format // Formatting

using BT = Common.Entities.BasicType;

namespace Common.Entities;

/// <summary>A key/value pair, with an optional namespace. It also stores what quote character was used.</summary>
/// <remarks>Stores all 4 values in data, not as <see cref="IEntity"/> objects.</remarks>
public class AttributeEntity : Entity
{
  public override BT Type => BT.Attribute;
  public string? Namespace
  {
    get => (string?) DataValues["Namespace"];
    set => DataValues["Namespace"] = value;
  }
  public required string Key
  {
    get => (string) DataValues["Key"]!;
    set => DataValues["Key"] = value;
  }
  public required string Value
  {
    get => (string) DataValues["Value"]!;
    set => DataValues["Value"] = value;
  }
  public string? Quote
  {
    get => (string) DataValues["Quote"]!;
    set => DataValues["Quote"] = value;
  }

  public override bool Equals (IEntity? other) =>
    other is AttributeEntity ae &&
    Key.Equals(ae.Key, SCO) &&
    Value.Equals(ae.Value, SCO) &&
    ((Namespace.IsEmpty && ae.Namespace.IsEmpty) || (Namespace?.Equals(ae.Namespace, SCO) == true));
  public override string Serialize () => $"{(Namespace is not null ? $"{Namespace}:" : "")}{Key}={Quote}{Value}{Quote}";
}
