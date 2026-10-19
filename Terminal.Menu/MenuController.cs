using System.Drawing;

namespace Terminal.Menu;

public class MenuHandler;
[Flags]
public enum MenuAreaType
{
  /// <summary>No special handling</summary>
  None,
  AllowSelection,
  LogArea,

}

public static class SampleMenuClass
{
  public static MenuDefinition Initial = new()
  {
    MenuID = "SampleInitial",
    Areas = [
      new MenuArea_Select() {
        AreaID = 0,
        Origin = new(0, 4),
        Dimensions = new Size(50, 4),
        Options = ["op 1", "op2", "opthree", "4"],
      }
    ]
  };

}

public class MenuDefinition
{
  public required string MenuID { get; init; }
  public Collection<MenuArea> Areas { get; init; } = [];
  public int SelectedArea { get; protected set; }
}

public abstract class MenuArea
{
  protected static void Out (string text, ConsoleColor textcolor, ConsoleColor backcolor)
  {
    Controller.SaveConsoleDataToStack();
    Console.ForegroundColor = textcolor;
    Console.BackgroundColor = backcolor;
    Console.Write(text);
    Controller.RestoreConsoleData();
  }

  public int AreaID { get; set; }
  public bool IsDirty { get; set; }
  public Point Origin;
  public Size Dimensions;
  public abstract void Draw ();
  public virtual bool KeyPress ()
  {
    IsDirty = true;
    return true;
  }
  public virtual void Redraw (EventArgs args)
  {
    if (!IsDirty)
      return;

    Clear();
    Draw();
  }
  public void Clear () => Controller.Clear(Origin, Dimensions);
}

public class MenuArea_Select : MenuArea
{
  private int _index = 0;
  public string[] Options { get; init; }
  public Action<EventArgs>[] SelectionOptions { get; set; }
  public override void Draw ()
  {
    int count_of_menu_items = 0;
    for (int line = Origin.Y; line < Dimensions.Height; line++)
    {
      if (_index + Origin.Y == line) //Selected
      {
        Out("(x) - " + Options[count_of_menu_items], ConsoleColor.Cyan, ConsoleColor.Black);
        Out("\n", ConsoleColor.Black, ConsoleColor.Black);
      }
      else
      {
        Out("( ) - " + Options[count_of_menu_items], ConsoleColor.Green, ConsoleColor.Black);
        Out("\n", ConsoleColor.Black, ConsoleColor.Black);
      }
      count_of_menu_items++;
    }
  }
  public override bool KeyPress ()
  {
    bool handled = false;

    // Key checking


    if (!handled)
      return base.KeyPress();
  }
}

public interface IState
{
  ConsoleColor BackColor { get; }
  ConsoleColor TextColor { get; }
  bool IsActive { get; }
  string? MenuID { get; }
  int AreaID { get; }
  Point Position { get; }
  string TypedData { get; }
  ConsoleKeyInfo? KeyInfo { get; }
  ConsoleKeyInfo? LastKeyInfo { get; }
  bool CursorVisible { get; }
}

public class MenuState : IState
{
  public ConsoleColor BackColor { get; set; }
  public ConsoleColor TextColor { get; set; }
  public bool IsActive { get; set; }
  public string? MenuID { get; set; }
  public int AreaID { get; set; } = -1;
  public Point Position { get; set; }
  public string TypedData { get; set; } = SE;
  public static MenuState InitialState => new()
  {
    AreaID = -1,
    Position = new(),
    IsActive = false,
    TypedData = SE,
    MenuID = SE,
    KeyInfo = null,
  };
  public ConsoleKeyInfo? KeyInfo { get; set; }
  public ConsoleKeyInfo? LastKeyInfo { get; set; }
  public bool CursorVisible { get; set; }

  public void BecomeStale () => KeyInfo = null;

  public static MenuState CopyFrom (IState cur) => new()
  {
    AreaID = cur.AreaID,
    BackColor = cur.BackColor,
    TextColor = cur.TextColor,
    CursorVisible = cur.CursorVisible,
    IsActive = cur.IsActive,
    KeyInfo = cur.KeyInfo,
    LastKeyInfo = cur.LastKeyInfo,
    MenuID = cur.MenuID,
    Position = cur.Position,
    TypedData = cur.TypedData
  };
}

