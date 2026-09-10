#pragma warning disable CA1710 // Identifiers should have correct suffix
#pragma warning disable format // Formatting

using BT = Common.Entities.BasicType;

namespace Common.Entities;

public class RawEntity : Entity, IPrintable
{
  public override BT Type => BT.Raw;
  public void Print (int indent)
  {
    LogPart(Debug);
  }
  public override string Serialize () => "RawEntity Data:" + DataValues.TextJoin(",") + " | Children: " + Children.TextJoin(",");

}
