using System.Collections.Immutable;

using static Parser.Condition.KeyOption;

namespace Parser.Condition;

public class ParsedExpression (string expr, ParsedExpression? parent = null) : IExpression
{
  #region Public Properties
  public string Expression { get; private set; } = expr;
  public Dictionary<int, string> SubExpressions { get; } = [];
  public void SaveSubExpression (int pos, string expr)
  {
    ParsedExpression root = this;
    while (root.Parent is not null)
    {
      root = root.Parent;
    }

    root.SubExpressions[pos] = expr;
  }
  public string LoadSubExpression (int pos)
  {
    ParsedExpression root = this;
    while (root.Parent is not null)
    {
      root = root.Parent;
    }

    return root.SubExpressions[pos];
  }
  #endregion
  [AllowNull]
  public DataStore Data { get; protected set; }
  protected ImmutableArray<IValueNode>? Sequence { get; private set; }
  [AllowNull]
  private Collection<IValueNode> _workingSequence;
  private ParsedExpression? Parent { get; } = parent;

  public static explicit operator ParsedExpression (string expr) => new(expr);

  protected object? GetValue (ConditionValue @ref) => @ref.Type switch
  {
    LoadKey => Data[@ref.Value],
    CountOfKey => Data.CanLoad(@ref.Value) ? Data[@ref.Value] is IEnumerable<object> ien ? ien.ICount : 1 : 0,
    CheckKeyExists => Data.CanLoad(@ref.Value),
    TypeOfKey => Data.CanLoad(@ref.Value) ? Data[@ref.Value].GetType().Name : "null",
    Literal => @ref.Value,
    KeyOption.Decimal => decimal.TryParse(@ref.Value, out decimal d) ? d : Err.ThrowBadDef($"Condition String Tried to GetValue from a decimal literal {@ref}."),
    False => false,
    True => true,
    Null => null,
    Embedded => new ParsedExpression(@ref.Value, this).Evaluate(Data),
    >= OpStart => Err.ThrowBadDef($"Condition String Tried to GetValue from an operator {@ref}."),
    _ => Err.ThrowBadDef($"Condition String Tried to GetValue from an unknown type {@ref}.")
  };
  protected static dynamic? GetNum (object? left)
  {
    dynamic? ret_l = null;

    if (left is string s && int.TryParse(s, out int i))
      ret_l = i;
    else if (left is string s2 && decimal.TryParse(s2, out decimal d))
      ret_l = d;
    else if (left is int or decimal)
      ret_l = left;
    else if (left is bool b)
      ret_l = b ? 1 : 0;

    return ret_l;
  }
  [MemberNotNull(nameof(Sequence))]
  protected void Parse ()
  {
    _workingSequence = [];

    if (Expression.IsEmpty)
    {
      Sequence = [];
      return;
    }

    Dictionary<string, KeyOption> literalReference = new()
    {
      ["int"] = Integer,
      ["dec"] = KeyOption.Decimal,
      ["literal"] = Literal,
    };

    Dictionary<string, KeyOption> groupKeyReference = new()
    {
      ["exists"] = CheckKeyExists,
      ["countof"] = CountOfKey,
      ["typeof"] = TypeOfKey,
    };

    Dictionary<string, KeyOption> groupReference = new()
    {
      ["true"] = True,
      ["false"] = False,
      ["null"] = Null,
      ["gteq"] = OpGteq,
      ["lteq"] = OpLteq,
      ["gt"] = OpGt,
      ["lt"] = OpLt,
      ["eq"] = OpEq,
      ["noteq"] = OpNotEq,
      ["seqeq"] = OpSeqEq,
      ["and"] = OpAnd,
      ["or"] = OpOr,
      ["add"] = OpAdd,
      ["sub"] = OpSub
    };

    string innerGetter = new RxS(@"\((?'inner'[^()]+)\)");

    while (true)
    {
      Match m = Regex.Match(Expression, innerGetter, ROSL | ROIPW | ROML, TimeSpan.FromSeconds(1));

      if (!m.Success)
        break;

      int rem_from = m.Index;
      int rem_qty = m.Length;
      SaveSubExpression(rem_from, m.Groups["inner"].Value);
      this.Expression = Expression.ReplaceRange(rem_from, rem_qty, '\0');
    }

    // https://regex101.com/r/aTwSlR/5
    string conditionPattern = new RxS(@"(?:(?:\b(?'exists'exists)|(?'countof'countof)|(?'typeof'typeof))\b)?\[(?<varname>.*?)\]|\{(?<literal>.*?)\}|(?'gteq'>=)|(?'lteq'<=)|(?'gt'>)|(?'lt'<)|(?'eq'==)|(?'noteq'!=)|(?:\b(?'like'like)|(?'seqeq'matches)|(?'is'is)\b)|(?'and'&&)|(?'or'\|\|)|(?'int'-?\d+)|(?'dec'-?\d*\.\d+)|(?:\b(?'true'(?i:true))|(?'false'(?i:false))|(?'null'(?i:null))|(?'embed'\0+)\b)");
    foreach (Match m in Regex.Matches(Expression, conditionPattern, ROSL | ROIPW | ROML, TimeSpan.FromSeconds(1)))
    {
      void processLiterals ()
      {
        foreach (KeyValuePair<string, KeyOption> item in literalReference)
        {
          if (m.Groups.ContainsKey(item.Key))
            _workingSequence.Add(new ConditionValue(item.Value, m.Groups[item.Key].Value));
        }
      }
      void processKeyGroups ()
      {
        foreach (KeyValuePair<string, KeyOption> item in groupKeyReference)
        {
          if (m.Groups.ContainsKey(item.Key))
            _workingSequence.Add(new ConditionValue(item.Value, m.Groups["varname"].Value));
        }
      }
      void processGroups ()
      {
        foreach (KeyValuePair<string, KeyOption> item in groupReference)
        {
          if (m.Groups.ContainsKey(item.Key))
            _workingSequence.Add(new ConditionValue(item.Value, null));
        }
      }

      processKeyGroups();
      processGroups();
      processLiterals();

      if (m.HasValidGroup("embed"))
      {
        string expr = LoadSubExpression(m.Index);
        _workingSequence.Add(new ConditionValue(Embedded, expr));
      }
    }
    Sequence = [.. _workingSequence];
  }
  protected static object? Operate (KeyOption op, object? lobj, object? robj)
  {
    if (!op.IsOperator)
      return null;

