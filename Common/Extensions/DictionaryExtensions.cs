namespace Common.Extensions;

/// <summary>Extensions for Dictionary objects.</summary>
public static class DictionaryExtensions
{
  // Dictionary <TKey, TValue>
  extension<TKey, TValue> (IDictionary<TKey, TValue> idict) where TKey : notnull
  {
    public void Add ((TKey Key, TValue Value) item) =>
      idict.Add(item.Key, item.Value);
    public void Add (KeyValuePair<TKey, TValue> item) =>
      idict[item.Key] = item.Value;
    public void AddRange (IDictionary<TKey, TValue> items) =>
      idict.AddRange([.. items]);
    public void AddRange (IEnumerable<KeyValuePair<TKey, TValue>> items) =>
      items.Foreach(idict.Add);
    public void AddRange (IEnumerable<(TKey Key, TValue Value)> items) =>
      items.Foreach(item => idict.Add(item.Key, item.Value));
    public void AddRange (IEnumerable list)
    {
      foreach (var item in list)
      {
        if (item is KeyValuePair<TKey, TValue> tv)
        {
          idict.Add(tv);
        }
        else if (item is IHasInternalKey<TKey> iik && item.GetType().IsAssignableTo(typeof(TValue)))
        {
          idict.Add(iik.Key, (TValue) item);
        }
        else
        {
          throw new ArgumentException("The list was incompatible with the target IDictionary type.", nameof(list));
        }
      }
    }
  }
  // When TValue : IHasInternalKey<TKey>
  extension<TKey, TValue> (IDictionary<TKey, TValue> idict) where TKey : notnull where TValue : IHasInternalKey<TKey>
  {
    public void Add (TValue item) =>
      idict.Add(item.Key, item);
  }
}
