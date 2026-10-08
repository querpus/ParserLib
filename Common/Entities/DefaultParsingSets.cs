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

public interface ITokenTask
{
  void Initialize (EToken token);
  void Execute (ParsingContext context);
}

/// <summary>Creates an <see cref="IEntity"/> and pushes it to the top of the parent stack.</summary>
public readonly struct PushParentStackTokenTask (Type child_type) : ITokenTask
{
  public Type ChildType { get; } = child_type;
  public readonly void Initialize (EToken token) { }
  public readonly void Execute (ParsingContext context)
  {
    var stack = context.GetStack("Parent");
    var entity = ChildType.InvokeMember(SE, BFCI, null, null, null, CIIC) as IEntity;
    if (entity is null)
    {
      ParsingException.ThrowNullData("Parent");
    }
    if (stack.TryPeek(out var parent) && parent is IEntity ie)
    {
      ie.AddChild(entity);
    }
    stack.Push(entity);
  }
}
/// <summary>Pops the parent stack.</summary>
public readonly struct PopParentStackTokenTask : ITokenTask
{
  public readonly void Execute (ParsingContext context)
  {
    var stack = context.GetStack("Parent");

    if (!stack.TryPop(out _))
    {
      ParsingException.ThrowStackMismatch();
    }
  }
  public readonly void Initialize (EToken token) { }
}
/// <summary>Represents a token task that sets a named static value in a ParsingContext.</summary>
/// <remarks>Execution requires Value to be non-null; if Value is null a ParsingException is thrown. On execution,
/// the task stores Value in the provided ParsingContext under Key.</remarks>
/// <param name="key">The name of the static entry to set in the parsing context.</param>
public struct SetStaticTokenTask (string key) : ITokenTask
{
  public string Key { get; } = key;
  public dynamic? Value { get; set; }
  public void Initialize (EToken token)
  {
    if (token.Value.IsEmpty)
    {
      ParsingException.ThrowNullData(Key);
    }

    Value = token.Value;
  }
  public readonly void Execute (ParsingContext context)
  {
    if (Value is null)
    {
      ParsingException.ThrowNullData(Key);
    }
    context.SetStatic(Key, Value);
  }
}
/// <summary>Creates an <see cref="IEntity"/> and adds it to the <see cref="IEntity"/> on the top of the parent stack. Does not push or pop the stack.</summary>
public readonly struct AddChildToParentTokenTask (Type child_type) : ITokenTask
{
  public Type ChildType { get; } = child_type;
  public readonly void Initialize (EToken token) { }
  public readonly void Execute (ParsingContext context)
  {
    var stack = context.GetStack("Parent");
    var entity = ChildType.InvokeMember(SE, BFCI, null, null, null, CIIC) as IEntity;
    if (entity is null)
    {
      ParsingException.ThrowNullData("Parent.Children");
    }
    if (stack.TryPeek(out var parent) && parent is IEntity ie)
    {
      ie.AddChild(entity);
    }
  }
}
/// <summary>Adds data to the <see cref="IEntity"/> on the top of the parent stack.</summary>
public struct AddDataToParentTokenTask (string key) : ITokenTask
{
  public string Key { get; } = key;
  public dynamic? Value { get; set; }
  public void Initialize (EToken token)
  {
    if (token.Value.IsEmpty)
    {
      ParsingException.ThrowNullData(Key);
    }
    Value = token.Value;
  }
  public readonly void Execute (ParsingContext context)
  {
    var stack = context.GetStack("Parent");
    if (stack.TryPeek(out var parent) && parent is IEntity ie)
    {
      if (Value is null)
      {
        ParsingException.ThrowNullData(Key);
      }
      ie.DataValues[Key] = Value;
    }
  }
}
/// <summary>Adds data to the <see cref="IEntity"/> on the top of the property stack.</summary>
public struct AddDataToPropertyTokenTask (string key) : ITokenTask
{
  public string Key { get; } = key;
  public dynamic? Value { get; set; }
  public void Initialize (EToken token)
  {
    if (token.Value.IsEmpty)
    {
      ParsingException.ThrowNullData(Key);
    }
    Value = token.Value;
  }
  public readonly void Execute (ParsingContext context)
  {
    var stack = context.GetStack("Property");
    if (stack.TryPeek(out var prop) && prop is IEntity ie)
    {
      if (Value is null)
      {
        ParsingException.ThrowNullData(Key);
      }
      ie.DataValues[Key] = Value;
    }
  }
}
/// <summary>Adds a simple boolean value to the <see cref="IEntity"/> on the top of the parent stack.</summary>
public struct SetBooleanToParentTokenTask (string key) : ITokenTask
{
  public string Key { get; } = key;
  public bool Value { get; set; }
  public void Initialize (EToken token)
  {
    if (token.Value.IsEmpty)
    {
      ParsingException.ThrowNullData(Key);
    }
    Value = token.Value.IsNotEmpty;
  }
  public readonly void Execute (ParsingContext context)
  {
    var stack = context.GetStack("Parent");
    if (stack.TryPeek(out var parent) && parent is IEntity ie)
    {
      ie.DataValues[Key] = Value;
    }
  }
}
/// <summary>Adds data to the <see cref="IEntity"/> that was last added to the <see cref="IEntity"/> at the top of the parent stack's children.</summary>
/// <param name="key">The key of the data to add.</param>
public struct AddDataToLastSibling (string key) : ITokenTask
{
  public string Key { get; } = key;
  public dynamic? Value { get; set; }
  public void Initialize (EToken token)
  {
    if (token.Value.IsEmpty)
    {
      ParsingException.ThrowNullData(Key);
    }
    Value = token.Value;
  }
  public readonly void Execute (ParsingContext context)
  {
    var stack = context.GetStack("Parent");
    if (stack.TryPeek(out var parent) && parent is IEntity ie)
    {
      if (Value is null)
      {
        ParsingException.ThrowNullData($"Parent.Children[^1].DataValues[{Key}]");
      }
      ie.Children[^1].DataValues[Key] = Value;
    }
  }
}
public static class DefaultParsingSets
{
  [SS("regex")] private const string elem_close = @"(?'element_close' < \s* /) \s* (?'tag_name' [:\w]+ ) \s* (?'tag_close' > )";
  [SS("regex")] private const string elem_single = @"(?'element_single' < ) \s* (?'tag_name' [:\w]+ ) (\s+ (?'a_name' [:\w]+) (?'a_eq'=) (?'a_qt'['""]) (?'a_value'(?!\k<a_qt>).*) (?'a_end'\k<a_qt>) )* \s*  (?'tag_close' / \s* > )";
  [SS("regex")] private const string elem_header = @"(?'element_header' <\?) \s* (?'tag_name' xml ) (\s+ (?'a_name' [:\w]+) (?'a_eq'=) (?'a_qt'['""]) (?'a_value'(?!\k<a_qt>).*) (?'a_end'\k<a_qt>) )* \s* (?'header_close' \?> )";
  [SS("regex")] private const string elem_open = @"(?'element_open' < ) \s* (?'tag_name' [:\w]+ ) (\s+ (?'a_name' [:\w]+) (?'a_eq'=) (?'a_qt'['""]) (?'a_value'(?!\k<a_qt>).*) (?'a_end'\k<a_qt>) )* \s* (?'tag_close' > )";
  [SS("regex")] private const string content = $"(?'content'([^<]|{comment})*)";
  [SS("regex")] private const string comment = "(?'comment'<!-- ([^-]| -[^-])* -->)";
  [SS("regex")] private const string non_element_content = $@"({content}|{comment}|\s+)*";

