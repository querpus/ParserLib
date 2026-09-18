#pragma warning disable CA1710 // Identifiers should have correct suffix
#pragma warning disable format // Formatting

using BT = Common.Entities.BasicType;

namespace Common.Entities;

public class ObjectEntity : Entity
{
  public override BT Type => BT.Object;

  public int Count => Children.Count;

  public IEntity this[string key]
  {
    get => Properties[key];
    set
    {
      if (Properties.ContainsKey(key) && value is PropertyEntity pe)
        Properties[key] = pe.Value!;
      else if (Properties.ContainsKey(key) && value is IEntity ent)
        Properties[key] = ent;
      else if (value is PropertyEntity pe2)
        Properties.Add(key, pe2.Value!);
      else
        Properties.Add(key, value);
    }
  }
  public IList<PropertyEntity> GetPropertyEntities () => [.. Properties.Select(i => new PropertyEntity() { Key = i.Key, Value = i.Value, Origin = Origin, Parent = Parent })];
  public void AddProperty (PropertyEntity property) => Properties.Add(property.Key, property.Value ?? new NullEntity());
  public void AddProperties (IEnumerable<PropertyEntity> properties) => properties.Foreach(AddProperty);
  public override bool Equals (IEntity? other) =>
    other is ObjectEntity oe && Properties.SequenceEqual(oe.Properties);
  public override string Serialize () => GetPropertyEntities().TextJoin(",");
  public bool Contains (string key) => Properties.ContainsKey(key);
  public IEnumerator<IEntity> GetEnumerator () => .GetEnumerator();
}
