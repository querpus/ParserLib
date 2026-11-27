using System.Collections.ObjectModel;
using System.Linq;

using Parser.Exceptions;
using Parser.Nodes;

using IBasicToken = Parser.Nodes.IToken;
using ResIPL = Specification.IPL.Properties.Resources;
using Result = (Parser.OpStatus Status, Parser.DataStore Data);
using ResWAD = Specification.WAD.Properties.Resources;
using ResZDoom = Specification.ZDoom.Properties.Resources;
using SpecIPL = Specification.IPL.Definition;
using SpecJSON = Specification.JSON.Definition;
using SpecXML = Specification.XML.Definition;
using SpecZDoom = Specification.ZDoom.Definition;

namespace TestConsole;

internal static class Program
{
  #region Constants
  internal const string LaptopPath = @"C:\Users\johntay4\source\repos\Git";
  internal const string CheckPath = @"C:\Users\johntay4\Work";
  internal const string DesktopPath = @"D:\Git";
  internal const int LogLine = 10;
  internal const string This = nameof(Program);
  #endregion
  #region Fields
  internal static string? UserInput;
  #endregion
  #region Properties
  [AllowNull]
  internal static Library LibRef { get; set; }
  #endregion
  #region Basic Methods
  internal static void PressEnterToContinue ()
  {
    LogPart(MsgClass.BlueValue, "[");
    LogPart(MsgClass.Prompt, "Press enter to continue.");
    LogPart(MsgClass.BlueValue, "]");
    NewLine();
    _ = GetInput();
  }
  [MemberNotNull(nameof(UserInput))]
  internal static string GetInput () => UserInput = Console.ReadLine()?.ToUpperInvariant() ?? SE;
  internal static string FinishPath (string path) => Directory.Exists(CheckPath) ? $@"{LaptopPath}\{path}" : $@"{DesktopPath}\{path}";
  #endregion
  #region Menu Definition
  internal static Action<IList<object>> DoTest => item => _ = item[0] is string s && item[1] is Spec sp ? TestTextParser(s, sp) : throw new InvalidOperationException("Invalid data passed to TestTextParser");
  // MenuItem 2 "Quit"
  //internal static MenuItem Exit = new(2, "Quit", [new MenuQuitAction() { Key = CK.Enter }]);
  //internal static MenuItem LoadItem = new(0, "Load", []);
  //internal static MenuItem Test = new(1, "Test", [new TestAction() { Key = CK.Enter }]);
  //internal static MenuBase InitialMenu = new BasicMenu()
  //{
  //  Name = "main",
  //  Items = [Exit, Test],
  // CommonActions = { }
  // };
  #endregion

