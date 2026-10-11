#pragma warning disable CA1710 // Identifiers should have correct suffix
#pragma warning disable format // Formatting

using NT = Common.NodeTree.NodeTarget;

namespace Common.NodeTree;

public class NodeContext
{
  public dynamic? GetTargetValue (NT target, Token? token = null) => target switch
  {
    NT.Null => null,
    NT.Current => Current,
    NT.Parent => Parent,
    NT.Header => Header,
    NT.Root => Root,
    NT.RootNodes => RootNodes,
    NT.PropKey => PropKey,
    NT.TokenGroup when token is not null => token.Value.Group,
    NT.TokenValue when token is not null => token.Value.Value,
    _ => throw new InvalidOperationException(),
  };
  public void SetTargetValue (NT target, dynamic value, Token? token = null)
  {
    INode assignRootNodes (dynamic value)
    {
      RootNodes.Add(value);
      return value;
    }

#pragma warning disable IDE0059 // Unnecessary assignment of a value
    dynamic? c = target switch
    {
      NT.Null => value,
      NT.Current => Current = value,
      NT.Parent => throw new InvalidOperationException(),
      NT.Header => Header = value,
      NT.Root => Root = value,
      NT.RootNodes => assignRootNodes(value),
      NT.PropKey => value is string ? PropKey = value : throw new InvalidOperationException(),
      NT.TokenGroup => throw new InvalidOperationException(),
      NT.TokenValue => throw new InvalidOperationException(),
      _ => throw new InvalidOperationException(),
    };
#pragma warning restore IDE0059 // Unnecessary assignment of a value
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
  public void PushAndAddTo (INode node)
  {
    AddNodeTo(node, NT.Parent);
    NodeStack.Push(node);
  }
  public void AddNodeTo (INode node, NT target)
  {
    INode? parent = GetTargetValue(target, null) as INode;
    if (parent is null && target is NT.Parent)
    {
      if (SingleRoot && Root is not null)
        Root.AddChild(node);
      else if (!SingleRoot && RootNodes is not null)
        RootNodes.Add(node);
      else
        throw new InvalidOperationException();
    }
    if (target is NT.RootNodes && Root is null)
    {
      RootNodes.Add(node);
    }
    else if (parent is not null)
    {
      parent.AddChild(node);
    }
    else
    {
      throw new InvalidOperationException("Not handled yet");
    }
  }
  public void AssignNodeTo (INode node, NT target) => SetTargetValue(target, node);
  public TNode GenerateNode<TNode> (NT target) where TNode : INode, new()
  {
    WarnIfNotNull(target);
    TNode node = new();
    SetTargetValue(target, node);
    return node;
  }
  public INode GenerateNode (NT target, string className)
  {
    WarnIfNotNull(target);
    INode? node = className.CreateClass() as INode ?? throw new InvalidOperationException();
    SetTargetValue(target, node);
    return node;
  }
  public void WarnIfNotNull (NT target)
  {
    if (GetTargetValue(target) is not null)
    {
      Debug.Log(MsgClass.Warning, $"{target} was not null before assignment.", "StandardRuleSets");
    }
  }
  public void WarnIfNull (NT target)
  {
    if (Current is null)
    {
      Debug.Log(MsgClass.Warning, $"{target} was already null.", "StandardRuleSets");
    }
  }
  public void ClearNode (NT target)
  {
    WarnIfNull(target);
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
