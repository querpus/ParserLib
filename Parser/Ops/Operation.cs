namespace Parser.Ops;

public class SampleOperation : Operation
{
  protected string NewInputKey { get; init; }
  protected string ModInputKey { get; init; }
  protected string NewOutputKey { get; init; }
  protected string ModOutputKey { get; init; }
  public SampleOperation (string input_key, string input_key2, string output_key, string output_key2)
  {
    NewInputKey = input_key;
    ModInputKey = input_key2;
    NewOutputKey = output_key;
    ModOutputKey = output_key2;
  }
  protected override void Execute ()
  {
    Data[NewOutputKey] = NewInputKey;
    Data[ModOutputKey] = ModInputKey;
  }
}

/// <summary>
/// The abstract base class for operations. All operations should inherit from <see cref="Operation"/>.<br/>
/// All usage or references should be stored as <see cref="IOperation"/> objects.
/// </summary>
public abstract class Operation : IOperation
{
  private const string Area = nameof(Operation);
  #region Throwing Functions
  /// <summary>Throws an <see cref="OperationException"/> if condition is <see langword="true"/>.</summary>
  /// <param name="condition">The condition to check.</param>
  /// <param name="msg">The exception message.</param>
  /// <exception cref="OperationException"></exception>
  protected static void ThrowIf ([DoesNotReturnIf(true)] bool condition, string msg)
  {
    if (condition) throw new OperationException(msg);
  }
  #endregion
  #region Stored Keys & Data
  /// <summary>The status of the operation.</summary>
  protected OpStatus Status { get; set; } = OpStatus.AtStart;
  #endregion
  #region Operation Flags
  public bool ContinueOnFail { get; set; }
  public bool SkipOperation { get; set; }
  public virtual bool NoExecution { get; init; }
  #endregion
  #region Reference Properties
  /// <summary>The reference to the parser.</summary>
  [AllowNull] protected XParser Parser { get; private set; }
  /// <summary>The reference to the <see cref="DataStore"/>.</summary>
  [AllowNull] protected DataStore Data => Parser.Data;
  /// <summary>The reference to the specification.</summary>
  [AllowNull]
  protected Spec Spec
  {
    get
    {
      if (Parser.Spec is null)
        _ = Err.ThrowNoSpec("Spec is null and Operation executed.");
      return Parser.Spec;
    }
  }
  #endregion
  #region Constructors
  /// <summary>Constructor for an operation object.</summary>
  protected Operation () { }
  #endregion
  /// <summary>This method is the entry point for all operations.</summary>
  /// <param name="parser_ref">The reference to the parser executing this operation.</param>
  /// <returns>An <see cref="OpStatus"/> reflecting the result of the operation. Failures will be thrown,
  /// and then caught by the <see cref="XParser"/>.</returns>
  public OpStatus DoOperation ([NotNull] XParser parser_ref)
  {
    DebugIn(Area, "DoOperation");

    if (SkipOperation)
      return OpStatus.Skipped;

    if (this is IPlaceholderOperation ipo)
      ipo.CheckUnpacked();

    Parser = parser_ref;

    if (!NoExecution)
    {
      DebugIn(this.TypeName, nameof(Execute));
      Execute();
      DebugOut();

      if (Status is OpStatus.AtStart)
        Log(MsgClass.Warning, $"Status was not defined in operation {this.TypeName}.", this);
    }

    DebugOut();
    return Status;
  }
  /// <summary>
  /// Performs the operation, storing and loading data from <see cref="Data"/>.<br/>
  /// The <see cref="Status"/> property should be assigned a value,
  /// if the value is <see cref="OpStatus.AtStart"/> upon completion, a warning will display.
  /// </summary>
  /// <exception cref="OperationException"></exception>
  /// <exception cref="OperationBadDefinitionException">Method was not overridden, or NoExecute was not set.</exception>
  protected virtual void Execute () => throw Err.ThrowBadDef("Method not overridden, or NoExecute not set.");
  protected static IOperation JumpTo (int pos) => new JumpOperation { TargetIndex = pos };
  public void ApplyProperties (bool cont, bool skip)
  {
    ContinueOnFail = cont || ContinueOnFail;
    SkipOperation = skip || SkipOperation;
  }
  public override string ToString () => this.TypeName;
}
