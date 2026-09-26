#pragma warning disable CA1710 // Identifiers should have correct suffix
#pragma warning disable format // Formatting

namespace Common.Entities;
/// <summary>Base class for entities.</summary>
public abstract class Entity : IEntity, IEquatable<IEntity>, ITextSerializer
{
  /// <summary>Gets or sets the parent entity.</summary>
  /// <remarks>This is <see langword="null"/> if the current entity is a root entity.</remarks>
  public IEntity? Parent { get; set; }
  public virtual string? Origin { get; set; }
  /// <summary>Gets the child entities.</summary>
  /// <remarks>These are the values of a JSON object or <see cref="ArrayEntity"/>, or any non-keyed objects stored within this entity.
  /// This would also be the content between the open and closing XML tags.</remarks>
  public virtual IList<IEntity> Children { get; } = [];
  /// <summary>Whether or not this item is omitted and ignored when reading data.</summary>
  public virtual bool Omit { get; set; }
  /// <summary>Gets the property values.</summary>
  /// <remarks>These are keyed values, like the properties of a JSON object, or the attributes of an XML Element.</remarks>
  public virtual Dictionary<string, IEntity> Properties { get; } = [];
  /// <summary>Gets the data values.</summary>
  /// <remarks>These are the values that are stored in the regular expression groups, or any other data that is not stored as a child or property.</remarks>
  public virtual Dictionary<string, object?> DataValues { get; } = [];
  public virtual bool Equals (IEntity? other) =>
    other is Entity entity &&
    Properties.SequenceEqual(entity.Properties) &&
    DataValues.SequenceEqual(entity.DataValues) &&
    Children.SequenceEqual(entity.Children);
  public abstract string Serialize ();
  /// <summary>The serialized representation of this entity.</summary>
  /// <returns>Returns the serialized entity by default.</returns>
  public override string? ToString () => Serialize();
  /// <summary>Sets this object's Parent property.</summary>
  /// <param name="parent"></param>
  public void SetParent (IEntity parent) => Parent = parent;
  public void AddChild (IEntity child)
  {
    child.SetParent(this);
    Children.Add(child);
  }
  public void AddChildren (IEnumerable<IEntity> children) => children.Foreach(AddChild);
  public void AddToDataCollection (string key, object data)
  {
    if (!DataValues.TryGetValue(key, out dynamic? value))
    {
      value = new Collection<object>();
      DataValues[key] = value;
    }

    if (DataValues.ContainsKey(key))
    {
      value!.Add(data);
    }
  }
  public virtual Entity ToEntity () => this;
  protected abstract void Assign (Match match);
}

public static class SerializerExt
{
  extension (IEnumerable<ITextSerializer> attributes)
  {
    public string Serialize () => attributes.Select(d => d.Serialize()).TextJoin(" ");
  }
}
