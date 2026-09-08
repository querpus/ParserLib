using System;
using System.Xml.Linq;

using Common.Entity;
using Common.Extensions;

using static Common.Names;

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
    IParsedEntity parsedEntity = EntityFactory.FromXElement(xml_data, null);
    DocumentEntity xMLDocumentEntity = Assert.IsType<DocumentEntity>(parsedEntity);
    Assert.NotNull(xMLDocumentEntity.RootNode);
    Assert.Equal(BasicType.Element, xMLDocumentEntity.RootNode.Type);
  }

  //[Theory]
  //[InlineData("tcf:Dec")]
  //public void ChkTokenParse (string parse)
  //{
  //  parse += "";
  //  //ChkToken<string> test = new(parse) { TokenRule = TokenRuleType.None };
  //}
}

public class CommonTests
{
  [Theory]
  [InlineData("\u0000\u0010\u0020")]
  public void CharDisplayTest (string data)
  {
    string result = SE;
    foreach (char c in data)
    {
      result += c.Display;
    }
    Assert.Equal("␀␐␠", result);
  }

  [Theory]
  [InlineData(new byte[] { 0, 16, 5, 0 }, 0x00100500, true)]
  [InlineData(new byte[] { 0, 16, 5, 0 }, 0x00051000, false)]
  public void SpanToInt (byte[] v, int value, bool big_endian) => Assert.Equal(value, v.ToInt32(big_endian));
}
