#pragma warning disable CA1710 // Identifiers should have correct suffix
#pragma warning disable format // Formatting

namespace Common.Entities;

public class SymbolEntity : Entity
{
  public required string Content
  {
    get => (string) DataValues["Content"]!;
    set => DataValues["Content"] = value;
  }

  public static implicit operator string (SymbolEntity ce) => ce.Content;
  public static implicit operator SymbolEntity (string s) => new()
  {
    Content = s,
    Origin = s
  };
  public static bool operator == (SymbolEntity left, string right) => left.Content.Is(right);
  public static bool operator != (SymbolEntity left, string right) => !(left == right);

  public override bool Equals (IEntity? other) =>
    other is SymbolEntity ce && Content.Is(ce.Content);
  public override string Serialize () => Content;

  public override int GetHashCode () => Content.GetHashCode(SCO);
  public override bool Equals (object? obj) => obj switch
  {
    string s => this == s,
    IEntity ipe => Equals(ipe),
    _ => false
  };
  protected override void Assign (Match match)
  {
    Content = match.Value;
    Origin = match.Value;
  }
}
