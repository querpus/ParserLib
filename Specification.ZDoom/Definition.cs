#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable IDE1006 // Naming Rule Violation

using static Parser.DefinitionStaticFunctions;

namespace Specification.ZDoom;

[DefinitionExport]
public static class Definition
{
  // Common Rules (No Backtracking, Very Efficient Atomic Groups)
  private static readonly TokenRule s_cLineComment = new(RT.TokenComment, "None", @"(?>\/\/[^\n\r\v]*)");
  private static readonly TokenRule s_cBlkComment = new(RT.TokenComment, "None", @"(?>\/\*(?>[^*]|\*[^\/])*\*\/)");
  private static readonly TokenRule s_cString = new(RT.Competitive, "String", @"(?>""(?>[^""\\]|\\"")*"")");
  private static readonly TokenRule s_char = new(RT.Competitive, "Char", @"(?>'(?>[^'\\]|\\')*')");
  private static readonly TokenRule s_int = new(RT.TokenMatch, "Int", @"\b(?>-?\d+)|0x(?>[0-9a-f]{1,8})\b");
  private static readonly TokenRule s_dec = new(RT.TokenMatch, "Dec", @"\b(?>-?(?>\d+(?>\.\d*)?|\.\d+))\b");
  private static readonly TokenRule s_langref = new(RT.Competitive, "LangRef", @"(?>""\$\w+"")");
  private static readonly TokenRule s_classname = new(RT.TokenMatch, "Classname", "Actor|Ammo|Clip|(Red|Blue|Yellow)Card|Health|Armor(Bonus)?|(Blue|Green)Armor|(Caco|Cyber)?demon|Imp|Shells|Rocket(Box)?|(Custom)?Inventory|FastProjectile|DoomPlayer|MapSpot|DoomImp|Zombieman|ShotgunGuy");
  private static readonly TokenRule s_name = new(RT.TokenMatch, "Name", @"[\w]+");
  private static readonly TokenRule s_nameWithDot = new(RT.TokenMatch, "NameWithDot", @"\w+(\.\w+)+");
  private static readonly TokenRule[] s_op_pba = TokenRule.MakeSingleCharRules("[]{}()", RT.TokenExact, "Ao", "Ac", "Bo", "Bc", "Po", "Pc");

  /// <summary>https://regex101.com/r/En5C8c/7</summary>
  [DefinitionExport]
  public static Spec ZScript => new()
  {
    FileInferences = [],
    RxOpt = ROML | ROIPW | ROIC | ROEC,
    Name = "zdoom.zscript",
    Operations = [
      new TokenizeOperation { InputKey = "text", OutputKey = "tokens", Tokenizer = "xparser" }
    ],
    IsTextFile = true,
    SC = SCOIC,
    TokenCompatLookup = new()
    {
      ["StateCmd"] = ["GotoCmd", "LoopCmd", "BasicCmd"],
      ["StateEntry"] = ["StateCmd", "State", "FrameDef"]
    },
    TokenType = typeof(string),
    DefaultRuleSet = RT.IgnoreCase,
    TokenRules = [
      s_cLineComment,
      s_cBlkComment,
      s_langref,
      s_cString,
      new(RT.TokenMatch, "FlagName", @"(?<=\+|\-)[\w.]+"),
      .. TokenRule.MakeSingleCharRules("{}();=,:+-", RT.TokenExact, "Bo", "Bc", "Po", "Pc", "Sc", "Eq", "Cm", "Co", "Pl", "Mn"),
      s_int,
      s_dec,
      .. TokenRule.MakeWordMatchRules(true, [
        "projectile", "monster",
        "native","const",
        "int", "void", "class",
        "if", "switch", "while",
        "extends","mixin","replaces",
        "let",
        "fail","wait","goto","stop",
        "fast","bright",
        "states","default",

      ]),
      s_nameWithDot,
      s_classname,
      s_name
    ],
    GroupTokenRules = [
      new("ClassHeader_Replaces", "x:Replaces v:(Name|String)"),
      new("ClassHeader", "x:class n:(Name|String) x:Co t:(Name|String) lo:ClassHeader_Replaces ro:Int"),
      new("ClassHeader", "x:class n:(Name|String) lo:ClassHeader_Replaces ro:Int"),

      new("FrameDef", "n:(Name|String) v:Name v:Num fo:Bright x:Sc"),
      new("AddFlag", "f:Pl n:FlagName x:Sc"),
      new("SubFlag", "s:Mn n:FlagName x:Sc"),

      new("State", "n:Name|NameWithDot x:Co"),

      new("StatesBlock_Open", "x:States x:Bo"),
      new("StateBlock", "x:StatesBlockOpen sa:StateEntry x:Bc")
      ]
  };

