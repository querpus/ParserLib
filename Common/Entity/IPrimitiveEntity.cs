#pragma warning disable CA1710 // Identifiers should have correct suffix
#pragma warning disable format // Formatting

namespace Common.Entity;
public interface IPrimitiveEntity : IParsedEntity
{
  /// <summary>Gets the content of the primitive entity, as it should be serialized.</summary>
  string Content { get; }
}

public interface IValueEntity : IParsedEntity
{
  string Value { get; }
}
