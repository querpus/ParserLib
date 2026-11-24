using Parser.Condition;

namespace Parser.Ops;

public class OperationIf : Operation
{
  public Collection<IfBlockConditional> Options { get; init; } = [];
}