public class CurrentState : IState
{
  public ConsoleColor TextColor { get => Console.ForegroundColor; set => Console.ForegroundColor = value; }
  public ConsoleColor BackColor { get => Console.BackgroundColor; set => Console.BackgroundColor = value; }
  public bool IsActive => Controller.IsActive;
  public string? MenuID => Controller.CurrentMenu?.MenuID;
  public int AreaID => Controller.CurrentMenu?.SelectedArea ?? -1;
  public Point Position
  {
    get => new(Console.CursorLeft, Console.CursorTop);
    set => Console.SetCursorPosition(value.X, value.Y);
  }
  public string TypedData { get; set; } = SE;
  public ConsoleKeyInfo? KeyInfo { get; set; }
  public ConsoleKeyInfo? LastKeyInfo { get; set; }
  public bool CursorVisible
  {
    get => Console.CursorVisible;
    set => Console.CursorVisible = value;
  }

  public void UpdateFrom (IState cur)
  {
    //AreaID = cur.AreaID,
    BackColor = cur.BackColor;
    TextColor = cur.TextColor;
    CursorVisible = cur.CursorVisible;
    //IsActive = cur.IsActive,
    KeyInfo = cur.KeyInfo;
    LastKeyInfo = cur.LastKeyInfo;
    //MenuID = cur.MenuID;
    Position = cur.Position;
    TypedData = cur.TypedData;
  }
}

public static class Controller
{
  public static CurrentState Current { get; set; } = new CurrentState();
  public static IState? Saved { get; set; } = MenuState.InitialState;
  private static string MaxWidthEmptyString
  {
    get
    {
      if (field is null || field.Length < Console.LargestWindowWidth)
      {
        string space = " ";
        for (int i = 1; i < Console.LargestWindowWidth; i++)
        {
          space += space;
        }
        field = space;
      }
      return field;
    }
  }
  private static string GetEmptyString (int length) => MaxWidthEmptyString[..length];
  private static Point CurrentPos
  {
    get => new(Console.CursorLeft, Console.CursorTop);
    set => Console.SetCursorPosition(value.X, value.Y);
  }
  private static bool CursorVisible
  {
    get => Console.CursorVisible;
    set => Console.CursorVisible = value;
  }
  internal static MenuDefinition? CurrentMenu { get; set; }
  public static bool IsActive { get; private set; }
  public static void SaveConsoleDataToStack () => Saved = MenuState.CopyFrom(Current);
  public static void RestoreConsoleData ()
  {
    if (Saved is not null)
      Current.UpdateFrom(Saved);
    else
      throw new InvalidOperationException();
    Saved = null;
  }

  public static void Clear (Point origin, Size dimensions)
  {
    SaveConsoleDataToStack();

    int line_start = origin.Y;
    int line_exclusive_end = origin.Y + dimensions.Height;
    int col_start = origin.X;
    int col_length = dimensions.Width;

    for (int line = line_start; line < line_exclusive_end; line++)
    {
      Console.SetCursorPosition(col_start, line);
      Console.Write(GetEmptyString(col_length));
    }

    RestoreConsoleData();
  }
  /// <summary>Clears the specified line from the console.</summary>
  /// <param name="line">The line to clear.</param>
  public static void Clear (int line)
  {
    SaveConsoleDataToStack();
    Console.SetCursorPosition(0, line);
    Console.Write(GetEmptyString(Console.WindowWidth));
    RestoreConsoleData();
  }
  /// <summary>Clears all the content from the console.</summary>
  public static void Clear () => Console.Clear();
  public static ConsoleColor TextColor { get => Console.ForegroundColor; set => Console.ForegroundColor = value; }
  public static ConsoleColor BackColor { get => Console.BackgroundColor; set => Console.BackgroundColor = value; }
}

/// <summary>The menu controller. Can be static since there is only one console.</summary>
public static class MenuController
{
  #region Public Members

