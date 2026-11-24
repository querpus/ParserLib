

## Marker Tokenizer Spec (Deprecated)

### Data Keys

    * initial - Data that was originally passed as input.
    * text - Text Data that was origninally passed as input.


### Token Types

    * ws - Use for line endings and whitespace.
    * blkcomment - Use for multiline or block comments.
    * lncomment - Use for single line comments.
    * int - Use for integers.
    * dec - Use for floating point, fixed point, and decimal numbers.
    * char - Use for single characters, or character codes.
    * bool - Use for logical boolean values (typically true or false)
    * string - Use for double or single quoted strings.

  ### Group Names

    Match level items
    * m_NAME where NAME is the marker name. A simple boolean marker.
    * m_prop_NAME where NAME is the property name. A string property stored with NAME as the key.
    * m_prop_key_INDEX where INDEX is the key to match the value to. A string property will use this content as the key.
    * t_TYPE where TYPE is the token type. Used for token generation.
    * x_TYPE where TYPE is the token type. Used for ignoring comments and whitespace.
