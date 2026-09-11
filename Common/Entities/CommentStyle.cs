#pragma warning disable CA1710 // Identifiers should have correct suffix
#pragma warning disable format // Formatting

namespace Common.Entities;
public class CommentStyle
{
  /// <summary>The character or character sequence that begins this comment.</summary>
  public required string Open { get; set; }
  /// <summary>The character or character sequence that ends this comment.</summary>
  /// <remarks>Leave <see langword="null"/> for linefeed or EOF terminated.</remarks>
  public string? Close { get; set; }
  /// <summary>The regular expression that captures this comment.</summary>
  [SS("Regex")]
  public required string Expression { get; set; }
}
