using System.Xml.Linq;

using static Parser.OpStatus;

namespace Parser;

/// <summary>A parser the takes a file's content and turns it into tokens or objects.</summary>
public sealed class XParser
{
  private record struct State (int Index, int NextIndex, OperationStrategy Strategy, ReadOnlyCollection<IOperation> ParsedOperations, Collection<Action<XParser>> Actions, ReadOnlyDictionary<string, int> Labels)
  {
    public static State Snapshot (XParser parser) => new()
    {
      Index = parser.OpIndex,
      NextIndex = parser.NextOpIndex,
      Strategy = parser.CurrentStrategy,
      ParsedOperations = new(parser.ParsedOperations),
      Actions = [],
      Labels = new(parser.Labels)
    };
    public static readonly State Empty = new()
    {
      Index = 0,
      NextIndex = 0,
      Strategy = [],
      ParsedOperations = [],
      Actions = [],
      Labels = ReadOnlyDictionary<string, int>.Empty
    };
    public static State Initialize (OperationStrategy strategy)
    {
      (var parsedOperations, var actions) = OperationLoad(strategy);
      return new()
      {
        Index = 0,
        NextIndex = 0,
        Strategy = strategy,
        ParsedOperations = [.. parsedOperations],
        Actions = actions,
        Labels = ReadOnlyDictionary<string, int>.Empty
      };
    }
  }

