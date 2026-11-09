#pragma warning disable CA1710 // Identifiers should have correct suffix
#pragma warning disable format // Formatting



namespace Parser.Nodes;

public class NodeRule
{
  public string? TokenName { get; init; }
  public string? ExactText { get; init; }
  /// <summary>The default action if none is defined.</summary>
  /// <param name="context">The processing context.</param>
  /// <param name="token">The token encountered.</param>
  public static void DefaultAction (NodeContext context, IToken token) => Log(MsgClass.GreenInfo, $"No Execution Defined for '{token.Name}'", "NodeRule");
  public Action<NodeContext, IToken> Execute { get; init; } = DefaultAction;
  /// <summary>Creates a node rule that discards the token's contents after optionally appending <paramref name="replace_with"/>.</summary>
  /// <param name="groupName">The token type to look for.</param>
  /// <param name="exact_text">The exact text to look for.</param>
  /// <returns>The created node rule.</returns>
  public static NodeRule Discard (string groupName, string? exact_text = null) => new()
  {
    TokenName = groupName,
    ExactText = exact_text,
    Execute = (_, _) => { }
  };
  /// <summary>Creates a node rule that displays an error and optionally halts the processing if the token is encountered.</summary>
  /// <param name="groupName">The token type to look for.</param>
  /// <param name="throw_error">Set to <see langword="true"/> to halt the token processing, <see langword="false"/> simply displays a warning.</param>
  /// <param name="error_message">The message to display.</param>
  /// <returns>The created node rule.</returns>
  /// <exception cref="ValidationException">The node rule was matched and <paramref name="throw_error"/> was <see langword="true"/>.</exception>
  public static NodeRule SyntaxError (string groupName, bool throw_error = false, string error_message = EmptyString) => new()
  {
    TokenName = groupName,
    Execute = (_, _) =>
    {
      if (!throw_error)
      {
        Log(MsgClass.Warning, "Warning: " + error_message, "NodeRule");
      }
      else
      {
        Log(MsgClass.Error, "Error: " + error_message, "NodeRule");
        throw new ValidationException("Error: " + error_message);
      }
    }
  };
}
