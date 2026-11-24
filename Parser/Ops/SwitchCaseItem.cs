using Parser.Condition;

namespace Parser.Ops;

public class SwitchCaseItem
{
  public bool IsDefaultCase => Expression is null;
  public int CasePosition { get; internal set; }
  public string? Value { get; init; }
  public ParsedExpression? Expression => Value is null ? null : new(Value);
  public Collection<IOperation> Operations { get; init; } = [];
}
