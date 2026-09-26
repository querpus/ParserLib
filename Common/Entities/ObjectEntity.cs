#pragma warning disable CA1710 // Identifiers should have correct suffix
#pragma warning disable format // Formatting

namespace Common.Entities;

/// <summary>An entity representing a json style object.<br/><br/>
/// Uses only standard DataValues.<br/>
/// Uses Properties Dictionary for children.
/// </summary>
public class ObjectEntity : Entity, IEnumerable<IEntity>
{
  public int Count => Children.Count;

  public IEntity? this[string key]
  {
    get => Properties.TryGetValue(key, out IEntity? value) ? value : null;
    set
    {
      if (value is PropertyEntity pe)
      {
        Properties[key] = pe.Value!;
      }
      else
      {
        Properties[key] = value is IEntity ent
          ? ent
          : throw new InvalidOperationException("Tried to assign a null entity to ObjectEntity Property.");
      }
    }
  }
  public IList<PropertyEntity> GetPropertyEntities () => [.. Properties.Select(i => new PropertyEntity
  {
    Key = i.Key,
    Value = i.Value,
    Origin = Origin,
    Parent = Parent
  })];
  public void AddProperty (PropertyEntity property) => Properties.Add(property.Key, property.Value ?? new NullEntity());
  public void AddProperties (IEnumerable<PropertyEntity> properties) => properties.Foreach(AddProperty);
  public override bool Equals (IEntity? other) =>
    other is ObjectEntity oe && Properties.SequenceEqual(oe.Properties);
  public override string Serialize () => GetPropertyEntities().TextJoin(",");
  public bool Contains (string key) => Properties.ContainsKey(key);
  public IEnumerator<IEntity> GetEnumerator () => GetPropertyEntities().GetEnumerator();
  IEnumerator IEnumerable.GetEnumerator () => GetEnumerator();
  protected override void Assign (Match match) { }
}
