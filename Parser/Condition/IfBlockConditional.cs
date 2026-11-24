namespace Parser.Condition;

public class IfBlockConditional
{
  /// <summary>The if condition string. This is <see langword="null"/> for the <see langword="else"/> block.</summary>
  public string? Condition { get; init; }
  public Collection<IOperation> Operations { get; init; } = [];
}
