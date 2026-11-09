#pragma warning disable CA1710 // Identifiers should have correct suffix
#pragma warning disable format // Formatting

namespace Parser.Nodes;
public abstract class Node : INode
{
  public Collection<INode> Children { get; } = [];
  public Dictionary<string, INode> Properties { get; } = [];
  public Dictionary<string, object> Data { get; } = [];
  public override string ToString () => $"Node (C:{Children.Count}, P:{Properties.Count}, D:{Data.Count})";
  public abstract string Serialize ();
  public bool Equals (INode? other) => ((INode) this).Equals(other);
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
