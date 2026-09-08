//#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using Common.RegExp;

namespace Common.Extensions;

public static class IEnumerableExtensions
{
  /// <summary>Generic IEnumerable Extensions</summary>
  /// <typeparam name="T">The object type</typeparam>
  extension<T> (IEnumerable<T> list)
  {
    public IEnumerable<T> OfTypes<TAllowedType1, TAllowedType2> ()
    {
      foreach (T? item in list)
      {
        if (item is TAllowedType1 or TAllowedType2)
          yield return item;
      }
    }
    public IEnumerable<T> NotOfType<TDisallowedType> ()
    {
      foreach (T? item in list)
      {
        if (item is not TDisallowedType)
          yield return item;
      }
    }
    /// <summary>Allows indexing for ienumerable types.</summary>
    /// <param name="list">The list.</param>
    /// <param name="index">The index to retrieve.</param>
    /// <returns>The object at position <paramref name="index"/>.</returns>
    public T At (int index) => list.ToArray()[index];
    public Collection<T> ToCollection () => [.. list];
    public string TextJoin (string separator = EmptyString)
    {
      string result = string.Empty;

      if (list is null)
        return result;

      foreach (T? item in list)
      {
        string? itemString = item?.ToString();
        if (string.IsNullOrEmpty(itemString))
          continue;

        if (result.IsNotEmpty)
          result += separator;
        result += itemString;
      }
      return result;
    }
    public int LastIndex => list.Count() - 1;
    public bool IsEmpty => list?.Any() != true;
    public void Foreach (Action<T> action)
    {
      foreach (T? item in list)
      {
        action(item);
      }
    }
  }
  extension(IEnumerable list)
  {
    public int ICount => list.AsCollection<object>().Count;
  }

  extension<T> (IReadOnlyCollection<T>? list)
  {
    public int LastIndex => list is null ? -1 : list.Count - 1;
  }

  extension<T> (IEnumerable<IEnumerable<T>> listlist)
  {
    public Collection<T> Flatten () =>
      [.. listlist.Aggregate((list1, list2) => list1.Concat(list2))];
  }

  extension(IEnumerable list)
  {
    // IEnumerable

    public string TextJoin (string separator = EmptyString)
    {
      if (list is null)
        return SE;

      string result = SE;
      foreach (object item in list)
      {
        string? itemString = item?.ToString();
        if (string.IsNullOrEmpty(itemString))
          continue;
        if (result.IsNotEmpty)
          result += separator;
        result += itemString;
      }
      return result;
    }
    public Collection<string> ToStringCollection () => [.. list.Cast<string>()];
  }

  extension([NotNullWhen(false)] IEnumerable? list)
  {
    public bool IsEmpty => list is null || list.AsCollection<object>().Count == 0;
  }

  extension(IEnumerable<string> list)
  {
    public RxS AggregateRegex () => list.TextJoin("|");
  }

  extension(IEnumerable<ReplaceNode> nodes)
  {
    public string ReplaceByNodes (string input, StringComparison sc) => nodes.Aggregate(input, (text, node) => text = node.ReplaceText(text, sc));
  }
}
