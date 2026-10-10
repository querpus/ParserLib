#pragma warning disable CA1710 // Identifiers should have correct suffix
#pragma warning disable format // Formatting

namespace Common.NodeTree;
public sealed class XMLElementNode : Node
{
  private static string SerializeAttribute (KeyValuePair<string, ITextSerializer> kvp) => $"{kvp.Key}={kvp.Value.Serialize()}";
  private string SerializeAttributes () => Properties.Select(SerializeAttribute).TextJoin(" ");
  public override string Serialize ()
  {
    string? name = Data.TryGetValue("Name", out object? value) && value is string n ? n : null;
    bool isSingle = Children.Count == 0;
    bool isHeader = name is "xml";

    string result = $"<{(isHeader ? "?" : "")}{name} {SerializeAttributes()}{(isSingle ? " /" : "")}{(isHeader ? " ?" : "")}>";

    if (!isSingle)
    {
      result += Children.Select(c => c.Serialize()).TextJoin("");
      result += $"</{name}>";
    }

    return result;
  }
  /// <summary>Gets the element name and namespace.</summary>
  public string? Name => Data.TryGetValue("Name", out object? value) && value is string name ? name : null;
  /// <summary>Gets the local element name.</summary>
  public string? LocalName => Name?.Contains(':') == true ? Name[(Name.IndexOf(':') + 1)..] : Name;
  /// <summary>Gets the namespace, or <see langword="null"/> if one is not specified on this element.</summary>
  /// <remarks>This does not account for globally set or namespaces set by this element's parents.</remarks>
  public string? Namespace => Name?.Contains(':') == true ? Name[..Name.IndexOf(':')] : null;
  public Dictionary<string, ITextSerializer> Attributes => Properties;
}

public sealed class XMLContentNode : Node
{
  public override string Serialize () => Content ?? SE;
  /// <summary>Gets the content of this node.</summary>
  public string? Content => Data.TryGetValue("Content", out object? obj) ? obj as string : null;
}

public sealed class IPLCommandNode : Node
{
  public override string Serialize () => $"{CommandLetter}{Values.TextJoin(",")};";
  /// <summary>Gets the content of this node.</summary>
  public char? CommandLetter => Data.TryGetValue("CommandLetter", out object? obj) ? (char) obj : null;
  public Collection<string> Values => Data.TryGetValue("Values", out object? obj) ? obj as Collection<string> ?? [] : [];
}

public sealed class XMLCommentNode : Node
{
  public override string Serialize () => $"{Content}";
  /// <summary>Gets the content of this node.</summary>
  public string Content => Data.TryGetValue("Content", out object? obj) ? obj as string ?? SE : SE;
}

public sealed class JSONObjectNode : Node
{
  private static string SerializeProperty (KeyValuePair<string, ITextSerializer> kvp) => $"\"{kvp.Key}\":{kvp.Value.Serialize()}";
  private string SerializeProperties () => Properties.Select(SerializeProperty).TextJoin(",");
  public override string Serialize () => "{" + SerializeProperties() + "}";

  public bool IsRoot => Data.TryGetValue("IsRoot", out object? obj) && obj is true;
}

public sealed class JSONArrayNode : Node
{
  private static string SerializeChild (ITextSerializer node) => $"{node.Serialize()}";
  private string SerializeChildren () => Children.Select(SerializeChild).TextJoin(",");
  public override string Serialize () => "[" + SerializeChildren() + "]";

  public bool IsRoot => Data.TryGetValue("IsRoot", out object? obj) && obj is true;
}

public sealed class INISection : Node
{
  public override string Serialize () => "[" + Name + "]";
  public string? Name => Data.TryGetValue("Name", out object? value) && value is string name ? name : null;
}

public sealed class REGSection : Node
{
  public override string Serialize () => "[" + Name + "]";
  public string? Name => Data.TryGetValue("Name", out object? value) && value is string name ? name : null;
  public string? Key => Name?[(Name.LastIndexOf("\\", SCO) + 1)..];
  public string? Path => Name?[..Name.LastIndexOf("\\", SCO)];
}
