#pragma warning disable CA1710 // Identifiers should have correct suffix
#pragma warning disable format // Formatting

namespace Common.Entities;
public sealed class ParsingContext
{
  #region Private Fields
  /// <summary>The number of objects this is within.</summary>
  /// <remarks>
  /// A depth of zero could still mean you are within a document object (if one is used).
  /// There are no more potentially recursive objects to exit, you are at the surface.
  /// </remarks>
  /// <value>This value is clamped to be within 0 and 32767.</value>
  public int Depth { get; set; }
  private readonly Dictionary<string, Stack<dynamic>> _depthProperties = [];
  private readonly Dictionary<string, dynamic?> _staticProperties = [];
  #endregion
  public ParsingInfo? ParsingSet { get; set; }
  public string? OriginText { get; set; }
  public DocumentEntity? Document { get; set; }
  /// <summary>Gets or sets the "Parent" depth property, which is used on most entities in some way.</summary>
  public IEntity? Parent
  {
    get => PeekStack("Parent");
    set
    {
      if (value is not null)
        PushStack("Parent", value);
    }
  }
  #region Public Methods
  public dynamic? GetStatic (string name) =>
    _staticProperties.TryGetValue(name, out dynamic? value) ? value : null;
  public void SetStatic (string name, dynamic? value) => _staticProperties[name] = value;
  public Stack<dynamic> GetStack (string name)
  {
    bool hasStack = _depthProperties.TryGetValue(name, out Stack<dynamic>? stack);

    if (!hasStack || stack is null)
    {
      stack = new Stack<dynamic>();
      _depthProperties[name] = stack;
    }

    return stack;
  }
  public TEntity? PeekStack<TEntity> (string name) where TEntity : class, IEntity, new()
  {
    bool hasStack = _depthProperties.TryGetValue(name, out Stack<dynamic>? stack);

    return hasStack && stack is not null ? (TEntity) stack.Peek() : null;
  }

  public void PushStack (string name, dynamic? value)
  {
    Stack<dynamic> stack = GetStack(name);
    stack.Push(value);
  }
  public dynamic? PeekStack (string name)
  {
    Stack<dynamic> stack = GetStack(name);

    return stack.Count == 0 ?  null : stack.Peek();
  }
  public dynamic PopStack (string name)
  {
    Stack<dynamic> stack = GetStack(name);
    return stack.Count > 0 ? stack.Pop() : throw new InvalidOperationException("Stack is empty");
  }
  #endregion
}
