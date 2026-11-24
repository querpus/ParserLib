#pragma warning disable IDE0028 // Simplify collection initialization

using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace Specification.IPL;

/// <summary>Contains the details of an IPL command.</summary>
public class CommandDataSet : IEquatable<CommandDataSet>, ISerializable, IComparable<CommandDataSet>, IReadOnlyCollection<object>
{
  /// <summary>The data that created this object.</summary>
  public MatchDataSet Origin { get; }

  /// <summary>The full text representation of this object at creation.</summary>
  public string FullCommandText { get; set; }
  /// <summary><see langword="true"/> if the command was escaped, <see langword="false"/> otherwise.</summary>
  public bool IsEscaped { get; set; }
  /// <summary><see langword="true"/> if the command was shifted, <see langword="false"/> otherwise.</summary>
  public bool IsShifted { get; set; }
  /// <summary>The command executed.</summary>
  public string CmdLetter { get; set; }
  /// <summary>The field content if any is set.</summary>
  public string? FieldText { get; set; }
  /// <summary>The number of labels to print in sequence.</summary>
  public int PrintQty { get; set; }
  /// <summary>The number of labels to print in each set.</summary>
  public int BatchPrintQty { get; set; }
  /// <summary>The field number defined for this field data.</summary>
  public int FieldNum { get; set; }
  /// <summary>The data attached to this command.</summary>
  public Dictionary<int, object> Data { get; } = [];
  /// <summary>The properties assigned to a line command.</summary>
  public Collection<CommandDataSet> Properties { get; } = [];
  /// <inheritdoc/>
  public int Count => Data.Count;
  /// <summary>The format the command was called in.</summary>
  public int Format { get; set; }
  /// <summary>The mode that the command was called in.</summary>
  public IPLPrinterMode Mode { get; set; }
  /// <summary>The mode that this command assigns, if any.</summary>
  public IPLPrinterMode CmdMode => CmdLetter switch
  {
    "R" when !IsEscaped && !IsShifted => IPLPrinterMode.Print,
    "C" when IsEscaped && !IsShifted => IPLPrinterMode.Advanced,
    "c" when IsEscaped && !IsShifted => IPLPrinterMode.Emulation,
    "P" when IsEscaped && !IsShifted => IPLPrinterMode.Program,
    "T" when IsEscaped && !IsShifted => IPLPrinterMode.TestAndService,
    "g" when IsEscaped && !IsShifted => IPLPrinterMode.DirectGraphics,
    "h" when !IsEscaped && IsShifted => IPLPrinterMode.PrintHeadLoading,
    _ => IPLPrinterMode.None
  };
  /// <summary>The command type.</summary>
  public ICT Type => CmdLetter switch
  {
    "<RS>" or "<US>" => ICT.Qty,
    "<EOT>" => ICT.Advanced,
    string when CmdLetter.StartsWith('<', SCO) => ICT.Simple,
    string when CmdMode != IPLPrinterMode.None => ICT.Mode,
    string when "BDGHILQUW".Contains(CmdLetter, SCO) && !IsEscaped && !IsShifted => ICT.Line,
    string when "bcdfhijklmrowxyu".Contains(CmdLetter, SCO) && !IsEscaped && !IsShifted => ICT.Prop,
    "A" or "F" when !IsEscaped && !IsShifted => ICT.SetFormat,
    "E" when !IsEscaped && !IsShifted => ICT.ClearFormat,
    "E" when IsEscaped && !IsShifted => ICT.SelectFormat,
    "P" or "R" when !IsEscaped && !IsShifted => ICT.Simple,
    "C" or "P" or "c" when IsEscaped && !IsShifted => ICT.Simple,
    string when Mode is IPLPrinterMode.Print && !IsEscaped && !IsShifted => ICT.FieldData,
    "F" when IsEscaped && Mode is IPLPrinterMode.Print => ICT.FieldSet,
    _ => ICT.Unknown
  };

