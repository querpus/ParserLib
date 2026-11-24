namespace Parser.Ops;

/// <summary>Prompts for input, and stores the input.
/// Optionally validates the input as well.</summary>
public sealed class PromptOperation : Operation
{
  public required string Message { get; init; }
  public string? UserKey { get; init; }
  public Predicate<string>? Validation { get; init; }

  /// <summary>Executes the prompt operation, consisting of displaying the prompt message and reading user input.
  /// This can also optionally validate this input. If no validator is provided, the input will be accepted as valid.</summary>
  /// <exception cref="OperationBadResultException">Thrown when the validation fails.</exception>
  protected override void Execute ()
  {
    Log(MsgClass.Prompt, Message, this);
    string userInput = Console.ReadLine()!;

    if (Validation?.Invoke(userInput) is null or true)
    {
      Status = OpStatus.Pass;
      Data[UserKey] = userInput;
      return;
    }

    throw Err.ThrowBadResult("Validation Operation Failed.");
  }
}
