#pragma warning disable CA1710 // Identifiers should have correct suffix
#pragma warning disable format // Formatting

namespace Common.Entity;
/// <summary>Represents matching criteria for an indicated item, including an optional token type, a required capture group, an
/// optional exact value, and whether exact-value comparison ignores case.</summary>
/// <remarks>TokenType may be null if unspecified. Group must be present and have length > 0 for a match.
/// ExactValue may be null; when specified, the capture's value must equal ExactValue. IgnoreCase controls case
/// sensitivity when ExactValue is compared.</remarks>
public struct IndicationRule : IEquatable<IndicationRule>
{
  /// <summary>Gets or sets the token type, for example 'Bearer'.</summary>
  /// <remarks>May be null if the token type is unspecified.</remarks>
  public string? TokenType { get; set; }
  /// <summary>The group that must be present and have a length > 0.</summary>
  public string? Group { get; set; }
  /// <summary>Exact string value to match.</summary>
  public string? ExactValue { get; set; }
  /// <summary>if <see langword="true"/>, it ignores case on the exact value matching.</summary>
  public bool IgnoreCase { get; set; }

  public override readonly bool Equals (object? obj) => obj is IndicationRule item && Equals(item);
  public readonly bool Equals (IndicationRule other) => TokenType == other.TokenType && Group == other.Group && ExactValue == other.ExactValue && IgnoreCase == other.IgnoreCase;
  public override readonly int GetHashCode () => HashCode.Combine(TokenType, Group, ExactValue, IgnoreCase);
  public readonly bool Matches (Match match) =>
    (Group is null || match.Groups[Group].Success) &&
    (ExactValue is null || match.Value.Is(ExactValue));
  public static bool operator == (IndicationRule left, IndicationRule right) => left.Equals(right);
  public static bool operator != (IndicationRule left, IndicationRule right) => !(left == right);
}
