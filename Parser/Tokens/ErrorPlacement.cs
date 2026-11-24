#pragma warning disable CA1710 // Identifiers should have correct suffix

using MC = Common.MsgClass;

namespace Parser.Tokens;

public class ErrorPlacement
{
  public required Match Error { get; init; }
  public required string Text { get; init; }

  private MC _mcHead;
  private MC _mcPrev;
  private MC _mcOuter;
  private MC _mcInner;
  [AllowNull]
  private string _prevLine;
  [AllowNull]
  private string _errorLine;
  private int _errLineNo;
  private int _startOuter;
  private int _pointCol;
  private int _startInner;
  private int _endInner;
  private int _endOuter;
  public void WriteError ()
  {
    _mcHead = MC.Debug;
    _mcPrev = MC.Warning;
    _mcOuter = MC.Error;
    _mcInner = MC.Critical;

    Group? inner = Error.Groups.ContainsKey("error_pos") ? Error.Groups["error_pos"] : null;
    Group? outer = Error.Groups.ContainsKey("error_surround") ? Error.Groups["error_surround"] : null;

    (_errLineNo, _startInner) = Text.Get2DPosition(inner is not null ? inner.Index : Error.Index);

    string[] lines = Text.Split('\n');

    _errorLine = lines[_errLineNo];

    if (_errLineNo > 0)
      _prevLine = lines[_errLineNo - 1];

    if (outer is not null)
    {
      (_, _startOuter) = Text.Get2DPosition(outer.Index);
      _endOuter = _startOuter + outer.Length - 1;
    }
    else
    {
      _endOuter = _errorLine.Length - 1;
    }

    _endInner = _startInner + (inner is not null ? inner.Length : Error.Length) - 1;

    _pointCol = (_startInner + _endInner) / 2;

    WritePrevLine();
    WriteErrorLine();
    WritePointLine();
  }
  public void WritePrevLine ()
  {
    LogHead(_mcHead);
    LogPart(_mcPrev, "  ");
    LogPart(_mcPrev, _prevLine);
    NewLine();
  }
  public void WriteErrorLine ()
  {
    LogHead(_mcHead);
    LogPart(_mcPrev, "> ");
    if (_startOuter == 0)
    {
      LogPart(_mcOuter, _errorLine[0.._startInner]);
    }
    else
    {
      LogPart(_mcPrev, _errorLine[0.._startOuter]);
      LogPart(_mcOuter, _errorLine[_startOuter.._startInner]);
    }
    LogPart(_mcInner, _errorLine[_startInner.._endInner]);
    if (_endOuter == _errorLine.Length - 1)
    {
      LogPart(_mcOuter, _errorLine[_endInner..]);
    }
    else
    {
      LogPart(_mcOuter, _errorLine[_endInner.._endOuter]);
      LogPart(_mcPrev, _errorLine[_endOuter..]);
    }
    NewLine();
  }
  public void WritePointLine ()
  {
    LogHead(_mcHead);
    LogPart(_mcPrev, "  ");
    LogPart(_mcPrev, new string(' ', _pointCol - 1));
    LogPart(_mcPrev, "^");
    NewLine();
  }
}
