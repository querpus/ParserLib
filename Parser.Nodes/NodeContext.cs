#pragma warning disable CA1710 // Identifiers should have correct suffix
#pragma warning disable format // Formatting

using System.ComponentModel;

using NT = Parser.Nodes.NodeTarget;

namespace Parser.Nodes;

public class NodeContext : IPrintable, INotifyPropertyChanged
{
  public event PropertyChangedEventHandler? PropertyChanged;
  #region Private Methods
  private void Notify (string property) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
  private INode? GetTargetNode (NT target) => target switch
  {
    NT.None => null,
    NT.Current => Current,
    NT.Parent => Parent,
    NT.Header => Header,
    NT.Root => Root,
    NT.RootNodes => throw new InvalidNodeException("RootNodes is not a supported node."),
    NT.Context => throw new InvalidNodeException("Context is not a supported node."),
    NT.AnyRootOrParent => Parent ?? (SingleRoot ? Root : throw new InvalidNodeException("RootNodes not supported.")),
    _ => throw new InvalidNodeException($"Cannot access nodetype {target}"),
  };
  private object? GetTargetData (NT target, string data_key)
  {
    if (target is NT.Context)
    {
      return Data.TryGetValue(data_key, out string? data) ? data : null;
    }

    INode? initial = target switch
    {
      NT.None => null,
      NT.Current => Current,
      NT.Parent => Parent,
      NT.Header => Header,
      NT.Root => Root,
      NT.RootNodes => throw new InvalidNodeException("RootNodes not supported."),
      NT.AnyRootOrParent => Parent ?? (SingleRoot ? Root : throw new InvalidNodeException("RootNodes not supported.")),
      _ => throw new InvalidNodeException($"Cannot access nodetype {target}"),
    };

    if (initial is null)
    {
      return null;
    }

    return initial.Data.TryGetValue(data_key, out object? value) ? value : null;
  }
  private static string? GetTokenData (NT target, IToken token) => target switch
  {
    NT when token is null => throw new InvalidTokenException($"Attempted to read {target} data from null token."),
    NT.None => null,
    NT.TokenGroup when token is not null => token.Name,
    NT.TokenValue when token is not null => token.Value,
    _ => throw new InvalidNodeException($"{target} is not a supported token property."),
  };
  private void SetTargetNode (NT target, INode? value)
  {
    INode assignRootNodes (dynamic value)
    {
      RootNodes.Add(value);
      Notify(nameof(RootNodes));
      return value;
    }
    INode? clearRootNodes (dynamic? value)
    {
      RootNodes.Clear();
      Notify(nameof(RootNodes));
      return value;
    }
    if (GetTargetNode(target) is not null && target is not NT.RootNodes and not NT.AnyRootOrParent)
    {
      Log(MsgClass.Warning, $"{target} was not null before assignment.", this);
    }

    _ = target switch
    {
      NT.None => value,
      NT.Current => Current = value,
      NT.Parent or NT.AnyRootOrParent => throw new InvalidOperationException("Cannot assign to parent. Must push. Parent is just top of node stack."),
      NT.Header => Header = value,
      NT.Root => Root = value,
      NT.RootNodes when value is not null => assignRootNodes(value),
      NT.RootNodes when value is null => clearRootNodes(value),
      NT.TokenGroup or NT.TokenValue => throw new InvalidOperationException("Cannot assign to token properties."),
      _ => throw new InvalidOperationException(),
    };
  }
  private void SetTargetData (NT target, string data_key, object? value)
  {
    if (target is  NT.Context)
    {
      if (value is null)
        _ = Data.Remove(data_key);
      else if (target is NT.Context && value.GetType().IsAssignableTo(typeof(string)))
        Data[data_key] = (string) value;
      return;
    }

    INode? initial = target switch
    {
      NT.None => null,
      NT.Current => Current,
      NT.Parent => Parent,
      NT.Header => Header,
      NT.Root => Root,
      NT.RootNodes => throw new InvalidNodeException("RootNodes not supported."),
      NT.AnyRootOrParent => Parent ?? (SingleRoot ? Root : throw new InvalidNodeException("RootNodes not supported.")),
      _ => throw new InvalidNodeException($"Cannot access nodetype {target}"),
    };

    if (initial is null && value is null)
      return;
    else if (initial is null)
      throw new InvalidNodeException($"Node {target} was null.");
    else if (value is null)
      _ = initial.Data.Remove(data_key);
    else
      initial.Data[data_key] = value;
  }
  #endregion
  /// <summary>Set this to <see langword="true"/> if there must be a single node that everything is within.
  /// Set this to <see langword="false"/> if there can be multiple top-level objects.</summary>
  public required bool SingleRoot { get; init; }
  /// <summary>The name of the current property.</summary>
  public string? PropKey { get; set; }
  /// <summary>Gets the stack of nodes used to track the current node hierarchy during traversal or processing.</summary>
  /// <remarks>Init-only and initialized to an empty stack.</remarks>
  public Stack<INode> NodeStack { get; init; } = [];
  /// <summary>The currently selected node. (Child Node)</summary>
  public INode? Current { get; set; }
  /// <summary>The root nodes when <see cref="SingleRoot"/> is <see langword="false"/>.</summary>
  public ObservableCollection<INode> RootNodes { get; } = [];
  /// <summary>The root node.</summary>
  public INode? Root { get; set; }
  /// <summary>Document Header.</summary>
  public INode? Header { get; set; }
  public INode? Parent => HasParent ? NodeStack.Peek() : null;
  [MemberNotNullWhen(false, nameof(Parent))]
  public bool IsRoot => NodeStack.Count == 0;
  [MemberNotNullWhen(true, nameof(Parent))]
  public bool HasParent => NodeStack.Count > 0;
  [MemberNotNullWhen(true, nameof(Current))]
  public bool HasCurrent => Current is not null;
  public IEnumerable<INode> Nodes { get; private set; } = [];
  /// <summary>Dictionary for storing additional data that must be carried throughout the processing of the nodes.</summary>
  public Dictionary<string, string> Data { get; } = [];

