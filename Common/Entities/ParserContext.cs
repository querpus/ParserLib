#pragma warning disable CA1710 // Identifiers should have correct suffix
#pragma warning disable format // Formatting

namespace Common.Entities;
public sealed class ParsingContext
{
  public ParsingSet? ParsingSet { get; set; }
  public string? OriginText { get; set; }
  public DocumentEntity? Document { get; set; }
  public IEntity? Parent { get; set; }
  public Collection<IEntity> PropKeys { get; } = [];
  public string? Key { get; set; }
  public int Depth { get; set; }
  public int CurrentIndex { get; set; }
  [AllowNull]
  public dynamic CurrentItem { get; set; }

  public void Descend (int amt, IEntity? propkey = null, IEntity? child = null)
  {
    Depth += amt;
    if (propkey is not null)
    {
      if (Depth >= PropKeys.Count)
      {
        PropKeys.Add(propkey);
      }
      else
      {
        PropKeys[Depth] = propkey;
      }
    }
    if (child is not null)
    {
      if (Parent is null)
        child.SetParent(Document!);
      else
        child.SetParent(Parent);
      Parent = child;
    }
  }
  public void Ascend (int amt)
  {
    int adj =  Depth - amt;

    if (adj < 0)
    {
      Debug.Log(MsgClass.Warning, $"Depth was {adj}, clamping at 0.", this);
    }

    Depth = Math.Clamp(adj, 0, 0x7fff);
    Parent = Parent?.Parent;

  }
  public T? GetParentAs<T> () where T : IEntity
  {
    dynamic? parent = Parent;
    return (T?) parent;
  }
  public T? GetPropKeyAs<T> () where T : IEntity
  {
    dynamic? propKey = PropKeys[Depth];
    return (T?) propKey;
  }
}
