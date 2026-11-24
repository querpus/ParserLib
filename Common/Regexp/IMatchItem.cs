namespace Common.RegExp;

/// <summary>An item that matches a regex pattern.</summary>
public interface IMatchItem : IIndexSortable
{
  /// <summary>The length of the match.</summary>
  int Len { get; }
  /// <summary>The next position after the match.</summary>
  int AfterIndex { get; }
  /// <summary>The position of the last character in the match.</summary>
  int FinalIndex { get; }
  /// <summary>The content of the match item, which is the matched string.</summary>
  string Content { get; }
  /// <summary>The name of the match item, which is typically used for named capturing groups in regex.</summary>
  string Name { get; }
  /// <summary>Whether the match item is null or empty.</summary>
  bool IsNull { get; }
}
