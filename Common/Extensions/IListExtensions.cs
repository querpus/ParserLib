//#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace Common.Extensions;

/// <summary><see cref="IList{T}"/> extensions.</summary>
public static class IListExtensions
{
  /// <summary>List extensions with <typeparamref name="T"/> as the item type.</summary>
  /// <typeparam name="T">The type of items in the <see cref="IList{T}"/>.</typeparam>
  /// <param name="list">The list that the methods are executing from.</param>
  extension<T> ([NotNullWhen(true)] ICollection<T>? list)
  {
    /// <summary>Checks if an <see cref="IList{T}"/> is <see langword="null"/> or empty.</summary>
    /// <returns><see langword="true"/> if the list is empty or null, <see langword="false"/> otherwise.</returns>
    public bool IsEmpty => list is null || list.Count == 0;
  }
  extension<T> (IList<T> list)
  {
    #region Queue Functions for IList<T>
    /// <summary>Adds an item to the queue.</summary>
    /// <param name="item">The item to add.</param>
    public void Enqueue (T item)
    {
      if (item is null)
        return;

      list.Add(item);
    }
    public T Dequeue ()
    {
      if (list.IsEmpty)
        throw new ArgumentOutOfRangeException(nameof(list), "Cannot Dequeue, list is empty.");

      T item = list[0];
      list.RemoveAt(0);
      return item;
    }
    #endregion Queue Functions for IList<T>
    /// <summary>Adds a range of items to an <see cref="IList{T}"/>.</summary>
    /// <param name="additions">The items to add to the <see cref="IList{T}"/>.</param>
    /// <exception cref="ANEx">The calling list was null.</exception>
    public void AddRange (IEnumerable<T> additions)
    {
      ANEx.ThrowIfNull(list);

      if (additions is null)
        return;

      foreach (T item in additions)
        list.Add(item);
    }
    public void InsertMany (int index, IEnumerable<T> additions)
    {
      list ??= [];

      if (additions is null)
        return;

      Action<T> doThing = list.Count <= index ? list.Add : i => list.Insert(index, i);

      foreach (T item in additions)
        doThing.Invoke(item);
    }
    public void RemoveLast ()
    {
      if (list.IsEmpty)
        return;

      list.RemoveAt(list.Count - 1);
    }

    /// <summary>Performs a peek as if this was a stack.</summary>
    /// <returns>The value at the top of the stack. If the list is empty, this returns <see langword="null"/> or <see langword="default"/>.</returns>
    public T? Peek () => list.IsEmpty ? default : list[^1];
    /// <summary>Performs a 'pop' action, but discards the popped item.</summary>
    public void Drop () => list?.RemoveLast();
    /// <summary>Performs a 'pop' action like a stack would.</summary>
    /// <returns>The value that was popped from the stack. If the list is empty, this returns <see langword="null"/> or <see langword="default"/>.</returns>
    public T? Pop ()
    {
      if (list.IsEmpty)
        return default;

      T item = list[^1];
      list.RemoveLast();
      return item;
    }
  }

  extension<T> (IList<T> list)
  {
    public void RemoveCount (int count, int startat = 0)
    {
      if (list is null)
        return;

      for (int i = 0; i <= count; i++)
        list.RemoveAt(startat);
    }
    public void Replace (int atIndex, IEnumerable<T> replaceWith)
    {
      if (list is null)
        return;

      list.RemoveAt(atIndex);
      list.InsertMany(atIndex, replaceWith);
    }
  }

  extension([NotNull] IList<string> list)
  {
    public void TrimEmpty ()
    {
      list ??= [];
      while (list.Remove(SE)) { }
    }
  }
}
