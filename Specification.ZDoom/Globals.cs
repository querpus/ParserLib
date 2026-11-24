global using System.Collections.Generic;
global using System.Collections.ObjectModel;
global using System.Diagnostics.CodeAnalysis;

global using Common.Extensions;

global using Parser;
global using Parser.Inference;
global using Parser.Ops;
global using Parser.Ops.Text;
global using Parser.Tokens;

global using static Common.Names;

global using RT = Parser.Tokens.TokenRuleType;
global using SS = System.Diagnostics.CodeAnalysis.StringSyntaxAttribute;

using System.Reflection;
using System.Resources;

[assembly: AssemblyCompany("Specification.ZDoom")]
#if DEBUG
[assembly: AssemblyConfiguration("Debug")]
#elif RELEASE
[assembly: AssemblyConfiguration("Release")]
#endif
[assembly: AssemblyFileVersion("1.0.0.2")]
[assembly: AssemblyInformationalVersion("1.0.0.2-Prerelease")]
[assembly: AssemblyProduct("Specification.ZDoom")]
[assembly: AssemblyTitle("Specification.ZDoom")]
[assembly: AssemblyVersion("1.0.0")]
[assembly: NeutralResourcesLanguage("en-US")]
