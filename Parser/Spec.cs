#pragma warning disable CA1822 // Mark members as static

using System.Xml.Serialization;

using Parser.Inference;
using Parser.Nodes;

namespace Parser;

/// <summary>A class containing the operations, requirements, and instructions for reading a file.</summary>
/// <remarks>This class is immutable, and should never be altered during operation.</remarks>
[XmlRoot(elementName: "Spec", IsNullable = false, Namespace = "Parser/Spec")]
public class Spec : IHasTokenRuleSet, IHasNodeRuleSet, ISerializable
{
  /// <summary>The name of this Spec.</summary>
  [XmlElement(DataType = "string", Order = 1)]
  public required string Name { get; init; }
  /// <summary>A <see cref="ReadOnlyCollection{T}"/> of <see cref="IOperation"/> objects that are executed in order to produce the result.</summary>
  public OperationStrategy Operations { get; init; } = [];
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

  public string Serialize ()
  {
    XMLElementNode spec_root = new() { Name = "Spec" };
    XMLElementNode token_rules = new() { Name = "TokenRules" };
    XMLElementNode group_token_rules = new() { Name = "GroupTokenRules" };
    XMLElementNode instructions = new() { Name = "Instructions" };
    XMLElementNode inferences = new() { Name = "FileInferences" };
    XMLElementNode token_lookups = new() { Name = "TokenCompat" };

    spec_root.Children.Add(token_rules);
    spec_root.Children.Add(group_token_rules);
    spec_root.Children.Add(instructions);
    spec_root.Children.Add(inferences);
    spec_root.Children.Add(token_lookups);

    XMLContentNode attr_value_textfile = new() { Content = IsTextFile.ToString() };
    XMLContentNode attr_value_ignorecase = new() { Content = (SC == SCOIC).ToString() };

    foreach (var rule in TokenRules)
    {
      XMLElementNode xml_rule = new() { Name = "Rule" };
      XMLContentNode attr_value_assign = new() { Content = rule.TypeToAssign };
      XMLContentNode attr_value_type = new() { Content = $"{rule.Type}" };
      XMLContentNode content_data = new() { Content = rule.RuleStringData };

      xml_rule.Children.Add(content_data);

      if (!rule.TypeToAssign.Like("None", SE, "-"))
        xml_rule.Attributes.Add("assign", attr_value_assign);
      if (rule.Type != RT.None)
        xml_rule.Attributes.Add("type", attr_value_type);

      token_rules.Children.Add(xml_rule);
    }

    foreach (var rule in GroupTokenRules)
    {
      XMLElementNode xml_rule = new() { Name = "Rule" };
      XMLContentNode attr_value_assign = new() { Content = rule.TypeToAssign };
      XMLContentNode attr_value_type = new() { Content = $"{rule.Type}" };
      XMLContentNode content_data = new() { Content = rule.RuleStringData };

      xml_rule.Children.Add(content_data);

      if (!rule.TypeToAssign.Like("None", SE, "-"))
        xml_rule.Attributes.Add("assign", attr_value_assign);
      if (rule.Type != RT.None)
        xml_rule.Attributes.Add("type", attr_value_type);

      group_token_rules.Children.Add(xml_rule);
    }

    return spec_root.Serialize();
  }
  /*
  public class XMLString ()
  {
    private Collection<IXMLObject> Elements { get; } = [];
    private Collection<IXMLObject> Closing { get; } = [];

    public bool IsComplete => Closing.Count == 0 && Elements.Count > 0;

    public void AddElementOpen (string name)
    {
      XMLElementOpen open = new() { Tag = name };
      Elements.Add(open);
      Closing.Add(open.ClosingElement);
    }
    public void AddElementSingle (string name) => Elements.Add(new XMLElementSingle() { Tag = name });
    public void AddElementSingle (string name, Collection<IProperty<string>> attributes)
    {
      XMLElementSingle single = new()
      {
        Tag = name,
        Attributes = [.. attributes.Select(att => new XMLAttr() { Key = att.Key, Value = att.Value })]
      };

      Elements.Add(single);
    }
    public void AddElementOpen (string name, Collection<IProperty<string>> attributes) =>
      AddElementOpen(name, [.. attributes.Select(att => (att.Key, att.Value ?? SE))]);
    public void AddElementOpen (string name, Collection<(string Key, string Value)> attributes)
    {
      XMLElementOpen open = new()
      {
        Tag = name,
        Attributes = [.. attributes.Select(att => new XMLAttr() { Key = att.Key, Value = att.Value })]
      };
      Elements.Add(open);
      Closing.Add(open.ClosingElement);
    }
    public void CloseLastElement ()
    {
      if (!Closing.IsEmpty)
        Elements.Add(Closing.Pop()!);
    }

    public void AddContent (string content) => Elements.Add(new XMLContent() { Content = content });
    public void AddLineFeed ()
    {
      int depth = Closing.Count;
      string indent = new(' ', depth * 2);
      Elements.Add(new XMLContent() { Content = $"\n{indent}" });
    }
    public void CloseAllElements ()
    {
      while (Closing.Count > 0)
        CloseLastElement();
    }

    public string Serialize () => ToString();
    public override string ToString ()
    {
      string result = string.Empty;
      foreach (IXMLObject element in Elements)
        result += element.Serialize();
      return result;
    }
  }*/

}
