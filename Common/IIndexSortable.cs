#pragma warning disable CA1710 // Identifiers should have correct suffix

namespace Common;

/// <summary>Reresents an object sortable by its index.</summary>
public interface IIndexSortable : IComparable<IIndexSortable>, IComparable
{
  /// <summary>The index of this object.</summary>
  int Index { get; }
  /// <summary>Checks if the index is valid.</summary>
  bool IsValidIndex => Index >= 0;
}
