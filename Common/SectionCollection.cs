//#pragma warning disable CA1710 // Identifiers should have correct suffix

using Common.Extensions;

namespace Common;

/// <summary>Now uses Pos struct for speed.</summary>
public sealed class SectionCollection (string full_text) : ICollection<Pos>
{
  private List<Pos> _sections = [];
  private BitArray BitArray { get; init; } = new(full_text.Length);
  public string FullText { get; } = full_text;
  public int TextLength => FullText?.Length ?? DNE;
  public int Count => _sections.Count;
  bool ICollection<Pos>.IsReadOnly => false;

  public Pos this[int index] => _sections[index].ToSection(FullText ?? SE);
  public string GetText (Pos index) => $"{FullText[index]}";
  private void Compress ()
  {
    _sections.Sort();
    List<Pos> merged = [];
    foreach (Pos s in _sections)
    {
      if (merged.Count == 0 || merged[^1].End + 1 < s.Start)
        merged.Add(s);
      else
        merged[^1] = new Pos(merged[^1].Start, Math.Max(merged[^1].End, s.End) - merged[^1].Start + 1);
    }
    _sections = merged;
  }
  public bool IsWithin (int point) => _sections.Any(item => item.IsWithin(point));
  public bool Overlaps (Pos section) => _sections.Any(ea => ea.Start <= section.End && ea.End >= section.Start);
  public void Add (Pos section)
  {
    _sections.Add(section);

    for (int i = section.Start; i < section.Length; i++)
    {
      BitArray.Set(i, true);
    }

    Compress();
  }
  public void Add (int start, int length)
  {
    _sections.Add(new(start, length));

    for (int i = 0; i < length; i++)
    {
      BitArray.Set(start + i, true);
    }

    Compress();
  }
  public SectionCollection Inverse ()
  {
    int start = DNE;
    SectionCollection result = new(FullText)
    {
      BitArray = BitArray.Not()
    };
    for (int i = 0; i < TextLength; i++)
    {
      List<Pos> relevant_sections = [.. _sections.Where(s => s.Start <= i && s.End >= i)];
      if (relevant_sections.Any(s => s.IsWithin(i)))
      {
        if (start != DNE)
        {
          result.Add(start, i - start);
          start = DNE;
        }
      }
      else if (start == DNE)
      {
        start = i;
      }
    }
    if (start != DNE)
      result.Add(start, TextLength - start);
    return result;
  }
  public void Clear () => _sections.Clear();
  bool ICollection<Pos>.Contains (Pos item) => _sections.Any(i => i.Equals(new(item.Start, item.Length)));
  void ICollection<Pos>.CopyTo (Pos[] array, int arrayIndex) => _sections.CopyTo(array, arrayIndex);
  public IEnumerator<Pos> GetEnumerator () => _sections.GetEnumerator();
  public bool Remove (Pos item)
  {
    bool rem = item.Start != DNE && _sections.Remove(new(item.Start, item.Length));
    if (rem)
    {
      for (int b = item.Start; b < item.Length; b++)
        BitArray.Set(b, false);
    }
    return rem;
  }
  IEnumerator IEnumerable.GetEnumerator () => GetEnumerator();
  public void AddRange (IEnumerable<Pos> children) => children.Foreach(Add);
}
