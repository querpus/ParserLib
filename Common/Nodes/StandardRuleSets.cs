#pragma warning disable CA1710 // Identifiers should have correct suffix
#pragma warning disable IDE0055 // Formatting

namespace Common.Nodes;

public static partial class StandardRuleSets
{
  public static string GenerateRegexForMatchTokenizer ( IEnumerable<(string Name, string Regex)> list)
  {
    Collection<string> result = [];
    int index = 0;

    // Makes an index group to id the regex string used, and the name group to assign the token type.
    foreach (var item in list)
    {
      result.Add($"(?'_i_{index}' (?'_g_{item.Name}' {item.Regex}))");
      index++;
    }
    return result.TextJoin("|");
  }
  public static string GenerateRegexForErrorChecking (IEnumerable<(string ID, string Regex)> list)
    {
      string result = SE;
      int index = 0;

      foreach (var item in list)
      {
        result += (result.IsNotEmpty ? "|" : "(?'error'") + $"(?'_e_{index}' (?'_g_{item.ID}' {item.Regex}))";
        index++;
      }
      return result + ")";
    }
}
