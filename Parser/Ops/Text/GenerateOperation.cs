using Parser.Condition;

using OS = Parser.OpStatus;

namespace Parser.Ops.Text;

/// <summary>
/// Takes an input as a collection of <see cref="MatchDataSet"/> objects,
/// and generates a <see cref="Dictionary{TKey, TValue}"/> of items from it,
/// provided that the specified group (or groups) are present.
/// </summary>
/// <typeparam name="TIn">The accepted input.</typeparam>
/// <typeparam name="TOut">The type of object to create.</typeparam>
/// <remarks><code>
/// Inputs: IDictionary&lt;int, MatchData>, IEnumerable&lt;MatchData>
/// Output: <see cref="Dictionary{TKey,TValue}">Dictionary&lt;int, TOutput></see></code>
/// <br/>
/// Statuses:
/// <code>
/// <see cref="OS.Pass"/>: Operation completed successfully.<br/>
/// <see cref="OS.Skipped"/>: Operation completed successfully, but no work was done.<br/>
/// <see cref="OS.FailOverride"/>: Operation failed, but is allowed to continue.<br/>
/// <see cref="OS.FailBadInputType"/>: Operation was provided the wrong type as input.<br/>
/// <see cref="OS.FailBadInputNull"/>: The data at the key was <see langword="null"/> or missing.<br/>
/// <see cref="OS.FailNoSuchVarName"/>: The key was not found in the <see cref="DataStore"/>.
/// </code>
/// </remarks>
public class GenerateOperation<TIn, TOut> : Operation where TOut : notnull
{
  protected Dictionary<int, TOut> Results { get; } = [];
  protected Predicate<TIn> Predicate { get; }
  protected Func<TIn, TOut> Function { get; }
  public required string InputKey { get; init; }
  public required string OutputKey { get; init; }
  /// <summary>
  /// Generates a <typeparamref name="TOut"/> from a <typeparamref name="TIn"/>.
  /// Validates the <typeparamref name="TOut"/> against the <paramref name="predicate"/> and stores the result.
  /// </summary>
  /// <param name="func">The generation function.</param>
  /// <param name="predicate">The condition that the generation function requires.</param>
  public GenerateOperation (Func<TIn, TOut> func, Predicate<TIn> predicate)
  {
    Predicate = predicate;
    Function = func;
  }
  public GenerateOperation (Func<TIn, TOut> func)
  {
    Predicate = _ => true;
    Function = func;
  }

  protected override void Execute ()
  {
    if (Data[InputKey] is IEnumerable<TIn> mdds)
    {
      Collection<TIn> mddList = [.. mdds];
      for (int i = 0; i < mddList.Count; i++)
      {
        TIn mdd = mddList[i];

        if (Predicate(mdd))
        {
          TOut? result = Function(mdd);
          Results.Add(i, result);
        }
      }
      Data[OutputKey] = Results;
      Status = OS.Pass;
    }
    else if (Data[InputKey] is TIn mdd)
    {
      if (Predicate(mdd))
      {
        Data[OutputKey] = Function(mdd);
        Status = OS.Pass;
      }
      else
      {
        Status = OS.Skipped;
      }
    }
    else
    {
      throw Err.ThrowBadInput($"{typeof(TIn)} or {typeof(IEnumerable<TIn>)}", Data[InputKey].TypeName);
    }
  }
}

/// <summary>Assembles an array of strings into objects.</summary>
public class ConstructOperation : Operation
{
  public required string TargetType { get; init; }
  public required string InputKey { get; init; }
  public required string OutputKey { get; init; }
  public ParsedExpression? Condition { get; init; }

  protected override void Execute ()
  {
    object items = Data[InputKey];

    if (items is not IEnumerable<string> list)
    {
      throw Err.ThrowBadInput("IEnumerable<string>", items.TypeName);
    }

    Collection<object> result = [];

    Type? typ = Type.GetType(TargetType, false) ?? throw Err.ThrowBadDef($"Target type of {TargetType} does not exist.");
    ConstructorInfo? cons = typ.GetConstructor([typeof(string)]) ?? throw Err.ThrowBadDef($"No constructor for given type {TargetType}");
    foreach (string item in list)
    {
      object? target = cons.Invoke(null, [item]);
      if (target is not null)
        result.Add(target);
      else
        throw Err.ThrowBadResult($"Constructor for {TargetType} failed to produce an object.");
    }

    Data[OutputKey] = result;
  }
}
