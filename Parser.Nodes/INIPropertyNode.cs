#pragma warning disable CA1710 // Identifiers should have correct suffix
#pragma warning disable format // Formatting



namespace Parser.Nodes;
public sealed class INIPropertyNode : Node
{
  public override string Serialize () => Key + "=" + Value;
  public string? Key => Data.TryGetValue("Key", out object? value) && value is string name ? name : null;
  public string? Value => Data.TryGetValue("Value", out object? value) && value is string name ? name : null;
}
