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

  private readonly Collection<string> _keys = [];
  public void AddProperty (PropertyEntity property)
  {
    string key = property.Key!;
    if (_keys.Contains(key))
    {
      throw new InvalidOperationException($"Duplicate property found {property.DataValues["Key"]}");
    }
    _keys.Add(key);
    Children.Add(property);
  }
  public void AddProperties (IEnumerable<PropertyEntity> properties) => properties.Foreach(AddProperty);
  public override string Serialize () => Children.TextJoin(",");
  public bool ContainsKey (string key) => _keys.Contains(key);
  public IEnumerator<IEntity> GetEnumerator () => Children.GetEnumerator();
  IEnumerator IEnumerable.GetEnumerator () => GetEnumerator();
  public override void Assign (Match match) { }
}
