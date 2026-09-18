using System;
using System.Xml.Linq;

using Common.Entities;
using Common.Extensions;

using Parser.Ops.Text;

namespace UnitTest;

public class ParserTests
{
  [Fact]
  public void LibraryTest ()
  {
    Library lib = Library.InitializeLibrary(AppDomain.CurrentDomain);
    Assert.True(lib.Count > 3);
    Assert.Equal("ipl", lib.LookupOrDefault("ipl").Name);
  }

  [Theory]
  [InlineData(@"ParserLib\Specification.XML\Samples\operation.xml")]
  public void EntityTest (string file)
  {
    string path = Helper.GitDir + file;
    Assert.True(File.Exists(path));
    string content = File.ReadAllText(Helper.GitDir + file);
    Assert.True(content.Length > 10);
    XElement xml_data = XElement.Parse(content);
    IEntity parsedEntity = EntityFactory.FromXElement(xml_data, null);
    DocumentEntity xMLDocumentEntity = Assert.IsType<DocumentEntity>(parsedEntity);
    Assert.NotNull(xMLDocumentEntity.RootNode);
    Assert.Equal(BasicType.Element, xMLDocumentEntity.RootNode.Type);
  }

  [Theory]
  [InlineData("blah")]
  public void Parser_DataStorageAndAccess (string initial_string)
  {
    Spec spec = new()
    {
      FileInferences = [],
      Name = "test",
      Operations = [
        new ExtractOperation { Pattern="\\w+", ExtractedKey="test_key", InputKey="text", OutputKey="out_key"}
        ]
    };

    XParser textParser = new();
    OpStatus status = textParser.ParseData(spec, initial_string);
    Assert.True(status.IsPass);
    Assert.Equivalent(initial_string, textParser.Data["test_key"].AsCollection()[0]);
    Assert.Equivalent("", textParser.Data["out_key"]);
  }

  //[Theory]
  //[InlineData("tcf:Dec")]
  //public void ChkTokenParse (string parse)
  //{
  //  parse += "";
  //  //ChkToken<string> test = new(parse) { TokenRule = TokenRuleType.None };
  //}
}
