using Common.RegExp;

using SysRegex = System.Text.RegularExpressions.Regex;

namespace Common;
/// <summary>A node that defines a string replacement operation.</summary>
public sealed class ReplaceNode : IEquatable<ReplaceNode>, IEquatable<IProperty<string?>>, IProperty<string?>
{
  #region Static Members
  /// <summary>Creates a <see cref="ReplaceNode"/> from the specified parameters.</summary>
  /// <param name="lf">The string to look for.</param>
  /// <param name="rw">The string to replace with</param>
  /// <returns>A new <see cref="ReplaceNode"/> object.</returns>
  public static ReplaceNode From ([SS("regex")] string lf, string? rw) => new(lf, rw);
  public static ReplaceNode From ((string LookFor, string? ReplaceWith) tp) => new(tp.LookFor, tp.ReplaceWith);
  public static ReplaceNode From (KeyValuePair<string, string?> kvp) => new(kvp.Key, kvp.Value);
  public static ReplaceNode From (IProperty<string?> prop)
  {
    prop.ThrowIfNull();
    return new(prop.Key, prop.Value);
  }

  public static implicit operator ReplaceNode ((string LookFor, string? ReplaceWith) tuple) => From(tuple);
  public static implicit operator ReplaceNode (KeyValuePair<string, string?> kvp) => From(kvp);
  public static implicit operator ReplaceNode (Tuple<string, string?> prop) => From(prop.ToValueTuple());
  public static implicit operator KeyValuePair<string, string?> ([NotNull] ReplaceNode node) => node.ToKVP();
  #endregion
  /// <summary>The regular expression to look for.</summary>
  public RxS LookFor { get; private set; }
  /// <summary>The string to replace matches with. If <see langword="null"/>, matches will be removed.</summary>
  public string? ReplaceWith { get; private set; }
  string IProperty<string?>.Key { get => LookFor; set => LookFor = value; }
  string? IProperty<string?>.Value { get => ReplaceWith; set => ReplaceWith = value; }

  /// <summary>Creates an empty node.</summary>
  private ReplaceNode ()
  {
    LookFor = SE;
    ReplaceWith = null;
  }
  public ReplaceNode ([SS("Regex")] string lookFor, string? replaceWith)
  {
    LookFor = lookFor;
    ReplaceWith = replaceWith;
  }
  public ReplaceNode ((string LookFor, string? ReplaceWith) tuple)
  {
    LookFor = tuple.LookFor;
    ReplaceWith = tuple.ReplaceWith;
  }
  public ReplaceNode (KeyValuePair<string, string?> kvp)
  {
    LookFor = kvp.Key;
    ReplaceWith = kvp.Value;
  }

  public KeyValuePair<string, string?> ToKVP () => new(LookFor, ReplaceWith);
  /// <summary>Checks if the 2 nodes are equal to each other.</summary>
  /// <param name="other">The other <see cref="ReplaceNode"/>.</param>
  /// <returns><see langword="true"/> if the <see cref="ReplaceNode"/> objects are equal to each other, otherwise <see langword="false"/>.</returns>
  public bool Equals (ReplaceNode? other) =>
    other is not null && LookFor == other.LookFor && ReplaceWith == other.ReplaceWith;

  public bool Equals (ReplaceNode? other, StringComparison sc) =>
    LookFor.Content.Equals(other?.LookFor, sc) && (
      (ReplaceWith is not null &&
      other.ReplaceWith is not null &&
      ReplaceWith.Equals(other.ReplaceWith, sc)) || (ReplaceWith is null && other.ReplaceWith is null));

  /// <inheritdoc/>
  public override bool Equals (object? obj) => Equals(obj as IProperty<string>, SCO);
  /// <inheritdoc/>
  public override int GetHashCode () => HashCode.Combine(LookFor, ReplaceWith);

  /// <summary>Executes the replace on a given string.</summary>
  /// <param name="input">The string to operate on.</param>
  /// <param name="options">The regex options.</param>
  /// <param name="timeOut">The <see cref="TimeSpan"/> to limit the operation to. Defaults to 5 seconds.</param>
  /// <returns>The modified <see langword="string"/>.</returns>
  public string ReplaceRegex (string input, RegexOptions options, TimeSpan? timeOut = null) =>
    SysRegex.Replace(input, LookFor, ReplaceWith ?? SE, options, timeOut ?? TimeSpan.FromSeconds(5));
  /// <summary>Recursively replaces text literallly, and does not interpret the string as a regular expression.</summary>
  /// <param name="input">The text to operate on.</param>
  /// <param name="sc"><see cref="StringComparison"/> properties.</param>
  /// <returns>A <see cref="string"/> with the text replaced as directed.</returns>
  public string ReplaceText (string input, StringComparison sc = SCO) => input.RecursiveReplace(LookFor, ReplaceWith ?? SE, sc);
  public bool Equals (IProperty<string?>? other) =>
    LookFor.Content.Equals(other?.Key, SCO) && (
      (ReplaceWith is not null &&
      other.Value is not null &&
      ReplaceWith.Equals(other.Value, SCO)) || (ReplaceWith is null && other.Value is null));
  public int CompareTo (IProperty<string?>? other)
  {
    int keys = LookFor.CompareTo(other?.Key);
    return keys == 0 ? ReplaceWith?.CompareTo(other?.Value, SCO) ?? 1 : keys;
  }

  public static bool operator == (ReplaceNode left, ReplaceNode right) => left is null ? right is null : left.Equals(right, SCO);
  public static bool operator != (ReplaceNode left, ReplaceNode right) => !(left == right);
  public static bool operator < (ReplaceNode left, ReplaceNode right) => left is null ? right is not null : left.CompareTo(right) < 0;
  public static bool operator <= (ReplaceNode left, ReplaceNode right) => left is null || left.CompareTo(right) <= 0;
  public static bool operator > (ReplaceNode left, ReplaceNode right) => left?.CompareTo(right) > 0;
  public static bool operator >= (ReplaceNode left, ReplaceNode right) => left is null ? right is null : left.CompareTo(right) >= 0;
}
