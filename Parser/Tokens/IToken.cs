#pragma warning disable CA1710 // Identifiers should have correct suffix

namespace Parser.Tokens;

public interface IToken : IEquatable<IToken>, Nodes.IToken
{
  Spec Spec { get; }
  /// <summary>Assigned at match, cleared if match fails, kept if passes.</summary>
  TokenRef? AssignTo { get; set; }
  string ContentNoNewLine => Content.Replace(["\n", "\r"], ["<LF>", "<CR>"]).ToString(true);
  string ToString (int indent);
}
