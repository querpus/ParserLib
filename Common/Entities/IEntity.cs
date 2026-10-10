#pragma warning disable CA1710 // Identifiers should have correct suffix
#pragma warning disable format // Formatting

namespace Common.Entities;

/// <summary>An interface for storing entities.</summary>
public interface IEntity : ITextSerializer, IEquatable<IEntity>
{
  IList<IEntity> Children { get; }
  Dictionary<string, object?> DataValues { get; }
  /// <summary>Returns <see langword="true"/> if the entity has been initialized successfully, <see langword="false"/> otherwise.</summary>
  bool IsValid { get; }
  /// <summary>Gets the origin of the parsed entity.</summary>
  /// <remarks>This is null for entities that are not derived from a single source.</remarks>
  string? Origin { get; set; }
  /// <summary>Static equality method.</summary>
  /// <param name="obj_a">Entity 'A'.</param>
  /// <param name="obj_b">Entity 'B'.</param>
  /// <returns><see langword="true"/> if 'A' is equal to 'B', <see langword="false"/> otherwise.</returns>
  static bool Equals (IEntity? obj_a, IEntity? obj_b) =>
    (obj_a is null && obj_b is null) || (obj_a is not null && obj_b is not null && obj_a.Equals(obj_b));
  bool Equals (object? obj);
  int GetHashCode ();
  /// <summary>Adds a child to the children list.</summary>
  /// <param name="child">The child to add.</param>
  void Add (IEntity child);
  void Add (IEnumerable<IEntity> children);
  void AddToDataCollection (string key, object data);
  void DoAssign (Match match);
  void Assign (Match match);
  IEntity ToEntity ();
}
