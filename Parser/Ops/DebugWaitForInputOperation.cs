namespace Parser.Ops;

/// <include file="operation.xml" path="/doc/members/member[@name=&quot;T:Parser.Ops.Operation&quot;]"></include>
/// <summary>Constructs an operation that waits for a specific key press or the enter key.</summary>
/// <param name="key">The key to wait for.</param>
public class DebugWaitForInputOperation (ConsoleKey key = ConsoleKey.Enter) : Operation()
{

  protected override void Execute ()
  {
    ConsoleKeyInfo keyInfo;
    Log(MsgClass.Prompt, $"Press the {key} key to continue.", this);
    do keyInfo = Console.ReadKey(true);
    while (keyInfo.Key != key);
  }
}
