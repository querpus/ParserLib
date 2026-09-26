#pragma warning disable CA1710 // Identifiers should have correct suffix
#pragma warning disable format // Formatting

namespace Common.Entities;

public interface IContentEntity
{
  string Content { get; }
  string Serialize ();
}
/// <summary>An entity representing loose unquoted content.<br/><br/>
/// Uses DataValues:<br/>
/// * <c>Content</c> - The entire contents of the document as text.
/// </summary>
public class ContentEntity : Entity, IContentEntity
{
  public virtual required string Content
  {
    get => (string) DataValues["Content"]!;
    set => DataValues["Content"] = value;
  }
  public override bool Equals (IEntity? other) =>
    other is IContentEntity ce && Content.Is(ce.Content);
  public override string Serialize () => Content;

  protected override void Assign (Match match)
  {
    Content = match.Value;
    Origin = match.Value;
  }
}
