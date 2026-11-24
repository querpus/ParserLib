global using System;
global using System.Collections;
global using System.Collections.Generic;
global using System.Collections.Immutable;
global using System.Collections.ObjectModel;
global using System.Diagnostics.CodeAnalysis;
global using System.Globalization;
global using System.IO;
global using System.Linq;
global using System.Reflection;
global using System.Resources;
global using System.Text.RegularExpressions;

global using Common.Extensions;

global using static Common.Names;

global using ANEx = System.ArgumentNullException;
global using SS = System.Diagnostics.CodeAnalysis.StringSyntaxAttribute;

#if DEBUG
[assembly: AssemblyConfiguration("Debug")]
#else
[assembly: AssemblyConfiguration("Release")]
#endif

[assembly: AssemblyCompany("Magna AIM Systems")]
[assembly: AssemblyFileVersion("1.0.0.0")]
[assembly: AssemblyInformationalVersion("1.0.0")]
[assembly: AssemblyProduct("CommonLibrary")]
[assembly: AssemblyTitle("Common Library")]
[assembly: AssemblyVersion("1.0.0.0")]
[assembly: NeutralResourcesLanguage("en-US")]

