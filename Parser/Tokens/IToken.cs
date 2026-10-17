#pragma warning disable CA1710 // Identifiers should have correct suffix

namespace Parser.Tokens;

public interface IToken : IEquatable<IToken>, Common.Nodes.IToken
{
  Spec Spec { get; }
  /// <summary>Assigned at match, cleared if match fails, kept if passes.</summary>
  TokenRef? AssignTo { get; set; }
  string Type { get; }
  bool HasType { get; }
  IToken? Parent { get; set; }
  string Content { get; }
  string? Common.Nodes.IToken.Value => Content;
  string ContentNoNewLine => Content.Replace(["\n", "\r"], ["<LF>", "<CR>"]);
  string ToString (int indent);
  void Print (int indent);
}
