//#pragma warning disable CA1710 // Identifiers should have correct suffix

namespace Common;

/// <summary>A lightweight variant of section that holds a start position and a length for operating on a <see cref="Span{T}"/>, <see cref="Memory{T}"/> or <see langword="string"/>.</summary>
/// <param name="start">The start position.</param>
/// <param name="length">The length.</param>
public readonly struct Pos (int start, int length) : IEquatable<Pos>, IIndexSortable
{
  public int Start { get; } = start;
  public int Length { get; init; } = length;
  public int End
  {
    readonly get => Start + Length - 1;
    init
    {
      Length = value + 1 - Start;

      if (Length < 0) throw new ArgumentOutOfRangeException(nameof(value));
    }
  }
  public readonly bool IsNull => Start == DNE;
  public static Pos Null { get; } = new(DNE, 0);
  readonly int IIndexSortable.Index => Start;

  public readonly Section ToSection (string full_text) => new(Start, Length, full_text);
  public readonly bool IsWithin (int point) => point >= Start && point <= End;
  public readonly bool Overlaps (Pos other) => other.Start <= End && other.End >= Start;
  public readonly bool Equals (Pos other) => Start == other.Start && Length == other.Length;
  public override readonly bool Equals ([NotNullWhen(true)] object? obj) => obj is Pos p && Equals(p);
  public override readonly int GetHashCode () => HashCode.Combine(Start, Length);
  public readonly int CompareTo (IIndexSortable? other) => Start.CompareTo(other?.Index);
  readonly int IComparable.CompareTo (object? other) => CompareTo(other is IIndexSortable isort ? isort : null);
  public override string ToString () => IsNull ? "Null Pos" : $"Pos {Start}x{Length}";
  public static bool operator == (Pos left, Pos right) => left.Equals(right);
  public static bool operator != (Pos left, Pos right) => !(left == right);
  public static bool operator < (Pos left, Pos right) => left.CompareTo(right) < 0;
  public static bool operator <= (Pos left, Pos right) => left.CompareTo(right) <= 0;
  public static bool operator > (Pos left, Pos right) => left.CompareTo(right) > 0;
  public static bool operator >= (Pos left, Pos right) => left.CompareTo(right) >= 0;
}
