using Microsoft.Win32;

namespace Common;

/// <summary>An <see cref="IEqualityComparer{T}"/> comparer to determine if the name of the <see cref="IProperty{T}"/> is the same as another.</summary>
public sealed class EntryNameEqual : IEqualityComparer<IProperty<object>>, IEqualityComparer<KeyValuePair<string, object>>, IEqualityComparer<RegistryKey>
{
  public static EntryNameEqual Invariant { get; } = new() { Compare = SCI };
  public static EntryNameEqual Ordinal { get; } = new() { Compare = SCO };
  public static EntryNameEqual InvariantIgnoreCase { get; } = new() { Compare = SCIIC };
  public static EntryNameEqual OrdinalIgnoreCase { get; } = new() { Compare = SCOIC };
  private StringComparison Compare = SCO;
  public bool Equals (IProperty<dynamic>? x, IProperty<dynamic>? y) =>
    (x is null && y is null) || (x?.Key.Equals(y?.Key, Compare) ?? false);
  public bool Equals (KeyValuePair<string, dynamic> x, KeyValuePair<string, dynamic> y) => x.Key.Equals(y.Key, Compare);
  public bool Equals (RegistryKey? x, RegistryKey? y) => (x is null && y is null) || (x?.Name.Equals(y?.Name, Compare) ?? false);
  public int GetHashCode (IProperty<dynamic> obj) => obj?.Key.GetHashCode(Compare) ?? 0;
  public int GetHashCode (KeyValuePair<string, dynamic> obj) => obj.Key.GetHashCode(Compare);
  public int GetHashCode (RegistryKey obj) => obj.Name.GetHashCode(Compare);
}
