// Comments always allowed on outside
// Comments allowed within Spec block
// Default keyword should not be used unless you are editing the default for unknown files!
// Spec Syntax:
//   Spec "<spec_name_no_spaces>" [Default]
Spec "ini"
{
  // Bracket Boxes may have special parsing rules
  // Simple definitions must end with a semicolon.
  Format = Text;
  Inferences
  {
    ExtIs Or { "ini", "url", "vnc", "inf" }
  }
  Tokens
  {
    // Options Include:
    //   DotAsNewLine
    //   RightToLeft
    // May also use this format:
    //   RegexOptions = MultiLine | ExplicitCapture | IgnorePatternWhitespace | CaseInsensitive;
    RegexOptions
    {
      MultiLine,
      ExplicitCapture,
      IgnorePatternWhitespace,
      CaseInsensitive
    }
    // Comment Line
    // use "-" for "None" or omit type.
    // regex extends from after whitespace following type until the first line break
    // UNLESS the next line starts with a "=>" sequence
    // You Must use Regex Comments with (?#Comment)
    // Syntax:
    //   
    Rules
    {
      TokenComment "-"       ;.*  (?# Line Comment Symbol)
      Competitive  "Value"   (?<= =\s*)  (?'keep'[^\\=\n;]|\\.)*? \s* (?=$|;)  (?# Trim )
      Competitive  "Key"     (?<= ^\s*)   ([^"[\]\s\\=\n;]|\\.)+
      Competitive  "QKey"    (?<= ^\s*) "(([^"[\]\s\\=\n;]|\\.)+)"
        =>                   (?# Continuation to the next line)
      TokenExact   "Eq"      =
      TokenExtract "Section" \[ (?'keep'.*?) \]
    }
    Groups
    {
      // Group Constructs
      "Property"       n:Key x:Eq v:Value
      "SectionWProps"  n:Section pa:Property
    }
    // Constructs make tokens into objects.
    // They name a property and assign the specified token to it.
    // They process in order, construct smaller tokens and then move to bigger ones.
    // They actively update the hierarchy, so they must define a condition to execute on.
    // EVERYTHING TOP-LEVEL must be constructed or discarded
    // Anything not top-level and not constructed is automatically discarded
    Constructs
    {
      // Condition parses like the evaluate operation.
      //  Ex. [Variables_Go_In_Braces] = "Value"
      //  Ex. [IsBasic] = true
      // Condition Variables:
      //  TokenType - The Token Type Assigned
      //  TokenLength - The Length of the token
      //  TokenText - The actual text of the token
      //  IsComplex - If the token is a group
      //  IsBasic - If the token is not grouped
      //  HasGroup(GroupName) - If The Token Has the named group
      // Property Keyword Signifies a property is to be initialized on the constructed item.
      // CriticalProperty means that it must not be an empty string or null object, and the value must be present.
      // Syntax:
      //  Property [ListType] "PropertyName" = Complex_Token_Variable

      // Complex Token Variables:
      //  Name - The "Name" Complex Token Item
      //  Value - The "Value" Complex Token Item
      //  ValueList - A Collection of the "Value" Token Items
      //  PropertyList - A Collection of the "Property" Token Items
      // ListType Values:
      //  Collection - List
      //  HashSet - Unique Item List
      //  Dictionary - Unique Keys and Values
      Construct "Property"
      {
        Condition = [TokenType] = "Property" && IsComplex ;
        Type = KeyValuePair<string,string>;
        CriticalProperty "Key" = Name;
        Property "Value" = Value;
      }
      Construct "Section"
      {
        Condition = [TokenType] = "Section" && IsComplex ;
        Type = INISection;
        CriticalProperty "Name" = Name;
        Property Collection "Properties" = PropertyList;
      }
    }
  }
}