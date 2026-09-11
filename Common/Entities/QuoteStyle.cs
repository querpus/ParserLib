#pragma warning disable CA1710 // Identifiers should have correct suffix
#pragma warning disable format // Formatting

namespace Common.Entities;
public class QuoteStyle
{
  /// <summary>The character or character sequence that begins this <see langword="string"/> or <see langword="char"/>.</summary>
  public required string Open { get; set; }
  /// <summary>The character or character sequence that ends this <see langword="string"/> or <see langword="char"/>.</summary>
  /// <remarks>Leave this as <see langword="null"/> if it is the same as open.</remarks>
  public string? Close { get; set; }
  /// <summary>The character or character sequence that prevents this <see langword="string"/> or <see langword="char"/> from ending.</summary>
  /// <remarks>Leave this as <see langword="null"/> if there is no escape character.</remarks>
  public string? Escape { get; set; }
  /// <summary>The regular expression that captures this <see langword="string"/> or <see langword="char"/>.</summary>
  [SS("Regex")]
  public required string Expression { get; set; }
}
