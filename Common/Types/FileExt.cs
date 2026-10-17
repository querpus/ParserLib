namespace Common.Types;

/// <summary>A definition for a file extension.</summary>
public class FileExt
{
  /// <summary>The file extension.</summary>
  public required string Ext { get; set; }
  public required string Class { get; set; }
  public string? ContentType { get; set; }
  public string? PerceivedType { get; set; }
  public string? Icon { get; set; }
}

public class ShellCommand
{
  public required string Verb { get; set; }
  public string? NUIVerb { get; set; }
  public required string PathToProgram { get; set; }
  public string CommandLineArgs { get; set; } = SE;
  public string? MultiSelectModel { get; set; } = SE;
  public int LaunchExplorerFlags { get; set; }
  public bool ProgrammaticAccessOnly { get; set; }
  public bool Extended { get; set; }
  public string? DelegateExecute { get; set; }
}

public class DropHandler
{
  public required string CLSID { get; set; }
}
public class DefaultHandler
{
  public required string CLSID { get; set; }
}

/// <summary>A definition for an object class.</summary>
public class FileClass
{
  public string? NiceName { get; set; }
  public required string Name { get; set; }
  public string? Icon { get; set; }
  public int DefaultDropEffect { get; set; }
  public string? CLSID { get; set; }
  public string? CurVer { get; set; }
  public int EditFlags { get; set; }
}
