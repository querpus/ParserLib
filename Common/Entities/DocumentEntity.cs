#pragma warning disable CA1710 // Identifiers should have correct suffix
#pragma warning disable format // Formatting

using static Common.Chars;

namespace Common.Entities;

/// <summary>An entity representing a document.<br/><br/>
/// <b>DataValues:</b><br/>
/// * <c>RootNode</c> - The root node of the document.<br/>
/// * <c>Header</c> - The header of the document (if present).<br/>
/// * <c>Content</c> - The entire contents of the document as text.<br/>
/// <b>Children:</b><br/>
/// Only if the document does not have a root node.
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
  public override string Serialize ()
  {
    string result = SE;

    if (Header is not null)
    {
      result += Header.Serialize() + LFs;
    }

    if (RootNode is not null)
    {
      result += RootNode.Serialize() + LFs;
    }

    if (Children.Count > 0)
    {
      result += Children.Select(ent => ent.Serialize()).TextJoin(LFs) + LFs;
    }

    return result;
  }

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
  public override void AddChild (IEntity child)
  {
    if (RootNode is not null)
      throw new InvalidOperationException("Root node is defined, do not add children to the document. Add them to the root node.");

    base.AddChild(child);
  }
}
