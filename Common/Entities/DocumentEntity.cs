#pragma warning disable CA1710 // Identifiers should have correct suffix
#pragma warning disable format // Formatting

using BT = Common.Entities.BasicType;

namespace Common.Entities;

/// <summary>An entity representing a document.</summary>
public class DocumentEntity : Entity
{
  /// <summary>The entire contents of the file.</summary>
  public required string Content { get; init; }

  public IEntity? RootNode
  {
    get => PropertyValues.TryGetValue("RootNode", out IEntity? value) ? value : null;
    set => PropertyValues["RootNode"] = value!;
  }
  public IEntity? Header
  {
    get => PropertyValues.TryGetValue("Header", out IEntity? value) ? value : null;
    set => PropertyValues["Header"] = value!;
  }
  public override BT Type => BT.Document;

  public override bool Equals (IEntity? other) =>
    other is DocumentEntity je && Content.Equals(je.Content, SCO);
  public override string Serialize () => Content;
  public void SetRoot (IEntity? root)
  {
    if (root is null) return;
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
