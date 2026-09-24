#pragma warning disable CA1710 // Identifiers should have correct suffix
#pragma warning disable format // Formatting

namespace Common.Entities;

public interface IEntity
{
  IList<IEntity> Children { get; }
  Dictionary<string, IEntity> Properties { get; }
  Dictionary<string, object?> DataValues { get; }
  /// <summary>Gets the origin of the parsed entity.</summary>
  /// <remarks>This is null for entities that are not derived from a single source.</remarks>
  string? Origin { get; set; }
  /// <summary>The parent entity.</summary>
  IEntity? Parent { get; }
  /// <summary>Static equality method.</summary>
  /// <param name="obj_a">Entity 'A'.</param>
  /// <param name="obj_b">Entity 'B'.</param>
  /// <returns><see langword="true"/> if 'A' is equal to 'B', <see langword="false"/> otherwise.</returns>
  static bool Equals (IEntity? obj_a, IEntity? obj_b) =>
    (obj_a is null && obj_b is null) || (obj_a is not null && obj_b is not null && obj_a.Equals(obj_b));
  bool Equals (IEntity? other);
  bool Equals (object? obj);
  int GetHashCode ();
  string? ToString ();
  /// <summary>Sets the parent property after the type has been constructed.</summary>
  /// <param name="parent">The parent or encompassing object.</param>
  void SetParent (IEntity parent);
  void AddChild (IEntity child);
  void AddChildren (IEnumerable<IEntity> children);
  void AddToDataCollection (string key, object data);
}
