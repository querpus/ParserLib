#pragma warning disable CA1710 // Identifiers should have correct suffix
#pragma warning disable format // Formatting

namespace Common.Entity;
public interface IEntity
{
  IDictionary<string, IList<IParsedEntity>> PropertyCollections { get; }
  IList<IParsedEntity> Children { get; }
  IDictionary<string, IParsedEntity> PropertyValues { get; }
  IDictionary<string, object?> DataValues { get; }
}
