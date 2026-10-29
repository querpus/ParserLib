namespace Parser.Inference;

public class InferenceNodeGroup : InferenceNode
{
  public static InferenceNodeGroup NewAnd => new(IT.And);
  public static InferenceNodeGroup NewOr => new(IT.Or);
  protected Collection<InferenceNode> Nodes { get; } = [];
  public InferenceNodeGroup (IT type) : base(type, SE) { }
  public InferenceNodeGroup (IEnumerable<InferenceNode> nodes, IT type) : base(type, SE) => Nodes = [.. nodes];
  public void Add (InferenceNode node) => Nodes.Add(node);
  public override bool CheckFile (string filepath) =>
    Type is IT.And
    ? Nodes.All(item => item.CheckFile(filepath))
    : Type is IT.Or
      ? Nodes.Any(item => item.CheckFile(filepath))
      : throw new InvalidOperationException("Inference Node Groups can only be 'and' or 'or'.");
}
