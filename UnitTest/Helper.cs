namespace UnitTest;

internal static class Helper
{
  internal const string GitDirWork = @"C:\Users\johntay4\source\repos\Git\";
  internal const string GitDirHome = @"D:\Git\";
  internal const string Default = @"ParserLib\Specification.INI\Samples\default.ini";
  public static string GitDir => (Directory.Exists(@"C:\Program Files (x86)\Steam\")) ? GitDirHome : GitDirWork;
}
