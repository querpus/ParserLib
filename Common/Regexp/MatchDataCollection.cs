namespace Common.RegExp;

/// <summary>A collection of <see cref="MatchDataSet"/> objects.</summary>
public sealed class MatchDataCollection : Collection<MatchDataSet>
{
  /// <summary>Gets the debugging and internal values for this object.</summary>
  /// <returns>The debugging and internal values for this object.</returns>
  public override string ToString ()
  {
    const string i1 = "  ";
    const string i2 = "    ";
    string head = $"{typeof(MatchDataCollection)} Object" + Chars.CRLF;
    string mdcs = i1 + "Matches = {";
    if (Count > 0)
    {
      mdcs += Chars.CRLF;
      foreach (MatchDataSet mdc in this)
      {
        mdcs += i2 + mdc.ToString(i2) + Chars.CRLF;
      }
      mdcs += i1 + "}" + Chars.CRLF;
    }
    else
    {
      mdcs += " <Empty> }" + Chars.CRLF;
    }
    return head + mdcs;
  }
}
