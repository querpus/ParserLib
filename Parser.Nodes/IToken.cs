#pragma warning disable CA1710 // Identifiers should have correct suffix
#pragma warning disable format // Formatting

namespace Parser.Nodes;
/// <summary>The most basic token interface.</summary>
public interface IToken : IIndexSortable, ISerializable, IPrintable
{
  /// <summary>The value of this token. This is all of the text that this token consumed.</summary>
  string Content { get; }
  /// <summary>The name or type of this token.</summary>
  /// <remarks>This will be "None" by default.</remarks>
  /// <value>A value of "None", "-", or "" means there was no type assigned, likely due to it not needing to have one.</value>
  string Type { get; }
  bool HasType => Type?.Like("None", "-", SE) == false;
}
