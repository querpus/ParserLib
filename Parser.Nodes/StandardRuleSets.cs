#pragma warning disable CA1710 // Identifiers should have correct suffix
#pragma warning disable IDE0055 // Formatting

namespace Parser.Nodes;

public static partial class StandardRuleSets
{
  private static string SGrp(string special_group_letters, [SS("regex")] string inner_content, string? token_type = null, int? index = null)
  {
    string result = inner_content;
    foreach (char c in special_group_letters)
    {
      string group = c switch
      {
        'g' /*Token Type*/  => $"_g_{token_type}",
        'i' /*Regex Index*/ => $"_i_{index}",
        'e' /*Error Index*/ => $"_e_{index}",
      };
      result = RxS.GrpNm(group, result);

      if (c == 'e')
      {
        result = RxS.GrpNm("error", result);
      }
    }
    return result;
  }

  public static string GenerateRegexForMatchTokenizer (IEnumerable<(string Type, string Regex)> list) => list
    .Select((tuple, i) => SGrp("gi", tuple.Regex, tuple.Type, i))
    .Aggregate((s1, s2) => $"{s1}|{s2}");
  public static string GenerateRegexForErrorChecking (IEnumerable<(string ID, string Regex)> list) => list
    .Select((tuple, i) => SGrp("eg", tuple.Regex, tuple.ID, i))
    .Aggregate((s1, s2) => $"{s1}|{s2}");
}
