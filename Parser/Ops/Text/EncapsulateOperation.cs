namespace Parser.Ops.Text;

public class EncapsulateOperation<TParent, TChild> : Operation where TParent : class, ICollection<TChild>, new()
{
  public required string InputKey { get; init; }
  public required string OutputKey { get; init; }
  protected override void Execute ()
  {
    IEnumerable<TChild> list = [];
    list = Data[InputKey] switch
    {
      IEnumerable<TChild> col => col,
      IDictionary<int, TChild> dic => dic.Select(item => item.Value),
      _ => Err.ThrowBadInput("IEnumerable<TChild> or IDictionary<int,TChild>", Data[InputKey].TypeName)
    };
    TParent parent = new();

    foreach (TChild item in list)
      parent.Add(item);
    Data[OutputKey] = parent;
    Status = OpStatus.Pass;
  }
}