  [STAThread]
  internal static int Main (string[] args)
  {
    DebugIn("Program", "Main");
#if DEBUG
    Verbosity = LogClass.Debug;
#else
    Verbosity = LogClass.Verbose;
#endif
    LibRef = Library.InitializeLibrary(AppDomain.CurrentDomain);

    if (args.Length == 0)
    {
      Log(MsgClass.Warning, "No files specified.", nameof(Program));
    }
    else
    {
      ProcessArgs(args);
    }

    TestSelectionLoop();

    Log(MsgClass.Prompt, "Press enter to exit.", nameof(Program));
    _ = Console.ReadLine();
    return 0;
  }
  internal static void TestSelectionLoop ()
  {
    string file_xsd_specification = File.ReadAllText(FinishPath(Paths.xsd_specification));
    string file_xml_operation = File.ReadAllText(FinishPath(Paths.xml_operation));
    string file_json_launch = File.ReadAllText(FinishPath(Paths.json_launch));
    string file_xml_errors = File.ReadAllText(FinishPath(Paths.xml_errors));
    string file_ipl_label = File.ReadAllText(FinishPath(ResIPL.ipl_label));
    // TODO: Fix the ini in the wrong module.
    string file_ini_vncdefault = File.ReadAllText(FinishPath(ResIPL.ini_vncdefault));

    static void generic_spec<TTokenizer> (string file, Spec rule_spec) where TTokenizer : ITokenizer<IBasicToken>, new()
    {
      if (rule_spec.NodeRuleSet is null || rule_spec.TokenRuleSet is null)
      {
        Log(MsgClass.Error, "Invalid Spec - Cannot use for capture parser", This);
        return;
      }
      TTokenizer cTokenizer = new() { Rules = rule_spec.TokenRuleSet };
      IEnumerable<IBasicToken> cTokens = cTokenizer.Tokenize(file);
      CaptureTokenProcessor cProcessor = new(cTokens, rule_spec.NodeRuleSet);
      cProcessor.Process();
      cProcessor.Context?.Print(0);
      PressEnterToContinue();
    }

  Start:
    Console.Clear();
    Log(MsgClass.GreenInfo, $"Library contains: {LibRef.Values.Select(s => s.Name).TextJoin(", ")}", This);
    Log(MsgClass.Prompt, "Select a test. (wad/xml/mapinfo/acs/ini/json/ipl)", This);
    switch (GetInput())
    {
      case "XML":
        Log(MsgClass.Prompt, "Select a version. (capture/group/match/xparser)", This);
        switch (GetInput())
        {
          case "CAPTURE":
            generic_spec<CaptureTokenizer>(file_xsd_specification, SpecXML.Spec);
            generic_spec<CaptureTokenizer>(file_xml_operation, SpecXML.Spec);
            generic_spec<CaptureTokenizer>(file_xml_errors, SpecXML.Spec);
            goto Prompt;
          case "GROUP":
            generic_spec<GroupTokenizer>(file_xsd_specification, SpecXML.Spec);
            generic_spec<GroupTokenizer>(file_xml_operation, SpecXML.Spec);
            generic_spec<GroupTokenizer>(file_xml_errors, SpecXML.Spec);
            goto Prompt;
          case "XPARSER":
            InitialTest(LibRef["xml"], file_xml_operation, use_file_as_text_input: true);
            InitialTest(SpecXML.Spec, file_xml_errors, use_file_as_text_input: true);
            InitialTest(SpecXML.Spec, ResZDoom.xml_acs);
            goto Prompt;
          case "MATCH":
            MatchTokenizer mTokenizer = new() { Rules = StandardRuleSets.XMLTokenRuleSet };
            Collection<MatchToken> mTokens = mTokenizer.Tokenize(file_xsd_specification);
            mTokens.Foreach(t => Log(MsgClass.GreenInfo, t.ToString(true), This));
            goto Prompt;
          default:
            Log(MsgClass.Error, "Invalid choice.", This);
            goto Prompt;
          Prompt:
            PressEnterToContinue();
            break;
        }
        break;
      case "JSON":
        Log(MsgClass.Prompt, "Select a version. (capture/xparser)", This);
        switch (GetInput())
        {
          case "CAPTURE":
            generic_spec<CaptureTokenizer>(file_json_launch, SpecJSON.Spec);
            break;
          case "XPARSER":
            InitialTest(LibRef["json"], Paths.json_error);
            InitialTest(SpecJSON.Spec, Paths.json_launch);
            break;
          default:
            Log(MsgClass.Error, "Invalid choice.", This);
            break;
        }
        break;
      case "WAD":
        InitialTest("wad", ResWAD.wad_rpg03);
        InitialTest("wad", ResWAD.wad_pl2);
        InitialTest("wad", ResWAD.wad_tnt);
        break;
      case "MAPINFO":
        InitialTest(SpecZDoom.MapInfo, ResZDoom.mapinfo_common);
        break;
      case "DECORATE":
        InitialTest(SpecZDoom.Decorate, ResZDoom.dec_mkii);
        break;
      case "ACS":
        InitialTest(SpecZDoom.ACS, ResZDoom.acs_rpglevel);
        InitialTest(SpecZDoom.ACS, ResZDoom.acs_rpgmfunc);
        InitialTest(SpecZDoom.ACS, ResZDoom.acs_foolib);
        InitialTest(SpecZDoom.ACS, ResZDoom.acs_sample);
        break;
      case "SNDINFO":
        InitialTest(SpecZDoom.SndInfo, ResZDoom.sndinfo_test);
        break;
      case "UDMF":
        InitialTest(SpecZDoom.UDMF, ResZDoom.udmf_sample);
        InitialTest(SpecZDoom.UDMF, ResZDoom.udmf_map00);
        break;
      case "INI":
        InitialTest(LibRef["ini"], file_ini_vncdefault, use_file_as_text_input: true);
        break;
      case "ZS" or "ZSCRIPT":
        InitialTest(SpecZDoom.ZScript, ResZDoom.zs_demon);
        break;
      case "IPL":
        Log(MsgClass.Prompt, "Select a version. (capture/xparser)", This);
        switch (GetInput())
        {
          case "CAPTURE":
            generic_spec<CaptureTokenizer>(file_ipl_label, SpecIPL.Spec);
            break;
          case "XPARSER":
            InitialTest(SpecIPL.Spec, ResIPL.ipl_simple);
            InitialTest(SpecIPL.Spec, ResIPL.ipl_label);
            break;
          default:
            Log(MsgClass.Error, "Invalid choice.", This);
            break;
        }
        break;
      case "EXIT" or "QUIT":
        Environment.Exit(0);
        throw new QuitException();
      case "":
        Log(MsgClass.Warning, "Nothing selected, enter a test, or type exit or quit to exit the program.", This);
        break;
      default:
        Log(MsgClass.Warning, "Unknown test.", This);
        break;
    }
    Log(MsgClass.Prompt, "Press enter to return to the test selection.", This);
    _ = GetInput();
    goto Start;
  }
  internal static void ProcessArgs (string[] args)
  {
    OpStatus status;
    DataStore data;
    foreach (string path in args)
    {
      string? specName = LibRef.CheckFile(path);
      Spec? spec = LibRef.Lookup(specName);

      while (spec is null)
      {
        Log(MsgClass.Warning, $"Spec {specName} not found. Enter a valid Spec name", This);
        specName = GetInput();
        spec = LibRef.Lookup(specName);
      }

      (status, data) = XParser.ParseFile(spec, path);

      Log(MsgClass.Debug, "OpStatus is " + status, This);
      Log(MsgClass.Debug, "Result count = " + data.Count, This);
    }
  }

