namespace Parser.Inference;

[Flags]
public enum InferenceType
{
  None = 0,

  //Property to look at
  Ext = 0x1,
  FName = 1 << 1,
  FileHeader = 1 << 2,
  FileContent = 1 << 3,
  FileSize = 1 << 4,

  //Special for alternates
  And = 1 << 8,
  Or = 1 << 9,

  //Text Comparison
  Is = 1 << 12,      // Case sensitive equals
  End = 1 << 13,     // EndsWith (case insensitive)
  Start = 1 << 14,   // StartsWith (case insensitive)
  Contains = 1 << 15,// contatins (case insensitive)

  //Numeric Comparison
  Larger = 1 << 16, // greater than (numeric)
  Smaller = 1 << 17,// less than (numeric)
  Equal = 1 << 18,  // equal to (numeric)
  LargerOrEqual = Larger | Equal,
  SmallerOrEqual = Smaller | Equal,
  Like = 1 << 19,   // Case insensitive equals

  // Logical
  Not = 1 << 20     // Inverse can be applied to any combination.
}
