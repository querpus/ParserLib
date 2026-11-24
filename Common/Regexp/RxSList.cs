//#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace Common.RegExp;

/// <summary>A collection of </summary>
public sealed class RxSCollection : ICollection<RxS>, IEquatable<RxSCollection>, IComparable<RxSCollection>
{
  private readonly List<RxS> _list;
  public int Count => _list.Count;
  /// <summary>The elements in this collection combined with the '|' operator.</summary>
  public RxS Combined => RxS.Grp(_list.TextJoin("|"));

  bool ICollection<RxS>.IsReadOnly => false;

  public RxSCollection ([SS("regex")] params string[] list) => _list = [.. list];
  public RxSCollection ([SS("regex")] string item) => _list = [item];
  public RxSCollection () => _list = [];

  public static implicit operator Collection<RxS> (RxSCollection from) =>
    from is null ? [] : from.Count != 0 ? [.. from._list] : [];
  public static implicit operator RxS (RxSCollection from) =>
    from?.Count != 0 ? from?.Combined ?? SE : SE;
  public static implicit operator string (RxSCollection from) =>
    from is not null && from.Count != 0 ? from.Combined : SE;
  public static explicit operator Regex (RxSCollection from) =>
    from is not null && from.Count != 0 ? new(from.Combined) : throw new ANEx(nameof(from));
  public static implicit operator RxSCollection (RxS from) =>
    [from];
  public static implicit operator RxSCollection (string from) =>
    [from];
  public static implicit operator RxSCollection (Collection<RxS> from) =>
    [.. from];
  public IEnumerator<RxS> GetEnumerator () => _list.GetEnumerator();
  IEnumerator IEnumerable.GetEnumerator () => GetEnumerator();
  public void Add (RxS item) => _list.Add(item);
  public void Clear () => _list.Clear();
  public override string ToString () => Combined;
  public override int GetHashCode () => Combined.GetHashCode();
  public override bool Equals (object? obj) => Combined.Equals(obj?.ToString());
  /// <inheritdoc/>
  public bool Equals (RxSCollection? other) => Combined.Equals(other?.Combined);
  /// <inheritdoc/>
  public int CompareTo (RxSCollection? other) => Combined.CompareTo(other?.Combined);
  bool ICollection<RxS>.Contains (RxS item) => throw new NotSupportedException();
  void ICollection<RxS>.CopyTo (RxS[] array, int arrayIndex) => throw new NotSupportedException();
  bool ICollection<RxS>.Remove (RxS item) => throw new NotSupportedException();
  public static bool operator == (RxSCollection left, string right) => (left is not null || right is not null) && left?.Equals(right) == true;
  public static bool operator != (RxSCollection left, string right) => !(left == right);
  public static bool operator < (RxSCollection left, RxSCollection right) => left?.CompareTo(right) < 0;
  public static bool operator <= (RxSCollection left, RxSCollection right) => left?.CompareTo(right) <= 0;
  public static bool operator > (RxSCollection left, RxSCollection right) => left?.CompareTo(right) > 0;
  public static bool operator >= (RxSCollection left, RxSCollection right) => left?.CompareTo(right) >= 0;
}
