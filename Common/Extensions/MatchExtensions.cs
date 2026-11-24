//#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace Common.Extensions;

public static class MatchExtensions
{
  extension(Match match)
  {
    public bool HasValidGroup (string group_name) =>
      match.Groups.ContainsKey(group_name) &&
      match.Groups[group_name].Value.IsNotEmpty;
    public bool TryGetValidGroupContent (string group_name, [NotNullWhen(true)] out string? content)
    {
      if (match.HasValidGroup(group_name))
      {
        content = match.Groups[group_name].Captures.Count > 1 ? match.Groups[group_name].Captures[^1].Value : match.Groups[group_name].Value;
        return true;
      }
      content = null;
      return false;
    }
    /// <summary>Gets the last capture of the specified group.</summary>
    /// <param name="group_name">The regex group to retrieve.</param>
    /// <returns>The value of the last capture of the specified group, or <see langword="null"/> if the group does not exist or is empty.</returns>
    public string? GetGroup (string group_name) =>
      match.TryGetValidGroupContent(group_name, out string? content) ? content : null;
    public string[]? GetCaptures (string group_name)
    {
      if (!match.HasValidGroup(group_name))
        return null;

      return match.Groups[group_name].Captures.Count > 1 ? [.. match.Groups[group_name].Captures.Select(c => c.Value)] : [match.GetGroup(group_name)!];
    }
  }
}
