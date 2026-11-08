#pragma warning disable CA1710 // Identifiers should have correct suffix
#pragma warning disable format // Formatting

namespace Parser.Nodes;
public abstract class Node : INode
{
  public Collection<ISerializable> Children { get; } = [];
  public ImmutableList<INode> NodeChildren => [.. Children.OfType<INode>()];
  public Dictionary<string, ISerializable> Properties { get; } = [];
  public Dictionary<string, object> Data { get; } = [];
  public virtual string? Prefix { get; set; }
  public virtual string? Suffix { get; set; }
  public virtual string ChildDelim { get; set; } = SE;
  public virtual bool DelimAfterPrefix { get; set; }
  public override string ToString () => $"Node(Origin: {Prefix ?? "[No Prefix]"}||{Suffix ?? "[No Suffix]"}, Children: {Children.Count}, Properties: {Properties.Count}, Data: {Data.Count})";
  public abstract string Serialize ();
  public virtual string SerializeAsRead ()
  {
    string result = Prefix ?? SE;
    bool first = true;
    foreach (ISerializable child in Children)
    {
      if (first)
      {
        if (DelimAfterPrefix)
          result += ChildDelim;
        first = false;
      }
      else
      {
        result += ChildDelim;
      }

      if (child is INode node)
        result += node.SerializeAsRead();
      else
        result += child.Serialize();
    }

    return result;
  }
  public bool Equals (INode? other) => ((INode) this).Equals(other);
  public void AppendToPrefix (string data)
  {
    if (Prefix is null)
      Prefix = data;
    else
      Prefix += data;
  }
  public void AppendToPrefix (IToken data)
  {
    if (Prefix is null)
      Prefix = data.Value;
    else
      Prefix += data.Value;
  }
  public void AppendToSuffix (string data)
  {
    if (Suffix is null)
      Suffix = data;
    else
      Suffix += data;
  }
  public void AppendToSuffix (IToken data)
  {
    if (Suffix is null)
      Suffix = data.Value;
    else
      Suffix += data.Value;
  }
  protected static void PrintItem (int indent, object item)
  {
    switch (item)
    {
      case string s:
        LogPart(MsgClass.Forced, s);
        break;
      case QString q:
        LogPart(MsgClass.Forced, q);
        break;
      case IPrintable ip:
        ip.Print(indent);
        break;
      default:
        LogPart(MsgClass.Forced, item.ToString() ?? "<null>");
        break;
    }
  }
  protected static void PrintCollection<T> (int indent, string name, IEnumerable<T> list, Func<T, object>? selector = null, Func<T, string>? key_selector = null)
  {
    string sp = new(' ', indent);
    string sp2 = new(' ', indent + 2);

    Collection<T> clist = [.. list];
    if (clist.Count == 0)
    {
      LogPart(MsgClass.BlueInfo, sp + "No " + name);
      NewLine();
    }
    else
    {
      LogPart(MsgClass.BlueInfo, sp + name + ":");
      NewLine();

      for (int i = 0; i < clist.Count; i++)
      {
        T item = clist[i];

        object value = item is null ? "<null>" : item;
        string key = $"{i}";

        if (selector is not null && item is not null)
        {
          value = selector(item);
        }

        if (key_selector is not null)
        {
          key = key_selector(item);
        }

        LogPart(MsgClass.GreenInfo, sp2 + key);
        LogPart(MsgClass.Warning, ": ");
        PrintItem(indent + 4, value);
        NewLine();
      }
    }
  }
  public void Print (int indent)
  {
    string sp = new(' ', indent);
    string sp2 = new(' ', indent + 2);

    LogPart(MsgClass.Forced, this.TypeName);
    LogPart(MsgClass.BlueInfo, $" (D:{Data.Count}, C:{Children.Count}, P:{Properties.Count})");
    NewLine();

    PrintCollection(indent, "Data", Data.ToList(), kvp => kvp.Value, kvp => kvp.Key);
    PrintCollection(indent, "Properties", Properties.ToList(), kvp => kvp.Value, kvp => kvp.Key);
    PrintCollection(indent, "Children", Children);
  }
}

#pragma warning disable CA1815 // Override equals and operator equals on value types
public struct DataReference : ISerializable
#pragma warning restore CA1815 // Override equals and operator equals on value types
{
  public INode LinkedNode { get; set; }
  public string Key { get; set; }
  public readonly string Serialize () => LinkedNode.Data[Key] is ISerializable its ? its.Serialize() : LinkedNode.Data[Key].ToString() ?? SE;
}
