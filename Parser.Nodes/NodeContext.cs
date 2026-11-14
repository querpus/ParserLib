#pragma warning disable CA1710 // Identifiers should have correct suffix
#pragma warning disable format // Formatting

using System.ComponentModel;

using NT = Parser.Nodes.NodeTarget;

namespace Parser.Nodes;

public class NodeContext : IPrintable, INotifyPropertyChanged
{
  public event PropertyChangedEventHandler? PropertyChanged;
  #region Protected Internal Methods
  protected internal void Notify (string property)
  {
    if (property == "NodeStack")
      Notify("Parent");
    if (property == "Root")
      Notify("NodeStack");
    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
  }
  protected internal INode? GetTargetNode (NT target, bool do_not_throw_for_root_nodes, bool never_throw_ever = false)
  {
    try
    {
      return GetTargetNode(target);
    }
    catch (NotSupportedException) when (do_not_throw_for_root_nodes)
    {
      return null;
    }
    catch (InvalidNodeException) when (never_throw_ever)
    {
      return null;
    }
  }
  protected internal INode? GetTargetNode (NT target)
  {
    var targets = target.GetTargets(inclusive_mask: NT.NodeProperties);

    if (targets.Length > 1 && target != NT.RootOrParent)
      throw new InvalidNodeException($"Cannot access multiple node types. ({target})");

    return target switch
    {
      NT when target.HasFlag(NT.RootOrParent) => Parent ?? (SingleRoot ? Root : throw new NotSupportedException("RootNodes not supported.")),
      NT.None => null,
      NT.Current => Current,
      NT.Parent => Parent,
      NT.Header => Header,
      NT.Root => SingleRoot ? Root : throw new NotSupportedException("RootNodes not supported."),
      NT.Temp => Temp,
      _ => throw new InvalidNodeException($"Unknown node type. ({target})"),
    };
  }
  protected internal object? GetTargetData (NT target, string data_key)
  {
    if (target.GetTargets().Length > 1 && target != NT.RootOrParent)
    {
      throw new InvalidNodeException($"Cannot access multiple node types. ({target})");
    }

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
      NT.Temp => Temp,
      NT when target.HasFlag(NT.RootOrParent) => Parent ?? (SingleRoot ? Root : throw new InvalidNodeException("RootNodes not supported.")),
      _ => throw new InvalidNodeException($"Cannot access nodetype {target}"),
    };

    if (initial is null)
    {
      return null;
    }

