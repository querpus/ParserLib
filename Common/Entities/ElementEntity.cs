#pragma warning disable CA1710 // Identifiers should have correct suffix
#pragma warning disable format // Formatting

using System.Data;

using BT = Common.Entities.BasicType;

namespace Common.Entities;

public class ElementEntity : ElementOpenPlaceholder
{
  public bool IsHeader { get; set; }
  public override BT Type => BT.Element;

  public override string Serialize ()
  {
    string attrs = Attributes.Select(child => child.ToString()).TextJoin(" ");
    string children = Children.Select(child => child.ToString()).TextJoin(Chars.LFs);

    if (IsHeader)
      return $"<?xml {attrs}?>";

    string elem = $"<{Name} {attrs}";

    return Children.OfType<ElementEntity>().ICount == 0
    ? elem + " />"
    : elem + ">" + children + $"</{Name}>";
  }
  public void AddChild (IEntity child)
  {
    child.SetParent(this);
    Children.Add(child);
  }
  public void AddChildren (IEnumerable<IEntity> children) => children.Foreach(AddChild);
  public override bool Equals (IEntity? other) =>
    other is ElementEntity ee &&
    Attributes.SequenceEqual(ee.Attributes) &&
    Children.SequenceEqual(ee.Children) &&
    Name.Equals(ee.Name, SCO);
}
