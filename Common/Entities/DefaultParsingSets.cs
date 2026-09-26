#pragma warning disable CA1710 // Identifiers should have correct suffix
#pragma warning disable format // Formatting

namespace Common.Entities;

public static class DefaultParsingSets
{
  public static ParsingInfo XML { get; } = new ParsingInfo()
  {
    GeneratesSingleObject = true,
    IgnoreCase = false,
    SingleObject = new() {
      Class = typeof(DocumentEntity),
      SetAsNextLevelParent = true,
    },
    RegexOptions = ROIPW | ROML | ROEC,
    RegexString =
    """
    (?# Element Piece)
    (?'element'
      <   \s*
      (?# '?' for header definition)
      (?'header'\?)?   \s*
      (?'close' \/)?   \s*
      (?# optional namespace)
      ((?'ns'\w+)  \s* :)?
      (?'name'\w+)

      (?# attributes)
      (   \s+ 
          (?'attribute'
          ((?'a_ns'  \w+)     \s*     :     \s*)?
           (?'a_name'\w+)     \s*     =     \s*
         " (?'a_val'(  [^\n"\\]  |  \\[^\n]  )*  )"
        ))*

      (?'single'\s*\/)?
      \s*
      (?# Optional '?' for ending the header definition)
      \? ?
      >
    ) |

    (?# Leading or Trailing Whitespace)
    (?'ws'(?<=\>)\s+) |
    (?'ws'(?<=[^\s>])\s+) |
    (?# XML Content)
    (?'content'(?<=\>\s*)[^<]+?(?=\s*<)) |
    (?# XML Comment)
    (?'comment'<!-- ([^-]| -[^-])* -->)
    """,
    EntityOptions = [
    new() {
      GroupRequired = "header",
      Class = typeof(ElementEntity),
    }, new() {
      GroupRequired = "close",
      DepthChange = -1,
    }, new() {
      GroupRequired = "single",
    }, new() {
      GroupRequired = "element",
      DepthChange = 1,
      SetAsNextLevelParent = true,
      ChildType = typeof(ElementEntity),
    },new() {
      GroupRequired = "ws",
    }, new() {
      GroupRequired = "content",
    }, new() {
      GroupRequired = "comment",
    },],
  };
  public static ParsingInfo JSON { get; } = new ParsingInfo()
  {
    GeneratesSingleObject = true,
    SingleObject = new() {
      Class = typeof(DocumentEntity),
      SetAsNextLevelParent = true,
    },
    RegexOptions = ROIPW | ROML | ROEC,
    RegexString =
    """
    (?#primitives)
    (?'key'        " (?'key_name'\w+) " (?=\s*[:=])) |
    (?'str_value'   (?<=[:=]\s*) " (?'value'([^\\"]|\\.)*) " ) |
    (?'num_value'   (?<=[:=]\s*)   (?'value'[0-9.eExXbB-]+ )  ) |
    (?'bool_value'  (?<=[:=]\s*)   (?'value'true|false)      ) |
    (?'null_value'  (?<=[:=]\s*)   (?'value'null)            ) |
    (?#operators)
    (?'Op'            [[\]{},=:]) |
    (?#comments)
    (?'comment'        \/\/.* ) |
    (?'comment'        \/\*([^*]|\*[^/])*\*\/ ) |
    (?#whitespace)
    (?'ws'             \s+)
    """,
    IgnoreCase = false,
    EntityOptions = [
    new() {
      GroupRequired = "key",
      StorePieceTypes = new() { ["Key"] = "key" },
      SetPropKey = true
    }, new() {
      GroupRequired = "str_value",
      AddToPropKey = true,
      StorePieceTypes = new() { ["Value"] = "value" },
    }, new() {
      GroupRequired = "bool_value",
      AddToPropKey = true,
      StorePieceTypes = new() { ["Value"] = "value" },
    }, new() {
      GroupRequired = "num_value",
      AddToPropKey = true,
      StorePieceTypes = new() { ["Value"] = "value" },
    }, new() {
      GroupRequired = "null_value",
      AddToPropKey = true,
    }, new() {
      GroupRequired = "comment",
    }, new() {
      GroupRequired = "ws",
    }, new() {
      GroupRequired = "Op",
      ExactTextRequired = "{",
      DepthChange = 1,
      AddToPropKey = true,
      ChildType = typeof(ObjectEntity),
    }, new() {
      GroupRequired = "Op",
      ExactTextRequired = "}",
      DepthChange = -1,
    }, new() {
      GroupRequired = "Op",
      ExactTextRequired = "[",
      DepthChange = 1,
      AddToPropKey = true,
      ChildType = typeof(ArrayEntity),
    }, new() {
      GroupRequired = "Op",
      ExactTextRequired = ":",
    }, new() {
      GroupRequired = "Op",
      ExactTextRequired = ",",
    }, new() {
      GroupRequired = "Op",
      ExactTextRequired = "]",
      DepthChange = -1,
    }]
  };
}