  [DefinitionExport]
  public static Spec UDMF => new()
  {
    Name = "zdoom.udmf",
    FileInferences = [IfN(InferenceType.Ext | InferenceType.Like, "udmf")],
    RxOpt = ROML | ROIPW | ROIC | ROEC | ROSL,
    Operations = [
      new TokenizeOperation{ InputKey = "text", OutputKey = "tokens", Tokenizer = "xparser" },
      new TokenAssembleOperation { InputKey = "tokens", OutputKey = "tokens_assembled" },
    ],
    DefaultRuleSet = RT.IgnoreCase,
    TokenRules = [
      s_cString,
      s_cLineComment,
      s_cBlkComment,
      .. TokenRule.MakeSingleCharRules("{}=;", RT.TokenExact, "Bo", "Bc", "Eq", "Sc"),
      new(RT.TokenMatch, "Bool",  @"\b(true|false)\b"),
      s_dec,
      .. TokenRule.MakeWordMatchRules(true, "Namespace", "Vertex", "Thing", "SideDef", "LineDef"),
      new(RT.TokenMatch, "Sector", @"\bsector\b(?=[^=}]*\{)"),
      new(RT.TokenMatch, "Name", @"\b[a-z]\w*\b"),
      new(RT.StoreExtra | RT.IgnoredToken, "None", @"\s+"),
      new(RT.StoreOther, "Value", SE)],
    GroupTokenRules = [
      new("NamespaceDec", "n:Namespace x:Eq v:String x:Sc"),
      new("Property", "n:Name x:Eq v:Value x:Sc"),
      new("Structure", "n:Vertex x:Bo pm:Property x:Bc"),
      new("Structure", "n:Thing x:Bo pm:Property x:Bc"),
      new("Structure", "n:Sector x:Bo pm:Property x:Bc"),
      new("Structure", "n:LineDef x:Bo pm:Property x:Bc"),
      new("Structure", "n:SideDef x:Bo pm:Property x:Bc"),
      ],
    SC = SCOIC,
    IsTextFile = true,
    TokenType = typeof(string),
    TokenCompatLookup = new Dictionary<string, Collection<string>>()
    {
      ["Keyword"] = ["Vertex", "Thing", "Sector", "LineDef", "SideDef"],
      ["Op"] = ["Eq", "Sc", "Bo", "Bc"],
      ["Value"] = ["Bool", "Dec", "String"],
    },
  };
  /// <summary>This is the specification for the SndInfo ZDoom lump.</summary>
  [DefinitionExport]
  public static Spec SndInfo => new()
  {
    Name = "zdoom.sndinfo",
    RxOpt = ROML | ROIPW | ROIC | ROEC,
    FileInferences = [
      IfNOr(
        IfN(ExtIs, "sndinfo"),
        IfN(FName | Is, "sndinfo")
      )
    ],
    IsTextFile = true,
    TokenType = typeof(string),
    SC = SCOIC,
    TokenRules = [
      s_cString,
      s_cLineComment,
      s_cBlkComment,
    ],
    GroupTokenRules = [],
    Operations = [
      new TokenizeOperation{ InputKey = "text", OutputKey = "tokens" , Tokenizer = "xparser" },
      new TokenAssembleOperation { InputKey = "tokens", OutputKey = "tokens_assembled" },
    ],
  };

