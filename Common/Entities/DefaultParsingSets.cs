#pragma warning disable CA1710 // Identifiers should have correct suffix
#pragma warning disable format // Formatting

using static Common.Entities.DepthOperation;

namespace Common.Entities;

public static class DefaultParsingSets
{
  public static ParsingInfo XML { get; } = new()
  {
    GeneratesSingleObject = true,
    IgnoreCase = false,
    SingleObject = new() {
      Class = typeof(ElementEntity),
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
          ((?'a_ns'  \w*?)     \s*     :?     \s*)
           (?'a_name'\w+)     \s*     =     \s*
         (?'a_qt'["']) (?'a_val'(  [^\n\\](?<!\k<a_qt>)  |  \\[^\n]  )*?  ) \k<a_qt>
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
      DepthChange = Ascend,
      Class = null,
    }, new() {
      GroupRequired = "single",
      Class = typeof(ElementEntity),
    }, new() {
      GroupRequired = "element",
      DepthChange = Descend,
      SetAsNextLevelParent = true,
      Class = typeof(ElementEntity),
    },new() {
      GroupRequired = "ws",
    }, new() {
      GroupRequired = "content",
    }, new() {
      GroupRequired = "comment",
    },],
  };
  public static ParsingInfo JSON { get; } = new()
  {
    GeneratesSingleObject = true,
    SingleObject = new() {
      Class = typeof(ObjectEntity),
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
      SetPropKey = true,
      Class = typeof(PropertyEntity)
    }, new() {
      GroupRequired = "str_value",
      AddToProperty = true,
      StorePieceTypes = new() { ["Value"] = "value" },
      Class = typeof(StringEntity)
    }, new() {
      GroupRequired = "bool_value",
      AddToProperty = true,
      StorePieceTypes = new() { ["Value"] = "value" },
      Class = typeof(BooleanEntity)
    }, new() {
      GroupRequired = "num_value",
      AddToProperty = true,
      StorePieceTypes = new() { ["Value"] = "value" },
      Class = typeof(NumberEntity),
    }, new() {
      GroupRequired = "null_value",
      AddToProperty = true,
      Class = typeof(NullEntity)
    }, new() {
      GroupRequired = "comment",
      Class = typeof(CommentEntity)
    }, new() {
      GroupRequired = "ws",
      Class = typeof(WhitespaceEntity)
    }, new() {
      GroupRequired = "Op",
      ExactTextRequired = "{",
      DepthChange = Descend,
      AddToProperty = true,
      Class = typeof(ObjectEntity),
    }, new() {
      GroupRequired = "Op",
      ExactTextRequired = "}",
      DepthChange = Ascend,
      Class = null,
    }, new() {
      GroupRequired = "Op",
      ExactTextRequired = "[",
      DepthChange = Descend,
      AddToProperty = true,
      Class = typeof(ArrayEntity),
    }, new() {
      GroupRequired = "Op",
      ExactTextRequired = ":",
      Class = null,
    }, new() {
      GroupRequired = "Op",
      ExactTextRequired = ",",
      Class = null,
    }, new() {
      GroupRequired = "Op",
      ExactTextRequired = "]",
      DepthChange = Ascend,
      Class = null,
    }]
  };
  public static ParsingInfo INI { get; } = new()
  {
    RegexOptions = ROEC | ROML | ROIPW,
    GeneratesSingleObject = false,
    RegexString =
    """
    ^\s*(?'remove'-)?(?'section'\[(?'name'[^\]\n]+)\]) |
    (?<=^\s*)
    (?'quote'["'])?
    (?(?<=['"])
      (?'key'[^[\]\n=-]+)\k'quote'|
      (?'key'[^\s[\]\n="'-]+)
    )
    |
    (?'op'=) |
    (?<==\s*)
    (?'quote'["']?)
    (?(?<=['"])
      (?'value'[^\n]+)| (?#If Quoted)
      \s*(?'content'[^\n;"]+) (?#If Not Quoted)
    )
    \k'quote'
    (?<!\s) | 
    (?'comment';.*)|
    (?'ws'\s+)|
    (?'invalid'.)
    """,
    EntityOptions = [
      new() {
        Class = typeof(PropertyEntity),
        GroupRequired = "key",
        SetPropKey = true,
        StorePieceTypes = new() {
          ["Key"] = "key",
          ["Quote"] = "quote"
        },
      }, new() {
        Class = typeof(SectionEntity),
        GroupRequired = "section",
        StorePieceTypes = new() { ["Name"] = "name" },
        SetAsNextLevelParent = true,
        DepthChange = AscendAndDescend,
        OnlyAscendIfParentClass = typeof(SectionEntity)
      }, new() {
        Class = typeof(StringEntity),
        GroupRequired = "value",
        AddToProperty = true,
        StorePieceTypes = new() {
          ["Value"] = "value",
          ["Quote"] = "quote"
        },
      }, new() {
        Class = typeof(CommentEntity),
        GroupRequired = "comment"
      }, new() {
        Class = typeof(ContentEntity),
        GroupRequired = "content",
        StorePieceTypes = new() {
          ["Content"] = "content",
        },
      }
    ]
  };
}
