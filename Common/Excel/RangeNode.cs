#if HAS_OFFICE
using Common.RegExp;

using XlRange = Microsoft.Office.Interop.Excel.Range;

namespace Common.Excel;
/// <summary>A node within a range.</summary>
public class RangeNode : IEquatable<RangeNode>, IComparable<RangeNode>
{
  #region Static Members
  public static RangeNode Empty => new();
  private void Log (string msg) =>
    Debug.Log(MsgClass.Debug, msg, this);
  #endregion

  #region Public Properties
  /// <summary>The containing worksheet.</summary>
  public string Sheet { get; set; } = SE;
  public string Address { get; set; } = SE;
  public bool RowAbs { get; set; }
  /// <summary>The row index.</summary>
  public int? Row { get; set; }
  public bool ColAbs { get; set; }
  /// <summary>The column index;</summary>
  public string? Col { get; set; }
  /// <summary>Whether or not this is the whole row or not.</summary>
  public bool WholeRow { get; set; }
  public bool WholeCol { get; set; }

  public bool MultipleCells => Count > 1;
  public int Count { get; set; }

  public int? RowMin => Row;
  public string? ColMin => Col;
  public string MinAddress => Col + Row;
  public bool RowMaxAbs { get; set; }
  public int? RowMax { get; set; }
  public string? ColMax { get; set; }
  public bool ColMaxAbs { get; set; }
  public string MaxAddress => ColMax + Row;

  public string? Table { get; set; }
  public string? TColumn { get; set; }
  public bool ThisRow { get; set; }

  public bool IsEmpty => Address.IsEmpty();
  public bool IsError => Equals(Empty) && !IsEmpty;
  public IMatchItem ParseData { get; protected set; }
  #endregion

  protected XlRange? Rng { get; set; }

  public RangeNode () => ParseData = GroupDataSet.Null;
  public static RangeNode Generate (MatchDataSet mdd)
  {
    ANEx.ThrowIfNull(mdd);
    RangeNode temp = Empty;
    temp.SetParseData(mdd);

    if (mdd.HasGroup("letter"))
      temp.Col = mdd["letter"].Content;
    if (mdd.HasGroup("number"))
      temp.Col = mdd["number"].Content;

    temp.Sheet = mdd["sheet"].Content;

    temp.ColAbs = mdd.HasGroup("abscol");
    temp.RowAbs = mdd.HasGroup("absrow");
    temp.ColMaxAbs = mdd.HasGroup("maxabscol");
    temp.RowMaxAbs = mdd.HasGroup("maxabsrow");

    temp.Address = mdd.Content;

    //temp.rng = sheet.Range[temp.MinAddress, temp.MaxAddress];

    temp.WholeRow = !mdd.HasGroup("letter") && mdd.HasGroup("number");
    temp.WholeCol = !mdd.HasGroup("number") && mdd.HasGroup("letter");

    if (mdd.HasGroup("table"))
      temp.Table = mdd["table"].Content;
    if (mdd.HasGroup("column"))
      temp.TColumn = mdd["column"].Content;
    temp.ThisRow = mdd.HasGroup("thisrow");

    if (temp.IsError)
      Log("ERROR: " + mdd.Content);
    else
      Log(mdd.Content);

    return temp;
  }
  /// <inheritdoc/>
  public override bool Equals (object? obj) =>
    Address.Equals(((RangeNode?) obj)?.Address, SCOIC);
  /// <inheritdoc/>
  public override int GetHashCode () =>
    HashCodeExtensions.Combine(Row, Col, WholeRow, WholeCol, RowMax, ColMax, Table, TColumn, ThisRow);

  /// <inheritdoc/>
  public int CompareTo (RangeNode? other) => Address.CompareTo(other?.Address, SCOIC);
  /// <inheritdoc/>
  public bool Equals (RangeNode? other) => Address.Equals(other?.Address, SCOIC);
  public void SetParseData (MatchDataSet data) => throw new NotSupportedException();

  #region Operators
  public static bool operator == (RangeNode left, RangeNode right) =>
  left is null ? right is null : left.Equals(right);
  public static bool operator != (RangeNode left, RangeNode right) =>
    !(left == right);
  public static bool operator < (RangeNode left, RangeNode right) =>
    left is null ? right is not null : left.CompareTo(right) < 0;
  public static bool operator <= (RangeNode left, RangeNode right) =>
    left is null || left.CompareTo(right) <= 0;
  public static bool operator > (RangeNode left, RangeNode right) =>
    left?.CompareTo(right) > 0;
  public static bool operator >= (RangeNode left, RangeNode right) =>
    left is null ? right is null : left.CompareTo(right) >= 0;
  #endregion
}
#endif
