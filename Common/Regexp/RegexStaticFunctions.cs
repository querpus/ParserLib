//#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
using static Common.RegExp.RxS;

namespace Common.RegExp;
/// <summary>Static functions to shorthand frequent definitions.</summary>
public static class RegexStaticFunctions
{
  public static RxS Rx ([SS("Regex")] string regex) => RxS.Rx(regex);
  public static RxS Nm (string name, [SS("Regex")] string regex) => GrpNm(name, regex);
  public static RxS Gp ([SS("Regex")] string regex) => Grp(regex);
  public static RxS Or ([SS("Regex")] string first, [SS("Regex")] params Collection<string> additions) => RxS.Or(first, additions);

  public static RxS DQt ([SS("Regex")] string regex) => $"\"{regex}\"";
  public static RxS SQt ([SS("Regex")] string regex) => $"\'{regex}\'";
  /// <summary>Word Boundary (\b)</summary>
  public static RxS Bk => @"\b";
  /// <summary>Line Start (^)</summary>
  public static RxS Start => "^";
}
