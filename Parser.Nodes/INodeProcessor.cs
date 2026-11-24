#pragma warning disable CA1710 // Identifiers should have correct suffix
#pragma warning disable format // Formatting

namespace Parser.Nodes;
public interface INodeProcessor
{
  dynamic? Input { get; init; }
  /// <summary></summary>
  NodeContext? Context { get; }
  /// <summary>Processes and creates the <see cref="Context"/>.</summary>
  [MemberNotNull(nameof(Context))]
  void Process ();
  static NodeContext Process<TProcessor> (dynamic input) where TProcessor : INodeProcessor, new()
  {
    TProcessor engine = new() { Input = input };
    engine.Process();
    return engine.Context;
  }
}
