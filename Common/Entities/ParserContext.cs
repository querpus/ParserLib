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
  private readonly Dictionary<string, Stack<object>> _depthProperties = [];
  #endregion
  public ParsingInfo? ParsingSet { get; set; }
  public string? OriginText { get; set; }
  public DocumentEntity? Document { get; set; }
  /// <summary>Gets or sets the "Parent" depth property, which is used on most entities in some way.</summary>
  public IEntity? Parent
  {
    get => HasDepthProperty("Parent") ? GetDepthProperty("Parent") : null;
    set
    {
      if (value is not null)
        SetDepthProperty("Parent", value);
    }
  }
  #region Public Methods
  /// <summary>Gets a depth specific property by key name.</summary>
  /// <param name="name">The name of the property.</param>
  /// <returns>The object stored at this depth and key.</returns>
  /// <remarks>Predefined keys:<br/>
  /// * <c>Parent</c> - The current item's parent object, or <see langword="null"/> if top-level.<br/>
  /// * <c>Property</c> - The current item's property object, which stores a key/value pair.<br/>
  /// * <c>Child</c> - The newly created object that will become the new parent when descending.<br/>
  /// * <c>ChildType</c> - The type of child to create if the child is <see langword="null"/>
  /// </remarks>
  public dynamic? GetDepthProperty (string name) =>
    _depthProperties.TryGetValue(name, out Stack<object>? value) ? value.Peek() : null;
  public bool HasDepthProperty (string name) => _depthProperties.ContainsKey(name) && _depthProperties[name].Count > 0;
  public TValue? GetDepthProperty<TValue> (string name) where TValue : class =>
    _depthProperties.TryGetValue(name, out Stack<object>? value) ? value.TryPeek(out object? result) ? result as TValue : null : null;
  public TValue GetDepthProperty<TValue> (string name, TValue if_not_found) where TValue : struct =>
    _depthProperties.TryGetValue(name, out Stack<object>? value) ? value.TryPeek(out object? result) ? result is TValue actual_value ? actual_value : if_not_found : if_not_found : if_not_found;
  public void SetDepthProperty (string name, dynamic? value)
  {
    if (!_depthProperties.TryGetValue(name, out Stack<object>? prop))
    {
      prop = [];
      _depthProperties.Add(name, prop);
    }

    prop.Push(value);
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

    GetDepthProperty<Stack<object>>("Parent");
  }
  /// <summary>Changes the depth to move outward.</summary>
  /// <param name="amt">The number change to depth.</param>
  public void Ascend ()
  {
    Stack<object>? parents = GetDepthProperty<Stack<object>>("Parent");
    _ = parents?.Pop();
  }
  #endregion
}
