#pragma warning disable CA1710 // Identifiers should have correct suffix
#pragma warning disable format // Formatting

namespace Common.Nodes;
public sealed class INISectionNode : Node
{
  public override string Serialize () => "[" + Name + "]";
  public string? Name => Data.TryGetValue("Name", out object? value) && value is string name ? name : null;
}

public sealed class INIPropertyNode : Node
{
  public override string Serialize () => Key + "=" + Value;
  public string? Key => Data.TryGetValue("Key", out object? value) && value is string name ? name : null;
  public string? Value => Data.TryGetValue("Value", out object? value) && value is string name ? name : null;
}

public sealed class INICommentNode : Node
{
  public override string Serialize () => "; " + Content;
  public string? Content => Data.TryGetValue("Content", out object? value) && value is string name ? name : null;
}
