#pragma warning disable CA1710 // Identifiers should have correct suffix
#pragma warning disable format // Formatting

using System.Runtime.Serialization;

namespace Common.Entities;

public interface IContentEntity : ITextSerializer
{
  string? Content { get; }
}
/// <summary>An entity representing loose unquoted content.<br/><br/>
/// Uses DataValues:<br/>
/// * <c>Content</c> - The entire contents of the document as text.
/// </summary>
public class ContentEntity : Entity, IContentEntity
{
  public virtual string? Content
  {
    get => DataValues.TryGetValue("Content", out object? value) ? (string) value! : null;
    set => DataValues["Content"] = value;
  }
  public override bool IsValid => Content is not null;
  public override string Serialize () => Content ?? throw new SerializationException("Failed to serialize this entity. Content ws null");
  public override void Assign (Match match)
  {
    Content = match.Value;
    Origin = match.Value;
  }
}
