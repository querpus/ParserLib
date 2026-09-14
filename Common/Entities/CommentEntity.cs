#pragma warning disable CA1710 // Identifiers should have correct suffix
#pragma warning disable format // Formatting

namespace Common.Entities;

public class CommentEntity : ContentEntity
{
  public override BasicType Type => BasicType.Comment;
}
