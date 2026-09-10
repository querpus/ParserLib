#pragma warning disable CA1710 // Identifiers should have correct suffix
#pragma warning disable format // Formatting

using BT = Common.Entities.BasicType;

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

  public override BT Type => BT.String;
  public static StringEntity CreateFrom (Match match) => new()
  {
    Value = match.Groups["value"].Value,
    Origin = match.Value,
  };
  public override bool Equals (IEntity? other) =>
    other is StringEntity entity && Value.Equals(entity.Value, SCO);
}
