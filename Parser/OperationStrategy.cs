#pragma warning disable CA1822 // Mark members as static

namespace Parser;

/// <summary>An enhanced <see cref="IList{T}"/> that contains <see cref="IOperation"/> objects.</summary>
public sealed class OperationStrategy : IOperation, IList<IOperation>
{
  /// <summary>Gets the collection of operations.</summary>
  public Collection<IOperation> Operations { get; init; } = [];
  /// <summary>A reference to the parser.</summary>
  [NotNull]
  public XParser? Parser { get; private set; }
  /// <summary>The status that this operation will return when DoOperation completes.</summary>
  public OpStatus Status { get; private set; }
  /// <summary>Calls upon the <see cref="XParser"/> to save the current strategy state and enter a new strategy (This one).</summary>
  /// <param name="parser_ref">The currently running <see cref="XParser"/>.</param>
  /// <inheritdoc/>
  public OpStatus DoOperation ([NotNull] XParser parser_ref)
  {
    if (SkipOperation || Count == 0)
      return OpStatus.Skipped;

    Parser = parser_ref;

    Status = parser_ref.EnterStrategy(this);

    return Status;
  }

  #region IOperation Interface Implementation
  public bool ContinueOnFail { get; set; }
  public bool SkipOperation { get; set; }

  bool IOperation.NoExecution => false;

  public void ApplyProperties (bool cont, bool skip)
  {
    ContinueOnFail = cont;
    SkipOperation = skip;
  }
  #endregion
  #region IList<IOperation> Interface Implementation
  public int Count => Operations.Count;
  bool ICollection<IOperation>.IsReadOnly => false;
  public IOperation this[int index] { get => Operations[index]; set => Operations[index] = value; }
  int IList<IOperation>.IndexOf (IOperation item) => Operations.IndexOf(item);
  void IList<IOperation>.Insert (int index, IOperation item) => Operations.Insert(index, item);
  void IList<IOperation>.RemoveAt (int index) => Operations.RemoveAt(index);
  public void Add (IOperation item) => Operations.Add(item);
  public void Clear () => Operations.Clear();
  bool ICollection<IOperation>.Contains (IOperation item) => Operations.Contains(item);
  void ICollection<IOperation>.CopyTo (IOperation[] array, int arrayIndex) => Operations.CopyTo(array, arrayIndex);
  bool ICollection<IOperation>.Remove (IOperation item) => Operations.Remove(item);
  public IEnumerator<IOperation> GetEnumerator () => Operations.GetEnumerator();
  IEnumerator IEnumerable.GetEnumerator () => Operations.GetEnumerator();
  #endregion
}
