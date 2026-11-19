global using System;
global using System.Collections;
global using System.Collections.Generic;
global using System.Collections.ObjectModel;
global using System.Diagnostics.CodeAnalysis;
global using System.Linq;

global using Common;
global using Common.Extensions;
global using Common.RegExp;

global using Parser;
global using Parser.Ops.Text;

global using static Common.Names;

using System.Reflection;
using System.Resources;

[assembly: AssemblyCompany("Specification.INI")]
#if DEBUG
[assembly: AssemblyConfiguration("Debug")]
#elif RELEASE
[assembly: AssemblyConfiguration("Release")]
#endif
[assembly: AssemblyFileVersion("1.0.0.0")]
[assembly: AssemblyInformationalVersion("1.0.0.0-Prerelease")]
[assembly: AssemblyProduct("Specification.INI")]
[assembly: AssemblyTitle("Specification.INI")]
[assembly: AssemblyVersion("1.0.0.0")]
[assembly: NeutralResourcesLanguage("en-US")]
