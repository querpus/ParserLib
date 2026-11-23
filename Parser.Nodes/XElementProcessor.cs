#pragma warning disable CA1710 // Identifiers should have correct suffix
#pragma warning disable format // Formatting

using System.Xml.Linq;

namespace Parser.Nodes;

public class XElementProcessor : INodeProcessor
{
  public NodeContext? Context { get; private set; }
  public dynamic? Input { get; init; }

  public XElementProcessor () { }
  public XElementProcessor (XElement root) => Input = root;

  public void Process ()
  {
    var start_time = DateTime.Now;
    Context = new() { SingleRoot = true, Root = GetNodes(Input) };
    var end_time = DateTime.Now;
    TimeSpan duration = end_time - start_time;
    Log(MsgClass.Debug, $"XElement Processing Complete: processed in {duration}.", this);
  }

  private static XMLElementNode GetNodes (XElement element)
  {
    XMLElementNode result = new();

    result.Data["Name"] = element.Name.LocalName;

    if (element.HasElements)
    {
      element.Elements().Foreach(c => result.Children.Add(GetNodes(c)));
    }
    if (element.HasAttributes)
    {
      foreach (var attr in element.Attributes())
      {
        string attr_name = attr.Name.LocalName;
        string attr_value = attr.Value;
        result.Attributes.Add(attr_name, new XMLContentNode() { Content = attr_value });
      }
    }
    if (!element.Value.IsWhitespace && !element.Value.IsEmpty)
    {
      string value = element.Value;
      result.Children.Add(new XMLContentNode { Content = value });
    }
    return result;
  }
}
