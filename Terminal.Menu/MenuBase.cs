namespace Terminal.Menu;

public abstract class MenuBase
{
  #region Default Static Members
  public static Range DefaultLogLines { get; } = 12..;
  #endregion
  public required string Name { get; init; }
  public int Index { get; internal set; }
  public virtual required Collection<MenuItem> Items { get; init; }
  public virtual Collection<string> HeaderLineText { get; } = ["Select an option with the arrow keys, press enter to execute."];
  public virtual Collection<int> HeaderLines { get; } = [0];
  public virtual Collection<MenuAction> CommonActions { get; } = [];
  public Collection<MenuAction> AllActions
  {
    get
    {
      Collection<MenuAction> all = [.. CommonActions];
      all.AddRange(Items[Index].ItemActions);
      return all;
    }
  }
  #region Line Ranges
  public virtual Range OptionLines { get; } = 1..10;
  public int FirstOptionLine => OptionLines.Start.Value;
  public virtual Range LogLines { get; } = DefaultLogLines;
  public int FirstLogLine => LogLines.Start.Value;
  #endregion
  protected Dictionary<string, object> MData { get; } = [];
  public void WriteData (string key, object value) => MData[key] = value;
  public object ReadData (string key) => MData[key];

  protected MenuBase () => MenuController.AddNewMenu(this);
}
