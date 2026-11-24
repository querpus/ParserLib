#pragma warning disable CA1710 // Identifiers should have correct suffix

namespace Common;

/// <summary>A section of string content.</summary>
public class Section : IEquatable<Section>, IComparable<Section>, IEquatable<Pos>, IComparable<Pos>
{
  public int Start { get; init; }
  public int Length { get; init; }
  public int End => Start + Length - 1;
  public string Content { get; init; }

  public Section (int start, int length, string input)
  {
    Start = start;
    Length = length;
    Content = input[start..End];
  }
  public Section (Capture c)
  {
    c.ThrowIfNull();
    Start = c.Index;
    Length = c.Length;
    Content = c.Value;
  }
  public override bool Equals (object? obj) =>
    (obj is Section s && Equals(s)) || (obj is Pos p && Equals(p));
  public override int GetHashCode () => HashCode.Combine(Start, Length);
  public static bool operator == (Section left, Section right) => left?.Equals(right) ?? false;
  public static bool operator != (Section left, Section right) => !(left == right);
  public bool Equals (Section? other) => Start == other?.Start && Length == other.Length;
  public bool Equals (Pos other) => Start == other.Start && Length == other.Length;
  public int CompareTo (Section? other) => Start.CompareTo(other?.Start);
  public int CompareTo (Pos other) => Start.CompareTo(other.Start);
  public override string ToString () => $"Section-{Start}-{End} : {Content}";
  public static bool operator < (Section left, Section right) => left?.CompareTo(right) < 0;
  public static bool operator <= (Section left, Section right) => left?.CompareTo(right) <= 0;
  public static bool operator > (Section left, Section right) => left?.CompareTo(right) > 0;
  public static bool operator >= (Section left, Section right) => left?.CompareTo(right) >= 0;
  public static implicit operator Pos (Section? section) => section is null ? Pos.Null : new(section.Start, section.Length);
}
