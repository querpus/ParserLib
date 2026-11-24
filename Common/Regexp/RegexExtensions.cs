//#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace Common.RegExp;

/// <summary>A static class contianing extension methods for <see cref="Regex"/> related types.</summary>
public static class RegexExtensions
{
  extension(Match match)
  {
    /// <summary>Checks if the specified group exists in the match.</summary>
    /// <param name="group">The name of the group.</param>
    /// <returns><see langword="true"/> if the group exists; otherwise, <see langword="false"/>.</returns>
    /// <remarks>This implementation is case-insensitive.</remarks>
    public bool HasGroup (string group) => match.Groups.Keys.Any(item => item.Like(group));
    /// <summary>Checks if the specified group exists in the match and returns the number of captures.</summary>
    /// <param name="group">The name of the group.</param>
    /// <returns>0 if the group does not exist, otherwise the number of captures in the group, with a minimum value of 1 if the group exists.</returns>
    /// <remarks>This implementation is case-insensitive.</remarks>
    public int GroupCaptureCount (string group) => match.HasGroup(group) ? match.Groups[group].Captures.Count : 0;
    public Dictionary<string, string> ToDictionary () => [..
      from Group grp in match.Groups
      select new KeyValuePair<string, string>(grp.Name, grp.Value)
    ];
  }

  #region MatchCollection Extensions
  /// <summary>Exposes the <see cref="IEnumerator{Match}"/> for <paramref name="mc"/>.</summary>
  /// <param name="mc">The <see cref="MatchCollection"/> to enumerate.</param>
  /// <returns>The <see cref="IEnumerator{Match}"/>.</returns>
  public static IEnumerator<Match> GetEnumerator (this MatchCollection mc) => mc.ToImmutableList().GetEnumerator();
  public static MatchDataCollection ToMDDCollection (this MatchCollection mc) =>
    [.. mc.Select(item => item.ToMatchData())];
  #endregion
  #region Match Extensions
  public static Dictionary<string, GroupDataSet> ToGroupDictionary (this Match match) =>
    match is null ? [] :
    [.. from Group grp in match.Groups
        where grp.Value.Length > 0
        select new KeyValuePair<string, GroupDataSet>(grp.Name, grp.ToGroupData())];
  public static SortedDictionary<string, GroupDataSet> ToSortedDictionary (this Match match) =>
    match is null ? [] :
    [.. from Group grp in match.Groups
        where grp.Value.Length > 0
        select new KeyValuePair<string, GroupDataSet>(grp.Name, grp.ToGroupData())];
  public static MatchDataSet ToMatchData (this Match match) => new(match);
  #endregion
  #region Group Extensions
  public static KeyValuePair<string, string> ToKvp (this Group group)
  {
    group.ThrowIfNull();
    return new(group.Name, group.Value);
  }
  public static GroupDataSet ToGroupData (this Group group) => new(group);
  #endregion
  public static CaptureData ToCaptureData (this Capture cap, string groupname) =>
    new(cap, groupname);
}
