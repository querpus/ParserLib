#if HAS_OFFICE
using Common.RegExp;

using static Common.RegExp.RegexStaticFunctions;

using SysRegex = System.Text.RegularExpressions.Regex;

namespace Common.Excel;

/// <summary>A class representing a range of cells in Excel, which can include standard cell references, table references, and named ranges.</summary>
public class Range : IEquatable<Range>, IComparable<Range>, IEnumerable<RangeNode>
{
  #region Static Members
  protected static ReadOnlyCollection<string> ExcelRefRegex => new([ExcelRef, TableCell, TableCol, TableRef]);
  public static SysRegex RangeRegex { get; } = new(ExcelRefRegex.AggregateRegex(), ROIPW);
  /// <summary><c>[inside]</c></summary>
  /// <param name="inside">Regex inside braces.</param>
  /// <returns>A string Regex of a column structure.</returns>
  protected static string ColumnRx ([SS("Regex")] string inside) => @$"\[{Nm("column", inside)}\]";
  /// <summary>
  /// <para>Regex Development: <seealso href="https://regex101.com/r/I9S9Yi/7"/></para>
  /// <c>NoRangeOp</c> - No colon follows.<br/>
  /// <c>TableRx</c> - Table or named range.<br/>
  /// <c>CommaSep</c> - Optional comma.<br/>
  /// <c>TableRef</c> - Optional comma &amp; table or named range.<br/>
  /// <c>TableCol</c> - Optional comma &amp; table or named range &amp; column.<br/>
  /// <c>TableCell</c> - Optional comma &amp; table or named range &amp; this row, column.<br/>
  /// <c>ExcelRef</c> - A standard excel range reference.<br/>
  /// </summary>
  protected static readonly string
    InnerCol = Gp(Rx(@"\[[^\n\r\]\[]\]+").Or(@"\w+")),
    NoRangeOp = Rx(@"(?!\w*\:)"),
    TableRx = Nm("table", @"\w+") + NoRangeOp,
    CommaSep = Nm("next", @"\s*\,\s*").Opt,
    TableRef = CommaSep + TableRx,
    TableCol = CommaSep + TableRx + ColumnRx(@"\w+"),
    TableCell = CommaSep + Nm("cell", TableRef + ColumnRx($@"\[\@{InnerCol}\]")),
    ExcelRef = CommaSep + Nm("cellref", """
    (?<abscol>\$)?
    (?<letter>[A-Z]{1,3})
    (?<absrow>\$)?
    (?<number>[0-9]{1,7})
    (?:
      (?<range>\:)
      (?<maxabscol>\$)?
      (?<maxletter>[A-Z]{1,3})
      (?<maxabsrow>\$)?
      (?<maxnumber>[0-9]{1,7})
    )?
    """);

  protected static Collection<RangeNode> DoParse (string address)
  {
    Collection<RangeNode> result = [];
    Collection<MatchDataSet> mdds = RangeRegex.Matches(address).ToMDDCollection();

    if (mdds.Count == 0)
      goto RETURN_RESULT;

    foreach (MatchDataSet mdd in mdds)
    {
      RangeNode temp = RangeNode.Generate(mdd);
      if (!temp.IsEmpty)
        result.Add(temp);
    }

  RETURN_RESULT:
    return result;
  }
  #endregion

  public Range (string address)
  {
    List<RangeNode> list = [.. DoParse(address)];
    list.Sort();
    Nodes = [.. list];
  }

  public Collection<RangeNode> Nodes { get; init; }
  public int Count => Nodes.Aggregate(0, (total, node) => total += node.Count);

  public int CompareTo (Range? other) => throw new NotImplementedException();
  public bool Equals (Range? other) => GetHashCode() == other?.GetHashCode();
  public IEnumerator<RangeNode> GetEnumerator () => Nodes.GetEnumerator();
  IEnumerator IEnumerable.GetEnumerator () => GetEnumerator();

  public override bool Equals (object? obj)
  {
    if (obj is not Range and not RangeNode)
      return false;

    if (obj is Range rng)
    {
      for (int i = 0; i < Count; i++)
      {
        if (!Nodes[i].Equals(rng[i]))
          return false;
      }

      return true;
    }

    return obj is RangeNode node && Count == 1 && Nodes[0].Equals(node);
  }
  public override int GetHashCode ()
  {
    if (Nodes.Count == 0)
      return 0;

    int result = Nodes[0].GetHashCode();

    for (int i = 1; i < Nodes.Count; i++)
    {
      RangeNode node = Nodes[i];
      result = HashCode.Combine(result, node.GetHashCode());
    }
    return result;
  }

  public RangeNode this[int index] => Nodes[index];
  public static bool operator == (Range left, Range right) => left is null ? right is null : left.Equals(right);
  public static bool operator != (Range left, Range right) => !(left == right);
  public static bool operator < (Range left, Range right) => left is null ? right is not null : left.CompareTo(right) < 0;
  public static bool operator <= (Range left, Range right) => left is null || left.CompareTo(right) <= 0;
  public static bool operator > (Range left, Range right) => left?.CompareTo(right) > 0;
  public static bool operator >= (Range left, Range right) => left is null ? right is null : left.CompareTo(right) >= 0;
}
#endif
