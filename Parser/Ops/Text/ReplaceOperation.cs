//using Parser.Text.Tokens;

namespace Parser.Ops.Text;

/// <summary>
/// Takes an input as a <see langword="string"/> or a collection of strings,
/// and replaces matches with the replacement values. This operation
/// can supply multiple replacement nodes.
/// </summary>
/// <remarks><code>
/// Inputs: <see langword="string"/>, <see cref="IEnumerable{T}"/>(<see langword="string"/>)<br/>
/// Output: <c>Same as input</c>
/// </code><br/>
/// Statuses:<br/>
/// <see cref="OpStatus.Pass"/>: Operation completed successfully.<br/>
/// <see cref="OpStatus.FailBadInputType"/>: Operation was provided the wrong type as input.<br/>
/// <see cref="OpStatus.FailNoSuchVarName"/>: The data at the key was missing.<br/>
/// </remarks>
public class ReplaceOperation : Operation
{
  public Collection<ReplaceNode> Nodes { get; init; } = [(RX.LnEnd, Chars.LFs)];
  public required string InputKey { get; init; }
  public required string OutputKey { get; init; }

  protected override void Execute ()
  {
    if (Data[InputKey] is string s)
    {
      foreach (ReplaceNode node in Nodes)
      {
        s = node.ReplaceRegex(s, Spec.RxOpt);
      }
      Data[OutputKey] = s;
      Status = OpStatus.Pass;
    }
    else if (Data[InputKey] is IEnumerable<string> list)
    {
      foreach (ReplaceNode node in Nodes)
      {
        list = [.. list.Select(line => node.ReplaceRegex(line, Spec.RxOpt))];
      }
      Data[OutputKey] = list;
      Status = OpStatus.Pass;
    }
    else
    {
      throw Err.ThrowBadInput("string or IEnumerable<string>", Data[InputKey].TypeName);
    }
  }
}
