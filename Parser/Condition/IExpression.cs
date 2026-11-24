namespace Parser.Condition;

/// <summary>Represents a condition or a check, and its result.</summary>
public interface IExpression
{
  string Expression { get; }

  /// <summary>Evaluates the value and returns it.</summary>
  /// <returns>The calculated value or result of the expression. This method returns <see langword="null"/> if the evaluation fails.</returns>
  object? Evaluate (DataStore data);
  /// <summary>Evalutates the condition.</summary>
  /// <param name="data">The <see cref="DataStore"/> reference</param>
  /// <returns><see langword="true"/> if the condition is true, or a value that is not 0, -1, null, an empty collection, or an empty string. Returns <see langword="false"/> otherwise.</returns>
  /// <remarks>If the evaluation fails, it returns <see langword="false"/>.</remarks>
  /// <example>
  /// An empty <see cref="IEnumerable{T}"/> returns <see langword="false"/><br/>
  /// The number <c>4</c> returns <see langword="true"/><br/>
  /// The number <c>-1</c> returns <see langword="false"/><br/>
  /// The value <see langword="null"/> returns <see langword="false"/><br/>
  /// Any unlisted object returns <see langword="true"/>
  /// </example>
  bool LogicalEvaluate (DataStore data) => Evaluate(data) switch
  {
    null or 0 or -1 or false => false,
    true or > 0 or < 0 => true,
    string s when s.IsNotEmpty => true,
    string => false,
    IEnumerable ie when ie.IsEmpty => false,
    IEnumerable => true,
    _ => true
  };
}
