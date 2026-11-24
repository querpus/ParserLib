namespace Terminal.Menu;

public abstract class MenuAction
{
  public static MenuAction Quit { get; } = new MenuQuitAction();

  public ConsoleKey? Key { get; init; }
  public bool OnSelect { get; init; }
  public string Data { get; init; } = EmptyString;
  /// <summary>For external actions, this is a method that takes an <see cref="IList{T}"/> of parameters.</summary>
  public Action<IList<object>>? Action { get; init; }
  /// <summary>A key to identify this item.</summary>
  public string? KeyName { get; init; }
  /// <summary>The name of the next menu.</summary>
  public string? NextMenuName { get; }

  public abstract void Execute ();
}

public sealed class MenuQuitAction : MenuAction
{
  public override void Execute ()
  {
    MenuController.IsExiting = true;
    MenuController.CloseMenu();
  }
}
public sealed class MenuMoveAction : MenuAction
{
  public override void Execute ()
  {
    MenuItem? item = MenuController.SelectedMenuItem;
    MenuBase? menu = MenuController.ActiveMenu;
    if (item is null || menu is null || int.TryParse(Data, out int i))
      return;

    item.IsDirty = true;
    menu.Index = Math.Max(menu.Index - i, 0);

    item = MenuController.SelectedMenuItem;

    if (item is null)
      return;

    item.IsDirty = true;
    MenuController.HasMoved = true;
  }
}

