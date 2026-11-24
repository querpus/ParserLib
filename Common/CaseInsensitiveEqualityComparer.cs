#pragma warning disable CA1710 // Identifiers should have correct suffix
#pragma warning disable format // Formatting

namespace Common;
public sealed class CaseInsensitiveEqualityComparer : EqualityComparer<string>, IEqualityComparer<char>
{
  private CaseInsensitiveEqualityComparer (StringComparison sc) => ComparisonType = sc;
  public StringComparison ComparisonType { get; init; }
  public static CaseInsensitiveEqualityComparer Ordinal { get; } = new(SCOIC);
  public static CaseInsensitiveEqualityComparer Invariant { get; } = new(SCIIC);
  public override bool Equals (string? x, string? y) => string.Equals(x, y, ComparisonType);
  public bool Equals (char x, char y) => $"{x}".Equals($"{y}", ComparisonType);
  public int Compare (string? x, string? y) => x is not null ? x.CompareTo(y, ComparisonType) : y is null ? 0 : -1;
  public override int GetHashCode ([DisallowNull] string obj) => obj.GetHashCode(ComparisonType);
  public int GetHashCode ([DisallowNull] char obj) => obj;
}