  // Formats:
  //
  // Field
  //   FieldNum = 
  //   Data[1] = Text (string)
  // Qty
  //   Data[0] = Qty (int)
  // Simple
  //   No Data Stored
  // Line
  //   Data[0] = Number (int)
  // Property
  //   Data[0] = Value 1
  //   Data[1] = Value 2
  //   Data[2] = Value 3
  //   etc.
  // Format
  //  Data[0] = Number (int)

  #region Constructors
  /// <summary>Creates an empty object. IsNull would equal <see langword="true"/>.</summary>
  public CommandDataSet ()
  {
    Origin = [];
    FullCommandText = SE;
    Data = [];
    CmdLetter = SE;
  }
  /// <summary>Creates an object from a <see cref="MatchDataSet"/>. IsNull would equal <see langword="false"/>.</summary>
  /// <param name="mdd">The <see cref="MatchDataSet"/> to create an object from.</param>
  public CommandDataSet ([NotNull] MatchDataSet mdd) : this()
  {
    FullCommandText = mdd.Content;
    Origin = mdd;
    ParseMDD(mdd);
  }
  /// <summary>Creates an object from a <see cref="string"/>. IsNull would equal <see langword="false"/>.</summary>
  /// <param name="fullcmdstr">The <see cref="string"/> to create an object from.</param>
  public CommandDataSet (string fullcmdstr) : this()
  {
    FullCommandText = fullcmdstr;
    Origin = GenerateMDD(fullcmdstr);
    ParseMDD();
  }
  #endregion

  #region Private Methods
  private MatchDataSet GenerateMDD (string commandText)
  {
    FullCommandText = commandText;
    Match match = Definition.OpRegex.Match(FullCommandText);
    return new MatchDataSet(match);
  }
  private void ParseMDD (MatchDataSet? toParse = null)
  {
    MatchDataSet parseMe =
      toParse is null && Origin is null ? throw new InvalidOperationException("MDD Cannot be null") :
      toParse is null && Origin is not null ? Origin :
      toParse ?? throw new InvalidOperationException();
    CmdLetter = parseMe["letter"].Content;
    if (parseMe.HasGroup("qty"))
      CmdLetter = parseMe["qty"].Content;
    if (parseMe.HasGroup("simple"))
      CmdLetter = parseMe["simple"].Content;
    IsEscaped = parseMe.HasGroup("escape");
    IsShifted = parseMe.HasGroup("shift");
    if (parseMe.HasGroup("value") && parseMe["value"].Count > 0)
    {
      for (int i = 0; i < parseMe["value"].Count; i++)
      {
        Data[i] = parseMe["value"][i].Content;
      }
    }
    if (parseMe.HasGroup("fieldtext"))
    {
      FieldText = parseMe["fieldtext"].Content;
    }
  }
  #endregion

  /// <summary>Gets or sets the data associated with this command.</summary>
  /// <param name="index">The data index.</param>
  /// <returns>The data at the given index.</returns>
  public object this[int index]
  {
    get => Data[index];
    set => SetData(index, value);
  }

  /// <summary><see langword="true"/> if the command is empty. <see langword="false"/> otherwise.</summary>
  public bool IsNull => FullCommandText == SE && Count == 0;

