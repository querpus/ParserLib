#pragma warning disable CA1710 // Identifiers should have correct suffix
#pragma warning disable format // Formatting

namespace Common.NodeTree;
public abstract class Node : INode
{
  public Collection<ITextSerializer> Children { get; } = [];
  public Dictionary<string, ITextSerializer> Properties { get; } = [];
  public Dictionary<string, object> Data { get; } = [];
  public string? OriginHead { get; set; }
  public string? OriginTail { get; set; }
  public override string ToString () => $"Node(Origin: {OriginHead}||{OriginTail}, Children: {Children.Count}, Properties: {Properties.Count}, Data: {Data.Count})";
  public abstract string Serialize ();
}
