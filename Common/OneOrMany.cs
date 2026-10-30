//#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace Common;

public class OneOrMany<T> : IOneOrMany<T>
{
  protected Collection<T> Items { get; } = [];
  public T? Value
  {
    get => Items.Count != 1 ? default : Items[0];
    set
    {
      Items.Clear();
      if (value is not null)
        Items.Add(value);
    }
  }

  public IEnumerable<T> Values
  {
    get => Items;
    set
    {
      Items.Clear();
      Items.AddRange(value);
    }
  }

  public int Count => Items.Count;
  public bool IsSingle => Count == 1;
  public bool IsEmpty => Count == 0;
  public bool IsMany => Count > 1;

  public static implicit operator T? (OneOrMany<T> combo) => combo.Value;
  public static implicit operator ImmutableList<T>? (OneOrMany<T> combo) => [.. combo.Values];
}

