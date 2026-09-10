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
  /// <summary>This should be overridden by any inherited class.</summary>
  /// <remarks>This determines the class of the entity.</remarks>
  public abstract BasicType Type { get; }
  /// <summary>Gets the property collections.</summary>
  public virtual Dictionary<string, IList<IEntity>> PropertyCollections { get; } = [];
  /// <summary>Gets the child entities.</summary>
  public virtual IList<IEntity> Children { get; } = [];
  /// <summary>Gets the property values.</summary>
  public virtual Dictionary<string, IEntity> PropertyValues { get; } = [];
  /// <summary>Gets the data values.</summary>
  /// <remarks>These are the values that are stored in the regular expression groups.</remarks>
  public virtual Dictionary<string, object?> DataValues { get; } = [];
  public virtual bool Equals (IEntity? other) =>
    other is RawEntity cust &&
    PropertyCollections.SequenceEqual(cust.PropertyCollections) &&
    PropertyValues.SequenceEqual(cust.PropertyValues) &&
    DataValues.SequenceEqual(cust.DataValues) &&
    Children.SequenceEqual(cust.Children);
  public abstract string Serialize ();
  /// <summary>The serialized representation of this entity.</summary>
  /// <returns>Returns the serialized entity by default.</returns>
  public override string? ToString () => Serialize();
  public void SetParent (IEntity parent) => Parent = parent;
  public void StoreData (string piece_type, dynamic data) => DataValues[piece_type] = data;
  public void StoreEntity (string piece_type, IEntity ent) => PropertyValues[piece_type] = ent;
  public void StoreEntities (string piece_type, IEnumerable<IEntity> ents) => PropertyCollections[piece_type] = [.. ents];
}
/// <summary>This is returned if a method must return an <see cref="Entity"/> but one could not be created.</summary>
public class ErrorEntity : Entity
{
  public required string Message { get; init; }
  public override BasicType Type => BasicType.Invalid;
  public override bool Equals (IEntity? other) => false; // Error entities are never equal to anything else, even other error entities.
  public override string Serialize () => $"Error: {Message}";
}

public static class SerializerExt
{
  extension (IEnumerable<ITextSerializer> attributes)
  {
    public string Serialize () => attributes.Select(d => d.Serialize()).TextJoin(" ");
  }
}
