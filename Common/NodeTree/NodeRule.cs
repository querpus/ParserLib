#pragma warning disable CA1710 // Identifiers should have correct suffix
#pragma warning disable format // Formatting

namespace Common.NodeTree;

public class NodeRule
{
  public string? TokenName { get; init; }
  public string? ExactText { get; init; }
  public static void DefaultAction (NodeContext context, CaptureToken token) => Debug.Log(MsgClass.GreenInfo, $"No Execution Defined for '{token.Name}'", "NodeRule");
  public Action<NodeContext, CaptureToken> Execute { get; init; } = DefaultAction;
  public static NodeRule KeepAndIgnore (string groupName, bool appendToPrefix = false, bool appendToSuffix = false) => new()
  {
    TokenName = groupName,
    Execute = (context, token) =>
    {
      if (!context.HasCurrent) return;
      if (appendToPrefix) context.Current.AppendToPrefix(token.Value);
      if (appendToSuffix) context.Current.AppendToSuffix(token.Value);
    }
  };
  public static NodeRule Discard (string groupName, bool appendToPrefix = false, bool appendToSuffix = false, string replace_with = EmptyString) => new()
  {
    TokenName = groupName,
    Execute = (context, _) =>
    {
      if (replace_with.IsEmpty) return;
      if (!context.HasCurrent) return;
      if (appendToPrefix) context.Current.AppendToPrefix(replace_with);
      if (appendToSuffix) context.Current.AppendToSuffix(replace_with);
    }
  };
  public static NodeRule ReplaceEmpty (string groupName, bool appendToPrefix = false, bool appendToSuffix = false, string if_empty_insert_text = EmptyString) => new()
  {
    TokenName = groupName,
    Execute = (context, token) =>
    {
      if (!context.HasCurrent) return;
      if (appendToPrefix) context.Current.AppendToPrefix(token.Value.IsEmpty ? if_empty_insert_text : token.Value);
      if (appendToSuffix) context.Current.AppendToSuffix(token.Value.IsEmpty ? if_empty_insert_text : token.Value);
    }
  };
  public static NodeRule SyntaxError (string groupName, bool throw_error = false, string error_message = EmptyString) => new()
  {
    TokenName = groupName,
    Execute = (_, _) =>
    {
      if (!throw_error)
      {
        Debug.Log(MsgClass.Warning, "Warning: " + error_message, "NodeRule");
      }
      else
      {
        Debug.Log(MsgClass.Error, "Error: " + error_message, "NodeRule");
        throw new ValidationException("Error: " + error_message);
      }
    }
  };
}

public class ValidationException : Exception
{
  public ValidationException () { }

  public ValidationException (string? message) : base(message) { }

  public ValidationException (string? message, Exception?innerException) : base(message, innerException) { }
}
