//#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace Common.RegExp;

/// <summary>Represents a group in a match.</summary>
public class GroupDataSet : CaptureData, IReadOnlyCollection<CaptureData>, IEquatable<GroupDataSet>
{
  /// <summary>The captures within this group.</summary>
  public Collection<CaptureData> Captures { get; init; } = [];

  internal GroupDataSet () { }
  /// <summary>Creates a new <see cref="GroupDataSet"/> from a <see cref="Group"/>.</summary>
  /// <param name="g">The <see cref="Group"/> to base this object on.</param>
  /// <param name="index">The group number of this group.</param>
  public GroupDataSet (Group? g, int index = -1) : base(g, g is null ? SE : g.Name, index) =>
    Captures = g is null ?
    [] :
    [.. g.Captures.Select((item, i) => new CaptureData(item, g.Name, i))];
  /// <summary>Manual Constructor.</summary>
  /// <param name="name">Group name.</param>
  /// <param name="content">Group content.</param>
  /// <param name="pos">Position in text.</param>
  /// <param name="len">Length of group.</param>
  /// <param name="index">TODO: Identify what i meant to do with this.</param>
  protected GroupDataSet (string name, string content, int pos, int len, int index = -1) : base(content, pos, len, name, index) =>
    Captures = [];
  /// <summary>Gets the capture data at the given index.</summary>
  /// <param name="index">The index of the data.</param>
  /// <returns>The data at the given index.</returns>
  public CaptureData this[int index] => Captures[index];
  /// <inheritdoc/>
  public IEnumerator<CaptureData> GetEnumerator () => Captures.GetEnumerator();
  IEnumerator IEnumerable.GetEnumerator () => GetEnumerator();

  public int IndexOfLastUnderscore => Name.LastIndexOf('_', SCO);
  public string NiceName => Name[(IndexOfLastUnderscore + 1)..];
  public bool IsRemoveGroup => Name.StartsWith("x_", SCO) && Content.Length > 0;
  public bool IsMatchProperty => Name.StartsWith("m_prop_", SCOIC);
  public bool IsToken => Name.StartsWith("t_", SCOIC);
  public bool IsMatchPropertyKey => Name.StartsWith("m_prop_key_", SCOIC);
  public bool IsMatchPropertyValue => Name.StartsWith("m_prop_value_", SCOIC);
  /// <summary>Whether the group is a named group or not.</summary>
  public bool IsNamedGroup => Name.IsNamedGroup && Content.Length > 0;
  public override bool IsNull => Content.Length == 0 || Captures.Count == 0;
  public virtual int Count => IsNull ? 0 : Captures.Count > 0 ? Captures.Count : 1;
  public static implicit operator GroupDataSet (Group group) => FromGroup(group);
  /// <summary>A null or empty group data object.</summary>
  public static GroupDataSet Null { get; } = new(string.Empty, string.Empty, -1, -1);
  public static GroupDataSet FromGroup (Group group) => group is null ? Null : new(group.Name, group.Value, group.Index, group.Length);

  /// <summary>Creates a string representation of the group and its captures.</summary>
  /// <returns>The string representation of the group and its captures.</returns>
  public override string ToString () =>
    Count > 1 ? $"[ {Captures.Select(static item => item.ToString()).TextJoin(", ")} ]" :
    Count == 1 ? Captures[0].ToString() :
    "<null data>";
  public KeyValuePair<string, GroupDataSet> ToKVP () => new(Name, this);
  /// <inheritdoc/>
  public bool Equals (GroupDataSet? other) => other?.IsNull != false ? IsNull : other.Content.Equals(Content, SCO);

  /// <inheritdoc/>
  public override bool Equals (object? obj) => Equals(obj as GroupDataSet);

  /// <inheritdoc/>
  public override int GetHashCode () => HashCode.Combine(Name, Content, Index, Len);
}
