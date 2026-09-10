#pragma warning disable CA1710 // Identifiers should have correct suffix
#pragma warning disable format // Formatting

using BT = Common.Entities.BasicType;

namespace Common.Entities;

/// <summary>An entity representing a number or decimal.</summary>
public class NumberEntity : Entity
{
  public required decimal Value
  {
    get => (decimal) DataValues["Value"]!;
    set => DataValues["Value"] = value;
  }
  public override BT Type => BT.Number;

  public string Content => $"{Value}";

  public override bool Equals (IEntity? other) => other is NumberEntity ne && ne.Value == Value;
  public override string Serialize () => $"{Value}";
}
