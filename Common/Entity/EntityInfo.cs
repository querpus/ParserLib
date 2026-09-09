#pragma warning disable CA1710 // Identifiers should have correct suffix
#pragma warning disable format // Formatting

using BT = Common.Entity.BasicType;

namespace Common.Entity;

public class EntityInfo
{

  #region Functional Properties
  /// <summary>
  /// Can be any predefined type, or it can be the special value <see cref="BT.Custom"/>.
  /// This determines the class of entity that is produced.
  /// </summary>
  public BT Type { get; set; }
  /// <summary>The conditions that must be present for this entity to be produced.</summary>
  public IndicationRule IndicatedItem { get; set; }
  /// <summary>The change in depth this token indicates.</summary>
  /// <remarks>
  /// Normally 0, 1, or -1.<br/>
  /// <c> 1</c>: Increase depth (descend). For example, an opening bracket '{'.<br/>
  /// <c> 0</c>: No change in depth. A comma ',' or a keyword in a statement. This is the default value.<br/>
  /// <c>-1</c>: Decrease depth (ascend). For example, a closing bracket '}'.
  /// </remarks>
  public int DepthChange { get; init; }
  /// <summary>Gets a value indicating whether this item stores itself as a value in the property context variable</summary>
  /// <remarks>When <see langword="true"/>, this entity is added to the currently active property at this depth.
  /// Defaults to <see langword="false"/>.
  /// </remarks>
  public bool AddToPropKey { get; init; }
  /// <summary>Gets a value indicating whether the property context variable should be set.</summary>
  /// <remarks>When <see langword="true"/>, this entity is set to be the currently active property at this depth.
  /// Defaults to <see langword="false"/>.
  /// </remarks>
  public bool SetPropKey { get; init; }
  /// <summary>
  /// Adds this entity to the parent stack, meaning it will recieve all tokens that are passed as data once the depth descends.<br/>
  /// This does not have to be the depth changing token.
  /// </summary>
  public bool SetAsNextLevelParent { get; init; }
  /// <summary>Creates this entity outside of the parsing loop as the initial container for all other tokens.</summary>
  public bool CreateEmptyAtStart { get; init; }
  #endregion
  #region Data Properties
  /// <summary>The constant value if this entity always has the same value.</summary>
  public string? ConstantValue { get; init; }
  /// <summary>The type of piece this entity stores as.</summary>
  /// <remarks>
  /// Format:<br/>
  /// Key is PieceType <see langword="string"/>.<br/>
  /// Value is GroupName <see langword="string"/>.<br/>
  /// Multiple Captures on the group mean a <see cref="Collection{T}"/> is made with an entry for each capture.
  /// </remarks>
  public Dictionary<string, string> StorePieceTypes { get; init; } = [];
  #endregion Data Properties
  #region Informative Properties
  /// <summary>Whether or not the entity stores data into its parent.</summary>
  public bool StoresData { get; set; }
  /// <summary>Whether or not this entity causes an structural change to parsing.</summary>
  public bool DefinesStructure { get; set; }
  #endregion
  #region Validation Properties
  /// <summary>Only allow this entity at top-level, not as a child.</summary>
  public bool OnlyAtTopLevel { get; set; }
  #endregion

  #region Overrides and Equality
  public override bool Equals (object? obj) => obj is EntityInfo rules && GetHashCode() == rules.GetHashCode();
  public override int GetHashCode () => HashCode.Combine(Type, IndicatedItem, DepthChange, SetPropKey, SetAsNextLevelParent, CreateEmptyAtStart, ConstantValue, StorePieceTypes, HashCode.Combine(StoresData, DefinesStructure, OnlyAtTopLevel));
  public static bool operator == (EntityInfo left, EntityInfo right) => left.Equals(right);
  public static bool operator != (EntityInfo left, EntityInfo right) => !(left == right);
  public bool Equals (EntityInfo other) => GetHashCode() == other.GetHashCode();
  #endregion
}
