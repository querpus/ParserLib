#pragma warning disable CA1710 // Identifiers should have correct suffix

namespace Parser.Tokens;

/// <summary>A class that holds the results of a token grouping operation.</summary>
/// <param name="Tokens">The token list, of physical tokens. These are actual segments of text.</param>
/// <param name="Parents">The grouping constructs.</param>
/// <param name="Hierarchy">The whole assembly in a full hierarchy.</param>
public record class TokenAssemblyResult (TokenCollection Tokens, TokenCollection Parents, TokenCollection Hierarchy) : IPrintable
{
  bool IPrintable.HasHeaderLine => false;
  public override string ToString () => Tokens + "\n\n\n" + Parents + "\n\n\n" + Hierarchy;
  public void Print (int indent)
  {
    Tokens.Print(indent);
    NewLine();
    NewLine();
    NewLine();
    Parents.Print(indent);
    NewLine();
    NewLine();
    NewLine();
    Hierarchy.Print(indent);
  }
}
