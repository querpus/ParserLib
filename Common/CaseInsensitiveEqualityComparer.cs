#pragma warning disable CA1710 // Identifiers should have correct suffix
#pragma warning disable format // Formatting

namespace Common;
public sealed class CaseInsensitiveEqualityComparer : IEqualityComparer<string>, IEqualityComparer, IComparer<string>
{
  private CaseInsensitiveEqualityComparer () { }
  public StringComparison ComparisonType { get; init; } = StringComparison.OrdinalIgnoreCase;
  public static CaseInsensitiveEqualityComparer Ordinal { get; } = new CaseInsensitiveEqualityComparer();
  public static CaseInsensitiveEqualityComparer Invariant { get; } = new CaseInsensitiveEqualityComparer()
  {
    ComparisonType = StringComparison.InvariantCultureIgnoreCase
  };
  public bool Equals (string? x, string? y) => string.Equals(x, y, ComparisonType);
  public new bool Equals (object? x, object? y) =>
    x is string sx && y is string sy
      ? Equals(sx, sy)
      : x is null && y is null;
  public int GetHashCode (string obj) => obj.ToLowerInvariant().GetHashCode();
  public int GetHashCode (object obj) =>
    obj is null
    ? 0
    : obj is string x
      ? GetHashCode(x)
      : obj.GetHashCode();
  public int Compare (string? x, string? y) => x is not null
    ? x.CompareTo(y, ComparisonType)
    : y is null ? 0 : -1;
}
