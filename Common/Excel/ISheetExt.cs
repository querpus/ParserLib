#if HAS_OFFICE
using Microsoft.Office.Interop.Excel;

using XlRange = Microsoft.Office.Interop.Excel.Range;

namespace Common.Excel;

public class SheetExt (Worksheet sheet) : ISheetExt
{
  public Worksheet XlSheet { get; } = sheet;
}

public interface ISheetExt
{
  Worksheet XlSheet { get; }
  Collection<string> GetRangeValues (Worksheet sheet, Range rng)
  {
    if (rng is null)
      return [];

    Collection<string> result = [];
    Collection<ICollection<XlRange>> cells = [.. rng.Nodes.Select(i => (ICollection<XlRange>) sheet.Range[i.MinAddress, i.MaxAddress].Cells)];

    return result;
  }
  Range GetRange (Worksheet sheet, string address) => new(address);
}
#endif
