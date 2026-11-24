//#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace Common.RegExp;

/// <summary>A box that contains a string representing a regular expression. Basically just an extension of <see cref="string"/>.</summary>
/// <seealso cref="string" />
public readonly struct RxS : IEquatable<string>, IComparable<string>, IEquatable<RxS>
{
  /// <summary>Gets the string value of this regular expression.</summary>
  /// <value>The content of this regular expression.</value>
  public string Content { get; init; }
  /// <summary>Gets the length of the content.</summary>
  /// <value>The length of the content.</value>
  public int Length => Content.Length;
  /// <summary>Retrieves the regular expression stored.</summary>
  /// <returns>The regular expression as a string.</returns>
  public override string ToString () => Content;
  public int CompareTo ([SS("Regex")] string? other) => Content.CompareTo(other, SCO);

  public static implicit operator string (RxS rx) => rx.Content;
  public static implicit operator RxS ([SS("Regex")] string str) => Rx(str);
  /// <summary>Concatenates 2 regular expressions.</summary>
  /// <param name="rx1">The left expression.</param>
  /// <param name="rx2">The right expression.</param>
  /// <returns>A regular expression formed from the 2 regular expressions.</returns>
  public static RxS operator + (RxS rx1, RxS rx2) => $"{rx1}{rx2}";
  /// <summary>Concatenates 2 regular expressions.</summary>
  /// <param name="rx1">The left expression.</param>
  /// <param name="rx2">The right expression.</param>
  /// <returns>A regular expression formed from the 2 regular expressions.</returns>
  public static RxS operator + (RxS rx1, [SS("Regex")] string rx2) => $"{rx1}{rx2}";
  public static RxS operator ! (RxS rx1) => NegLkAhd(rx1);
  /// <summary>Negative lookahead shorthand.</summary>
  /// <param name="rx1">Lefthand regular expression.</param>
  /// <param name="rx2">Righthand regular expression.</param>
  /// <returns>A regular expression that is a joining of the 2 expressions.</returns>
  public static RxS operator >> (RxS rx1, [SS("Regex")] string rx2) => rx1 + NegLkAhd(rx2);
  /// <summary>Negative lookbehind shorthand.</summary>
  /// <param name="rx1">Lefthand regular expression.</param>
  /// <param name="rx2">Righthand regular expression.</param>
  /// <returns>A regular expression that is a joining of the 2 expressions.</returns>
  public static RxS operator << (RxS rx1, [SS("Regex")] string rx2) => rx1 + NegLkBhd(rx2);
  /// <summary>Positive lookahead shorthand.</summary>
  /// <param name="rx1">Lefthand regular expression.</param>
  /// <param name="rx2">Righthand regular expression.</param>
  /// <returns>A regular expression that is a joining of the 2 expressions.</returns>
  public static RxS operator >= (RxS rx1, [SS("Regex")] string rx2) => rx1 + PosLkAhd(rx2);
  /// <summary>Positive lookbehind shorthand.</summary>
  /// <param name="rx1">Lefthand regular expression.</param>
  /// <param name="rx2">Righthand regular expression.</param>
  /// <returns>A regular expression that is a joining of the 2 expressions.</returns>
  public static RxS operator <= (RxS rx1, [SS("Regex")] string rx2) => rx1 + PosLkBhd(rx2);
  /// <summary>Adds the lazy quantifier to the expression.</summary>
  /// <value><c>?</c></value>
  public RxS Lazy => $"{Content}?";
  /// <summary>Adds a 'zero or many' quantifier to the expression.</summary>
  /// <value><c>*</c></value>
  public RxS Any => $"{Grp(Content)}*";
  /// <summary>Adds a 'one or many' quantifier to the expression.</summary>
  public RxS Many => $"{Grp(Content)}+";
  /// <summary>Adds an optional quantifier to the expression.</summary>
  public RxS Opt => $"{Grp(Content)}?";

  public RxS Bk => @$"{Content}\b";
  public RxS End => $"{Content}$";

  public RxS WS => @$"{Content}\s+";
  public RxS WSO => @$"{Content}\s*";

  public RxS Or ([SS("Regex")] string content) => $"{Content}|{Grp(content)}";
  public RxS Or (IEnumerable<string> list) => $"{Content}|{Grp(list.TextJoin("|"))}";
  public static RxS Or ([SS("Regex")] string content, params Collection<string> values) => Grp($"{content}|{values.TextJoin("|")}");

  public RxS ([SS("Regex")] string s) => Content = s;
  public RxS (RxS rx) => Content = rx;

  /// <summary>The beginning of the text, not a line beginning.</summary>
  /// <value><c>\A</c></value>
  public static RxS Start => Rx(@"\A");
  /// <summary>The beginning of the text, or a line beginning if the appropriate flag is enabled.</summary>
  /// <value><c>^</c></value>
  public static RxS LnStart => Rx("^");
  public static RxS TruEnd => Rx(@"\z");

  public static RxS Rx ([SS("Regex")] string rx) => new(rx);
  public static RxS Grp ([SS("Regex")] string rx) => $"(?:{rx})";
  public static RxS GrpNm (string name, [SS("Regex")] string rx) => $"(?<{name}>{rx})";
  public static RxS PosLkAhd ([SS("Regex")] string s) => $"(?={s})";
  public static RxS NegLkAhd ([SS("Regex")] string s) => $"(?!{s})";
  public static RxS PosLkBhd ([SS("Regex")] string s) => $"(?<={s})";
  public static RxS NegLkBhd ([SS("Regex")] string s) => $"(?<!{s})";
  public static RxS If (int backRef, [SS("Regex")] string ifMatch, [SS("Regex")] string ifNot) => $"(?({backRef}){ifMatch}|{ifNot})";
  public static RxS If ([SS("Regex")] string expr, [SS("Regex")] string ifMatch, [SS("Regex")] string ifNot) => $"(?({expr}){ifMatch}|{ifNot})";
  public static RxS BackRef (string name) => $@"\k<{name}>";
  public static RxS BackRef (int num) => $@"\k<{num}>";
  public static RxS Atomic ([SS("Regex")] string rx) => $"(?>{rx})";
  public static RxS CharGroup (string allowed) => $"[{allowed}]";
  public static RxS NChar (string notAllowed) => $"[^{notAllowed}]";
  public static RxS OOr (IEnumerable<string> list) => list.AggregateRegex();

  public RxS Add ([SS("Regex")] string append) => $"{Content}{Grp(append)}";

  public RxS Qty (int qty) => Grp(this) + $"{{{qty}}}";
  public RxS Qty (int min, int max) => Grp(this) + $"{{{min},{max}}}";

  // Substitution

  public static RxS Ref (int group) => $"\\{group}";
  public static RxS Ref (string name) => $"${{{name}}}";

  /// <summary>The contents before a match.</summary>
  /// <value><c>$`</c></value>
  public static RxS BeforeMatch => "$`";
  /// <summary>The contents after a match.</summary>
  /// <value><c>$'</c></value>
  public static RxS AfterMatch => "$'";
  public static RxS EntireMatch => "$&";
  public static RxS EntireInput => "$_";
  public static RxS LastGroup => "$+";

  /// <summary>Checks if the regular expression matches the other.</summary>
  /// <param name="other">The other regular expression.</param>
  /// <returns>true if the regular expression matches, false otherwise</returns>
  public bool Is (RxS other) => Content.Is(other);
  /// <summary>Checks if the regular expression matches the other, ignoring case.</summary>
  /// <param name="other">The other regular expression.</param>
  /// <returns>true if the regular expression matches ignoring case, false otherwise</returns>
  public bool Like (RxS other) => Content.Like(other);

  public override int GetHashCode () => Content.GetHashCode(SCO);
  public static bool operator == (RxS left, RxS right) => left.Equals(right);
  public static bool operator == (RxS left, [SS("regex")] string right) => left.Equals(right);
  public static bool operator != (RxS left, [SS("regex")] string right) => !(left == right);
  public static bool operator != (RxS left, RxS right) => !(left == right);

  public static bool operator < (RxS left, RxS right) => left.CompareTo(right) < 0;
  public static bool operator > (RxS left, RxS right) => left.CompareTo(right) > 0;

  /// <summary>Compares the regular expressions for equality.</summary>
  /// <param name="other">The string to compare to.</param>
  /// <returns><see langword="true"/> if the two regular expressions are based on the same pattern string, <see langword="false"/> otherwise.</returns>
  public bool Equals ([SS("Regex")] string? other) => Content.Equals(other, SCO);
  /// <summary>Compares the regular expressions for equality.</summary>
  /// <param name="other">The regular expression to compare to.</param>
  /// <returns><see langword="true"/> if the two regular expressions are based on the same pattern string, <see langword="false"/> otherwise.</returns>
  public bool Equals (RxS other) => Equals(other.Content);
  public override bool Equals (object? obj) => obj switch
  {
    null => false,
    string s => Equals(s),
    RxS rx => Equals(rx),
    RxSCollection rxc => Equals(rxc.Combined.Content),
    _ => Equals(obj.ToString() ?? SE)
  };
}
