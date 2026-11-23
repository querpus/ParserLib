#pragma warning disable CA1822 // Mark members as static

using System.Xml.Serialization;

using Parser.Inference;
using Parser.Nodes;

namespace Parser;

public interface IOperationStrategy
{
  IReadOnlyCollection<IOperation> Operations { get; }
  IReadOnlyDictionary<string, int> Labels { get; }
}

public sealed class OperationStrategy : IOperation, IOperationStrategy
{
  private readonly Collection<IOperation> _operations = [];
  private readonly Dictionary<string, int> _labels = [];
  [NotNull]
  public XParser? Parser { get; private set; }
  public OpStatus Status { get; private set; }
  /// <summary>Loads the operations into a flat pattern.</summary>
  private void OperationLoad()
  {
    if (Operations.IsEmpty || Operations[^1] is not OperationEnd)
      _operations.Add(new OperationEnd());

    // Unpack all operations in main list
    for (int i = 0; i < Operations.Count; i++)
    {
      IOperation op = Operations[i];
      int oldCount = Operations.Count;
      if (op is IPlaceholderOperation ipo)
      {
        int newCount = ipo.Unpack(_operations, i, this);
        Log(MsgClass.Debug, $"Operation Group Expanded From {oldCount} to {newCount}.", this);
      }
    }
  }
  public OpStatus DoOperation([NotNull] XParser parser_ref)
  {
    if (SkipOperation)
      return OpStatus.Skipped;

    Parser = parser_ref;

    OperationLoad();

    int index = 0;

    for (; index < Operations.Count && index >= 0; index++)
    {
      if (Status is OpStatus.AtStart)
        Log(MsgClass.Warning, $"Status was not defined in operation {this.TypeName}.", this);
    }

    return Status;
  }

  public IReadOnlyCollection<IOperation> Operations => _operations;
  public bool ContinueOnFail { get; set; }
  public bool SkipOperation { get; set; }

  bool IOperation.NoExecution => false;

  public IReadOnlyDictionary<string, int> Labels => _labels;

  public void ApplyProperties(bool cont, bool skip)
  {
    ContinueOnFail = cont;
    SkipOperation = skip;
  }
}

/// <summary>A class containing the operations, requirements, and instructions for reading a file.</summary>
/// <remarks>This class is immutable, and should never be altered during operation.</remarks>
[XmlRoot(elementName: "Spec", IsNullable = false, Namespace = "Parser/Spec")]
public class Spec : IHasTokenRuleSet, IHasNodeRuleSet
{
  /// <summary>The name of this Spec.</summary>
  [XmlElement(DataType = "string", Order = 1)]
  public required string Name { get; init; }
  /// <summary>A <see cref="ReadOnlyCollection{T}"/> of <see cref="IOperation"/> objects that are executed in order to produce the result.</summary>
  public ReadOnlyCollection<IOperation> Operations { get; init; } = [];
  /// <summary>A <see cref="ReadOnlyCollection{T}"/> of <see cref="InferenceNode"/> objects that specify what files use this specification.</summary>
  public ReadOnlyCollection<InferenceNode> FileInferences { get; init; } = [];
  /// <summary>Determines whether to use a byte parser or a text one.</summary>
  [XmlElement(DataType = "boolean", Order = 2)]
  public bool IsTextFile { get; init; }
  /// <summary>The default regex options to use.</summary>
  public RegexOptions RxOpt
  {
    get => field |= (SC == SCOIC ? ROIC : RON);
    init;
  }
  /// <summary>The default string comparison type to use.</summary>
  /// <remarks>This will automatically set <see cref="RegexOptions.IgnoreCase"/> if this is set to <see cref="StringComparison.OrdinalIgnoreCase"/>.</remarks>
  [XmlElement(DataType = "boolean", ElementName = "CaseInsensitive", Order = 3)]
  public StringComparison SC { get; init; } = SCO;

  /// <summary>Define the names of equilivent groups of tokens.</summary>
  public Dictionary<string, Collection<string>> TokenCompatLookup { get; init; } = [];
  /// <summary>The default rule that is added to every rule definition.</summary>
  /// <remarks>Useful for adding ignore case to the entire defintion.</remarks>
  public RT DefaultRuleSet { get; init; } = RT.None;
  /// <summary>Token rules for the tokenize operation.</summary>
  public TokenRuleCollection TokenRules { get; init; } = [];
  /// <summary>Group token rules for the token assembly operation.</summary>
  public Collection<TokenRule> GroupTokenRules { get; init; } = [];
  /// <summary>A rule set for a capture tokenizer.</summary>
  public TokenRuleSet? TokenRuleSet { get; init; }
  public NodeRuleSet? NodeRuleSet { get; init; }
}
