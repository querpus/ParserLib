#pragma warning disable CA1710 // Identifiers should have correct suffix
#pragma warning disable format // Formatting

namespace Common.Entities;

public class ArrayEntity : Entity
{
  public void AddValue (IEntity child) => AddChild(child);
  public void AddValues (IEnumerable<IEntity> children) => children.Foreach(AddValue);

  public override bool Equals (IEntity? other) =>
    other is ArrayEntity ce && Children.SequenceEqual(ce.Children);
  public override string Serialize () => Children.TextJoin(",");
  protected override void Assign (Match match) { }
}
