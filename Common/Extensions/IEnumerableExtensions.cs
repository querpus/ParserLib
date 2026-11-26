//#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using Common.RegExp;

namespace Common.Extensions;

/// <summary><see cref="IEnumerable"/>, <see cref="IReadOnlyCollection{T}"/> and <see cref="IEnumerable{T}"/> Extensions</summary>
public static class IEnumerableExtensions
{
  /// <summary>Generic IEnumerable Extensions</summary>
  /// <typeparam name="TBaseType">The base object type</typeparam>
  extension<TBaseType> (IEnumerable<TBaseType> list) where TBaseType : class
  {
    public IEnumerable<TBaseType> OfTypes<TAllowedType1, TAllowedType2> ()
      where TAllowedType1 : TBaseType
      where TAllowedType2 : TBaseType =>
      Restrict(list, [typeof(TAllowedType1), typeof(TAllowedType2)], must_assign: true);
    public IEnumerable<TBaseType> NotOfType<TDisallowedType> ()
      where TDisallowedType : TBaseType =>
      Restrict(list, [typeof(TDisallowedType)]);
    public IEnumerable<TBaseType> NotOfTypes<TDisallowedType1, TDisallowedType2> ()
      where TDisallowedType1 : TBaseType
      where TDisallowedType2 : TBaseType =>
      Restrict(list, [typeof(TDisallowedType1), typeof(TDisallowedType2)]);
    private IEnumerable<TBaseType> Restrict ([NotNull] IEnumerable<Type> types, bool must_assign = false)
    {
      if (types.IsEmpty)
        return list;
      Collection<TBaseType> result = [];
      foreach (TBaseType item in list)
      {
        bool assigned = false;
        Type item_type = item.GetType();
        foreach (Type cannot_be in types)
        {
          if (item_type.IsAssignableTo(cannot_be))
          {
            assigned = true;
            break;
          }
        }
        if (assigned == must_assign)
        {
          result.Add(item);
        }
      }
      return [.. result];
    }
  }
  /// <summary>Generic IEnumerable Extensions</summary>
  /// <typeparam name="T">The object type</typeparam>
  extension<T> (IEnumerable<T> list)
  {
    /// <summary>Allows indexing for ienumerable types.</summary>
    /// <param name="list">The list.</param>
    /// <param name="index">The index to retrieve.</param>
    /// <returns>The object at position <paramref name="index"/>.</returns>
    public T At (int index) => list.ToArray()[index];
    public T At (Index index) => list.ToArray()[index];
    public T this[Index i] => list.At(i);
    public T this[int i] => list.At(i);
    public string TextJoin (string separator = EmptyString)
    {
      if (list is null)
        return SE;

      string result = SE;

      foreach (T? item in list)
      {
        string itemString = item?.ToString()!;
        if (itemString.IsEmpty)
          continue;

        if (result.IsNotEmpty)
          result += separator;
        result += itemString;
      }
      return result;
    }
    public string TextJoin (Func<T, string> selector, string separator = EmptyString)
    {
      if (list is null)
        return SE;

      string result = SE;

      foreach (T item in list)
      {
        string itemString = selector(item);
        if (itemString.IsEmpty)
          continue;

        if (result.IsNotEmpty)
          result += separator;
        result += itemString;
      }
      return result;
    }
    /// <summary>The position of the last item.</summary>
    /// <remarks>If the list is empty, this returns -1.</remarks>
    public int LastIndex => list.Count() - 1;
    public bool IsEmpty => list?.Any() != true;
    /// <summary>Executes an <see cref="Action{T}"/> <see langword="delegate"/> for every item in the <see cref="IEnumerable{T}"/>.</summary>
    /// <param name="action"></param>
    public void Foreach (Action<T> action)
    {
      foreach (T? item in list)
      {
        action(item);
      }
    }
    public void Foreach (Action<T, int> action)
    {
      int index = 0;
      foreach (T? item in list)
      {
        action(item, index++);
      }
    }
  }
  extension<T> (IReadOnlyCollection<T>? list)
  {
    /// <summary>The position of the last item.</summary>
    /// <remarks>If the list is empty, this returns -1.</remarks>
    public int LastIndex => (list?.Count ?? 0) - 1;
  }
  extension<T> (IEnumerable<IEnumerable<T>> listlist)
  {
    public Collection<T> Flatten () =>
      [.. listlist.Aggregate((list1, list2) => list1.Concat(list2))];
  }
  extension(IEnumerable list)
  {
    /// <summary>Iterates the IEnumerable, and counts the items.</summary>
    public int ICount
    {
      get
      {
        int i = 0;
        foreach (var _ in list)
        {
          i++;
        }
        return i;
      }
    }

    public string TextJoin (string separator = EmptyString)
    {
      if (list.IsEmpty)
        return SE;

      string result = SE;
      foreach (object item in list)
      {
        string itemString = item?.ToString()!;
        if (itemString.IsEmpty)
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
  extension([NotNullWhen(true)] IEnumerable? list)
  {
    public bool IsNotEmpty => list?.AsCollection<object>().Count > 0;
  }
  extension(IEnumerable<string> list)
  {
    public RxS AggregateRegex () => list.TextJoin("|");
  }
  extension(IEnumerable<ReplaceNode> nodes)
  {
    public string ReplaceByNodes (string input, StringComparison sc) => nodes.Aggregate(input, (text, node) => text = node.ReplaceText(text, sc));
  }
  extension(IEnumerable<IPrintable> items)
  {
    public void Print (int indent) => items.Foreach(i => i.Print(indent));
  }
  extension(IEnumerable<ISerializable> items)
  {
    public string Serialize (string delimiter = "") => items.TextJoin(delimiter);
  }
}
