#pragma warning disable CA1822 // Mark members as static

using Common.Nodes;

using Parser.Inference;

namespace Parser;

/// <summary>A class containing the operations, requirements, and instructions for reading a file.</summary>
/// <remarks>This class is immutable, and should never be altered during operation.</remarks>
public class Spec : IHasTokenRuleSet, IHasNodeRuleSet
{
  /// <summary>The name of this Spec.</summary>
  public required string Name { get; init; }
  /// <summary>A <see cref="ReadOnlyCollection{T}"/> of <see cref="IOperation"/> objects that are executed in order to produce the result.</summary>
  public ReadOnlyCollection<IOperation> Operations { get; init; } = [];
  /// <summary>A <see cref="ReadOnlyCollection{T}"/> of <see cref="InferenceNode"/> objects that specify what files use this specification.</summary>
  public ReadOnlyCollection<InferenceNode> FileInferences { get; init; } = [];
  /// <summary>Determines whether to use a byte parser or a text one.</summary>
  public bool IsTextFile { get; init; }
  /// <summary>The default regex options to use.</summary>
  public RegexOptions RxOpt
  {
    get => field | (SC == SCOIC ? ROIC : RON);
    init;
  }
  /// <summary>The type of token type to use.</summary>
  public Type TokenType { get; init; } = typeof(string);
  /// <summary>The default string comparison type to use.</summary>
  /// <remarks>This will automatically set <see cref="RegexOptions.IgnoreCase"/> if this is set to <see cref="StringComparison.OrdinalIgnoreCase"/>.</remarks>
  public StringComparison SC { get; init; } = SCO;

  /// <summary>Define the names of equilivent groups of tokens.</summary>
  public Dictionary<dynamic, Collection<dynamic>> TokenCompatLookup { get; init; } = [];
  /// <summary>The default rule that is added to every rule definition.</summary>
  /// <remarks>Useful for adding ignore case to the entire defintion.</remarks>
  public RT DefaultRuleSet { get; init; } = RT.None;
  /// <summary>Token rules for the tokenize operation.</summary>
  public TokenRuleCollection TokenRules { get; init; } = [];
  /// <summary>Group token rules for the token assembly operation.</summary>
  public TokenRuleCollection GroupTokenRules { get; init; } = [];
  public TokenRuleSet? TokenRuleSet { get; init; }
  public NodeRuleSet? NodeRuleSet { get; init; }
}