  public static void StartMenu (string menu_name)
  {
    IsMenuActive = true;
    HideCursor();
    SetCurrentMenu(menu_name);
    Draw(MenuDrawType.None);
  }
  public static void StartMenu (MenuBase menu)
  {
    IsMenuActive = true;
    HideCursor();
    SetCurrentMenu(menu);
    while (IsMenuActive && !IsExiting)
    {
      Draw(MenuDrawType.None);
      Await();
    }
  }
  #endregion
  #region Internal Properties
  internal static bool IsExiting { get; set; }
  internal static bool IsMenuActive { get; set; }
  [MemberNotNullWhen(true, nameof(NextMenu))]
  internal static bool IsChangingMenu { get; set; }
  internal static bool HasMoved { get; set; }
  internal static string? NextMenu { get; set; }
  internal static ConsoleKeyInfo UserKey { get; set; }
  internal static MenuBase? ActiveMenu { get; set; }
  internal static MenuItem? SelectedMenuItem => ActiveMenu?.Items[ActiveMenu.Index];
  internal static int ActiveMenuItemCount => ActiveMenu?.Items.Count ?? 0;
  internal static int LineCount { get; set; }
  internal static int FirstItemLine { get; set; }
  internal static string? CurrentMenuName { get; set; }
  internal static IEnumerable<MenuBase> AllMenus { get; private set; } = [];
  internal static void AddNewMenu (MenuBase menu) => AllMenus = AllMenus.Concat([menu]);
  #endregion
  [MemberNotNull(nameof(ActiveMenu), nameof(CurrentMenuName))]
  internal static void SetCurrentMenu ([NotNull] MenuBase menu)
  {
    menu.ThrowIfNull();
    CurrentMenuName = menu.Name;
    ActiveMenu = menu;
    LineCount = ActiveMenuItemCount;
  }
  [MemberNotNull(nameof(ActiveMenu), nameof(CurrentMenuName))]
  internal static void SetCurrentMenu (string menu_name)
  {
    CurrentMenuName = menu_name;
    ActiveMenu = AllMenus.FirstOrDefault(m => m.Name == CurrentMenuName) ?? throw new ArgumentException($"menu_name: \"{menu_name}\" was not found.");
    FirstItemLine = ActiveMenu.Items.IsEmpty ? ErrVal : ActiveMenu.OptionLines.Start.Value;
    LineCount = ActiveMenuItemCount;
  }
  [MemberNotNull(nameof(NextMenu))]
  internal static void SetNextMenu (string next_menu)
  {
    MenuBase? next = AllMenus.FirstOrDefault(item => item.Name.Like(next_menu));
    next.ThrowIfNull($"No menu with name of '{next_menu}' found.");
    NextMenu = next_menu;
    IsChangingMenu = true;
  }
  internal static void HideCursor () => Console.CursorVisible = false;
  internal static void ShowCursor () => Console.CursorVisible = true;
  internal static void Draw (MenuDrawType type)
  {
    if (type is MenuDrawType.Erase)
    {
      goto MenuLoop;
    }
    if (type is MenuDrawType.Redraw)
    {
      IsChangingMenu = false;
    }

    ActiveMenu.ThrowIfNull();

    if (IsChangingMenu)
    {
      SetCurrentMenu(NextMenu);
      Draw(type: MenuDrawType.Redraw);
      return;
    }

  MenuLoop:

    for (int i = 0; i < ActiveMenuItemCount; i++)
    {
      if (type is MenuDrawType.Erase || ActiveMenu is null)
      {
        EraseLine(FirstItemLine + i, false);
      }
      else if (ActiveMenu.Items[i].IsDirty || type is MenuDrawType.Redraw)
      {
        ActiveMenu.Items[i].Draw(ActiveMenu.Index);
        ActiveMenu.Items[i].IsDirty = false;
      }
    }
  }
  internal static void EraseLine (int line, bool keep_original_pos)
  {
    int current_line = Console.CursorTop;
    int current_pos = Console.CursorLeft;
    Console.CursorTop = line;
    Console.CursorLeft = 0;
    string result = SE;
    int length = Console.WindowWidth;
    for (int i = 0; i < length; i++)
    {
      result += " ";
    }
    Console.WriteLine(result);
    Console.CursorTop = keep_original_pos ? current_line : line;
    Console.CursorLeft = keep_original_pos ? current_pos : 0;
  }
  internal static void CloseMenu ()
  {
    IsMenuActive = false;
    ShowCursor();
    CurrentMenuName = null;
    ActiveMenu = null;
  }
  internal static void Await ()
  {
    ActiveMenu.ThrowIfNull();

    Collection<MenuAction> all = ActiveMenu.AllActions;

    MenuAction? checkForValidKey (ConsoleKeyInfo key)
    {
      ConsoleKey ckey = key.Key;
      foreach (MenuAction data in all)
      {
        if (data.Key == ckey)
        {
          return data;
        }
      }
      return null;
    }
    MenuAction? checkForSelection ()
    {
      foreach (MenuAction data in all)
      {
        if (data.OnSelect)
        {
          return data;
        }
      }
      return null;
    }
    MenuAction? action;
    do
    {
      UserKey = Console.ReadKey();
      action = checkForValidKey(UserKey);
    }
    while (action is null);

    action.Execute();

    if (IsExiting)
      return;

    if (HasMoved && checkForSelection() is MenuAction a)
    {
      a.Execute();
    }

    return;
  }
}
