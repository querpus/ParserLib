using System;
using System.Reflection;
using System.Resources;
using System.Windows.Forms;

[assembly: AssemblyCompany("ParserVisualizer")]
[assembly: AssemblyConfiguration("Debug")]
[assembly: AssemblyFileVersion("1.0.0.0")]
[assembly: AssemblyInformationalVersion("1.0.0.0-Prerelease")]
[assembly: AssemblyProduct("ParserVisualizer")]
[assembly: AssemblyTitle("ParserVisualizer")]
[assembly: AssemblyVersion("1.0.0.0")]
[assembly: NeutralResourcesLanguage("en-US")]

namespace ParserVisualizer;

internal static class Program
{
  /// <summary>
  ///  The main entry point for the application.
  /// </summary>
  [STAThread]
  internal static void Main ()
  {
    // To customize application configuration such as set high DPI settings or default font,
    // see https://aka.ms/applicationconfiguration.
    ApplicationConfiguration.Initialize();
    ParserForm form = new();
    Application.Run(form);
    form.Dispose();
  }
}
