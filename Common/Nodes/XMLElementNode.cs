#pragma warning disable CA1710 // Identifiers should have correct suffix
#pragma warning disable format // Formatting

namespace Common.Nodes;
public sealed class XMLElementNode : Node
{
  private static string SerializeAttribute (KeyValuePair<string, ISerializable> kvp) => $"{kvp.Key}={kvp.Value.Serialize()}";
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
  public bool HasElements => Children.Any(c => c is XMLElementNode);
  /// <summary>Gets the element name and namespace.</summary>
  public string? Name => Data.TryGetValue("Name", out object? value) && value is string name ? name : null;
  public string? Content
  {
    get
    {
      var content_nodes = Children.NotOfTypes<ISerializable, XMLCommentNode, XMLElementNode>();

      if (content_nodes.Any())
      {
        string content = content_nodes.Select(node => node.Serialize()).TextJoin("").Trim();
        return content.Length > 0 ? content :  null;
      }
      return null;
    }
  }

  /// <summary>Gets the local element name.</summary>
  public string? LocalName => Name?.Contains(':') == true ? Name[(Name.IndexOf(':') + 1)..] : Name;
  /// <summary>Gets the namespace, or <see langword="null"/> if one is not specified on this element.</summary>
  /// <remarks>This does not account for globally set or namespaces set by this element's parents.</remarks>
  public string? Namespace => Name?.Contains(':') == true ? Name[..Name.IndexOf(':')] : null;
  public Dictionary<string, ISerializable> Attributes => Properties;
}
