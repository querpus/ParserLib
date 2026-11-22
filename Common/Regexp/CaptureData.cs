//#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using Common.RegExp;

namespace Common.RegExp;

/// <summary>Data for one capture.</summary>
public class CaptureData : IMatchItem, IEquatable<CaptureData>
{
  /// <inheritdoc/>
  public int Index { get; init; } = DNE;
  /// <inheritdoc/>
  public int Len { get; init; }
  /// <inheritdoc/>
  public int AfterIndex => Index + Len;
  /// <inheritdoc/>
  public int FinalIndex => AfterIndex - 1;
  /// <inheritdoc/>
  public string Content { get; init; }
  /// <inheritdoc/>
  public virtual string Name { get; }
  /// <summary>The Capture index in the group.</summary>
  public virtual int CaptureIndex { get; init; }
  /// <inheritdoc/>
  public virtual bool IsNull => Index < 0 || Len <= 0 || Content.Length == 0;

  internal CaptureData ()
  {
    Content = SE;
    Name = SE;
  }
  /// <summary>Constructor from a <see cref="Capture"/>.</summary>
  /// <param name="c">The <see cref="Capture"/> to make this object from.</param>
  /// <param name="groupName">The name of the group.</param>
  /// <param name="index">The index of the capture.</param>
  public CaptureData (Capture? c, string groupName = EmptyString, int index = -1)
  {
    if (c is null)
    {
      Index = -1;
      Len = 0;
      Content = SE;
    }
    else
    {
      Index = c.Index;
      Len = c.Length;
      Content = c.Value;
    }
    CaptureIndex = index;
    Name = groupName;
  }
  /// <summary>Manual constructor.</summary>
  /// <param name="content">The text of the capture.</param>
  /// <param name="pos">The position in the string.</param>
  /// <param name="len">The length of the string.</param>
  /// <param name="name">The name of the group.</param>
  /// <param name="index">The index of the capture.</param>
  protected CaptureData (string content, int pos, int len, string name, int index)
  {
    Name = name;
    Content = content;
    Index = pos;
    Len = len;
    CaptureIndex = index;
  }

  public override string ToString () => Content.ContainsAny(SCO, Chars.NewLines)
      ? $"\"{Content.Replace([Chars.CRLF, Chars.CRs, Chars.LFs], "<NL>")}\" @ {Index} ({Len})"
      : $"\"{Content}\" @ {Index} ({Len})";
  public bool Equals (CaptureData? other) => other is not null && Index == other.Index && Name == other.Name && CaptureIndex == other.CaptureIndex && Content.Is(other.Content);
  public override bool Equals (object? obj) => Equals(obj as CaptureData);
  public override int GetHashCode () => HashCode.Combine(Index, CaptureIndex, Name, Content);
  public int CompareTo (IIndexSortable? other) => Index.CompareTo(other?.Index);
  public int CompareTo (object? obj) => CompareTo(obj as IIndexSortable);

  public static bool operator == (CaptureData left, CaptureData right) => left is null ? right is null : left.Equals(right);
  public static bool operator != (CaptureData left, CaptureData right) => !(left == right);
  public static bool operator < (CaptureData left, CaptureData right) => left is null ? right is not null : left.CompareTo(right) < 0;
  public static bool operator <= (CaptureData left, CaptureData right) => left is null || left.CompareTo(right) <= 0;
  public static bool operator > (CaptureData left, CaptureData right) => left?.CompareTo(right) > 0;
  public static bool operator >= (CaptureData left, CaptureData right) => left is null ? right is null : left.CompareTo(right) >= 0;
}
