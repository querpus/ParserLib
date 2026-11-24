global using System;
global using System.Collections.Generic;
global using System.Collections.ObjectModel;
global using System.Diagnostics.CodeAnalysis;
global using System.Linq;

global using Common.Extensions;

global using static Common.Names;

using System.Reflection;
using System.Resources;

[assembly: AssemblyCompany("Console.Menu")]
#if DEBUG
[assembly: AssemblyConfiguration("Debug")]
#elif RELEASE
[assembly: AssemblyConfiguration("Release")]
#endif
[assembly: AssemblyFlags(AssemblyNameFlags.None)]
[assembly: AssemblyProduct("Parser")]
[assembly: AssemblyTitle("Parser")]
[assembly: AssemblyVersion("1.0.0.0")]
[assembly: AssemblyFileVersion("1.0.0.0")]
[assembly: AssemblyInformationalVersion("1.0.0.0-Pre")]
[assembly: NeutralResourcesLanguage("en-US")]
