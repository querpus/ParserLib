#pragma warning disable CA1710 // Identifiers should have correct suffix
#pragma warning disable format // Formatting

namespace Common.Entities;

public class PropertyEntity : Entity
{
  public required string Key
  {
    get => (string) DataValues["Key"]!;
    set => DataValues["Key"] = value;
  }
  public string? Quote
  {
    get => (string) DataValues["Quote"]!;
    set => DataValues["Quote"] = value;
  }
  public IEntity? Value { get; set; }
  public override bool Equals (IEntity? other) =>
    other is PropertyEntity pe &&
    Key.Equals(pe.Key, SCO) &&
    (Value?.Equals(pe.Value) ?? (pe.Value is null));
  public override string Serialize () => $"{Quote}{Key}{Quote}:{Value}";
  protected override void Assign (Match match)
  {
    Key = match.Groups["key"].Value;
    Quote = match.Groups["quote"].Value;
  }
}