  internal static void InitialTest (string spec, string file)
  {
    DebugIn("Program", "InitialTest");
    if (!LibRef.TryLookup(spec, out Spec? lookup_spec))
    {
      Log(MsgClass.Error, $"Cannot start test of '{Path.GetFileName(file)}' with '{spec}', the spec was not found.", This);
    }
    else
    {
      InitialTest(lookup_spec, file);
    }
    DebugOut();
  }
  internal static Result StepAndOutput<T> (T data)
  {
    XParser parser = new();
    parser.SetInput(data);
    int i = 0;
    OpStatus result = OpStatus.AtStart;
    foreach (var os in parser.StepThrough())
    {
      result = os;
      Log(MsgClass.Warning, "The operation " + parser.ParsedOperations[i++] + "resulted in " + os, This);
      parser.QueryOperation();
    }
    return (result, parser.Data);
  }
  internal static void InitialTest (Spec spec, string file, bool use_file_as_text_input = false)
  {
    ClearLog();
    if (use_file_as_text_input)
      Log(MsgClass.Warning, $"Starting test, using text data with '{spec.Name}'.", This);
    else
      Log(MsgClass.Warning, $"Starting test of '{Path.GetFileName(file)}' with '{spec.Name}'.", This);
    XParser parser = new(spec);
    OpStatus status = OpStatus.AtStart;

    if (spec.IsTextFile && !use_file_as_text_input)
    {
      try
      {
        string data = File.ReadAllText(FinishPath(file));
        status = StepAndOutput(data);
      }
      catch (FileNotFoundException)
      {
        Log(MsgClass.Error, "The path at \"" + FinishPath(file) + "\" is not valid.", This);
      }
    }
    else if (!use_file_as_text_input)
    {
      byte[] data;
      try
      {
        data = File.ReadAllBytes(FinishPath(file));
      }
      catch (FileNotFoundException)
      {
        Log(MsgClass.Error, $"File {file} not found.", This);
        data = [];
      }

      status = StepAndOutput(data);
    }
    else if (spec.IsTextFile && use_file_as_text_input)
    {
      status = StepAndOutput(file);
    }
    else
    {
      Log(MsgClass.Error, "Error: The spec provided is not a text file, but text input was provided.", This);
      return;
    }
    Log(MsgClass.Warning, $"Operations have concluded with status {status}.", This);
  }

  internal static XParser TestTextParser (string path, Spec spec)
  {
    OpStatus status;
    DebugIn("Program", "TestTextParser");
    if (spec.IsTextFile)
    {
      string content = File.ReadAllText(path);
      Parser = new(spec);
      status = StepAndOutput(content);
    }
    else
    {
      Memory<byte> bytes = File.ReadAllBytes(path);
      Parser = new(spec);
      status = StepAndOutput(bytes);
    }

    Log(MsgClass.BlueInfo, $"The {spec.Name} test resulted in {status}.", This);
    DebugOut();
    return Parser;
  }
}
