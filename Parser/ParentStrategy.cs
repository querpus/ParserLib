namespace Parser;

public struct ParentStrategy
{
  public int Index { get; set; }
  public OperationStrategy Strategy { get; set; }
  public ReadOnlyCollection<IOperation> ParsedOperations { get; set; }
  public ReadOnlyDictionary<string, int> Labels { get; set; }
}
