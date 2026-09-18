#pragma warning disable CA1710 // Identifiers should have correct suffix
#pragma warning disable format // Formatting

using BT = Common.Entities.BasicType;

namespace Common.Entities;

public class ObjectEntity : Entity
{
  public IReadOnlyList<PropertyEntity> Properties => (IReadOnlyList<PropertyEntity>) Children.OfType<PropertyEntity>();
  public override BT Type => BT.Object;

  public int Count => Children.Count;

  public IEntity this[string key]
  {
    get => (IEntity?) Properties.SingleOrDefault(e => e.Key == key) ?? new ErrorEntity($"Key '{key}' not found in ObjectEntity.");
    set
    {
      if (this[key] is PropertyEntity pe)
        pe.Value = value;
      else
        AddProperty(new() { Key = key, Value = value });
    }
  }
  public void AddProperty (PropertyEntity property) => Children.Add(property);
  public void AddProperties (IEnumerable<PropertyEntity> properties) => properties.Foreach(AddProperty);
  public override bool Equals (IEntity? other) =>
    other is ObjectEntity oe && Properties.SequenceEqual(oe.Properties);
  public override string Serialize () => Properties.TextJoin(",");
  public bool Contains (string key) => Properties.Any(prop => prop.Key.Equals(key, SCO));
  public IEnumerator<IEntity> GetEnumerator () => Children.GetEnumerator();
}