  private static string recurse_xml (int depth)
  {
    string recurse = $@"({elem_open} {non_element_content} {elem_close})";

    for (int i = 0; i < depth; i++)
    {
      recurse = $"({elem_open} ({recurse}|{non_element_content}|{elem_single})* {elem_close})";
    }
    Log(MsgClass.Warning, recurse, "DefaultParsingSets");
    return recurse;
  }

  public static ParsingInfo XML { get; } = new()
  {
    GeneratesSingleObject = true,
    IgnoreCase = false,
    RegexOptions = ROIPW | ROML | ROEC,
    RegexString = recurse_xml(255),
    TokenRules = [
    new() {
      TokenName = "element_open",
      TokenTasks = [
        new PushParentStackTokenTask(typeof(ElementEntity)),
      ],
    }, new() {
      TokenName = "tag_name",
      TokenTasks = [
        new AddDataToParentTokenTask("Name"),
      ],
    }, new() {
      TokenName = "a_name",
      Execute = (context, token) => {
        context.Parent?.Add(new AttributeEntity() {
          Origin = token.Value,
          Key = token.Value,
          
        });
      },
      TokenTasks = [
        new SetStaticTokenTask("AttributeName"),
        new AddDataToPropertyTokenTask("Name"),
      ],
    }, new() {
      TokenName = "a_qt",
      TokenTasks = [
        new AddDataToPropertyTokenTask("Quote"),
      ],
    }, new() {
      TokenName = "a_value",
      TokenTasks = [
        new AddDataToPropertyTokenTask("Value"),
      ],
    }],
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
