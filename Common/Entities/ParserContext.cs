#pragma warning disable CA1710 // Identifiers should have correct suffix
#pragma warning disable format // Formatting

using static Common.MsgClass;

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
  private int _depth;
  private readonly Dictionary<string, Stack<dynamic>> _depthProperties = [];
  private readonly Dictionary<string, dynamic?> _staticProperties = [];
  #endregion
  public ParsingInfo? ParsingSet { get; set; }
  public string? OriginText { get; set; }
  public DocumentEntity? Document { get; set; }
  /// <summary>Gets or sets the "Parent" depth property, which is used on most entities in some way.</summary>
  public IEntity? Parent
  {
    get => GetProperty("Parent");
    set
    {
      if (value is not null)
        SetProperty("Parent", value);
    }
  }
  #region Public Methods
  public dynamic? GetStatic (string name) =>
    _staticProperties.TryGetValue(name, out dynamic? value) ? value : null;
  public void SetStatic (string name, dynamic? value) => _staticProperties[name] = value;
  /// <summary>Gets a depth specific property by key name.</summary>
  /// <param name="name">The name of the property.</param>
  /// <returns>The object stored at this depth and key.</returns>
  /// <remarks>Predefined keys:<br/>
  /// * <c>Parent</c> - The current item's parent object, or <see langword="null"/> if top-level.<br/>
  /// * <c>Property</c> - The current item's property object, which stores a key/value pair.<br/>
  /// </remarks>
  public dynamic? GetProperty (string name) =>
    _depthProperties.TryGetValue(name, out Stack<object>? value) && value.Count > 0 ? value.Peek() : null;
  public Stack<dynamic>? GetStack (string name) =>
    _depthProperties.TryGetValue(name, out Stack<dynamic>? value) ? value : null;
  public void SetProperty (string name, dynamic? value)
  {
    Stack<dynamic> stack = GetStack(name) ?? [];
    stack.Push(value);
  }
  /// <summary>Increase the current depth by the specified amount, store the provided values in DepthProperties at the new depth,
  /// and if a child is specified set its parent (to Document when no current Parent, otherwise to Parent) and update Parent
  /// to the child.</summary>
  /// <remarks>Mutates Depth, updates DepthProperties entries at the incremented depth, and updates Parent and
  /// the child's parent reference. Document is used with a null-forgiving assertion when assigning the child's parent
  /// if Parent is null.</remarks>
  /// <param name="amt">Number of depth levels to increase.</param>
  /// <param name="child">The new object to set as the current parent.</param>
  public void Descend (IEntity child)
  {
    _depth++;

    if (Parent is not null)
    {
      Parent.AddChild(child);
    }
    else if (Document is not null)
    {
      Document.SetRoot(child);
    }
    else
    {
      Debug.Log(Warning, $"No parent of child {child} when descending.", this);
    }

    Parent = child;
  }
  /// <summary>Changes the depth to move outward.</summary>
  /// <param name="amt">The number change to depth.</param>
  public void Ascend ()
  {
    _depth--;
    _ = GetStack("Parent")?.Pop();
  }
  #endregion
}