  /// <summary>Defines a mapinfo Spec. <see href="https://regex101.com/r/iWWPub/1">Regex</see></summary>
  [DefinitionExport]
  public static Spec MapInfo { get; } = new()
  {
    DefaultRuleSet = RT.IgnoreCase,
    Name = "zdoom.mapinfo",
    FileInferences = [
      IfN(ExtIs, "mapinfo"),
      IfN(ExtIs, "zmapinfo"),
      IfN(FName | Is, "mapinfo"),
      IfN(FName | Is, "zmapinfo")],
    Operations = [
      new TokenizeOperation { InputKey = "text", OutputKey = "tokens", Tokenizer = "xparser" },
      new DebugPrintKeyOperation { InputKey = "tokens" },
      new TokenAssembleOperation { InputKey = "tokens", OutputKey = "tokens_assembled" },
      new DebugPrintKeyOperation { InputKey = "tokens_assembled" },
    ],
    RxOpt = ROIC | ROEC | ROML,
    IsTextFile = true,
    TokenType = typeof(string),
    TokenRules = [
      s_langref,
      s_cString,
      s_cLineComment,
      s_cBlkComment,
      .. TokenRule.MakeSingleCharRules("{}()=;,", RT.TokenExact, "Op"),
      .. TokenRule.MakeWordMatchRules(true,
        "Doomednums", "AddDefaultMap", "GameInfo",
        "Skill", "Map", "DamageType", "Episode",
        "Cluster", "Include", "Intermission",
        "Cast", "Fader", "GotoTitle", "Image",
        "Scroller", "TextScreen", "Wiper", "Cutscene"),
      new (RT.TokenMatch, "PropertyName", @"\b(Background2?|Draw(Conditional)?|Music|Sound|Time|Cast(Class|Name)|AttackSound|FadeType|InitialDelay|Scroll(Direction|Time)|WipeType)\b | \b(\d+|\w+)\b(?=\s*=)"),
      s_int,
      s_dec,
      new (RT.TokenMatch, "Name", @"\b\w+\b"),
    ],
    GroupTokenRules = [
      new ("Property", "n:PropertyName x:Op{=} v:Value xo:Op{,} vo:Value xo:Op{,} vo:value")
    ],
    SC = SCOIC,
    TokenCompatLookup = {
      ["Value"] = ["Int", "Dec", "String", "Char", "Bool", "Name"],
      ["String"] = ["LangRef"],
      ["Dec"] = ["Int"]
    },
  };

  [DefinitionExport]
  public static Spec Decorate => new()
  {
    FileInferences = [],
    RxOpt = ROML | ROIPW | ROIC | ROEC,
    IsTextFile = true,
    SC = SCOIC,
    TokenType = typeof(string),
    Name = "zdoom.decorate",
    Operations = [
      new TokenizeOperation{ InputKey = "text", OutputKey = "tokens" , Tokenizer = "xparser" },
      new DebugPrintKeyOperation { InputKey = "tokens" },
      new TokenAssembleOperation { InputKey = "tokens", OutputKey = "tokens_assembled" },
      new DebugPrintKeyOperation { InputKey = "tokens_assembled" },
    ],
    TokenRules = [
      s_cString,
      s_cLineComment,
      s_cBlkComment,
      /* Keywords */
      .. TokenRule.MakeWordMatchRules(true, "const", "include", "states", "actor", "int", "replaces", "native"),
      /* State Commands */
      .. TokenRule.MakeWordMatchRules(true, "stop", "fail", "wait", "loop", "goto"),
      /* State Options */
      .. TokenRule.MakeWordMatchRules(true, "bright"),
      /* Symbols */
      .. s_op_pba,
      .. TokenRule.MakeSingleCharRules("=+-*/", RT.TokenExact, "Eq", "Pl", "Mi", "Mu", "Sl"),
      new(RT.TokenMatch, "StateName", @"\b[\w\.-]+(?=\:)"),
      new(RT.TokenMatch, "FlagName", @"(?<=[+-])[\w.]+\b"),
      new(RT.TokenMatch, "Sprite_FrameDef", @"^(?<=\s*)""?[\w\/\\\?]{4}""?"),
      new(RT.TokenMatch, "Frame_FrameDef", @"(?<=^\s*""?\w{4}""?)\s+[a-z]+" ),

      s_langref,
      s_nameWithDot,
      s_classname,
      s_name,
      s_int,
      s_dec,
    ],
    GroupTokenRules = [
      new("ClassHeader_Replaces", "x:Replaces v:(Name|String)"),
      new("ClassHeader", "x:actor n:(Name|String) x:Co t:(Name|String) lo:ClassHeader_Replaces ro:Int"),
      new("ClassHeader", "x:actor n:(Name|String) lo:ClassHeader_Replaces ro:Int"),

      new("FrameDef", "n:(Name|String) v:Name v:Num fo:Bright"),
      new("AddFlag", "f:Pl n:FlagName"),
      new("SubFlag", "s:Mn n:FlagName"),

      new("State", "n:Name|NameWithDot x:Co"),

      new("StatesBlock", "x:States x:Bo sa:StateEntry x:Bc")
    ]
  };

