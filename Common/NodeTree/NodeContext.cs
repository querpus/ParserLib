#pragma warning disable CA1710 // Identifiers should have correct suffix
#pragma warning disable format // Formatting

using NT = Common.NodeTree.NodeTarget;

namespace Common.NodeTree;

public class NodeContext
{
  public dynamic? GetTargetValue (NT target, CaptureToken? token = null) => target switch
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
  public void SetTargetValue (NT target, dynamic? value)
  {
    INode assignRootNodes (dynamic value)
    {
      RootNodes.Add(value);
      return value;
    }
    INode? clearRootNodes (dynamic? value)
    {
      RootNodes.Clear();
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
      NT.RootNodes when value is not null => assignRootNodes(value),
      NT.RootNodes when value is null => clearRootNodes(value),
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
    if (target is NT.Parent)
    {
      if (GetTargetValue(target, null) is not INode parent)
      {
        if (SingleRoot)
        {
          if (Root is not null)
            Root.AddChild(node);
          else
            Root = node;
        }
        else
        {
          RootNodes.Add(node);
        }
      }
      else
      {
        parent.AddChild(node);
      }
    }
    else if (target is NT.RootNodes)
    {
      RootNodes.Add(node);
    }
    else if (target is NT.Current)
    {
      if (Current is not null)
      {
        Current.AddChild(node);
      }
      else
      {
        throw new InvalidOperationException("Current is null");
      }
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
    SetTargetValue(target, null);
    Debug.Log(MsgClass.GreenInfo, $"Node stored in {target} was cleared.", this);
  }

  [MemberNotNullWhen(false, nameof(Parent))]
  public bool IsRoot => NodeStack.Count == 0;
  [MemberNotNullWhen(true, nameof(Parent))]
  public bool HasParent => NodeStack.Count > 0;
  [MemberNotNullWhen(true, nameof(Current))]
  public bool HasCurrent => Current is not null;
}
