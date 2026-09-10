#pragma warning disable CA1710 // Identifiers should have correct suffix
#pragma warning disable format // Formatting

namespace Common.Entity;
public interface IPrimitiveEntity : IParsedEntity
{
  /// <summary>Gets the content of the primitive entity, as it should be serialized.</summary>
  /// <remarks>This would include quotes for string objects.</remarks>
  string Content { get; }
  /// <summary>Gets the data of the entity, as it should be stored internally.</summary>
  [AllowNull]
  dynamic Value { get; }
}
