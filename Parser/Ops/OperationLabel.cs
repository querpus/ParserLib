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

  public int Unpack ([NotNull] Collection<IOperation> operations, int index, XParser parser_ref)
  {
    parser_ref.Labels.Add(Name, index);
    return operations.Count;
  }
}
