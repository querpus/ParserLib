//using Parser.Text.Tokens;

namespace Parser.Ops.Text;
/// <summary>
/// Takes an input as a <typeparamref name="TIn"/> object,
/// and generates a <typeparamref name="TOut"/> from it,
/// provided that the validation function passes.
/// </summary>
/// <typeparam name="TIn">The type of object to supply as input.</typeparam>
/// <typeparam name="TOut">The type of object written to the output key.</typeparam>
/// <param name="operation">The function to perform to change from a <typeparamref name="TIn"/> to a <typeparamref name="TOut"/>.</param>
/// <param name="validation">The function that determines whether or not the conversion was a success.</param>
/// <remarks><code>
/// Inputs: <typeparamref name="TIn"/><br/>
/// Output: <typeparamref name="TOut"/>
/// </code>
/// </remarks>
/// <exception cref="OperationBadResultException"/>
/// <exception cref="OperationNoSuchVarException"/>
/// <exception cref="OperationBadInputTypeException"/>
public class ExternalOperation<TIn, TOut> (Func<TIn, TOut> operation, Func<TOut, bool> validation) : Operation where TIn : class where TOut : class
{
  private readonly Func<TIn, TOut> _operation = operation;
  private readonly Func<TOut, bool> _validation = validation;
  public required string InputKey { get; init; }
  public required string OutputKey { get; init; }

  protected override void Execute ()
  {
    if (Data[InputKey] is TIn casted)
    {
      TOut result = _operation(casted);
      Status = _validation(result) ? OpStatus.Pass : Err.ThrowBadResult("Validation Failed");
      Data[OutputKey] = result;
    }
    else
    {
      throw Err.ThrowBadInput($"{typeof(TIn)}", Data[InputKey].TypeName);
    }
  }
}
