#pragma warning disable CA1710 // Identifiers should have correct suffix
#pragma warning disable format // Formatting

namespace Common.Nodes;
public sealed class IPLCommandNode : Node
{
  public override string Serialize () => $"{CommandLetter}{Values.TextJoin(",")};";
  /// <summary>Gets the content of this node.</summary>
  public char? CommandLetter
  {
    get => Data.TryGetValue("CommandLetter", out object? obj) ? (char) obj : null;
    set => Data["CommandLetter"] = value ?? '\0';
  }
  public bool Escaped
  {
    get => Data.TryGetValue("Escaped", out object? obj) && obj is true;
    set => Data["Escaped"] = value;
  }
  public Collection<string> Values
  {
    get => Data.TryGetValue("Values", out object? obj) ? obj as Collection<string> ?? [] : (Collection<string>) (Data["Values"] = new Collection<string>());
    set => Values.AddRange(value);
  }
  public void AddValue (string value) => Values.Add(value);
}
