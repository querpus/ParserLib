#pragma warning disable CA1710 // Identifiers should have correct suffix
#pragma warning disable format // Formatting



namespace Parser.Nodes;
public sealed class JSONPropertyNode : Node
{
  public override string Serialize () => Key;
  /// <summary>The value of this property.</summary>
  public INode? Value
  {
    get => Data.TryGetValue("Value", out object? obj) ? obj as INode : null;
    set => Data["Value"] = (object?) value ?? new JSONContentNode() { Content = "null" };
  }
  public string Key
  {
    get => Data.TryGetValue("Key", out object? obj) ? obj as string ?? SE : SE;
    set => Data["Key"] = value ?? throw new InvalidOperationException("Tried to assign null to Key.");
  }
}
