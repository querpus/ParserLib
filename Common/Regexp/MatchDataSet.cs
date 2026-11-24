namespace Common.RegExp;
/// <summary>
/// A collection of groups and their captures from a regex match. This object includes additional functions and properties to make working with regex matches easier.</summary>
public sealed class MatchDataSet : GroupDataSet,
  ICollection<GroupDataSet>,
  IEquatable<MatchDataSet>,
  IEquatable<string>
{
  /// <summary>
  /// The group name, unused in this class.</summary>
  public override string Name => SE;
  /// <summary>
  /// The capture groups.</summary>
  public SortedDictionary<string, GroupDataSet> Groups { get; init; } = [];
  /// <summary>
  /// The number of groups in the match.</summary>
  public override int Count => Groups.Count;
  /// <summary>
  /// Gets a value indicating whether the match has no groups.</summary>
  public override bool IsNull => Count == 0;
  /// <summary>
  /// The original <see cref="Match"/> object from which this <see cref="MatchDataSet"/> was created.</summary>
  public Match? Origin { get; }
  bool ICollection<GroupDataSet>.IsReadOnly { get; }
  /// <summary>
  /// Represents data extracted from a regular expression match, including its groups,  and provides additional
  /// functionality for working with the match.
  /// </summary>
  /// <remarks>The <see cref="Groups"/> property is initialized by converting the groups in the provided
  /// <see cref="Match"/> object into a sorted dictionary for easier access and manipulation.</remarks>
  /// <param name="m">The <see cref="Match"/> object containing the results of a regular expression match.  Cannot be <see
  /// langword="null"/>.</param>
  /// <param name="i"></param>
  public MatchDataSet (Match? m, int i = -1) : base(m)
  {
    Groups = m is null ? [] : m.ToSortedDictionary();
    Origin = m;
    Index = i;
  }

  /// <summary>
  /// Empty constructor for serialization purposes only.</summary>
  public MatchDataSet () { }
  /// <summary>Checks if the specified group exists in the match.</summary>
  /// <param name="group">The name of the group.</param>
  /// <returns><see langword="true"/> if the group exists; otherwise, <see langword="false"/>.</returns>
  public bool HasGroup (string group) => Groups.Keys.Any(item => item.Like(group));
  /// <summary>
  /// Determines whether the specified group exists and contains no content.</summary>
  /// <param name="group">The name of the group to check. Cannot be null or empty.</param>
  /// <returns><see langword="true"/> if the group exists and its content is empty; otherwise, <see langword="false"/>.</returns>
  public new IEnumerator<GroupDataSet> GetEnumerator () => Groups.Values.GetEnumerator();
  /// <summary>
  /// Gets the GroupData associated with the specified group name.</summary>
  /// <param name="groupName">The group to look up.</param>
  /// <returns></returns>
  public GroupDataSet this[string groupName] => Groups.TryGetValue(groupName, out GroupDataSet? value) ? value : Null;
  /// <summary>
  /// Throws an <see cref="AbsentGroupException"/> if the specified group does not exist.</summary>
  /// <param name="groupName">The name of the group to check for.</param>
  /// <exception cref="AbsentGroupException"></exception>
  public void ThrowIfAbsent (string groupName)
  {
    if (!HasGroup(groupName))
      throw new AbsentGroupException(groupName);
  }
  /// <summary>Throws an <see cref="EmptyGroupException"/> if the specified group exists but has a length of 0,
  /// or an <see cref="AbsentGroupException"/> if the specified group does not exist.</summary>
  /// <param name="groupName">The name of the group to check for.</param>
  /// <exception cref="EmptyGroupException"></exception>
  /// <exception cref="AbsentGroupException"></exception>"
  public void ThrowIfEmpty (string groupName)
  {
    ThrowIfAbsent(groupName);

    if (this[groupName].Content.IsEmpty)
      throw new EmptyGroupException(groupName);
  }
  /// <summary>Returns a string representation of the current <see cref="MatchDataSet"/> object.</summary>
  /// <param name="indent">A string used for indentation in the output. Defaults to an empty string if not provided.</param>
  /// <remarks>The returned string includes a header and a detailed list of all groups in the match. If the
  /// match is empty, the string will indicate that no groups are present.</remarks>
  /// <returns>A string that represents the current <see cref="MatchDataSet"/> object, including its groups and their
  /// values.</returns>
  internal string ToString (string indent)
  {
    const string i1 = "  ";
    const string i2 = "    ";

    string head = indent + $"{typeof(MatchDataSet)} Object" + Chars.LFs;
    string grps = indent + i1 + "Groups = {";

    if (Count > 0)
    {
      grps += Chars.CRLF;
      foreach (KeyValuePair<string, GroupDataSet> grp in Groups)
      {
        grps += indent + i2 + grp.Key + " = " + grp.Value + Chars.LFs;
      }
      grps += indent + i1 + "}" + Chars.LFs;
    }
    else
    {
      grps += " <Empty> }" + Chars.LFs;
    }

    return head + grps;
  }

  /// <summary>Returns a string representation of the current <see cref="MatchDataSet"/> object.</summary>
  /// <remarks>The returned string includes a header and a detailed list of all groups in the match. If the
  /// match is empty, the string will indicate that no groups are present.</remarks>
  /// <returns>A string that represents the current <see cref="MatchDataSet"/> object, including its groups and their
  /// values.</returns>
  public override string ToString () => ToString(SE);
  /// <summary>
  /// Adds the specified <see cref="GroupDataSet"/> item to the collection.</summary>
  /// <param name="item">The <see cref="GroupDataSet"/> item to add to the collection. Cannot be null.</param>
  public void Add (GroupDataSet item) => Add(item);
  void ICollection<GroupDataSet>.Clear () => throw new NotSupportedException();
  bool ICollection<GroupDataSet>.Contains (GroupDataSet item) => throw new NotSupportedException();
  void ICollection<GroupDataSet>.CopyTo (GroupDataSet[] array, int arrayIndex) => throw new NotSupportedException();
  bool ICollection<GroupDataSet>.Remove (GroupDataSet item) => throw new NotSupportedException();
  public bool Equals (MatchDataSet? other)
  {
    if (other?.IsNull != false)
    {
      return IsNull;
    }

    if (other.Count == Count)
    {
      foreach (GroupDataSet item in other)
      {
        if (!this[item.Name].Content.Equals(item.Content, SCO))
          return false;
      }
      return true;
    }

    return false;
  }

  public override bool Equals (object? obj) => Equals(obj as MatchDataSet);
  public override int GetHashCode () => Content.GetHashCode(SCO);
  public bool Equals (string? other) => Content.Equals(other, SCO);
}
