#pragma warning disable CA1710 // Identifiers should have correct suffix
#pragma warning disable format // Formatting

using System.ComponentModel;

using NT = Common.Nodes.NodeTarget;

namespace Common.Nodes;

public class NodeContext : IPrintable, INotifyPropertyChanged
{
  public event PropertyChangedEventHandler? PropertyChanged;
  #region Private Methods
  private static NT ExtractFields (NT target, scoped out NT extracted)
  {
    extracted = target & (NT.Prefix | NT.Suffix);
    return target & ~(NT.Prefix | NT.Suffix);
  }
  private static NT ExtractAllFields (NT target, scoped out NT extracted)
  {
    extracted = target & (NT.Prefix | NT.Suffix | NT.TokenGroup | NT.TokenValue);
    return target & ~(NT.Prefix | NT.Suffix | NT.TokenGroup | NT.TokenValue);
  }
  private dynamic? GetTargetValue (NT target, CaptureToken? token = null)
  {
    NT extracted = target & (NT.Prefix | NT.Suffix);
    NT stripped = target & ~(NT.Prefix | NT.Suffix);

    dynamic? initial = stripped switch
    {
      NT.Null => null,
      NT.Current => Current,
      NT.Parent => Parent,
      NT.Header => Header,
      NT.Root => Root,
      NT.RootNodes => RootNodes,
      NT.PropKey => PropKey,
      NT.TokenGroup when token is not null => token.Value.Name,
      NT.TokenValue when token is not null => token.Value.Value,
      NT.AnyRootOrParent => (dynamic?) Parent ?? (SingleRoot ? Root : RootNodes),
      _ => throw new InvalidOperationException(),
    };

    return extracted switch
    {
      NT.Null => initial,
      NT.Prefix => initial?.Prefix,
      NT.Suffix => initial?.Suffix,
      _ => throw new InvalidOperationException("Cannot have prefix and suffix flags both on a 'get' operation."),
    };
  }
  private void SetTargetValue (NT target, dynamic? value)
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
    if (ExtractAllFields(target, out NT fields) is not NT.Null && fields > NT.Null)
    {
      throw new InvalidOperationException($"Cannot set the field {fields} in this way.");
    }
    if (GetTargetValue(target) is not null && target is not NT.RootNodes and NT.AnyRootOrParent)
    {
      Debug.Log(MsgClass.Warning, $"{target} was not null before assignment.", this);
    }

#pragma warning disable IDE0059 // Unnecessary assignment of a value
    dynamic? c = target switch
    {
      NT.Null => value,
      NT.Current => Current = value,
      NT.Parent or NT.AnyRootOrParent => throw new InvalidOperationException("Cannot assign to parent. Must push. Parent is just top of node stack."),
      NT.Header => Header = value,
      NT.Root => Root = value,
      NT.RootNodes when value is not null => assignRootNodes(value),
      NT.RootNodes when value is null => clearRootNodes(value),
      NT.PropKey when value is not null => value is string ? PropKey = value : throw new InvalidOperationException("PropKey must be assigned a string or null."),
      NT.PropKey when value is null => PropKey = null,
      NT.TokenGroup or NT.TokenValue => throw new InvalidOperationException("Cannot assign to token properties."),
      _ => throw new InvalidOperationException(),
    };
#pragma warning restore IDE0059 // Unnecessary assignment of a value
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
  }
  public void ReplaceParent (INode node, NT target_for_pop = NT.Null, NT target_for_push = NT.AnyRootOrParent)
  {
    if (HasParent)
    {
      _ = PopTo(target_for_pop);
    }

    if (IsRoot)
    {
      PushAndAddTo(node, target_for_push);
      Debug.Log(MsgClass.GreenInfo, $"Replaced Parent and added to {target_for_push}.", this);
    }
    else
    {
      Debug.Log(MsgClass.Warning, "Warning: Context was not at root when pushing line command.", this);
    }
  }
  /// <summary>Pops a node from the node stack, and assigns it to the target field.</summary>
  /// <param name="target">The field to assign the popped node to.</param>
  /// <returns>The node that was taken off the top of the stack.</returns>
  /// <remarks>If you do not need the node, you can use <see cref="NT.Null"/> to prevent any assignment.</remarks>
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
    SetTargetValue(target, node);
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
      if (GetTargetValue(target) is INode parent)
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
      else
      {
        throw new InvalidOperationException("Current is null");
      }
    }
    if (!success)
    {
      throw new InvalidOperationException($"No Node Added to Target {target}.");
    }
  }
  /// <summary>Appends the currrent token to the prefix field or the suffix field on the target node.</summary>
  /// <param name="target">The target node and field.</param>
  /// <param name="token">The token to append.</param>
  /// <exception cref="InvalidOperationException">Both Suffix and Prefix were included in <paramref name="target"/> or an invalid <paramref name="target"/> was specified.</exception>
  public void AppendTokenTo (NT target, IToken token)
  {
    NT stripped = ExtractFields(target, out NT fields);
    dynamic? dyn_node = GetTargetValue(stripped);

    if (dyn_node is INode node && fields != NT.Null && token.Value is not null)
    {
      if (fields is NT.Prefix)
      {
        node.AppendToPrefix(token.Value);
      }
      else if (fields is NT.Suffix)
      {
        node.AppendToSuffix(token.Value);
      }
      else
      {
        throw new InvalidOperationException("Prevented dual assignment. Single assignment only.");
      }
    }
    else
    {
      throw new InvalidOperationException($"Invalid target {target}.");
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
      Debug.Log(MsgClass.Warning, $"{target} was not null before assignment.", this);
    }
  }
  public void ClearNode (NT target)
  {
    if (target is NT.Null)
    {
      Debug.Log(MsgClass.Warning, "Clearing the null node target. This is not ever neccessary.", this);
      return;
    }
    if (target.HasAnyFlag(NT.Fields))
    {
      Debug.Log(MsgClass.Warning, $"Cannot clear readonly field {target}.", this);
      return;
    }
    if (GetTargetValue(target) is null)
    {
      Debug.Log(MsgClass.Warning, $"{target} was already null.", this);
      return;
    }
    SetTargetValue(target, null);
    Debug.Log(MsgClass.GreenInfo, $"Node stored in {target} was cleared.", this);
  }

  public static void DoXMLOperation (NodeContext context, CaptureToken token, string action, string? assignTo, string? valueFrom, string? className)
  {
    NT target = assignTo?.ToNodeTarget() ?? NT.Null;
    NT source = valueFrom?.ToNodeTarget() ?? NT.Null;

    switch (action)
    {
      case "GenerateNodeTo" when className is not null:
        INode node = context.GenerateNode(target, className);
        node.AppendToPrefix(token.Value);
        break;
      case "AssignNodeTo" when target is not NT.Null && source is not NT.Null:
        context.AssignNodeTo(context.GetTargetValue(source), target);
        break;
      case "AddToNodeAt" when target is not NT.Null && source is not NT.Null:
        context.AddNodeTo(context.GetTargetValue(source), target);
        break;
      case "PushNodeToStack" when source is not NT.Null:
        context.AddNodeTo(context.GetTargetValue(source), target);
        break;
      case "PopNodeTo" when target is not NT.Null:
        _ = context.PopTo(target);
        break;
      default:
        throw new InvalidOperationException($"Improper XML element {action}");
    }
  }

  public void Print (int indent)
  {
    Debug.Log(MsgClass.Warning, "Root:", this);
    Root?.Print(2 + indent);
    Debug.Log(MsgClass.Warning, "Header:", this);
  }
}
