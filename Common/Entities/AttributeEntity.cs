#pragma warning disable CA1710 // Identifiers should have correct suffix
#pragma warning disable format // Formatting

namespace Common.Entities;

/// <summary>A key/value pair, with an optional namespace. It also stores what quote character was used.</summary>
/// <remarks>Stores all 4 values in data, not as <see cref="IEntity"/> objects.</remarks>
public class AttributeEntity : Entity
{
  public string? Namespace
  {
    get => (string?) DataValues.GetValueOrDefault("Namespace");
    set => DataValues["Namespace"] = value;
  }
  public string? Key
  {
    get => (string?) DataValues.GetValueOrDefault("Key");
    set => DataValues["Key"] = value;
  }
  public string Value
  {
    get => (string) DataValues.GetValueOrDefault("Value", SE)!;
    set => DataValues["Value"] = value;
  }
  public string Quote
  {
    get => (string) DataValues.GetValueOrDefault("Quote", "\"")!;
    set => DataValues["Quote"] = value;
  }
  [MemberNotNullWhen(true, nameof(Key))]
  public override bool IsValid => Key is not null;
  public override string Serialize () => $"{(Namespace is not null ? $"{Namespace}:" : "")}{Key}={Quote}{Value}{Quote}";
  public override void Assign (Match match) => throw new InvalidOperationException("Tried to create an attribute entity with an element match.");
  //protected void Assign (Match match, int index) { }
  //TODO: Assign with index.
}
