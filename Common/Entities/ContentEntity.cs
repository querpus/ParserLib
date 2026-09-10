#pragma warning disable CA1710 // Identifiers should have correct suffix
#pragma warning disable format // Formatting

using BT = Common.Entities.BasicType;

namespace Common.Entities;

public class ContentEntity : Entity
{
  public required string Content
  {
    get => (string) DataValues["Content"]!;
    init => DataValues["Content"] = value;
  }
  public override BT Type => BT.LooseContent;

  public override bool Equals (IEntity? other) =>
    other is ContentEntity ce && Content.Is(ce.Content);
  public override string Serialize () => Content;
}