  private void ApplyState (State state)
  {
    OpIndex = state.Index;
    NextOpIndex = state.NextIndex;
    CurrentStrategy = state.Strategy;
    ParsedOperations = [.. (state.ParsedOperations)];
    Labels = [.. state.Labels];

    state.Actions.Foreach(a => a(this));
  }
  private static Library LibRef => Library.Instance;
  #region Public Properties
  /// <summary>The current operation index.</summary>
  public int OpIndex { get; private set; }
  /// <summary>The next operation index.</summary>
  public int NextOpIndex { get; private set; }
  /// <summary>The current operation.</summary>
  public IOperation CurrentOp => ParsedOperations[OpIndex];
  /// <summary>The next operation.</summary>
  public IOperation NextOp => (NextOpIndex == -1 || NextOpIndex >= OpCount) ? new OperationEnd() : ParsedOperations[NextOpIndex];
  /// <summary>Gets the file data as a list of bytes.</summary>
  public Spec LocalDefaultSpec =>
    Data?.HasData != true ? LibRef["unknown"] :
    Data.CanLoad<string>("initial") ? LibRef["textbylines"] :
    Data.CanLoad<Memory<byte>>("initial") ? LibRef["binary"] :
    Data.CanLoad<XElement>("initial") ? LibRef["xelement"] :
    LibRef["unknown"];
  /// <summary>The flattened and parsed operation strategy.</summary>
  public Collection<IOperation> ParsedOperations { get; private set; } = [];
  public Spec? Spec { get; private set; }
  /// <summary>A sequence of <see cref="IOperation"/> objects that process data.</summary>
  public OperationStrategy CurrentStrategy { get; private set; } = [];
  public OpStatus EnterStrategy (OperationStrategy strategy)
  {
    var parent = State.Snapshot(this);
    OpStatus status = AtStart;
    var this_op = CurrentOp;
    try
    {
      var enter_state = State.Initialize(strategy);
      ApplyState(enter_state);

      status = ParseLoop();

      if (status.IsFail)
      {
        Log(MsgClass.Error, "EnterStrategy resulted in a fail.", this);
        throw new OperationException("EnterStrategy resulted in a fail.");
      }
    }
    catch (OperationException e) when (status.IsFail)
    {
      Log(MsgClass.Error, e.Message, this);
    }
    catch (OperationException e)
    {
      Log(MsgClass.Error, e.Message, this);
      throw new OperationException("An exception occured without a failure code.", e);
    }
    finally
    {
      ApplyState(parent);
    }
    return this_op.ContinueOnFail ? FailOverride : status;
  }
  /// <summary>Loads the operations into a flat pattern.</summary>
  private static (Collection<IOperation>, Collection<Action<XParser>>) OperationLoad (OperationStrategy strategy)
  {
    Collection<IOperation> result = [.. strategy.Operations];
    Collection<Action<XParser>> actions = [];

    if (result.IsEmpty || result[^1] is not OperationEnd)
    {
      result.Add(new OperationEnd());
    }
    else // Unpack all operations in main list
    {
      for (int i = 0; i < result.Count; i++)
      {
        IOperation op = result[i];
        int oldCount = result.Count;
        if (op is IPlaceholderOperation ipo)
        {
          int newCount = ipo.Unpack(result, i, out Action<XParser>? action);
          if (action is not null)
          {
            actions.Add(action);
          }
          if (newCount != oldCount)
            Log(MsgClass.Debug, $"Operation Group Expanded From {oldCount} to {newCount}.", nameof(XParser));
        }
      }
    }
    return (result, actions);
  }
  public Dictionary<string, int> Labels { get; private set; } = [];
  /// <summary>The dictionary storing all of the data from the parsed file.</summary>
  [NotNull] public DataStore? Data { get; private set; }
  /// <summary>Gets the result of a successful parse operation set.</summary>
  /// <remarks>This property returns <see langword="null"/> if the operation sequence has failed, or has not ran.</remarks>
  public object? Result => Data.CanLoad("result") ? Data["result"] : null;
  /// <summary>The number of operations in the currently loaded specification, after parsing the operation sequence.</summary>
  public int OpCount => ParsedOperations.Count;
  #endregion
  #region Constructor
  public XParser () { }
  public XParser (Spec spec) => InitializeParser(spec);
  public XParser (string file)
  {
    string? name = LibRef.CheckFile(file);
    Spec spec = LibRef.LookupOrDefault(name);
    InitializeParser(spec);
  }
  #endregion
  #region Private Methods
  private void InitializeData<T> (T data)
  {
    Spec.ThrowIfNull();
    if (Spec.Name.Like("unknown"))
    {
      Spec = data switch
      {
        string => LibRef["textbylines"],
        IEnumerable<byte> => LibRef["binary"],
        XElement => LibRef["xelement"],
        _ => throw new InvalidDataException("Data must by a byte array, a string, or ")
      };
    }
    data.ThrowIfNull();
    Data.Initialize(data);
  }
  /// <summary>Sets up the Specification and DataStore for the parser.</summary>
  /// <param name="spec">The specification to use.</param>
  [MemberNotNull(nameof(Data), nameof(Spec))]
  private void InitializeParser (Spec spec)
  {
    Data = new() { Parser = this };
    Spec = spec;

    ApplyState(State.Initialize(spec.Operations));
  }
  private void AdvanceOperation ()
  {
    if (NextOpIndex == -1 || NextOpIndex >= OpCount)
    {
      NextOpIndex = -1;
      return;
    }
    OpIndex = NextOpIndex;
    NextOpIndex++;
  }
  /// <summary>Performs all the operations, ending on a fail or a completion of the sequence.</summary>
  /// <returns>The <see cref="OpStatus"/> representing the result.</returns>
  private OpStatus ParseLoop ()
  {
    OpStatus status = AtStart;
    while (NextOpIndex >= 0 && !status.IsFail)
    {
      status = PerformOperation();
    }
    return status;
  }
  /// <summary>Performs the operation indicated by <see cref="OpIndex"/> and advances to the next operation.</summary>
  /// <returns>The <see cref="OpStatus"/> representing the result.</returns>
  private OpStatus PerformOperation ()
  {
    if (CurrentOp.SkipOperation)
    {
      Log(MsgClass.Warning, "Skip Operation Encountered", this);
      AdvanceOperation();
      return Skipped;
    }

    Log(MsgClass.Debug, $"Performing Operation {CurrentOp.TypeName}.", this);

    OpStatus result;
    void setExceptionData (OpStatus status, Exception toLog)
    {
      result = status;
      Log(MsgClass.Error, toLog.Message, this);
    }

    try { result = CurrentOp.DoOperation(this); }
    catch (OperationBadInputTypeException obit) { setExceptionData(FailBadInputType, obit); }
    catch (OperationBadDefinitionException obd) { setExceptionData(FailBadOpDefinition, obd); }
    catch (OperationBadResultException obr) { setExceptionData(FailBadOpResult, obr); }
    catch (OperationNoSuchVarException onsv) { setExceptionData(FailNoSuchVarName, onsv); }
    catch (UnknownOperationException uoe) { setExceptionData(FailNoSpec, uoe); }
    catch (OperationException o) { setExceptionData(Fail, o); }
    catch (Exception e) { setExceptionData(Error, e); }
    Log(MsgClass.Debug, $"Operation resulted in {result}.", this);

    if (result.IsFail && CurrentOp.ContinueOnFail)
      result = FailOverride;

    if (result is EndCommand || result.IsFail)
      NextOpIndex = -1;

    AdvanceOperation();
    return result;
  }
  #endregion
  /// <summary>Initializes the data and begins parsing.</summary>
  /// <param name="spec">The specification to use.</param>
  /// <param name="data">The data to pass to the parser.</param>
  /// <typeparam name="TData">The type of data to pass to the parser.</typeparam>
  /// <returns>The <see cref="OpStatus"/> representing the result.</returns>
  public OpStatus ParseData<TData> (Spec spec, TData data)
  {
    InitializeParser(spec);
    InitializeData(data);
    return ParseLoop();
  }
  /// <summary>Parses the specified file based on the spec assigned to this <see cref="XParser"/> object.</summary>
  /// <param name="spec">The spec to use.</param>
  /// <param name="path">The path to the file to parse.</param>
  /// <returns>The <see cref="OpStatus"/> representing the result.</returns>
  public static OpStatus ParseFile (Spec spec, string path)
  {
    XParser parser = new(spec);

    parser.Data["file_path"] = path;
    if (spec.IsTextFile)
    {
      string text = File.ReadAllText(path);
      parser.InitializeData(text);
      return parser.ParseLoop();
    }
    else
    {
      Memory<byte> contents = File.ReadAllBytes(path);
      parser.InitializeData(contents);
      return parser.ParseLoop();
    }
  }
  /// <summary>Sets the next operation to be the one at <paramref name="index"/>.</summary>
  /// <param name="index">The index of the next operation to execute.</param>
  public void SetNextOperationIndex (int index) => NextOpIndex = index;
  /// <summary>Assigns the input to the parser.</summary>
  /// <typeparam name="TData">The type of input. Can be a <see langword="string"/> or a <see langword="byte"/> array.</typeparam>
  /// <param name="input">The input to assign.</param>
  /// <remarks>A byte array could be a <see cref="Memory{T}"/> object, a <see cref="Span{T}"/>, a <see cref="List{T}"/>, or a simple <see cref="IEnumerable{T}"/></remarks>
  public void SetInput<TData> (TData input) => InitializeData(input);
  /// <summary>Incrementally steps through all the operations, requesting user confirmation to continue.</summary>
  /// <returns>The <see cref="OpStatus"/> codes representing the result of each operation.</returns>
  /// <exception cref="OperationBadDefinitionException">Called StepThrough without assigning or initializing input data.</exception>
  public IEnumerable<OpStatus> StepThrough ()
  {
    if (Data?.HasData != true)
      Err.ThrowBadDef("No input data to step through.");

    while (NextOpIndex != -1)
      yield return PerformOperation();
  }

