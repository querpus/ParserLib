#pragma warning disable CA1710 // Identifiers should have correct suffix
#pragma warning disable format // Formatting

using BT = Common.Entities.BasicType;

namespace Common.Entities;

public class ObjectEntity : Entity
{
  public Collection<IEntity> Properties => (Collection<IEntity>) PropertyCollections["Properties"];
  public override BT Type => BT.Object;
  public void AddProperty (IEntity property)
  {
    property.SetParent(this);
    PropertyCollections["Properties"].Add(property);
  }
  public void AddProperties (IEnumerable<IEntity> properties) => properties.Foreach(AddProperty);
  public override bool Equals (IEntity? other) =>
    other is ObjectEntity oe && Properties.SequenceEqual(oe.Properties);
  public override string Serialize () => Properties.TextJoin(",");
}
