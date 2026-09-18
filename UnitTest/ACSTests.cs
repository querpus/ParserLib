namespace UnitTest;

public class ACSTests
{
  internal const string Expression1 = "var_name + 34";
  internal const string Expression2 = "var_name != true";
  internal const string Expression3 = "!var_name";

  internal const string Statement1 = "int foo = 23;";
  internal const string Statement2 = "ACS_Execute(3, 4, 3);";
  internal const string Statement3 = "Terminate;";
  internal const string Statement4 = "char d = 'd';";
  internal const string Statement5 = "if (do_delay == true) delay(34);";
  internal const string Statement6 = "if (do_delay == true) { delay(34); }";

  [Theory]
  [InlineData(Expression1)]
  [InlineData(Expression2)]
  [InlineData(Expression3)]
  [InlineData(Statement1)]
  [InlineData(Statement2)]
  [InlineData(Statement3)]
  [InlineData(Statement4)]
  [InlineData(Statement5)]
  [InlineData(Statement6)]
  public void ACS_PieceParse (string to_parse)
  {
    TokenFactory tokenFactory = new(Specification.ZDoom.Definition.ACS);
    TokenAssembler tokenAssembler = new(Specification.ZDoom.Definition.ACS);

    TokenCollection tokens = tokenFactory.Produce(to_parse);
    Assert.NotEmpty(tokens);
    TokenAssemblyResult tc = tokenAssembler.Execute(tokens);
    _ = Assert.Single(tc.Hierarchy);
  }
}
