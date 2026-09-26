#pragma warning disable CA1710 // Identifiers should have correct suffix
#pragma warning disable format // Formatting

namespace Common.Entities;

/// <summary>An entity representing a document.<br/><br/>
/// Uses DataValues:<br/>
/// * <c>RootNode</c> - The root node of the document.<br/>
/// * <c>Header</c> - The header of the document (if present).<br/>
/// * <c>Content</c> - The entire contents of the document as text.
/// </summary>
public class DocumentEntity : ContentEntity
{
  public IEntity? RootNode
  {
    get => DataValues.TryGetValue("RootNode", out object? value) ? value as IEntity : null;
    set => DataValues["RootNode"] = value;
  }
  public IEntity? Header
  {
    get => DataValues.TryGetValue("Header", out object? value) ? value as IEntity : null;
    set => DataValues["Header"] = value;
  }
  public override string Serialize () => Content;
  public void SetRoot (IEntity? root)
  {
    if (root is null)
      return;
    RootNode = root;
    root.SetParent(this);
  }
  public void SetHeader (IEntity? header)
  {
    if (header is null)
      return;
    Header = header;
    header.SetParent(this);
  }
}
