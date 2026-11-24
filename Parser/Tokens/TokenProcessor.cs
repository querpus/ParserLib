#pragma warning disable RCS1079 // Throwing of new NotImplementedException

namespace Parser.Tokens;

public class TokenProcessor<TIn, TOut>
{
  protected Collection<TIn> Input { get; }
  protected Collection<TOut> Output { get; }
  protected Func<IEnumerable<TIn>, Collection<TOut>> Process { get; init; }

  [SetsRequiredMembers]
  protected TokenProcessor (IEnumerable<TIn> input, Func<IEnumerable<TIn>, Collection<TOut>> process)
  {
    Input = [.. input];
    Output = [];
    Process = process;
  }
}
