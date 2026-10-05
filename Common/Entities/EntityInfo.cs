#pragma warning disable CA1710 // Identifiers should have correct suffix
#pragma warning disable format // Formatting

namespace Common.Entities;

[Flags]
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
  EqualToIC   = 0x00100000,
  AllFlags    = Invert | GreaterThan | LessThan | EqualTo | EqualToIC,
}

public readonly struct SpecialReq
{
  public SpecialReqType Requirements { get; init; }
  public string? Group { get; init; }
  public dynamic? Value { get; init; }

  public static implicit operator SpecialReq ((SpecialReqType Requirements, dynamic? Value) tuple) =>
    new() { Value = tuple.Value, Requirements = tuple.Requirements };
  public bool RequirementsMet (Match match)
  {
    bool invert = Requirements.HasFlag(SpecialReqType.Invert);
    bool equalic = Requirements.HasFlag(SpecialReqType.EqualToIC);

    bool meets = Requirements.RemoveBit<SpecialReqType>(SpecialReqType.AllFlags) switch
    {
      SpecialReqType.GroupExists when Group is not null => match.HasValidGroup(Group),
      SpecialReqType.GroupValue when Group is not null => match.HasValidGroup(Group) && match.Groups[Group].Value.Equals(Value, equalic ? SCOIC : SCO),
      SpecialReqType.GroupLength when Group is not null => match.HasValidGroup(Group) && FuncParse(match.Groups[Group].Value.Length),
      SpecialReqType.None => true,
      SpecialReqType.MatchValue => match.Value.Contains(Value),
      SpecialReqType.MatchLength => FuncParse(match.Value.Length),
      _ => false
    };

    return invert ? !meets : meets;
  }

  private bool FuncParse(int len)
  {
    bool eq = Requirements.HasFlag(SpecialReqType.EqualTo);
    bool gr = Requirements.HasFlag(SpecialReqType.GreaterThan);
    bool ls = Requirements.HasFlag(SpecialReqType.LessThan);

    if (eq && ls)
      return len <= Value;
    if (eq && gr)
      return len >= Value;
    if (eq)
      return len == Value;
    if (ls)
      return len < Value;
    else
      return gr && (len > Value);
  }
}

public readonly struct SpecialValue
{
  public dynamic Value { get; init; }
  public ReadOnlyCollection<SpecialReq> SpecialReqs { get; init; }
}

public enum DepthOperation
{
  /// <summary>Indicates that we are popping the parent stack.</summary>
  Ascend = -1,
  /// <summary>Indicates the depth and parent to not change.</summary>
  Maintain = 0,
  /// <summary>Indicates that we are pushing 'NextParent' to the 'Parent' stack.</summary>
  Descend = 1,
  /// <summary>Indicates that we are popping the parent stack, then immediately pushing this entity to the stack.</summary>
  AscendAndDescend = 0x7fff
}

/// <summary>This is common data to describe variations of entity properties.</summary>
public class EntityInfo
{
  #region Functional Properties
  public Collection<ITokenTask> TokenTasks { get; init; } = [];
  /// <summary>This is the class that is created. Must be derived from <see cref="IEntity"/>.</summary>
  public Type? Class { get; set; }
  /// <summary>The group that must be present in the match for this entity to be produced if specified.</summary>
  public string? GroupRequired { get; set; }
  /// <summary>The group that indicates the end of this entity.</summary>
  public string? GroupTerminator { get; set; }
  /// <summary>The exact text that the match must be for this entity to be produced if specified.</summary>
  public string? ExactTextRequired { get; set; }
  /// <summary>If the exact text match requirement is not case sensitive.</summary>
  public bool IgnoreCaseWhenMatching { get; set; }
  /// <summary>The change in depth this token indicates.</summary>
  /// <remarks>
  /// Normally 0, 1, or -1.<br/>
  /// <c>Descend (-1) </c>: Increase depth (descend). For example, an opening bracket '{'.<br/>
  /// <c>Maintain (0) </c>: No change in depth. A comma ',' or a keyword in a statement. This is the default value.<br/>
  /// <c>Ascend (1)   </c>: Decrease depth (ascend). For example, a closing bracket '}'.<br/>
  /// <c>AscendAndDescend (0x7fff)</c>:
  /// </remarks>
  public DepthOperation DepthChange { get; init; }
  public Dictionary<string, object> AscendIf { get; init; } = [];
  public Dictionary<string, object> DescendIf { get; init; } = [];
  /// <summary>Adds this entity to the top entity in the 'Property' stack.</summary>
  /// <remarks>When <see langword="true"/>, this entity is added to the currently active property at this depth.
  /// Defaults to <see langword="false"/>.
  /// </remarks>
  public bool AddToProperty { get; init; }
  /// <summary>Gets a value indicating whether the property context variable should be set.</summary>
  /// <remarks>When <see langword="true"/>, this entity is set to be the currently active property at this depth.
  /// Defaults to <see langword="false"/>.
  /// </remarks>
  public bool SetPropKey { get; init; }
  public bool IgnoreExtra { get; init; }
  /// <summary>
  /// Adds this entity to the 'NextParent' static property, meaning it will become the next parent when we descend,<br/>
  /// This does not have to be the depth changing token.
  /// </summary>
  public bool SetAsNextLevelParent { get; init; }
  /// <summary>
  /// Only pop the 'Parent' stack if the top of the stack is this class.
  /// </summary>
  /// <remarks>
  /// Only valid for the <see cref="DepthOperation.AscendAndDescend"/> depth option.
  /// Ignored otherwise.
  /// </remarks>
  public Type? OnlyAscendIfParentClass { get; internal set; }

  #endregion
  #region Data Properties
  /// <summary>The type of piece this entity stores as.</summary>
  /// <remarks>
  /// Format:<br/>
  /// Key is value <see langword="string"/>.<br/>
  /// Value is GroupName <see langword="string"/>.<br/>
  /// Multiple Captures on the group mean a <see cref="Collection{T}"/> is made with an entry for each capture.
  /// </remarks>
  public Dictionary<string, string> StorePieceTypes { get; init; } = [];
  /// <summary>These are conditional assignments to an entity.</summary>
  public Collection<SpecialValue> SpecialValues { get; init; } = [];
  #endregion Data Properties
  #region Overrides and Equality

  /// <summary>Checks to see if a match will satisfy the entity</summary>
  /// <param name="match">The regex match that is being analyzed.</param>
  /// <returns><see langword="true"/> if the match satisfies the requirements, <see langword="false"/> if the match is <see langword="null"/> or does not meet them.</returns>
  public bool Matches (Match match)
  {
    bool exact_compare = ExactTextRequired is not null && (IgnoreCaseWhenMatching ? match.Value.Like(ExactTextRequired) : match.Value.Is(ExactTextRequired));
    return
      (GroupRequired is null || match.HasValidGroup(GroupRequired)) &&
      (ExactTextRequired is null || exact_compare);
  }

  /// <summary>Basic equality, quick method.</summary>
  /// <param name="obj">The other object.</param>
  /// <returns><see langword="true"/> if the object is an <see cref="EntityInfo"/> and the properties are the same. Otherwise <see langword="false"/>.</returns>
  public override bool Equals (object? obj) => obj is EntityInfo info && GetHashCode() == info.GetHashCode();
  public override int GetHashCode () => HashCode.Combine(Class, DepthChange, SetPropKey, SetAsNextLevelParent, StorePieceTypes, SpecialValues);
  public static bool operator == (EntityInfo left, Match right) => left.Matches(right);
  public static bool operator != (EntityInfo left, Match right) => !(left == right);
  #endregion
}
