using Specification.JSON;

namespace UnitTest;

public class JSONTests
{
  [Theory]
  [InlineData(@"ParserLib\Specification.JSON\Schemas\Terminal Settings.json")]
  public void JSON_FunctionalTest (string file)
  {
    string content = File.ReadAllText(Helper.GitDir + file);
    XParser parser = new();
    OpStatus result = parser.ParseData(Definition.Spec, content);

    if (result is not OpStatus.Pass and not OpStatus.EndCommand)
    {
      Assert.Fail("Operation not Pass or EndCommand");
    }
    Assert.True(parser.Data.Keys.Count > 3);
  }

  [Theory]
  [InlineData(/*lang=json,strict*/ @"{""key"":""value"",""key2"":""value2""}")]
  public void JSON_FunctionalTest2 (string content)
  {
    XParser parser = new();
    OpStatus result = parser.ParseData(Definition.Spec, content);

    if (result is not OpStatus.Pass and not OpStatus.EndCommand)
    {
      Assert.Fail("Operation not Pass or EndCommand");
    }
    Assert.True(parser.Data.Keys.Count > 3);
  }
}
