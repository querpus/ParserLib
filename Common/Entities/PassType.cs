#pragma warning disable CA1710 // Identifiers should have correct suffix
#pragma warning disable format // Formatting

namespace Common.Entities;

public enum PassType
{
  /// <summary>Do nothing.</summary>
  None,
  /// <summary>Converting a <see langword="string"/> or strings into regex <see cref="Match"/> objects.</summary>
  RegexMatching,
  /// <summary>Converting a <see cref="Match"/> into <see cref="IEntity"/> objects.</summary>
  MatchToEntity,
  /// <summary>Operating on the <see cref="IEntity"/> objects to make defined structures.</summary>
  EntityManipulation
}
