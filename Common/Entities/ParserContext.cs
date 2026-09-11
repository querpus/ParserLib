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
  private readonly Dictionary<string, Dictionary<int, object?>> _depthProperties = [];
  #endregion
  /// <summary>The collection of objects to operate on.</summary>
  [AllowNull]
  public IReadOnlyList<dynamic> WorkingSet { get; set; }
  public ParsingInfo? ParsingSet { get; set; }
  public string? OriginText { get; set; }
  public DocumentEntity? Document { get; set; }
  /// <summary>The index within the <see cref="WorkingSet"/> this item is.</summary>
  /// <value>An index between 0 and the count of <see cref="WorkingSet"/>.</value>
  /// <remarks>This returns <see cref="DNE"/> if the index is not specified or the <see cref="WorkingSet"/> not defined.</remarks>
  public int CurrentIndex { get; set; } = DNE;
  [AllowNull]
  public dynamic CurrentItem => WorkingSet[CurrentIndex];
  /// <summary>Gets or sets the "Parent" depth property, which is used on most entities in some way.</summary>
  public IEntity? Parent
  {
    get => GetDepthProperty("Parent");
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
  /// ^ <c>ChildType</c> - The type of child to create if the child is <see langword="null"/>
  /// </remarks>
  public dynamic? GetDepthProperty (string name) =>
    _depthProperties.TryGetValue(name, out Dictionary<int, object?>? value) ? value[_depth] : null;
  public TValue? GetDepthProperty<TValue> (string name) where TValue : class =>
    _depthProperties.TryGetValue(name, out Dictionary<int, object?>? value) ? value[_depth] as TValue : null;
  public TValue GetDepthProperty<TValue> (string name, TValue if_not_found) where TValue : struct =>
    _depthProperties.TryGetValue(name, out Dictionary<int, object?>? value) ? (TValue?) value[_depth] ?? if_not_found : if_not_found;
  public void SetDepthProperty (string name, dynamic? value) => _depthProperties[name][_depth] = value;
  /// <summary>Increase the current depth by the specified amount, store the provided values in DepthProperties at the new depth,
  /// and if a child is specified set its parent (to Document when no current Parent, otherwise to Parent) and update Parent
  /// to the child.</summary>
  /// <remarks>Mutates Depth, updates DepthProperties entries at the incremented depth, and updates Parent and
  /// the child's parent reference. Document is used with a null-forgiving assertion when assigning the child's parent
  /// if Parent is null.</remarks>
  /// <param name="amt">Number of depth levels to increase.</param>
  /// <param name="set_depth_values">Dictionary mapping property names to values to assign in DepthProperties for the new depth.</param>
  /// <param name="child">The new child if one is needed.</param>
  public void Descend (int amt, Dictionary<string, object> set_depth_values, IEntity child)
  {
    int adj = _depth + amt;

    if (adj > 0x7fff)
    {
      Debug.Log(Warning, $"Depth was {adj}, clamping at {0x7fff}.", this);
    }

    IEntity? previous_parent = Parent;

    _depth = Math.Clamp(adj, 0, 0x7fff);

    foreach (KeyValuePair<string, object> kvp in set_depth_values)
    {
      if (_depthProperties.TryGetValue(kvp.Key, out Dictionary<int, object?>? keyed_data))
      {
        keyed_data[_depth] = kvp.Value;
      }
      else
      {
        _depthProperties[kvp.Key] = new()
        {
          [_depth] = kvp.Value
        };
      }
    }

    Parent = child;

    if (previous_parent is not null)
    {
      Parent.SetParent(previous_parent);
      previous_parent.Children.Add(child);
    }
    else if (Document is not null)
    {
      Parent.SetParent(Document);
      Document.SetRoot(child);
    }
    else
    {
      Debug.Log(Warning, $"No parent of child {child} when descending.", this);
    }
  }
  public void Ascend (int amt)
  {
    int adj = _depth - amt;

    if (adj < 0)
    {
      Debug.Log(Warning, $"Depth was {adj}, clamping at 0.", this);
    }

    _depth = Math.Clamp(adj, 0, 0x7fff);

  }
  public T? GetParentAs<T> () where T : IEntity
  {
    dynamic? parent = Parent;
    return (T?) parent;
  }
  public T? GetPropKeyAs<T> () where T : IEntity
  {
    dynamic? propKey = PropKeys[_depth];
    return (T?) propKey;
  } 
  #endregion
}
