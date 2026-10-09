#pragma warning disable CA1710 // Identifiers should have correct suffix
#pragma warning disable format // Formatting
#pragma warning disable IDE1006 // Naming Styles

using static Common.Entities.DepthOperation;
using static Common.Debug;

namespace Common.Entities;

public class NewTokenizer
{
  public Collection<EToken> Tokens { get; } = [];
  public required ParsingInfo ParsingInfo { get; init; }
  protected int CurrentIndex { get; set; }

  public Collection<EToken> Tokenize (string input)
  {
    Tokens.Clear();
    CurrentIndex = 0;

    if (ParsingInfo.RegexString is null)
    {
      throw new InvalidOperationException("ParsingInfo.RegexString is null.");
    }

    var regex = new Regex(ParsingInfo.RegexString, ParsingInfo.RegexOptions);
    foreach (Match match in regex.Matches(input))
    {
      foreach (string groupName in regex.GetGroupNames())
      {
        Group group = match.Groups[groupName];
        if (group.Success && !groupName.Equals("0", SCO))
        {
          EToken token = new()
          {
            Value = group.Value,
            Group = groupName,
            CaptureIndex = group.Captures.Count - 1,
            Position = group.Index..(group.Index + group.Length)
          };
          Tokens.Add(token);
          Log(MsgClass.GreenInfo, $"{token}", this);
        }
      }
    }
    return Tokens;
  }
}

public class ParsingException : InvalidOperationException
{
  [DoesNotReturn]
  public static dynamic ThrowStackMismatch() => throw new("Stack mismatch. Attempted to pop from an empty stack.");
  [DoesNotReturn]
  public static dynamic ThrowNullData (string key) => throw new($"Attempted to write null data to key {key}.");
  public ParsingException (string message) : base(message) { }
  public ParsingException (string message, Exception innerException) : base(message, innerException) { }
  public ParsingException () { }
}

public static class DefaultParsingSets
{
  [SS("regex")] private const string elem_close = @"(?'element_close' < \s* /) \s* (?'tag_name' [:\w]+ ) \s* (?'close_tag_close' > )";
  [SS("regex")] private const string elem_single = @"(?'element_single' < ) \s* (?'tag_name' [:\w]+ ) (\s+ (?'a_name' [:\w]+) (?'a_eq'=) (?'a_qt'['""]) (?'a_value'(?:(?!\k<a_qt>).)*) (?'a_end'\k<a_qt>) )* \s*  (?'tag_close' / \s* > )";
  [SS("regex")] private const string elem_header = @"(?'element_header' <\?) \s* (?'tag_name' xml ) (\s+ (?'a_name' [:\w]+) (?'a_eq'=) (?'a_qt'['""]) (?'a_value'(?:(?!\k<a_qt>).)*) (?'a_end'\k<a_qt>) )* \s* (?'header_close' \?> )";
  [SS("regex")] private const string elem_open = @"(?'element_open' < ) \s* (?'tag_name' [:\w]+ ) (\s+ (?'a_name' [:\w]+) (?'a_eq'=) (?'a_qt'['""]) (?'a_value'(?:(?!\k<a_qt>).)*) (?'a_end'\k<a_qt>) )* \s* (?'open_tag_close' > )";
  [SS("regex")] private const string content = "(?'content'[^<]*)";
  [SS("regex")] private const string comment = "(?'comment'<!-- ([^-]| -[^-])* -->)";
  public static ParsingInfo XML { get; } = new()
  {
    GeneratesSingleObject = true,
    IgnoreCase = false,
    RegexOptions = ROIPW | ROML | ROEC,
    RegexString = $"{elem_close}|{elem_single}|{elem_open}|{elem_header}|{comment}|{content}",
    EntityOptions = [ new() {
      StorePieceTypes = {
        ["IsHeader"]  = "element_header",
        ["IsSingle"]  = "element_single",
        ["IsClose"]   = "element_close",
        ["Name"]      = "tag_name"
      },
    }, new() {
      GroupRequired = "a_name",
      GroupTerminator = "a_end",
      StorePieceTypes = new() {
        ["Name"] = "a_name",
        ["Namespace"] = "a_ns",
        ["Quote"] = "a_qt",
        ["Value"] = "a_value"
      },
      Class = typeof(AttributeEntity)
    }, new () {
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
    RegexOptions = ROIPW | ROML | ROEC,
    RegexString =
    """
    (?#primitives)
    (?'key'        (?'qt'["']) (?'key_name'\w+) \k<qt> (?=\s*[:=])) |
    (?'str_value'   (?<=[:=]\s*) (?'qt'["']) (?'value'([^\\"]|\\.)*) \k<qt> ) |
    (?'num_value'   (?<=[:=]\s*)   (?'value'[0-9.xXa-fA-F-]+ )  ) |
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
      StorePieceTypes = new() {
        ["Key"] = "key",
        ["Quote"] = "qt",
      },
      SetPropKey = true,
      Class = typeof(PropertyEntity)
    }, new() {
      GroupRequired = "str_value",
      AddToProperty = true,
      StorePieceTypes = new() {
        ["Value"] = "value",
        ["Quote"] = "qt"
      },
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
