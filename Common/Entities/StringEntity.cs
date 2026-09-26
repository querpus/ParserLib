#pragma warning disable CA1710 // Identifiers should have correct suffix
#pragma warning disable format // Formatting

namespace Common.Entities;

public class StringEntity : Entity
{
  /// <summary>Gets the serialized representation of the string value.</summary>
  /// <remarks>This contains quotes.</remarks>
  public override string Serialize () => $"\"{Value}\"";
  /// <summary>Gets or sets the string value.</summary>
  public required string Value
  {
    get => (string) DataValues["Value"]!;
    set => DataValues["Value"] = value;
  }
  public string? Content
  {
    get => Serialize();
    set
    {
      if (value?.Length >= 2 && value[0] == value[^1] && value[0] is '"' or '\'')
      {
        Quote = value[..1];
        Value = value[1..^1];
      }
      else if (value?.Length == 2 && value[0] == value[1] && value[0] is '"' or '\'')
      {
        Quote = value[..1];
        Value = SE;
      }
      else if (value is not null)
      {
        Quote = SE;
        Value = value;
      }
      else
      {
        Quote = SE;
        Value = SE;
      }
    }
  }
  public string? Quote
  {
    get => (string) DataValues["Quote"]!;
    set => DataValues["Quote"] = value;
  }
  public override bool Equals (IEntity? other) =>
    other is StringEntity entity && Value.Equals(entity.Value, SCO);
  protected override void Assign (Match match)
  {
    Value = match.Groups["value"].Value;
    Quote = match.Groups["quote"].Value;
    Origin = match.Value;
  }
}
