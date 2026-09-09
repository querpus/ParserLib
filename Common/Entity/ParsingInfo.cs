#pragma warning disable CA1710 // Identifiers should have correct suffix
#pragma warning disable format // Formatting

namespace Common.Entity;
public struct ParsingInfo : IEquatable<ParsingInfo>
{
  /// <summary>The kind of object the end result of the operations should be.</summary>
  /// <remarks> This is for validation purposes.<br/>
  /// If <see langword="true"/>,The parser will make a single <see cref="IParsedEntity"/> object from the data.<br/>
  /// If <see langword="false"/>, the parser will make a <see cref="Collection{T}"/> of <see cref="IParsedEntity"/> objects.</remarks>
  public bool GeneratesSingleObject { get; set; }
  /// <summary>The number of iteritive loops the parser must go through.</summary>
  public int TotalPasses { get; set; }
  /// <summary>Whether to ignore case on non-regex matches.</summary>
  public bool IgnoreCase { get; set; }
    public bool PropertyByDepth { get; internal set; }

    public readonly bool Equals (ParsingInfo other) =>
    GeneratesSingleObject == other.GeneratesSingleObject &&
    TotalPasses == other.TotalPasses &&
    IgnoreCase == other.IgnoreCase;
  public override readonly bool Equals (object? obj) =>
    obj is ParsingInfo other && Equals(other);
  public override readonly int GetHashCode () =>
    HashCode.Combine(GeneratesSingleObject, TotalPasses, IgnoreCase);
  public static bool operator == (ParsingInfo left, ParsingInfo right) => left.Equals(right);
  public static bool operator != (ParsingInfo left, ParsingInfo right) => !(left == right);
}
