#pragma warning disable CA1710 // Identifiers should have correct suffix
#pragma warning disable format // Formatting

namespace Common.Entity;
public interface IEntity
{
  Dictionary<string, IList<IParsedEntity>> PropertyCollections { get; }
  IList<IParsedEntity> Children { get; }
  Dictionary<string, IParsedEntity> PropertyValues { get; }
  Dictionary<string, object?> DataValues { get; }
}
