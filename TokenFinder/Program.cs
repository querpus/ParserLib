using System;
using System.Reflection;
using System.Windows.Forms;

using ParserDebuggerApp;

[assembly: AssemblyVersion("1.0.0")]

namespace TokenFinder;

internal static class Program
{
  [STAThread]
  internal static void Main ()
  {
    Application.EnableVisualStyles();
    Application.SetCompatibleTextRenderingDefault(false);
    Application.Run(new MainForm());
  }
}
