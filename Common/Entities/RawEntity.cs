#pragma warning disable CA1710 // Identifiers should have correct suffix
#pragma warning disable format // Formatting

namespace Common.Entities;

public class RawEntity : Entity, IPrintable
{
  public void Print (int indent)
  {
    Debug.LogPart(MsgClass.Debug, "RawEntity Data: ");
    Debug.LogPart(MsgClass.BlueInfo, DataValues.Keys.TextJoin(", "));
  }
  public override string Serialize () => "RawEntity Data:" + DataValues.TextJoin(",") + " | Children: " + Children.TextJoin(",");

}
