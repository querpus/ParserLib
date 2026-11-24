#pragma warning disable CA1710 // Identifiers should have correct suffix
#pragma warning disable format // Formatting

namespace Parser.Nodes;
public class JSONConverter
{
  private INode XMLContentSelect (INode node) => node switch
  {
    XMLContentNode content => new JSONContentNode { Content = content.Content ?? SE },
    XMLElementNode element => Convert(element) ?? throw new InvalidNodeException("Failed to convert inner element."),
    XMLCommentNode comment => new JSONContentNode { Content = comment.Content ?? SE },
    _ => throw new InvalidNodeException("Failed to convert content.")
  };
  public INode? Convert (INode node)
  {
    if (node is JSONArrayNode or JSONObjectNode)
    {
      return node;
    }

    if (node is XMLElementNode xml)
    {
      JSONObjectNode converted_node = new();
      converted_node.Properties.Add("name", new JSONContentNode() { Content = xml.Name! });
      converted_node.Properties.Add("attributes", new JSONObjectNode() { Properties = [.. xml.Attributes.Select(attr => new KeyValuePair<string, INode>(attr.Key, attr.Value))] });
      converted_node.Properties.Add("content", new JSONArrayNode() { Children = [.. xml.Children.Select(XMLContentSelect)] });
      return converted_node;
    }
    return null;
  }
}
