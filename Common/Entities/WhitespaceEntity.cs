#pragma warning disable CA1710 // Identifiers should have correct suffix
#pragma warning disable format // Formatting

using BT = Common.Entities.BasicType;

namespace Common.Entities;

public class WhitespaceEntity : Entity
{
  public required string Content
  {
    get => (string) DataValues["Content"]!;
    set => DataValues["Content"] = value;
  }
  public override BT Type => BT.IgnoredWhitespace;

  public override bool Equals (IEntity? other) =>
    other is ContentEntity ce && Content.Is(ce.Content);
  public override string Serialize () => Content;
}
