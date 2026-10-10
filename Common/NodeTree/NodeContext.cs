#pragma warning disable CA1710 // Identifiers should have correct suffix
#pragma warning disable format // Formatting

using Common.Entities;

using static Common.NodeTree.NodeTarget;

namespace Common.NodeTree;

public enum NodeTarget
{
  /// <summary>This node's parent.</summary>
  /// <remarks>Read only. Cannot write to this field.</remarks>
  Parent,
  Current,
  Header,
  Root,
  RootNodes,
  PropKey,
  /// <summary>The token group.</summary>
  /// <remarks>Read only. Cannot write to token.</remarks>
  TokenGroup,
  TokenValue,
}
public class NodeContext
{
  public dynamic? GetTargetValue (NodeTarget target, Token? token = null) => target switch
  {
    NodeTarget.Current => Current,
    NodeTarget.Parent => Parent,
    NodeTarget.Header => Header,
    NodeTarget.Root => Root,
    NodeTarget.RootNodes => RootNodes,
    NodeTarget.PropKey => PropKey,
    TokenGroup when token is not null => token.Value.Group,
    TokenValue when token is not null => token.Value.Value,
  };
  public void SetTargetValue (NodeTarget target, dynamic value, Token? token = null)
  {
    INode assignRootNodes (dynamic value)
    {
      RootNodes.Add(value);
      return value;
    }

    dynamic? c = target switch
    {
      NodeTarget.Current => Current = value,
      NodeTarget.Parent => throw new InvalidOperationException(),
      NodeTarget.Header => Header = value,
      NodeTarget.Root => Root = value,
      NodeTarget.RootNodes => assignRootNodes(value),
      NodeTarget.PropKey => PropKey = value,
      TokenGroup => throw new InvalidOperationException(),
      TokenValue => throw new InvalidOperationException(),
    };
  }
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
    AddNodeTo(node, NodeTarget.Parent, null);
    NodeStack.Push(node);
  }
  public void PushAndAddChild ()
  {
    if (HasCurrent)
      PushAndAddChild(Current);
    else
      ParsingException.ThrowNullData("Current");
  }
  public void AddNodeTo (INode node, NodeTarget target, Token? token)
  {
    INode? parent = GetTargetValue(target, token) as INode;
    if (parent is null && target is NodeTarget.Parent)
    {
      if (SingleRoot && Root is not null)
        Root.AddChild(node);
      else if (!SingleRoot && RootNodes is not null)
        RootNodes.Add(node);
      else
        throw new InvalidOperationException();
    }
    else if (parent is not null)
    {
      parent.AddChild(node);
    }
    else
    {
      throw new InvalidOperationException();
    }
  }
  [MemberNotNull(nameof(Current))]
  public TNode GenerateCurrent<TNode> () where TNode : INode, new()
  {
    WarnIfCurrentHasData();
    TNode node = new();
    Current = node;
    return node;
  }
  public void WarnIfCurrentHasData ()
  {
    if (Current is not null)
    {
      Debug.Log(MsgClass.Warning, "Current was not null before assignment.", "StandardRuleSets");
    }
  }
  public void WarnIfCurrentIsNull ()
  {
    if (Current is null)
    {
      Debug.Log(MsgClass.Warning, "Current was already null.", "StandardRuleSets");
    }
  }
  public void ClearCurrent ()
  {
    WarnIfCurrentIsNull();
    Current = null;
    Debug.Log(MsgClass.GreenInfo, "Current Cleared.", this);
  }

  [MemberNotNullWhen(false, nameof(Parent))]
  public bool IsRoot => NodeStack.Count == 0;
  [MemberNotNullWhen(true, nameof(Parent))]
  public bool HasParent => NodeStack.Count > 0;
  [MemberNotNullWhen(true, nameof(Current))]
  public bool HasCurrent => Current is not null;
}
