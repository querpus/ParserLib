#pragma warning disable CA1710 // Identifiers should have correct suffix

using T = Parser.Tokens.TokenRef;

namespace Parser.Tokens;

/// <summary>Builds a <see cref="ChkToken"/> from a <see langword="string"/>.<br/>
/// <list type="bullet">
/// <listheader>Syntax</listheader>
/// <item><c>prefix:value</c></item>
/// <item><c>prefix:(value1|value2|value3...)</c></item>
/// </list>
/// </summary>
/// <remarks>
/// <list type="table">
/// <listheader>Specifications</listheader>
/// <item>t</item>
/// </list>
/// <list type="table">
/// <listheader>Examples</listheader>
/// <item><c>n:Name</c> - Token Type 'Name', Store as a Name.</item>
/// <item><c>t:Dec</c> - Token Type 'Dec', store as a Value.</item>
/// <item><c>cyi:Script</c> - Content 'Script', Case insensitive, store as a Type.</item>
/// </list>
/// </remarks>
public sealed class ChkToken
{
  internal static Dictionary<char, RT> LetterReference { get; } = new()
  {
    ['A'] = RT.Any,
    ['I'] = RT.IgnoreCase,
    ['M'] = RT.Mult,
    ['O'] = RT.Opt,
  };

  public T AssignTo { get; private set; }
  public required RT TokenRule { get; init; } = RT.None;
  public string? CustomPropertyName { get; private set; }
  public Collection<string> AllowedTypes { get; init; } = [];
  public string RegexValidator { get; init; } = SE;
  public bool IgnoreCase => TokenRule.HasFlag(RT.IgnoreCase);
  /// <summary>Creates a <see cref="ChkToken"/> object from the <see langword="string"/>.</summary>
  /// <param name="definition">The definition string.</param>
  /// <param name="spec">The <see cref="Spec"/> reference.</param>
  /// <returns>A <see cref="ChkToken"/> object from the <see langword="string"/>.</returns>
  /// <exception cref="ArgumentException"/>
  public static ChkToken Parse (string definition, Spec spec)
  {
    RxS regex = RxS.Rx(@"^((?'prefix'[\w=!><]+)|\[(?'custom_prop'\w+)\])\:(?:\((?'type_def'[^,&|+}-]+)([,&+|-](?'type_def'[^,&+|}-]+))*\)|\{(?'literal_def'[^}]+)\}|(?'type_def'\w+))+");
    Regex regexobj = new(regex, ROEC, new TimeSpan(0, 0, 1));
    Match m = regexobj.Match(definition);
    if (!m.Success)
      _ = Err.ThrowBadDef($"Bad Token Sequence String. {definition}");

    RT rule = RT.None;
    string prefix = SE;
    string? prop = null;
    T assnTo = T.Custom;

    if (m.HasValidGroup("custom_prop"))
    {
      prop = m.Groups["custom_prop"].Value;
    }
    else
    {
      prefix = m.Groups["prefix"].Value;
      assnTo = GetTokenRef(prefix);
    }
    HashSet<string> allowed_types = [];
    string regex_validator = SE;

    //Automatically assign ignore case if the spec declares it.
    if (spec.SC == SCOIC)
    {
      rule |= RT.IgnoreCase;
    }

    foreach (char c in prefix.ToUpperInvariant().RemoveAllButChars("AOMI"))
    {
      rule |= LetterReference[c];
    }

    if (assnTo == T.Error)
    {
      _ = Err.ThrowBadDef($"Bad Prefix Char. ({prefix})");
    }

    if (m.Groups.ContainsKey("type_def"))
    {
      foreach (Capture c in m.Groups["type_def"].Captures)
        _ = allowed_types.Add(c.Value);
    }

    if (m.Groups.ContainsKey("literal_def"))
      regex_validator = m.Groups["literal_def"].Value;

    HashSet<string> expanded_types = spec is null ? allowed_types : AllAllowedTypes(allowed_types, spec);

    return new()
    {
      CustomPropertyName = prop,
      RegexValidator = regex_validator,
      AllowedTypes = [.. expanded_types],
      TokenRule = rule,
      AssignTo = assnTo,
    };
  }
  public static T GetTokenRef (string prefix)
  {
    prefix.ThrowIfNull("Prefix was null");
    prefix = prefix.ToUpperInvariant();
    prefix = prefix.RemoveChars("AOMI"); // Any, Opt, Mult, IgnoreCase

    return prefix.Length != 1
      ? T.Error
      : prefix[0] switch
      {
        'C' => T.Center,
        'D' => T.Inherit,
        'F' => T.AddFlag,
        'G' => T.SubFlag,
        'L' => T.Left,
        'N' => T.Name,
        'P' => T.Property,
        'Q' => T.Parameter,
        'R' => T.Right,
        'S' => T.Statement,
        'T' => T.Type,
        'V' => T.Value,
        'X' => T.Ignore,

        _ => T.Error,
      };
  }
  /// <summary>Gets all the allowed derived types of tf this token.</summary>
  /// <param name="types">The initially allowed types.</param>
  /// <param name="spec">The <see cref="Spec"/> to lookup from.</param>
  /// <returns>A list of all the allowed token types.</returns>
  internal static HashSet<string> AllAllowedTypes (IEnumerable<string> types, Spec spec)
  {
    HashSet<string> all_types_allowed = [.. types];

    foreach (string item in types)
    {
      if (spec.TokenCompatLookup.Keys.Any(i => i.Equals(item, SCOIC)))
      {
        string key = spec.TokenCompatLookup.Keys.Single(i => i.Equals(item, SCOIC));
        foreach (string? s in spec.TokenCompatLookup[key])
        {
          if (s is null) continue;

          bool added = all_types_allowed.Add(s);

          if (added)
          {
            all_types_allowed = AllAllowedTypes(all_types_allowed, spec);
          }
        }
      }
    }
    return [.. all_types_allowed];
  }
  internal bool Check_Type (IToken? token) => (token?.HasType == true && AllowedTypes.Any(type => token.Type.Like(type))) || AllowedTypes.Count == 0;

  public override string ToString () => $"ChkToken: {AllowedTypes.TextJoin("-")}" + (RegexValidator.Length > 0 ? $"{{{RegexValidator}}}" : "");
  internal bool IsStatisfiedBy (IToken token, Spec spec) =>
    token is not null &&
    (RegexValidator.IsEmpty || Regex.Match(token.Content, RegexValidator, ROEC | ROML | ROIPW | (TokenRule.HasFlag(RT.IgnoreCase) || spec.SC == SCOIC ? ROIC : RON)).Value.Length == token.Content.Length) &&
    Check_Type(token);
}
