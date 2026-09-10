#pragma warning disable CA1710 // Identifiers should have correct suffix
#pragma warning disable format // Formatting

using BT = Common.Entities.BasicType;

namespace Common.Entities;

public class CommentEntity : ContentEntity
{
  public override BT Type => BT.Comment;
  public override bool Equals (IEntity? other) =>
    other is CommentEntity ce && Content.Is(ce.Content);
}
