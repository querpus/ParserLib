//#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace Common.Extensions;

/// <summary>Extensions for HashCode objects.</summary>
public static class HashCodeExtensions
{
  extension(HashCode _)
  {
    public static int Combine<T1, T2, T3, T4, T5, T6, T7, T8, T9> (T1 v1, T2 v2, T3 v3, T4 v4, T5 v5, T6 v6, T7 v7, T8 v8, T9 v9)
    {
      int part2 = HashCode.Combine(v8, v9);
      return HashCode.Combine(v1, v2, v3, v4, v5, v6, v7, part2);
    }
    public static int Combine<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> (T1 v1, T2 v2, T3 v3, T4 v4, T5 v5, T6 v6, T7 v7, T8 v8, T9 v9, T10 v10)
    {
      int part2 = HashCode.Combine(v8, v9, v10);
      return HashCode.Combine(v1, v2, v3, v4, v5, v6, v7, part2);
    }
  }
}