    dynamic? left_num = GetNum(lobj);
    dynamic? right_num = GetNum(robj);

    if (op.UsesNumericInput && left_num is not null && right_num is not null)
    {
      return op switch
      {
        OpDiv => left_num / right_num,
        OpMul => left_num * right_num,
        OpMod => left_num % right_num,
        OpExp => Math.Pow(left_num, right_num),
        OpLt => left_num < right_num,
        OpGt => left_num > right_num,
        OpLteq => left_num <= right_num,
        OpGteq => left_num >= right_num,
        OpEq => left_num == right_num,
        OpNotEq => left_num != right_num,
        OpAdd => left_num + right_num,
        OpIs => left_num == right_num,
        OpAnd => left_num > 0 && right_num > 0,
        OpOr => left_num > 0 || right_num > 0,
        OpSub => left_num - right_num,
        OpRoot => Math.Pow(left_num, -right_num),
        OpXOr => left_num ^ right_num,
        _ => null
      };
    }

    if (op.UsesObjectInput)
    {
      return op switch
      {
        OpIs when robj is Type or null => lobj?.GetType().IsAssignableTo(robj as Type) ?? (robj is null),
        OpIs when lobj is string str => str.Is($"{robj}"),
        OpLike => $"{lobj}".Like($"{robj}"),
        OpSeqEq when lobj is IEnumerable<object> li && robj is IEnumerable<object> ri => li.SequenceEqual(ri),
        _ => null
      };
    }

    if (op.UsesLogicalInput)
    {
      return op switch
      {
        OpOr when lobj is bool l && robj is bool r => l || r,
        OpAnd when lobj is bool l && robj is bool r => l && r,
        OpXOr when lobj is bool l && robj is bool r => l ^ r,
        _ => null
      };
    }

    return null;
  }

  /// <summary>
  /// Evaluate the expression.
  /// </summary>
  /// <param name="data">The <see cref="DataStore"/> reference.</param>
  /// <remarks>If the evaluation fails, it always returns <see langword="false"/>.</remarks>
  public virtual object? Evaluate (DataStore data)
  {
    if (Sequence is null)
      Parse();

    OoOp current_level = OoOp.None, oplvl = OoOp.None;
    int left_index, right_index, op_index, i;
    _workingSequence = [.. Sequence];
    object? left, right;
    KeyOption op;

    void restartSequence ()
    {
      i = 0;
      clearOp();
      current_level++;
      clearLeft();
      clearRight();
    }
    void clearLeft ()
    {
      left = null;
      left_index = -1;
    }
    void clearRight ()
    {
      right = null;
      right_index = -1;
    }
    void clearOp ()
    {
      op_index = -1;
      op = Undefined;
    }
    void shiftVal ()
    {
      left = right;
      left_index = right_index;
      clearRight();
    }

    restartSequence();
    while (_workingSequence.Count != 1)
    {
      if (i >= _workingSequence.Count)
      {
        restartSequence();
      }

      IValueNode ivn = _workingSequence[i];

      if (ivn is ConditionValue cv)
      {
        if (left is null && !cv.Type.IsOperator)
        {
          left = GetValue(cv);
          left_index = i++;
          continue;
        }

        if (cv.Type.UsesLogicalInput && current_level < OoOp.And)
        {
          clearLeft();
          i++;
          continue;
        }

        else if (cv.IsOperator)
        {
          op = cv.Type;
          oplvl = op.OrderOfOperationsIndex;
          op_index = i++;
          continue;
        }

        if (left is not null && op is not Undefined && right is null)
        {
          right = GetValue(cv);
          right_index = i;
        }
      }
      else if (ivn is AssignedValue av)
      {
        if (left is null)
        {
          left = av.Value;
          left_index = i++;
          continue;
        }

        if (op is not Undefined && right is null)
        {
          right = av.Value;
          right_index = i;
        }

        if (right is not null && op is Undefined)
        {
          clearRight();
          clearLeft();
          left = av.Value;
          left_index = i++;
        }

        if (left is not null && right is not null)
        {
          shiftVal();
          right = av.Value;
          right_index = i;
        }
      }

      if (left is not null && op is not Undefined && right is not null && oplvl <= current_level)
      {
        _workingSequence.RemoveAt(right_index);
        _workingSequence.RemoveAt(op_index);
        _workingSequence.RemoveAt(left_index);
        _workingSequence.Insert(left_index, new AssignedValue() { Value = Operate(op, left, right) });
        i = left_index + 1;
        clearRight();
        clearLeft();
        clearOp();
      }
      else if (left is not null && op is not Undefined && right is not null)
      {
        shiftVal();
        clearOp();
        i++;
      }
    }

    return _workingSequence[0].Value;
  }
}
