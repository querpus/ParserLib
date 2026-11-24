#pragma warning disable CS1591 // XML Documentation

namespace Common.Extensions;

public static class SortedSetExtensions
{
  extension(SortedSet<char>? set)
  {
    public string ToString ()
    {
      string s = SE;
      foreach (char c in set ?? [])
        s += c;
      return s;
    }
  }

  extension([NotNull] SortedSet<char> set)
  {
    public void Add (string chars) => set.UnionWith(chars);
  }
}

public static class BitArrayExtensions
{
  extension(BitArray array)
  {
    public void SetRange (int start, int length)
    {
      for (int i = start; i < start + length; i++)
        array.Set(i, true);
    }
    public void SetRange (Range range) => array.SetRange(range.Start, range.End);
    public void SetRange (Index start, Index end)
    {
      for (
        int i = start.IsFromEnd
          ? array.Length - start.Value
          : start.Value;
        i <= (end.IsFromEnd
          ? array.Length - end.Value
          : end.Value);
        i++)
      {
        array.Set(i, true);
      }
    }
    public void SetRange (Pos pos)
    {
      for (int i = pos.Start; i < pos.Start + pos.Length; i++)
        array.Set(i, true);
    }
    public void UnSetRange (int start, int length)
    {
      for (int i = start; i < start + length; i++)
        array.Set(i, false);
    }
    public void UnSetRange (Pos pos)
    {
      for (int i = pos.Start; i < pos.Start + pos.Length; i++)
        array.Set(i, false);
    }
  }
}
