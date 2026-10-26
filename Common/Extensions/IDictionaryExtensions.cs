//#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace Common.Extensions;

public static class IDictionaryExtensions
{
  extension(IDictionary<string, string> dic)
  {
    public bool ContainsKey (IEnumerable<string> list) =>
      dic is not null && list?.Any(dic.ContainsKey) == true;
    public void Print (int indent)
    {
      var str_indent = new string(' ', indent);
      var child_indent = new string(' ', indent + 2);
      Debug.Log(MsgClass.GreenInfo, str_indent + "Dictionary", dic);
      foreach (var item in dic)
      {
        Debug.Log(MsgClass.GreenInfo, child_indent + item.Key + " : " + item.Value, dic);
      }
    }
  }
  extension(IDictionary<string, object> dic)
  {
    public bool ContainsKey (IEnumerable<string> list) =>
      dic is not null && list?.Any(dic.ContainsKey) == true;
    public void Print (int indent)
    {
      var str_indent = new string(' ', indent);
      var child_indent = new string(' ', indent + 2);
      Debug.Log(MsgClass.GreenInfo, str_indent + "Dictionary", dic);
      foreach (var item in dic)
      {
        Debug.Log(MsgClass.GreenInfo, child_indent + item.Key + " : " + item.Value, dic);
      }
    }
  }
  extension<TKey> (IDictionary<TKey, object> dic) where TKey : notnull
  {
    public bool TryGetValueAs<TValue> (TKey key, [MaybeNullWhen(false)][NotNullWhen(true)] out TValue? value)
    {
      bool result = dic.TryGetValue(key, out object? temp);

      value = (TValue?) temp;

      return value is not null && result;
    }
  }
  extension<TKey, TValue> (IDictionary<TKey, TValue> dic) where TKey : notnull
  {
    public bool ContainsKey (IEnumerable<TKey> list) =>
      dic is not null && list?.Any(dic.ContainsKey) == true;
    public IDictionary<TKey, TValue> Concat (IDictionary<TKey, TValue> addition, bool overwrite = true)
    {
      Dictionary<TKey, TValue> result = [];

      if (dic is not null)
      {
        foreach (KeyValuePair<TKey, TValue> kvp in dic)
          result[kvp.Key] = kvp.Value;
      }

      if (addition is not null)
      {
        foreach (KeyValuePair<TKey, TValue> kvp in addition)
        {
          if (overwrite || !result.ContainsKey(kvp.Key))
            result[kvp.Key] = kvp.Value;
        }
      }

      return result;
    }
  }
}
