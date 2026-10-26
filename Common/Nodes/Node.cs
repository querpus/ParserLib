#pragma warning disable CA1710 // Identifiers should have correct suffix
#pragma warning disable format // Formatting

namespace Common.Nodes;
public abstract class Node : INode
{
  public Collection<ISerializable> Children { get; } = [];
  public Dictionary<string, ISerializable> Properties { get; } = [];
  public Dictionary<string, object> Data { get; } = [];
  public virtual string? Prefix { get; set; }
  public virtual string? Suffix { get; set; }
  public virtual string ChildDelim { get; set; } = SE;
  public override string ToString () => $"Node(Origin: {Prefix ?? "[No Prefix]"}||{Suffix ?? "[No Suffix]"}, Children: {Children.Count}, Properties: {Properties.Count}, Data: {Data.Count})";
  public abstract string Serialize ();
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
  public void Print (int indent)
  {

  }
}

public struct DataReference : ISerializable
{
  public INode LinkedNode { get; set; }
  public string Key { get; set; }
  public readonly string Serialize () => LinkedNode.Data[Key] is ISerializable its ? its.Serialize() : LinkedNode.Data[Key].ToString() ?? SE;
}
