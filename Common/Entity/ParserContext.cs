#pragma warning disable CA1710 // Identifiers should have correct suffix
#pragma warning disable format // Formatting

namespace Common.Entity;
public sealed class ParsingContext
{
  public ParsingSet? ParsingSet { get; set; }
  public string? OriginText { get; set; }
  public IParsedEntity? Document { get; set; }
  public IParsedEntity? Parent { get; set; }
  public IParsedEntity? PropKey { get; set; }
  public string? Key { get; set; }
  public int Depth { get; set; }
  public int Pass { get; set; }
  

  public T? GetParentAs<T> () where T : IParsedEntity
  {
    dynamic? parent = Parent;
    return (T?) parent;
  }
  public T? GetPropKeyAs<T> () where T : IParsedEntity
  {
    dynamic? propKey = PropKey;
    return (T?) propKey;
  }
}
