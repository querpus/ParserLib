#pragma warning disable CA1710 // Identifiers should have correct suffix
#pragma warning disable format // Formatting

namespace Common.Entities;

/// <summary>An entity representing a json style object.<br/><br/>
/// Uses only standard DataValues.<br/>
/// Uses Properties Dictionary for children.
/// </summary>
public class ObjectEntity : Entity, IEnumerable<IEntity>, IReadOnlyDictionary<string, IEntity?>
{
  public int Count => Children.Count;

  public IEnumerable<string> Keys => Properties.Keys;
  public IEnumerable<IEntity> Values => Properties.Values;

  /// <summary>Gets or sets the Property at the specified key.</summary>
  /// <param name="key">The key to get/set.</param>
  /// <returns>The </returns>
  /// <exception cref="InvalidOperationException"></exception>
  public IEntity? this[string key]
  {
    get => Properties.TryGetValue(key, out IEntity? value) ? value : null;
    set =>
      Properties[key] = value is PropertyEntity pe
      ? pe.Value!
      : value is IEntity ent
        ? ent
        : throw new InvalidOperationException("Tried to assign a null entity to ObjectEntity Property.");
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
  public override string Serialize () => GetPropertyEntities().TextJoin(",");
  public bool ContainsKey (string key) => Properties.ContainsKey(key);
  public IEnumerator<IEntity> GetEnumerator () => GetPropertyEntities().GetEnumerator();
  IEnumerator IEnumerable.GetEnumerator () => GetEnumerator();
  public override void Assign (Match match) { }

  public bool TryGetValue (string key, [MaybeNullWhen(false)] out IEntity value) => Properties.TryGetValue(key, out value);
  IEnumerator<KeyValuePair<string, IEntity?>> IEnumerable<KeyValuePair<string, IEntity?>>.GetEnumerator () => Properties.GetEnumerator();
}
