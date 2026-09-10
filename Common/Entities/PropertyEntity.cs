#pragma warning disable CA1710 // Identifiers should have correct suffix
#pragma warning disable format // Formatting

using BT = Common.Entities.BasicType;

namespace Common.Entities;

public class PropertyEntity : Entity
{
  public override BT Type => BT.Property;
  public required string Key { get; set; }
  public IEntity? Value { get; set; }

  public override bool Equals (IEntity? other) =>
    other is PropertyEntity pe &&
    Key.Equals(pe.Key, SCO) &&
    (Value?.Equals(pe.Value) ?? pe.Value is null);
  public override string Serialize () => $"\"{Key}\":{Value}";
}