    return initial.Data.TryGetValue(data_key, out object? value) ? value : null;
  }
  protected internal static string? GetTokenData (NT target, IToken token) => target switch
  {
    not NT.None when token is null => throw new InvalidTokenException($"Attempted to read {target} data from null token."),
    NT.None => null,
    NT.TokenGroup => token.Name,
    NT.TokenValue => token.Value,
    _ => throw new InvalidNodeException($"{target} is not a supported token property."),
  };
  protected internal object GetTarget (NT target)
  {

  }
  protected internal INode? SetRoot (INode? value)
  {
    if (value is null)
    {
      Log(MsgClass.Warning, $"Attempted to set the root to null.", this);
      return value;
    }

    if (ReferenceEquals(value, Root))
    {
      Notify("Root");
      return value;
    }

    if (SingleRoot)
    {
      Root = value;
      NodeStack.Push(value);
      Notify("Root");
    }
    else
    {
      RootNodes.Add(value);
      Notify("RootNodes");
    }
    return value;
  }
  protected internal INode? SetParent (INode? value, bool replace_if_not_root = false)
  {
    if (value is null)
    {
      Log(MsgClass.Warning, $"Attempted to set the parent to null.", this);
    }
    else if (ReferenceEquals(value, Parent))
    {
      Notify("Parent");
      return value;
    }
    else if (HasParent && replace_if_not_root)
    {
      _ = NodeStack.Pop();
      Notify(nameof(NodeStack));
      if (HasParent)
      {
        Log(MsgClass.Warning, $"Replacing parent did not reach the root. ", this);
      }
    }
    else if (HasParent)
    {
      Parent.AddChild(value);
      Notify("Parent");
      NodeStack.Push(value);
      Notify("NodeStack");
    }
    else
    {
      Log(MsgClass.Warning, $"No parent defined, NodeStack was empty. Assign Root First.", this);
      NodeStack.Push(value);
      Notify("NodeStack");
    }
    return value;
  }
  protected internal INode? SetCurrent (INode? value)
  {
    if (ReferenceEquals(Current, value))
    {
      Notify(nameof(Current));
      return value;
    }
    if (Current is not null)
    {
      Log(MsgClass.Warning, "The node in Current was not properly cleared prior to being overwritten.", this);
    }
    Current = value;
    Notify(nameof(Current));
    return value;
  }
  protected internal INode? SetTemp (INode? value) => Temp = value;
  protected internal INode? SetHeader (INode? value)
  {
    if (ReferenceEquals(Header, value))
    {
      Notify(nameof(Header));
      return value;
    }
    if (Header is not null)
    {
      Log(MsgClass.Warning, "The node in Header was overwritten.", this);
    }
    Temp = value;
    Notify(nameof(Header));
    return value;
  }
  protected internal void SetTargetNode (NT target, INode? value)
  {
    if (target is NT.None)
    {
      return;
    }
    if (target.HasAnyFlag(NT.Fields))
    {
      Log(MsgClass.Warning, $"Target contains unreachable fields. ({target})", this);
    }
    if (!target.HasAnyFlag(NT.NodeProperties))
    {
      Log(MsgClass.Error, $"Target has no node properties. ({target})", this);
      return;
    }
    foreach (var item in target.Mask<NT>(NT.NodeProperties).GetTargets())
    {
      INode? _ = item switch
      {
        NT.Parent => SetParent(value),
        NT.Current => SetCurrent(value),
        NT.Header => SetHeader(value),
        NT.Root => SetRoot(value),
        NT.Temp => SetTemp(value),
        _ => throw new InvalidNodeException($"Unknown node type {item}."),
      };
    }
  }
  protected internal void SetTargetData (NT target, string data_key, object? value)
  {
    if (target is NT.None)
    {
      return;
    }

    if (target.HasAnyFlag(NT.TokenGroup | NT.TokenValue))
    {
      Log(MsgClass.Warning, $"Target contains read only fields. ({target})", this);
    }

    if (value is null)
    {
      throw new InvalidNodeException($"Attempted to assign null to {target}[{data_key}].");
    }

    if (target.HasFlag(NT.Context))
    {
      Data[data_key] = value.GetType().IsAssignableTo(typeof(string)) ? (string) value : $"{value}";
      return;
    }

    foreach (NT item in (target & NT.NodeProperties).GetTargets().Where(i => i != NT.None))
    {
      INode? node = item switch
      {
        NT.Current => SetCurrent(Current),
        NT.Parent => Parent,
        NT.Header => Header,
        NT.Temp => Temp,
        NT.Root => Root,
        NT when target == NT.RootOrParent => Parent ?? (SingleRoot ? Root : throw new NotSupportedException("RootNodes not supported.")),
        _ => throw new InvalidNodeException($"Unknown node type {target}"),
      } ?? throw new InvalidNodeException($"Node {target} was null.");

      node.Data[data_key] = value;
    }
  }
  #endregion
  #region Public Methods for Nodes

  public void WriteData (NT source, NT target, bool write_if_true_condition = true)
  {
    if (write_if_true_condition)
    {
      object src = GetTarget(source);
    }
    else
    {
      //Do not execute (conditional).
    }
  }

  #endregion
  /// <summary>Set this to <see langword="true"/> if there must be a single node that everything is within.
  /// Set this to <see langword="false"/> if there can be multiple top-level objects.</summary>
  public required bool SingleRoot { get; init; }
  /// <summary>Gets the stack of nodes used to track the current node hierarchy during traversal or processing.</summary>
  /// <remarks>Init-only and initialized to an empty stack.</remarks>
  public Stack<INode> NodeStack { get; init; } = [];
  public IToken Token { get; private set; }
  /// <summary>The currently selected node. (Child Node)</summary>
  public INode? Current { get; set; }
  /// <summary>The root nodes when <see cref="SingleRoot"/> is <see langword="false"/>.</summary>
  public ObservableCollection<INode> RootNodes { get; } = [];
  /// <summary>The root node.</summary>
  public INode? Root { get; set; }
  /// <summary>The document header.</summary>
  public INode? Header { get; set; }
  /// <summary>The top of the <see cref="NodeStack"/>.</summary>
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
  public INode? Temp { get; private set; }

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
  /// <summary>Pops a node from the node stack, and assigns it to the target field.</summary>
  /// <param name="target">The field to assign the popped node to.</param>
  /// <returns>The node that was taken off the top of the stack.</returns>
  /// <remarks>If you do not need the node, you can use <see cref="NT.None"/> to prevent any assignment.</remarks>
  public INode PopTo (NT target)
  {
    if (NodeStack.IsEmpty)
    {
      throw new InvalidNodeException("NodeStack is empty, cannot pop.");
    }
    INode node = NodeStack.Pop();
    SetTargetNode(target, node);
    return node;
  }
  public TNode PopTo<TNode> (NT target) where TNode : class, INode
  {
    if (NodeStack.IsEmpty)
    {
      throw new InvalidNodeException("NodeStack is empty, cannot pop.");
    }
    if (NodeStack.Pop() is not TNode node)
    {
      throw new InvalidNodeException($"Node is not {nameof(TNode)}.");
    }
    SetTargetNode(target, node);
    return node;
  }
  /// <summary>Adds the <paramref name="node"/> to the node at <paramref name="target"/> as a child</summary>
  /// <param name="node">The child node.</param>
  /// <param name="target">The target of the parent node.</param>
  /// <remarks>Additional targets will be have the node added to, except for <see cref="NT.RootOrParent"/>.
  /// This combination flag attempts to add it to the parent object,
  /// if that fails then add it to the root object, or add it to the <see cref="NT.RootNodes"/> list.</remarks>
  /// <exception cref="InvalidOperationException">No node was added to any target.</exception>
  public void AddNodeTo (INode node, NT target)
  {
    if (target.HasAnyFlag(NT.RootOrParent))
    {
      if (SingleRoot && IsRoot)
      {
        Root = node;
      }
      else if (!SingleRoot && IsRoot)
      {
        RootNodes.Add(node);
      }
      else if (Parent is not null)
      {
        Parent.AddChild(node);
      }
      else
      {
        throw new InvalidNodeException("AddNodeTo found an impossible state when passed RootOrParent");
      }
      return;
    }
    if (target.HasFlag(NT.Current))
    {
      if (Current is not null)
      {
        Current.AddChild(node);
      }
      else
      {
        Log(MsgClass.Error, "Attempted to add to null node 'Current'.", this);
        throw new InvalidNodeException("Attempted to add to null node 'Current'.");
      }
    }
    if (target.HasAnyFlag(NT.Fields))
    {
      Log(MsgClass.Error, $"Attempted to add to field '{target}'.", this);
      throw new InvalidNodeException("Attempted to add to null node 'Current'.");
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
    SetTargetNode(target, node);
    Nodes = Nodes.Append(node);
    return node;
  }
  public INode GenerateNode (NT target, string className)
  {
    WarnIfNotNull(target);
    INode? node = className.CreateClass() as INode ?? throw new InvalidOperationException($"{className} is not a proper INode.");
    SetTargetNode(target, node);
    Nodes = Nodes.Append(node);
    return node;
  }
  public void WarnIfNotNull (NT target)
  {
    if (GetTargetNode(target) is not null)
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
      Log(MsgClass.Error, $"Cannot clear field {target}.", this);
      throw new InvalidNodeException($"Attempted to clear field {target}.");
    }
    if (GetTargetNode(target) is null)
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
    if (target.HasFlag(NT.Root) && !SingleRoot)
    {
      RootNodes.Clear();
      attempt = true;
    }

    if (!attempt)
      Log(MsgClass.Warning, $"Did not clear {target}.", this);
    else
      Log(MsgClass.GreenInfo, $"Node stored in {target} was cleared.", this);
  }

  internal void DoXMLOperation (CaptureToken token, string action, string? assignTo, string? valueFrom, string? className)
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

  /// <summary>Prints the summary of this node context out to the standard output stream.</summary>
  /// <param name="indent">The number of spaces to indent the lines after the title.</param>
  public void Print (int indent)
  {
    LogPart(MsgClass.Warning, "NodeContext (Count:");
    LogPart(MsgClass.BlueValue, Nodes.ICount})");
    string sp = new(' ', indent);
    if (Header is not null)
    {
      LogPart(MsgClass.Warning, $"{sp}Header:");
      Header.Print(2 + indent);
    }
    if (Root is not null)
    {
      LogPart(MsgClass.Warning, $"{sp}Root:");
      Root.Print(2 + indent);
    }
  }

  public override string ToString ()
  {
    const string prefix = "NodeContext:";
    string single = SingleRoot ? "[Single]" : "";
    string current = Current is not null ? $"(Cur:{Current.Serialize()})" : "";
    string header = Header is not null ? $"(Head:{Header.Serialize()})" : "";
    string root = Root is not null ? $"(Root:{Root.Children.Count})" : "";
    string[] list = [ prefix, single, header, root, current ];

    return list.TextJoin(" ");
  }
}
