using System.Linq;

namespace Specification.XML;

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
}
