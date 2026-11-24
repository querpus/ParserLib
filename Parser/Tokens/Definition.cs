#pragma warning disable CA1710 // Identifiers should have correct suffix
#pragma warning disable RE0001 // Invalid regex pattern

namespace Parser.Tokens;

internal static class Definition
{
  public static Spec Dummy = new()
  {
    FileInferences = [],
    Name = "Dummy",
    Operations = [],
    RxOpt = ROIPW | ROIC | ROML | ROSL | ROEC,
    DefaultRuleSet = RT.IgnoreCase
  };

  public static TokenFactory Factory { get; } = new(Dummy, [
    new(RT.TokenExact, "Colon", ":"),
    new(RT.TokenExact, "Po", "("),
    new(RT.TokenExact, "Pc", ")"),
    new(RT.TokenMatch, "Prefix", @"\b[a-y]+(?=\:)"),
    new(RT.TokenMatch, "Content", @"(?<=c.*?\:)\w+\b"),
    new(RT.TokenMatch, "TypeName", @"(?<=t.*?\:)\w+\b"),
  ]);
}
