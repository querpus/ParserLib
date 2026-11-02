#pragma warning disable CA1710 // Identifiers should have correct suffix
namespace Parser.Tokens;

public class TokenRule
{
  public required RT Type { get; set; }
  public string? RuleStringData { get; set; }
  public string TypeToAssign { get; set; }
  public ChkSequence GroupSequence { get; } = [];

  #region Constructors
  [SetsRequiredMembers]
  public TokenRule (RT type, [SS("regex")] string ruleStringData)
  {
    TypeToAssign = "None";
    RuleStringData = ruleStringData;
    Type = type;
  }
  [SetsRequiredMembers]
  public TokenRule (RT type, string typeToAssign, [SS("regex")] string ruleStringData)
  {
    Type = type;
    RuleStringData = ruleStringData;
    TypeToAssign = typeToAssign;
  }
  [SetsRequiredMembers]
  public TokenRule (RT type, object typeToAssign, [SS("regex")] string ruleStringData)
  {
    Type = type;
    RuleStringData = ruleStringData;
    TypeToAssign = typeToAssign?.ToString() ?? SE;
  }
  public TokenRule () => TypeToAssign = SE;
  /// <summary>Use this constructor to make a basic non-recursive group rule.</summary>
  /// <param name="typeToAssign">The type to assign to the assembled token.</param>
  /// <param name="ruleStringData">The assembly definition.</param>
  [SetsRequiredMembers]
  public TokenRule (string typeToAssign, [SS("regex")] string ruleStringData)
  {
    Type = RT.None;
    RuleStringData = ruleStringData;
    TypeToAssign = typeToAssign;
  }
  /// <summary>Use this constructor to make a basic non-recursive group rule.</summary>
  /// <param name="typeToAssign">The type to assign to the assembled token.</param>
  /// <param name="ruleStringData">The assembly definition.</param>
  [SetsRequiredMembers]
  public TokenRule (object typeToAssign, [SS("regex")] string ruleStringData)
  {
    Type = RT.None;
    RuleStringData = ruleStringData;
    TypeToAssign = typeToAssign?.ToString() ?? SE;
  }
  #endregion
  #region Static Generation
  public static TokenRule[] CopyOfRuleSet (TokenRuleCollection rules)
  {
    Collection<TokenRule> new_rules = [];
    rules ??= [];
    foreach (TokenRule rule in rules)
    {
      TokenRule new_rule = new(rule.Type, rule.TypeToAssign, rule.RuleStringData ?? SE);
      new_rules.Add(new_rule);
    }
    return [.. new_rules];
  }
  /// <summary>Makes a series of single character TokenExact styled rules.</summary>
  /// <param name="chars">The characters to use.</param>
  /// <param name="type">The rule type. Should be TokenExact or SplitExact, plus any necessary modifiers.</param>
  /// <param name="typeToAssign">The type to assign. May be a single object applied to all, or a list of objects the same length as <paramref name="chars"/>.</param>
  /// <returns>An array of TokenRules.</returns>
  public static TokenRule[] MakeSingleCharRules (string chars, RT type, params Collection<string> typeToAssign) =>
    string.IsNullOrEmpty(chars)
      ? []
      : typeToAssign.Count switch
      {
        1 => [.. chars.Select(single_char => new TokenRule(type, typeToAssign[0], $"{single_char}"))],
        > 1 => [.. typeToAssign.Select((assign, index) => new TokenRule(type, assign, $"{chars[index]}"))]
      };
  /// <summary>Creates an array of token rules representing keywords.</summary>
  /// <returns>A list of TokenMatch rules that properly exempt ranges.</returns>
  public static TokenRule[] MakeWordMatchRules (bool ignore_case, params Collection<(string, string)> rules)
  {
    Collection<TokenRule> tokenRules = [];

    if (rules.IsEmpty || rules is null)
      return [.. tokenRules];

    string rx_ic = ignore_case ? "(?i)" : SE;

    foreach ((string word, string type) in rules)
    {
      TokenRule r = new(RT.TokenMatch, type, @$"{rx_ic}\b{word}\b");
      tokenRules.Add(r);
    }

    return [.. tokenRules];
  }
  public static TokenRule[] MakeWordMatchRules (bool ignore_case, params Collection<object> rules)
  {
    Collection<TokenRule> tokenRules = [];

    if (rules.IsEmpty || rules is null)
      return [.. tokenRules];

    foreach (object type in rules)
    {
      TokenRule r = new(RT.TokenMatch | (ignore_case ? RT.IgnoreCase : RT.None), type, @$"\b{type}\b");
      tokenRules.Add(r);
    }

    return [.. tokenRules];
  }
  #endregion
  public override string ToString () => $"TokenRule ({Type} => {TypeToAssign})";
}
