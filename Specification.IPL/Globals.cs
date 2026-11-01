global using System;
global using System.Collections.Generic;
global using System.Collections.ObjectModel;
global using System.Linq;
global using System.Text.RegularExpressions;

global using Common;
global using Common.RegExp;

global using Parser;

global using static Common.Debug;
global using static Common.Names;

global using ICT = Specification.IPL.IPLCommandType;
global using SS = System.Diagnostics.CodeAnalysis.StringSyntaxAttribute;

using System.Reflection;
using System.Resources;

[assembly: AssemblyCompany("Specification.IPL")]
#if DEBUG
[assembly: AssemblyConfiguration("Debug")]
#elif RELEASE
[assembly: AssemblyConfiguration("Release")]
#endif
[assembly: AssemblyFileVersion("1.0.0.0")]
[assembly: AssemblyInformationalVersion("1.0.0.0-Prerelease")]
[assembly: AssemblyProduct("Specification.IPL")]
[assembly: AssemblyTitle("Specification.IPL")]
[assembly: AssemblyVersion("1.0.0.0")]
[assembly: NeutralResourcesLanguage("en-US")]