  #region Public Get Methods
  /// <summary>Gets the data field at the given index as a <see langword="string"/>.</summary>
  /// <param name="index">The data index to access.</param>
  /// <returns>The data at the given index as a <see langword="string"/>.</returns>
  public string GetTextData (int index) => $"{Data[index]}";
  /// <summary>Gets the data field at the given index as an <see langword="int"/>.</summary>
  /// <param name="index">The data index to access.</param>
  /// <returns>The data at the given index as an <see langword="int"/>.</returns>
  public int GetIntData (int index)
  {
    DebugIn("CommandDataSet", "GetIntData");
    if (!Data.TryGetValue(index, out object? data_obj))
    {
      _ = Err.ThrowBadDef($"No data at index {index}.");
      throw null;
    }
    if (data_obj is int data_int)
    {
      return data_int;
    }
    _ = Err.ThrowBadInput("int", $"{data_obj.GetType()}");
    throw null;
  }
  /// <summary>Gets the data field at the given index as a <see langword="decimal"/>.</summary>
  /// <param name="index">The data index to access.</param>
  /// <returns>The data at the given index as a <see langword="decimal"/>.</returns>
  public decimal GetDecimalData (int index)
  {
    try
    {
      return (decimal) Data[index];
    }
    catch (InvalidCastException ice)
    {
      LogException(ice);
      return ErrVal;
    }
  }
  #endregion
  #region Public Set Methods
  /// <summary>Sets data at the given index to the specified value.</summary>
  /// <param name="index">The index to assign to.</param>
  /// <param name="data">The value to assign.</param>
  public void SetData (int index, object data) => Data[index] = data;
  /// <summary>Embeds a command inside of this command.</summary>
  /// <param name="data">The command to embed.</param>
  public void Add (CommandDataSet data) => Properties.Add(data);
  #endregion
  #region Interfaces & Overrides
  /// <inheritdoc/>
  public bool Equals (CommandDataSet? other) => FullCommandText.Equals(other?.FullCommandText, SCO);
  /// <inheritdoc/>
  public int CompareTo (CommandDataSet? other) => FullCommandText.CompareTo(other?.FullCommandText, SCO);
  /// <inheritdoc/>
  public override bool Equals (object? obj) => obj is CommandDataSet data && Equals(data);
  /// <inheritdoc/>
  public override int GetHashCode () => FullCommandText.GetHashCode(SCO);
  /// <inheritdoc/>
  public override string ToString () => Serialize();
  /// <inheritdoc/>
  public IEnumerator<object> GetEnumerator () => Data.Select(item => item.Value).GetEnumerator();
  IEnumerator IEnumerable.GetEnumerator () => GetEnumerator();
  /// <inheritdoc/>
  public string Serialize ()
  {
    string escape = IsEscaped ? "<ESC>" : SE;
    string shift = IsShifted ? "<SI>" : SE;
    string cmd = CmdLetter;
    string field = Count > 0 ? GetTextData(0) : SE;
    string props = Count > 1 ? $"{Data.Select(item => item.Value).Aggregate((i2, i3) => i2 = $"{i2},{i3}")}" : SE;
    return escape + shift + cmd + field + props;
  }
  /// <inheritdoc/>
  public static CommandDataSet Generate (MatchDataSet input) => new(input);
  #endregion
  #region Static Operators
  /// <summary>Checks basic equality with another CommonData object.</summary>
  /// <param name="left">The left object.</param>
  /// <param name="right">The right object.</param>
  /// <returns><see langword="true"/> if left equals right, <see langword="false"/> otherwise.</returns>
  public static bool operator == (CommandDataSet left, CommandDataSet right) => (left is null && right is null) || (left?.Equals(right) ?? false);
  /// <summary>Checks basic inequality with another CommonData object.</summary>
  /// <param name="left">The left object.</param>
  /// <param name="right">The right object.</param>
  /// <returns><see langword="true"/> if left does not equal right, <see langword="false"/> otherwise.</returns>
  public static bool operator != (CommandDataSet left, CommandDataSet right) => !(left == right);
  /// <summary>TODO: Doc</summary>
  /// <param name="left">The left object.</param>
  /// <param name="right">The right object.</param>
  public static bool operator < (CommandDataSet left, CommandDataSet right) => left?.CompareTo(right) < 0;
  /// <summary>TODO: Doc</summary>
  /// <param name="left">The left object.</param>
  /// <param name="right">The right object.</param>
  public static bool operator <= (CommandDataSet left, CommandDataSet right) => left?.CompareTo(right) <= 0;
  /// <summary>Whether or not the left object is greater than the right object.</summary>
  /// <param name="left">The left object.</param>
  /// <param name="right">The right object.</param>
  /// <returns><see langword="true"/> if the left object is greater than the right object, <see langword="false"/> otherwise.</returns>
  public static bool operator > (CommandDataSet left, CommandDataSet right) => left?.CompareTo(right) > 0;
  /// <summary>TODO: Doc</summary>
  /// <param name="left">The left object.</param>
  /// <param name="right">The right object.</param>
  public static bool operator >= (CommandDataSet left, CommandDataSet right) => left?.CompareTo(right) >= 0;
  #endregion
}
