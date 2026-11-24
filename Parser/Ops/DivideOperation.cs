#pragma warning disable IDE0060 // Remove unused parameter

using static Parser.OpStatus;

namespace Parser.Ops;

/// <summary>Divides 2 integers to find the quotient.</summary>
public class DivideOperation : Operation
{
  public string? DivisorKey { get; init; }
  public string? DividendKey { get; init; }
  public string? OutputKey { get; init; }

  public int Divisor { get; set; }
  public int Dividend { get; set; }

  protected override void Execute ()
  {
    if (DivisorKey is not null)
    {
      Divisor = Data[DivisorKey] is int dvs ? dvs : throw Err.ThrowBadInput("int", $"{Data[DivisorKey].GetType()}");
    }

    Dividend = Data[DividendKey] is int dvd ? dvd : throw Err.ThrowBadInput("int", $"{Data[DividendKey].GetType()}");

    if (Divisor == 0)
      throw Err.ThrowBadResult("Cannot divide by zero.");

    int quotient = Dividend / Divisor;
    Data[OutputKey] = quotient;
    Log(MsgClass.Debug, $"{Dividend} / {Divisor} = {quotient}", this);
    Status = Pass;
  }
}
