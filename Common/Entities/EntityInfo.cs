#pragma warning disable CA1710 // Identifiers should have correct suffix
#pragma warning disable format // Formatting

using BT = Common.Entities.BasicType;

namespace Common.Entities;

public enum SpecialReqType
{
  None, // Always pass
  GroupExists,
  GroupValue,
  MatchValue,
  MatchLength,
  GroupLength,

  Invert      = 0x00010000,
  GreaterThan = 0x00020000,
  LessThan    = 0x00040000,
  EqualTo     = 0x00080000,
  EqualToIC   = 0x00100000
}

public readonly struct SpecialReq
{
  public SpecialReqType Requirements { get; init; }
  public dynamic? Value { get; init; }

  public static implicit operator SpecialReq ((SpecialReqType Requirements, dynamic? Value) tuple) =>
    new() { Value = tuple.Value, Requirements = tuple.Requirements };
}

public readonly struct SpecialValue
{
  public dynamic Value { get; init; }
  public ReadOnlyCollection<SpecialReq> SpecialReqs { get; init; }
}

/// <summary>This is common data to describe variations of entity properties.</summary>
public class EntityInfo : IEquatable<Match>
{
  #region Functional Properties
  /// <summary>
  /// Can be any predefined type, or it can be the special value <see cref="BT.Raw"/>.
  /// This determines the class of entity that is produced.
  /// </summary>
  public BT Type { get; set; }
  /// <summary>If <see cref="Type"/> is <see cref="BT.External"/>, this is the class that is created.</summary>
  public Type? Class { get; set; }
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
  /// Adds this entity to the parent stack, meaning it will receive all tokens that are passed as data once the depth descends.<br/>
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
  /// <summary>These are conditional assignments to an entity.</summary>
  public Collection<SpecialValue> SpecialValues { get; init; } = [];
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
  /// <summary>Checks to see if a match will satisfy the entity</summary>
  /// <param name="match">The regex match that is being analyzed.</param>
  /// <returns><see langword="true"/> if the match satisfies the requirements, <see langword="false"/> if the match is <see langword="null"/> or does not meet them.</returns>
  public bool Equals (Match? match)
  {
    if (match is null)
      return false;

    Collection<bool> checks = [];
    checks.Add(IndicatedItem.Group is null || match.Groups.Cast<Group>().Any(g => g.Name.Like(IndicatedItem.Group)));
    checks.Add(IndicatedItem.ExactValue is null || match.Value.Equals(IndicatedItem.ExactValue, IndicatedItem.IgnoreCase ? SCOIC : SCO));
    return checks.All(b => b);
  }
  /// <summary>Basic equality, quick method.</summary>
  /// <param name="obj">The other object.</param>
  /// <returns><see langword="true"/> if the object is an <see cref="EntityInfo"/> and the properties are the same. Otherwise <see langword="false"/>.</returns>
  public override bool Equals (object? obj) => obj is EntityInfo info && Equals(info);
  public override int GetHashCode () => HashCode.Combine(Type, IndicatedItem, DepthChange, SetPropKey, SetAsNextLevelParent, CreateEmptyAtStart, ConstantValue, StorePieceTypes, HashCode.Combine(StoresData, DefinesStructure, OnlyAtTopLevel));
  public static bool operator == (EntityInfo left, Match right) => left.Equals(right);
  public static bool operator != (EntityInfo left, Match right) => !(left == right);
  #endregion
}
