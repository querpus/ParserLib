namespace Terminal.Menu;

[method: SetsRequiredMembers]
public class MenuItem (int index, string caption, IEnumerable<MenuAction> actions)
{
  public required string Caption { get; init; } = caption;
  public required int Index { get; init; } = index;
  public int Line => (MenuController.ActiveMenu?.OptionLines.Start.Value + Index) ?? ErrVal;
  public bool IsDirty { get; set; } = true;
  public Collection<MenuAction> ItemActions { get; init; } = [.. actions];
  public void Draw (int current_index)
  {
    if (Line is ErrVal)
    {
      throw new InvalidOperationException("Line was not defined.");
    }

    if (IsDirty)
    {
      string cursor = Line == current_index ? ">" : " ";
      MenuController.EraseLine(Line, true);
      Console.WriteLine(cursor + " " + Caption);
    }
  }
}
/*
public class MenuItem2D : MenuItem
{
  public int XAxis { get; internal set; }
}

public class MenuItemValue : MenuItem
{
  public int Value { get; internal set; }
}

public class MenuItemNoSelect : MenuItem
{
  //TODO: Figure this one out.
}
*/
