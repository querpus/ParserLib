#pragma warning disable CA1710 // Identifiers should have correct suffix
#pragma warning disable format // Formatting

using BT = Common.Entities.BasicType;

namespace Common.Entities;

public class ArrayEntity : Entity
{
  public void AddValue (IEntity child)
  {
    child.SetParent(this);
    PropertyCollections["Values"].Add(child);
  }
  public void AddValues (IEnumerable<IEntity> children) => children.Foreach(AddValue);
  public Collection<IEntity> Values => (Collection<IEntity>) PropertyCollections["Values"];
  public override BT Type => BT.Array;

  public override bool Equals (IEntity? other) =>
    other is ArrayEntity ce && Values.SequenceEqual(ce.Values);
  public override string Serialize () => Values.TextJoin(",");
}
