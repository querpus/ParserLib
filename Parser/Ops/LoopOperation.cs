namespace Parser.Ops;

public abstract class LoopOperation : Operation, IPlaceholderOperation
{

  /// <summary>The key name to store this loop operation's <see cref="CursorData"/>.</summary>
  public required string CursorKey { get; init; }
  /// <summary>Operations to perform.</summary>
  public IEnumerable<IOperation> Operations { get; init; } = [];

  /// <summary>Start of loop section.</summary>
  /// <remarks>This will be the postion of the actual loop body in the operation list.</remarks>
  protected int LoopIndex { get; set; }
  /// <summary>The current index of the unpacker.</summary>
  /// <remarks>This will be the position of the <see cref="LoopOperation"/> object in the operation list.</remarks>
  protected int CurrentIndex { get; set; }
  protected abstract IOperation ContinueOp { get; }
  protected abstract IOperation StartLoop { get; }
  /// <summary>The inner operations with the breaks and continues configured.</summary>
  protected IEnumerable<IOperation> InnerOperations => Operations.Select(item => item switch
  {
    OperationBreak ob => new OperationBreak(CurrentIndex + 1, CursorKey),
    OperationContinue oc => ContinueOp,
    _ => item
  });

  public int Unpack ([NotNull] Collection<IOperation> operations, int index, XParser? parser_ref = null)
  {
    CurrentIndex = index;
    LoopIndex = operations.Count;

    operations.Add(StartLoop);
    operations.AddRange(InnerOperations);
    operations.Add(ContinueOp);

    return operations.Count;
  }

  public void CheckUnpacked ()
  {
    if (LoopIndex == 0 || CursorKey.IsEmpty)
    {
      Err.ThrowUnpacked("Loop Pre-processing not complete.");
    }
  }
}
