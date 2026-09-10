#pragma warning disable CA1710 // Identifiers should have correct suffix
#pragma warning disable format // Formatting

using BT = Common.Entities.BasicType;

namespace Common.Entities;

public class BooleanEntity : Entity
{
  public required bool Value
  {
    get => (bool) DataValues["Value"]!;
    set => DataValues["Value"] = value;
  }
  public override BT Type => BT.Boolean;
  public string Content => Value ? bool.TrueString : bool.FalseString;
  public override bool Equals (IEntity? other) => other is BooleanEntity be && be.Value == Value;
  public override string Serialize () => Content;
}
