#pragma warning disable CA1822 // Mark members as static

namespace Parser;

public sealed class OperationStrategy : IOperation, IList<IOperation>
{
  public Collection<IOperation> Operations { get; init; } = [];
  [NotNull]
  public XParser? Parser { get; private set; }
  public OpStatus Status { get; private set; }
  public OpStatus DoOperation ([NotNull] XParser parser_ref)
  {
    if (SkipOperation || Count == 0)
      return OpStatus.Skipped;

    Parser = parser_ref;

    Status = parser_ref.EnterStrategy(this);

    return Status;
  }
  public bool ContinueOnFail { get; set; }
  public bool SkipOperation { get; set; }

  bool IOperation.NoExecution => false;

  public int Count => Operations.Count;

  bool ICollection<IOperation>.IsReadOnly => false;

  IOperation IList<IOperation>.this[int index] { get => Operations[index]; set => Operations[index] = value; }

  public void ApplyProperties (bool cont, bool skip)
  {
    ContinueOnFail = cont;
    SkipOperation = skip;
  }

  int IList<IOperation>.IndexOf (IOperation item) => Operations.IndexOf(item);
  void IList<IOperation>.Insert (int index, IOperation item) => Operations.Insert(index, item);
  void IList<IOperation>.RemoveAt (int index) => Operations.RemoveAt(index);
  void ICollection<IOperation>.Add (IOperation item) => Operations.Add(item);
  void ICollection<IOperation>.Clear () => Operations.Clear();
  bool ICollection<IOperation>.Contains (IOperation item) => Operations.Contains(item);
  void ICollection<IOperation>.CopyTo (IOperation[] array, int arrayIndex) => Operations.CopyTo(array, arrayIndex);
  bool ICollection<IOperation>.Remove (IOperation item) => Operations.Remove(item);
  IEnumerator<IOperation> IEnumerable<IOperation>.GetEnumerator () => Operations.GetEnumerator();
  IEnumerator IEnumerable.GetEnumerator () => Operations.GetEnumerator();
}