  internal static TokenRule Tm (string tokenType, [SS("regex")] string regex) => new(RT.TokenMatch, tokenType, regex);
  internal static readonly string[] op_token_types = ["Eq", "Cm", "Co", "Sc", "Pre", "Minus"];
  /// <summary>Defined Specification</summary>
  /// <remarks><see href="https://regex101.com/r/bNaEDc/1">Regex for Tokens</see></remarks>
  [DefinitionExport]
  public static Spec ACS => new()
  {
    FileInferences = [IfN(ExtIs, "acs")],
    Name = "zdoom.acs",
    RxOpt = ROML | ROIC | ROIPW | ROEC | ROSL | ROCI,
    Operations = [
      new TokenizeOperation{ InputKey = "text", OutputKey = "tokens" , Tokenizer = "xparser" },
      new TokenAssembleOperation { InputKey = "tokens", OutputKey = "tokens_assembled" },
      new DebugPrintKeyOperation { InputKey = "tokens_assembled" }
    ],
    IsTextFile = true,
    SC = SCOIC,
    TokenType = typeof(string),
    DefaultRuleSet = RT.IgnoreCase,
    TokenCompatLookup = {
      ["Value"]     = ["Int", "Char", "String", "Dec", "Expression", "ExprName", "FunctionCall", "ArrayValue", "ExpressionStandalone"],
      ["Stmt"]      = ["VarDecl", "BasicCmd", "FunctionCallStmt", "VarAssn", "VarInc", "ArrayDecl", "WaitStmt", "VarDeclAssn"],
      ["FuncStmt"]  = ["VarDecl", "BasicCmd", "FunctionCallStmt", "VarAssn", "VarInc", "ArrayDecl", "VarDeclAssn", "ReturnStmt"],
      ["Block"]     = ["IfBlock", "ElseBlock", "ElseIfBlock", "LoopBlock", "SwitchBlock", "ForBlock"],
      ["MapVar"]    = ["global", "world"],
      ["Loop"]      = ["until", "while"],
      ["Wait"]      = ["delay", "tagwait", "scriptwait", "polywait", "NamedScriptWait", "ScriptCallWaitStmt"],
      ["Name"]      = ["PreProcName", "ExprName", "FuncName", "FuncDefName", "ArrVarName", "VarName", "ParamName", "DefineName"],
      ["Literal"]   = ["Int", "String", "Char", "Dec"],
      ["SimpleJump"] = ["break", "continue", "terminate", "restart"]
    },
    TokenRules = [

      // Strings and Comments
      s_cString,
      s_char,
      s_cLineComment,
      s_cBlkComment,

      // Numeric Data
      s_int,
      s_dec,

      // Keywords
      .. TokenRule.MakeWordMatchRules(true, [
        "script", "function", "net", "if", "else",
        "do", "for", "switch", "case", "default", "return",
        "while", "until", "world", "global", "void", "terminate",
        "break", "continue", "terminate", "restart"
      ]),

      // Wait Functions
      .. TokenRule.MakeWordMatchRules(true, [
        "scriptwait", "delay", "polywait", "tagwait", "ACS_ExecuteWait", "namedscriptwait", "ACS_NamedExecuteWait"
      ]),

      // Script Functions
      Tm("ScriptFunc", @"\b(ACS_(Named)?Execute\w*)\b"),
      Tm("ScriptType", @"\b(enter|re(open|spawn)|death|kill|open|unloading|disconnect|lightning)\b"),

      // Operators
      Tm("IncDec", @"(\+\+|--)"),
      Tm("Unary", "!(?!=)|~"),
      Tm("Assign", "[-+*^/%|&]="),
      Tm("Assign", @"(<<|>>| \|\| |&&)="),
      Tm("Binary", "== | [!<>]="),
      Tm("Binary", @"(&&| \|\| |<<|>>)(?!=)"),
      .. TokenRule.MakeSingleCharRules("+/%|&^*><", RT.TokenExact , "Binary"),
      .. s_op_pba,
      .. TokenRule.MakeSingleCharRules("=,:;#-", RT.TokenExact , [.. op_token_types]),

      // Data Types
      Tm("Type", @"\b(int|str|char|bool)\b"),

      // Names
      Tm("FuncDefName",   @"(?<= \bfunction\s*\w+\s*) (?>[a-z_]\w*)"),
      Tm("FuncName",      @"(?>[a-z_]\w*) (?=\s*\()"),
      Tm("VarName",       @"(?<= \b(int|str|bool|char) \s+ ( \w+\s*\,\s* )*) (?>[a-z_]\w*) (?!\s*\(|\s*,\s*(int|bool|str|char)) (?=\s*(;|\=|,))"),
      Tm("ArrVarName",    @"(?<= \b(int|str|bool|char) \s+) (?>[a-z_]\w*) (?=\s*\[)"),
      Tm("ParamName",     @"(?<= \b(int|str|bool|char) \s+) (?>[a-z_]\w*) (?!\s*\()"),
      Tm("DefineName",    @"(?<= \#\w+\s+) (?>[a-z_]\w*)"),
      Tm("PreProcName",   @"(?<= \#) (?>[a-z]+)"),
      Tm("ExprName",      @"\b[a-z_]\w*\b"),
      new TokenRule(RT.StoreExtra | RT.IgnoredToken, "ws", "\\s+"),
      new TokenRule(RT.StoreOther, typeToAssign: "unmatched", SE)
    ],
    GroupTokenRules = [
      // Parameter Expressions
      new(              "ParamDef",               "t:Type n:ParamName xo:Cm"),
      new(              "PrintParameterValue",    "t:Name{s|i} x:Co qa:Value xo:Cm"),

      // Expressions
      new(RT.Recursive, "ArrayDim",               "x:Ao v:Value x:Ac"),
      new(RT.Recursive, "ArrayValue",             "n:ExprName vm:ArrayDim"),
      new(RT.Recursive, "Expression",             "l:Value c:(Binary|Minus) r:Value"),
      new(RT.Recursive, "Expression",             "c:(Unary|Minus) r:Value"),
      new(RT.Recursive, "ExpressionStandalone",   "l:Value c:IncDec"),
      new(RT.Recursive, "ExpressionStandalone",   "c:IncDec r:Value"),
      new(RT.Recursive, "FunctionCall",           "n:FuncName x:Po q:PrintParameterValue xo:Cm qo:PrintParameterValue x:Pc"),
      new(RT.Recursive, "FunctionCall",           "n:FuncName x:Po q:Value x:Pc"),
      new(RT.Recursive, "FunctionCall",           "n:FuncName x:Po q:Value xo:Cm qo:Value xo:Cm qo:Value xo:Cm qo:Value xo:Cm qo:Value xo:Cm qo:Value x:Pc"),
      new(RT.Recursive, "FunctionCall",           "n:FuncName x:Po x:Pc"),

      new(              "ScriptCallStmt",         "n:ScriptFunc x:Po q:Value x:Cm q:Value xa:Cm qa:Value xa:Cm qa:Value xa:Cm qa:Value x:Pc x:Sc"),

      new(              "Preprocessor",           "x:Pre ti:PreProcName{(lib)?define} n:DefineName v:Value"),
      new(              "Preprocessor",           "x:Pre ti:PreProcName{i(mport|nclude)|library} v:String"),

      // Statements
      new(              "VarDecl",                "t:Type n:VarName x:Sc"),
      new(              "VarDeclAssn",            "t:Type n:VarName x:Eq v:Value x:Sc"),
      new(              "VarAssn",                "l:(ArrayValue|ExprName) c:(Eq|Assign) r:Value x:Sc"),
      new(              "ArrayDecl",              "t:Type n:ArrVarName vm:ArrayDim x:Sc"),
      new(              "BasicCmd",               "n:SimpleJump x:Sc"),
      new(              "ReturnStmt",             "n:Return x:Sc"),
      new(              "ReturnStmt",             "n:Return v:Value x:Sc"),
      new(              "WaitStmt",               "t:Wait x:Po p:Value x:Pc x:Sc"),
      new(              "FunctionCallStmt",       "d:FunctionCall x:Sc"),
      new(              "CaseLabel",              "x:Case n:Value x:Co"),
      new(              "CaseLabel",              "n:Default x:Co"),

      new(RT.Recursive, "IfBlock",                "x:If x:Po v:Value x:Pc x:Bo sa:(Stmt|Block) x:Bc"),
      new(RT.Recursive, "IfBlock",                "x:If x:Po v:Value x:Pc s:(Stmt|Block)"),
      new(RT.Recursive, "LoopBlock",              "t:Loop x:Po v:Value x:Pc x:Bo sa:(Stmt|Block) x:Bc"),
      new(RT.Recursive, "LoopBlock",              "t:Loop x:Po v:Value x:Pc s:(Stmt|Block)"),
      new(RT.Recursive, "ElseIfBlock",            "x:Else x:If x:Po v:Value x:Pc x:Bo sa:(Stmt|Block) x:Bc"),
      new(RT.Recursive, "ElseIfBlock",            "x:Else x:If x:Po v:Value x:Pc s:(Stmt|Block)"),
      new(RT.Recursive, "ElseBlock",              "x:Else x:Bo sa:(Stmt|Block) x:Bc"),
      new(RT.Recursive, "ElseBlock",              "x:Else s:(Stmt|Block)"),
      new(RT.Recursive, "SwitchBlock",            "x:Switch x:Po v:Value x:Pc x:Bo sa:(CaseLabel|Stmt|Block) x:Bc"),
      new(RT.Recursive, "ForHeader",              "x:For x:Po qo:(VarAssn|VarDeclAssn) x:Sc qo:Value x:Sc qo:Value x:Pc"),
      new(RT.Recursive, "ForBlock",               "d:ForHeader x:Bo sa:(Stmt|Block) x:Bc"),

      new(              "FunctionHeader",         "x:Function t:(Type|Void) n:FuncDefName x:Po q:Void x:Pc"),
      new(              "FunctionHeader",         "x:Function t:(Type|Void) n:FuncDefName x:Po qm:ParamDef x:Pc"),
      new(              "FunctionFull",           "d:FunctionHeader x:Bo sa:(FuncStmt|Block) x:Bc"),

      new(              "ScriptHeader",           "x:Script n:Value ti:ScriptType{lightning} x:Po q:ParamDef x:Pc"),
      new(              "ScriptHeader",           "x:Script n:Value t:(ScriptType|Return)"),
      new(              "ScriptHeader",           "x:Script n:Value x:Po qa:ParamDef x:Pc"),
      new(              "ScriptHeader",           "x:Script n:Value x:Po q:Void x:Pc"),
      new(              "ScriptFull",             "d:ScriptHeader x:Bo sa:(Stmt|Block) x:Bc"),
    ],
  };
  [DefinitionExport]
  public static Spec ModelDef => new()
  {
    FileInferences = [IfNOr(
      IfN(ExtIs, "modeldef"),
      IfN(FName|Is, "modeldef"))],
    Name = "zdoom.modeldef",
    RxOpt = ROML | ROIC | ROIPW | ROEC,
    SC = SCOIC,
    IsTextFile = true,
    TokenType = typeof(string),
    GroupTokenRules = [
      //TODO: Start Group Defs
    ],
    DefaultRuleSet = RT.IgnoreCase,
    TokenRules = [
      s_cString,
      s_cLineComment,
      s_cBlkComment,
      .. s_op_pba,
      .. TokenRule.MakeSingleCharRules("=,;", RT.TokenExact , "Eq", "Cm", "Sc" ),
    ],
    Operations = [
      new TokenizeOperation{ InputKey = "text", OutputKey = "tokens" , Tokenizer = "xparser" },
      new DebugPrintKeyOperation { InputKey = "tokens" },
      new TokenAssembleOperation { InputKey = "tokens", OutputKey = "token_groups" },
      new DebugPrintKeyOperation { InputKey = "token_groups" },
    ]
  };

  [DefinitionExport]
  public static Spec SndSeq => new()
  {
    FileInferences = [IfNOr(
      IfN(ExtIs, "sndseq"),
      IfN(FName|Is, "sndseq"))],
    Name = "zdoom.sndseq",
    RxOpt = ROML | ROIC | ROIPW | ROEC,
    SC = SCOIC,
    IsTextFile = true,
    TokenType = typeof(string),
    GroupTokenRules = [
      //TODO: Start Group Defs
    ],
    DefaultRuleSet = RT.IgnoreCase,
    TokenRules = [
      s_cLineComment,
      s_cBlkComment,
      .. s_op_pba,
      .. TokenRule.MakeSingleCharRules("=,;", RT.TokenExact , "Eq", "Cm", "Sc" ),
    ],
    Operations = [
      new TokenizeOperation { InputKey = "text", OutputKey = "tokens", Tokenizer="xparser" },
      new DebugPrintKeyOperation { InputKey = "tokens" },
      new TokenAssembleOperation { InputKey = "tokens", OutputKey = "token_groups" },
      new DebugPrintKeyOperation { InputKey = "token_groups" },
    ]
  };
}
