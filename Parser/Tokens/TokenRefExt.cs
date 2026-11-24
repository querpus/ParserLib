#pragma warning disable CA1710 // Identifiers should have correct suffix

namespace Parser.Tokens;

public static class TokenRefExt
{
  extension(TokenRef type)
  {
    private bool IsTokenCollection => type switch
    {
      TokenRef.Value => false,
      TokenRef.Name => false,
      TokenRef.Center => false,
      TokenRef.Left => false,
      TokenRef.Right => false,
      TokenRef.Type => false,
      TokenRef.Statement => false,
      TokenRef.ValueList => true,
      TokenRef.AddFlagList => true,
      TokenRef.SubFlagList => true,
      TokenRef.ParameterList => true,
      TokenRef.PropertyList => true,
      TokenRef.StatementList => true,
      TokenRef.Error => throw new OperationException("Passed TokenRef.Error to IsTokenCollection."),
      TokenRef.Ignore => false,
      TokenRef.Parameter => false,
      TokenRef.Property => false,
      TokenRef.AddFlag => false,
      TokenRef.SubFlag => false,
      TokenRef.Custom => false,
      TokenRef.Inherit => false,
      _ => throw new OperationException("Passed an unknown value to IsTokenCollection."),
    };

    public bool IsUsed (Dictionary<TokenRef, IToken> token_pieces) =>
      token_pieces is not null &&
      token_pieces.TryGetValue(type, out IToken? value) &&
      (!type.IsTokenCollection || (type.IsTokenCollection && value is TokenCollection tc && tc.Count > 0));
  }
}
