#pragma warning disable CA1710 // Identifiers should have correct suffix
#pragma warning disable format // Formatting

namespace Common.Entities;

public class WhitespaceEntity : ContentEntity
{
  public override bool Equals (IEntity? other) =>
    other is WhitespaceEntity we && Content.Is(we.Content);
}
