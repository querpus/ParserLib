#pragma warning disable CA1710 // Identifiers should have correct suffix
#pragma warning disable format // Formatting

namespace Parser.Nodes;
public sealed class REGPropertyNode : Node
{
  private int PrefixIndex => Value?.IndexOf(':', SCO) ?? -1;
  public override string Serialize () => Key + " = " + Value;
  public string? Key => Data.TryGetValue("Key", out object? value) && value is string name ? name : null;
  public string? Value => Data.TryGetValue("Value", out object? value) && value is string name ? name : null;
  public Microsoft.Win32.RegistryValueKind ValueType => Data.TryGetValue("ValueType", out object? value) && value is Microsoft.Win32.RegistryValueKind rvk ? rvk : Microsoft.Win32.RegistryValueKind.None;
  public string? ValuePrefix => Value?.StartsWithAny(SCO, "'", Chars.QTs) == false ? Value[..PrefixIndex] : null;
  public string? NumValue => ValuePrefix is not null && Value is not null ? Value[(PrefixIndex + 1)..] : null;
  public bool RemoveProperty => Value?.StartsWith('-', SCO) ?? false;
  public bool DefaultProperty => Key?.StartsWith('@', SCO) ?? false;
}
