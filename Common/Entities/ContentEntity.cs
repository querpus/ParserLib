#pragma warning disable CA1710 // Identifiers should have correct suffix
#pragma warning disable format // Formatting

namespace Common.Entities;

public interface IContentEntity
{
  string Content { get; init; }
  string Serialize ();
}

public class ContentEntity : Entity, IContentEntity
{
  public virtual required string Content
  {
    get => (string) DataValues["Content"]!;
    init => DataValues["Content"] = value;
  }
  public override bool Equals (IEntity? other) =>
    other is IContentEntity ce && Content.Is(ce.Content);
  public override string Serialize () => Content;
}