  /// <summary>Pushes a node to the top of the node stack, and adds it as a child of the previous node on the stack.</summary>
  /// <param name="node">The child node to push.</param>
  /// <param name="target">The node to add this node to as a child.</param>
  /// <remarks>This assigns the root node if the stack is empty.</remarks>
  public void PushAndAddTo (INode node, NT target)
  {
    AddNodeTo(node, target);
    NodeStack.Push(node);
    Notify(nameof(NodeStack));
  }
  public void ReplaceParent (INode node, NT target_for_pop = NT.None, NT target_for_push = NT.AnyRootOrParent)
  {
    if (HasParent)
    {
      _ = PopTo(target_for_pop);

      if (IsRoot)
      {
        PushAndAddTo(node, target_for_push);
        Log(MsgClass.GreenInfo, $"Replaced Parent and added to {target_for_push}.", this);
      }
      else
      {
        Log(MsgClass.Warning, "Warning: Context was not at root when pushing a new parent.", this);
        PushAndAddTo(node, target_for_push);
      }
    }
    else
    {
      PushAndAddTo(node, target_for_push);
      Log(MsgClass.GreenInfo, $"Initial Parent set and added to {target_for_push}", this);
    }
  }
  /// <summary>Pops a node from the node stack, and assigns it to the target field.</summary>
  /// <param name="target">The field to assign the popped node to.</param>
  /// <returns>The node that was taken off the top of the stack.</returns>
  /// <remarks>If you do not need the node, you can use <see cref="NT.None"/> to prevent any assignment.</remarks>
  public INode PopTo (NT target)
  {
    if (NodeStack.IsEmpty)
    {
      throw new InvalidOperationException("NodeStack is empty, cannot pop.");
    }
    INode node = NodeStack.Pop();
    SetTargetValue(target, node);
    return node;
  }
  public TNode PopTo<TNode> (NT target) where TNode : class, INode
  {
    if (NodeStack.IsEmpty)
    {
      throw new InvalidOperationException("NodeStack is empty, cannot pop.");
    }
    if (NodeStack.Pop() is not TNode node)
    {
      throw new InvalidOperationException($"Node is not {nameof(TNode)}.");
    }
    SetTargetNode(target, node);
    return node;
  }
  /// <summary>Adds the <paramref name="node"/> to the node at <paramref name="target"/> as a child</summary>
  /// <param name="node">The child node.</param>
  /// <param name="target">The target of the parent node.</param>
  /// <remarks>Additional targets will be have the node added to, except for <see cref="NT.AnyRootOrParent"/>.
  /// This combination flag attempts to add it to the parent object,
  /// if that fails then add it to the root object, or add it to the <see cref="NT.RootNodes"/> list.</remarks>
  /// <exception cref="InvalidOperationException">No node was added to any target.</exception>
  public void AddNodeTo (INode node, NT target)
  {
    bool success = false;

    if (target.HasFlag(NT.AnyRootOrParent))
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
      return;
    }
    if (target.HasFlag(NT.Parent))
    {
      INode? parent = GetTargetNode(NT.Parent);
      if (parent is not null)
      {
        success = true;
        parent.AddChild(node);
      }
    }
    if (target.HasFlag(NT.RootNodes))
    {
      RootNodes.Add(node);
      success = true;
    }
    if (target.HasFlag(NT.Current))
    {
      if (Current is not null)
      {
        Current.AddChild(node);
        success = true;
      }
      else if (!success)
      {
        throw new InvalidOperationException("Current is null");
      }
      else
      {
        Log(MsgClass.Warning, "Attempted to add to null node 'Current'.", this);
      }
    }
    if (!success)
    {
      throw new InvalidOperationException($"No Node Added to Target {target}.");
    }
  }
  public void AssignNodeTo (INode node, NT target)
  {
    if (target.HasAnyFlag(NT.Fields))
    {
      throw new InvalidOperationException($"Cannot assign node to {target}.");
    }
    SetTargetNode(target, node);
  }

  public TNode GenerateNode<TNode> (NT target) where TNode : INode, new()
  {
    WarnIfNotNull(target);
    TNode node = new();
    SetTargetValue(target, node);
    Nodes = Nodes.Append(node);
    return node;
  }
  public INode GenerateNode (NT target, string className)
  {
    WarnIfNotNull(target);
    INode? node = className.CreateClass() as INode ?? throw new InvalidOperationException($"{className} is not a proper INode.");
    SetTargetValue(target, node);
    Nodes = Nodes.Append(node);
    return node;
  }
  public void WarnIfNotNull (NT target)
  {
    if (GetTargetValue(target) is not null)
    {
      Log(MsgClass.Warning, $"{target} was not null before assignment.", this);
    }
  }
  public void ClearNode (NT target)
  {
    bool attempt = false;

    if (target is NT.None)
    {
      Log(MsgClass.Warning, "Clearing the null node target. This is not ever neccessary.", this);
      attempt = true;
    }
    if (target.HasAnyFlag(NT.Fields))
    {
      Log(MsgClass.Warning, $"Cannot clear readonly field {target}.", this);
      attempt = true;
    }
    if (GetTargetValue(target) is null)
    {
      Log(MsgClass.Warning, $"{target} was already null.", this);
      attempt = true;
    }

    if (target.HasFlag(NT.Parent))
    {
      Log(MsgClass.Warning, "Cannot clear parent. Use pop.", this);
      attempt = true;
    }
    if (target.HasFlag(NT.Current))
    {
      Current = null;
      attempt = true;
    }
    if (target.HasFlag(NT.Header))
    {
      Header = null;
      attempt = true;
    }
    if (target.HasFlag(NT.Root) && SingleRoot)
    {
      Root = null;
      attempt = true;
    }
    if (target.HasFlag(NT.RootNodes) && !SingleRoot)
    {
      RootNodes.Clear();
    }

    if (!attempt)
      Log(MsgClass.Warning, $"Did not clear {target}.", this);
    else
      Log(MsgClass.GreenInfo, $"Node stored in {target} was cleared.", this);
  }

  public void DoXMLOperation (CaptureToken token, string action, string? assignTo, string? valueFrom, string? className)
  {
    NT target = assignTo?.ToNodeTarget() ?? NT.None;
    NT source = valueFrom?.ToNodeTarget() ?? NT.None;

    switch (action)
    {
      case "GenerateNodeTo" when className is not null:
        INode node = GenerateNode(target, className);
        break;
      case "AssignNodeTo" when target is not NT.None && source is not NT.None:
        INode? ant_node = GetTargetNode(source);
        if (ant_node is not null)
          AssignNodeTo(ant_node, target);
        else
          throw new InvalidNodeException($"Source node is null. Cannot assign {source} node to {target}.");
        break;
      case "AddToNodeAt" when target is not NT.None && source is not NT.None:
        INode? atna_node = GetTargetNode(source);
        if (atna_node is not null)
          AddNodeTo(atna_node, target);
        else
          throw new InvalidNodeException($"Source node is null. Cannot assign {source} node to {target}.");
        break;
      case "PushNodeToStack" when source is not NT.None:
        INode? pnts_node = GetTargetNode(source);
        if (pnts_node is not null)
          NodeStack.Push(pnts_node);
        else
          throw new InvalidNodeException($"Source node is null. Cannot push {source} node to the stack.");
        break;
      case "PopNodeTo" when target is not NT.None:
        _ = PopTo(target);
        break;
      default:
        throw new InvalidOperationException($"Improper XML element {action}");
    }
  }

  public void Print (int indent)
  {
    string sp = new(' ', indent);
    if (Header is not null)
    {
      Log(MsgClass.Warning, $"{sp}Header:", this);
      Header.Print(2 + indent);
    }
    if (Root is not null)
    {
      Log(MsgClass.Warning, $"{sp}Root:", this);
      Root?.Print(2 + indent);
    }
  }

  public override string ToString ()
  {
    const string prefix = "NodeContext:";
    string single = SingleRoot ? "[Single]" : "";
    string propkey = PropKey.IsNotEmpty ? $"(PK:{PropKey})" : "";
    string current = Current is not null ? $"(Cur:{Current.Serialize()})" : "";

    string[] list = [prefix, single, propkey, current];

    return list.TextJoin(" ");
  }
}
