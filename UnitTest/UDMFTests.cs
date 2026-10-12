using Specification.ZDoom;

namespace UnitTest;

public class UDMFTests
{
  [Theory]
  [InlineData(@"ParserLib\Specification.ZDoom\Samples\UDMF\map00.udmf")]
  [InlineData(@"ParserLib\Specification.ZDoom\Samples\UDMF\sample.udmf")]
  public void UDMF_TokenFactoryTest (string file)
  {
    //Load Data
    string input = File.ReadAllText(Helper.GitDir + file);

    //Load Spec
    Spec spec = Definition.UDMF;
    TokenRuleCollection rules = [];
    rules.AddRange(spec.TokenRules);
    TokenFactory factory = new(spec);
    TokenCollection result = [.. factory.Tokenize(input)];
    TokenAssembler assembler = new([.. spec.GroupTokenRules], spec);
    int count = result.Count;
    TokenAssemblyResult tar = assembler.Execute(result);
    int count2 = tar.Parents.Count;
    Assert.NotEmpty(tar.Parents);
    Assert.NotEqual(count, count2);
  }
}
