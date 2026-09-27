#pragma warning disable CA1710 // Identifiers should have correct suffix
#pragma warning disable format // Formatting

namespace Common.Entities;

public class BooleanEntity : Entity, IContentEntity
{
  public bool? Value
  {
    get => DataValues.TryGetValue("Value", out object? value) ? (bool) value! : null;
    set => DataValues["Value"] = value;
  }
  public string Content
  {
    get => Value ?? false ? bool.TrueString : bool.FalseString;
    set =>
      DataValues["Value"] = value.Equals(bool.TrueString, SCO)
      ? true
      : value.Equals(bool.FalseString, SCO)
      ? (object) false
      : throw new InvalidOperationException($"Assigned an invalid string ({value}) to a boolean entity.");
  }
  public override bool IsValid => Value is not null;
  public override string Serialize () => Content;
  protected override void Assign (Match match)
  {
    Value = bool.Parse(match.Groups["value"].Value);
    Origin = match.Value;
  }
}
