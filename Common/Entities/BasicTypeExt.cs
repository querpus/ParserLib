#pragma warning disable CA1710 // Identifiers should have correct suffix
#pragma warning disable format // Formatting

using BT = Common.Entities.BasicType;

namespace Common.Entities;

public static class BasicTypeExt
{
  extension(BT type)
  {
    /// <summary>This type is a primitive value.</summary>
    public bool IsPrimitive => type is BT.Number or BT.String or BT.Boolean or BT.Null;
    public bool IsDictionary => type is BT.Object or BT.Element or BT.Section;
    public bool IsCollection => type is BT.Array or BT.Element or BT.Object or BT.Document;
  }
}