  public void QueryOperation ()
  {
    string userInput;

    void promptUser () => userInput = Console.ReadLine() ?? SE;
    void checkAction (string input, dynamic data, Action<dynamic> actionOnData)
    {
      if (userInput.Like(input))
      {
        actionOnData(data);
      }
    }
    void checkPrompt (string input, string askmsg, Action<string> actionOnInput)
    {
      if (userInput.Like(input))
      {
        Log(MsgClass.Prompt, askmsg, this);
        promptUser();
        actionOnInput(userInput);
      }
    }

    string[] allow_continue = [SE, "next", "quit", "exit", "skip"];

    do
    {
      Log(MsgClass.Prompt, "Enter a command to analyze parser state.", this);
      promptUser();

      checkPrompt("quit", "are you sure? (y/n)", user => _ = user.Like("y") ? throw new QuitException() : "no quit");
      checkAction("data", Data.ToString(), data => Log(MsgClass.Debug, data, this));
      checkPrompt("print", "Enter the key to display.", obj =>
      {
        if (Data.TryLoad(obj, out object? data) && data is IPrintable ip) ip.Print();
      });
      checkAction("print all", Data, data =>
      {
        foreach (object? item in data)
        {
          if (item is IPrintable pr)
            pr.Print();
          else
            Log(MsgClass.GreenInfo, item.ToString2(), this);
        }
      });
      checkAction("show next", $"Next Operation: {NextOpIndex} : {NextOp}", data => Log(MsgClass.Debug, data, this));
      checkPrompt("data in", "Enter the key to display.", _ => Log(MsgClass.Debug, $"[{userInput}] = {(Data.TryLoad(userInput, out object? data) ? data : "<Load Failure>")}", this));
    } while (!userInput.EqualsAny(SCOIC, [.. allow_continue]) || userInput.IsNotEmpty);
  }
}
