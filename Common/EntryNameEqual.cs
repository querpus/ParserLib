using Microsoft.Win32;

namespace Common;

/// <summary>An <see cref="IEqualityComparer{T}"/> comparer to determine if the name of the <see cref="IProperty{T}"/> is the same as another.</summary>
public sealed class EntryNameEqual : IEqualityComparer<IProperty<object>>, IEqualityComparer<KeyValuePair<string, object>>, IEqualityComparer<RegistryKey>
{
  public static EntryNameEqual Invariant { get; } = new() { _compare = SCI };
  public static EntryNameEqual Ordinal { get; } = new() { _compare = SCO };
  public static EntryNameEqual InvariantIgnoreCase { get; } = new() { _compare = SCIIC };
  public static EntryNameEqual OrdinalIgnoreCase { get; } = new() { _compare = SCOIC };
  private StringComparison _compare = SCO;
  public bool Equals (IProperty<dynamic>? x, IProperty<dynamic>? y) =>
    (x is null && y is null) || (x?.Key.Equals(y?.Key, _compare) ?? false);
  public bool Equals (KeyValuePair<string, dynamic> x, KeyValuePair<string, dynamic> y) => x.Key.Equals(y.Key, _compare);
  public bool Equals (RegistryKey? x, RegistryKey? y) => (x is null && y is null) || (x?.Name.Equals(y?.Name, _compare) ?? false);
  public int GetHashCode (IProperty<dynamic> obj) => obj?.Key.GetHashCode(_compare) ?? 0;
  public int GetHashCode (KeyValuePair<string, dynamic> obj) => obj.Key.GetHashCode(_compare);
  public int GetHashCode (RegistryKey obj) => obj.Name.GetHashCode(_compare);
}
