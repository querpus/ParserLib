#pragma warning disable CA1710 // Identifiers should have correct suffix
#pragma warning disable format // Formatting

namespace Common.Entities;

/// <summary>An entity representing a number or decimal.</summary>
public class NumberEntity : Entity
{
  public bool IsInteger => Value % 1 == 0;
  /// <summary>Gets or sets the value as a decimal.</summary>
  public required decimal Value
  {
    get => (decimal) DataValues["Value"]!;
    set => DataValues["Value"] = value;
  }

  /// <summary>Gets or sets the value as a string.</summary>
  public string Content
  {
    get => $"{Value}";
    set => Value = decimal.Parse(value, CIIC);
  }

  public override bool Equals (IEntity? other) => other is NumberEntity ne && ne.Value == Value;
  public override string Serialize () => $"{Value}";
}
