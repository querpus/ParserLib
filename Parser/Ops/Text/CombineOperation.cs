//using Parser.Text.Tokens;

namespace Parser.Ops.Text;

/// <summary><include file='..\operation.xml' path='doc/members/member[@name="M:Parser.Ops.Operation"]/*'/></summary>
public class CombineOperation : Operation
{
  /// <summary>The <see langword="char"/> or <see langword="string"/> to bridge the combined values of the data at <see cref="InputKey"/>.</summary>
  public object? Delimeter { get; init; }
  /// <summary>The input key.</summary>
  public required string InputKey { get; init; }
  /// <summary>The output key.</summary>
  public required string OutputKey { get; init; }
  protected override void Execute ()
  {
    if (Data[InputKey] is IEnumerable<string> list)
    {
      Status = OpStatus.Pass;
      Data[OutputKey] = Delimeter switch
      {
        null => list.TextJoin(),
        string s => list.TextJoin(s),
        char c => list.TextJoin(new(c, 1)),
        _ => list.TextJoin($"{Delimeter}"),
      };
    }
    else
    {
      Status = Data[InputKey] is string ? OpStatus.Skipped : Err.ThrowBadInput("string or IEnumerable<string>", Data[InputKey].TypeName);
    }
  }
}
