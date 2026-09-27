#pragma warning disable CA1710 // Identifiers should have correct suffix
#pragma warning disable format // Formatting

namespace Common.Entities;

public class PropertyEntity : Entity
{
  public string? Key
  {
    get => DataValues.TryGetValue("Key", out object? value) ? (string) value! : null;
    set => DataValues["Key"] = value;
  }
  public string? Quote
  {
    get => DataValues.TryGetValue("Quote", out object? value) ? (string) value! : null;
    set => DataValues["Quote"] = value;
  }
  public IEntity? Value
  {
    get => DataValues.TryGetValue("Value", out object? value) ? (IEntity) value! : null;
    set => DataValues["Value"] = value;
  }
  public override bool IsValid => Key is not null && Quote is not null;
  public override string Serialize () => $"{Quote}{Key}{Quote}:{Value}";
  protected override void Assign (Match match)
  {
    Key = match.Groups["key"].Value;
    Quote = match.Groups["quote"].Value;
    // Value is set later
  }
}
