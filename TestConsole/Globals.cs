global using System;
global using System.Collections.Generic;
global using System.Diagnostics.CodeAnalysis;
global using System.IO;
global using System.Reflection;
global using System.Resources;

global using Common;
global using Common.Extensions;

global using Parser;

global using static Common.Debug;
//global using Terminal.Menu;

global using static Common.Names;

[assembly: AssemblyCompany("Testing Console App")]
#if DEBUG
[assembly: AssemblyConfiguration("Debug")]
#elif RELEASE
[assembly: AssemblyConfiguration("Release")]
#endif
[assembly: AssemblyFlags(AssemblyNameFlags.None)]
[assembly: AssemblyProduct("Testing Console App")]
[assembly: AssemblyTitle("Testing Console App")]
[assembly: AssemblyVersion("1.0.0.0")]
[assembly: AssemblyFileVersion("1.0.0.0")]
[assembly: AssemblyInformationalVersion("1.0.0.0-Pre")]
[assembly: NeutralResourcesLanguage("en-US")]
