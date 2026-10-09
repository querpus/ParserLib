#pragma warning disable CA1710 // Identifiers should have correct suffix
#pragma warning disable format // Formatting

using Common.Entities;

namespace Common.NodeTree;
public class NodeContext
{
  public required bool SingleRoot { get; init; }
  public string? PropKey { get; set; }
  /// <summary>Gets the stack of nodes used to track the current node hierarchy during traversal or processing.</summary>
  /// <remarks>Init-only and initialized to an empty stack.</remarks>
  public Stack<INode> NodeStack { get; init; } = [];
  /// <summary>The currently selected node. (Child Node)</summary>
  public INode? Current { get; set; }
  public Collection<INode> RootNodes { get; } = [];
  /// <summary>The root node.</summary>
  public INode? Root { get; set; }
  /// <summary>Document Header.</summary>
  public INode? Header { get; set; }
  public INode? Parent => HasParent ? NodeStack.Peek() : null;

  /// <summary>Pushes a node to the top of the node stack, and adds it as a child of the previous node on the stack.</summary>
  /// <param name="node">The child node to push.</param>
  /// <remarks>This assigns the root node if the stack is empty.</remarks>
  public void PushAndAddChild (INode node)
  {
    if (IsRoot && SingleRoot)
    {
      Root = node;
    }
    else if (IsRoot)
    {
      RootNodes.Add(node);
    }
    else
    {
      Parent.AddChild(node);
    }
    NodeStack.Push(node);
  }

  public void PushAndAddChild ()
  {
    if (HasCurrent)
      PushAndAddChild(Current);
    else
      ParsingException.ThrowNullData("Current");
  }

  [MemberNotNullWhen(false, nameof(Parent))]
  public bool IsRoot => NodeStack.Count == 0;
  [MemberNotNullWhen(true, nameof(Parent))]
  public bool HasParent => NodeStack.Count > 0;
  [MemberNotNullWhen(true, nameof(Current))]
  public bool HasCurrent => Current is not null;
}
