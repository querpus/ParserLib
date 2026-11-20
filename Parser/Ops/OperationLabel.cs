namespace Parser.Ops;

/// <summary>A label that can be referenced from a jump statement.</summary>
public sealed class OperationLabel : Operation, IPlaceholderOperation
{
  /// <summary>The name of this label.</summary>
  public required string Name { get; init; }
  public override bool NoExecution => true;

  public void CheckUnpacked ()
  {
    if (string.IsNullOrEmpty(Name.Trim()))
    {
      Err.ThrowUnpacked("Name is null or empty.");
    }
  }

  public int Unpack ([NotNull] Collection<IOperation> operations, int index, Dictionary<string, int> label_map)
  {
    label_map.Add(Name, index);
    return operations.Count;
  }
}
