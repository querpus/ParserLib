global using System;
global using System.Collections.Generic;
global using System.Collections.ObjectModel;

global using Common;
global using Common.Extensions;
global using Common.RegExp;

global using Parser;
global using Parser.Ops;
global using Parser.Ops.Text;
global using Parser.Tokens;

global using static Common.Names;
global using static Parser.DefinitionStaticFunctions;

global using Debug = Common.Debug;

using System.Reflection;
using System.Resources;

[assembly: AssemblyCompany("Specification.JSON")]
#if DEBUG
[assembly: AssemblyConfiguration("Debug")]
#elif RELEASE
[assembly: AssemblyConfiguration("Release")]
#endif
[assembly: AssemblyFileVersion("1.0.0.0")]
[assembly: AssemblyInformationalVersion("1.0.0.0-Prerelease")]
[assembly: AssemblyProduct("Specification.JSON")]
[assembly: AssemblyTitle("Specification.JSON")]
[assembly: AssemblyVersion("1.0.0.0")]
[assembly: NeutralResourcesLanguage("en-US")]
