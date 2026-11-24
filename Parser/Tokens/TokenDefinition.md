
# _OBSOLETE_

# Token Rules Guide

  This is a guide to explain the new token system, and the rule syntax.

  The type parameter 'T' represents the token type, often expressed as an enum. Any enum will be accepted here.
  Strings are also accepted, the formal requirement is that it is not nullable.

## 1. Tokenizer Flags

* `None` = `0` No flags defined. Using `None` is likely an error.<br/>
### These specify the type of operation for the Tokenizer.

* `TokenExact` - This Token Rule will exactly match the string provided.
* `TokenMatch` - This Token Rule will match to the regex provided.
* `SplitMatch` - This Token Rule will split the input at the regex provided, limiting future matches.
* `SplitExact` - This Token Rule will split the input at the exact string provided, limiting future matches.
* `StoreExtra` - This Token Rule will store the unmatched data parts that match the regex provided as tokens with this type.
* `StoreOther` - This Token Rule will store the unmatched data parts as tokens with this type.
* `TokenExtract` - This Token Rule will store the unmatched data parts as tokens with this type.
* `ErrorMatch` - This Token Rule will fail the assembly if matched.
* `Competitive` - All Token Rules with this flag will run concurrently and exclusively as a `TokenMatch`. You do not need to include TokenMatch.
### These are flags and modifiers.
* `IgnoredToken` - Flags the created token as ignored.
* `IgnoreCase` - Exact matches and regex will ignore case.
* `Recursive` - The rule will execute until no matches occur.
* `Any` - Shorthand for `Opt` and `Mult`.
* `Opt` - This token sequence entry is not required, but will be consumed if present.
* `Mult` - This token sequence entry can have additional entries, and will consume them if present.
### This is the mask to remove all of the flags to get the type.
* `FlagBits` = `Mult | Opt | Recursive | IgnoreCase | IgnoredToken | ExemptAllWithin | FromTokens`


## Token Assembly Syntax

    (1) prefix:token_type
    (2) prefix:(type1-type2-type3)
    (3) prefix:(type1|type2|type3){literal1|literal2}
    (4) prefix:{literal}
    (5) prefix:token_type{literal}

No spaces (or tabs, or line feeds) can be between any single token assembly structure.
The hyphens in the 2nd example can be interchanged with '&', '|', or '+', like in the 3rd example. <br><br>
If you classify many tokens specifically, but need a general token to represent multiple other possibiliies, be sure to define `TokenCompatLookup` in your Spec to make your rule strings more clear.

## Prefix Letters

Some letters are optional. You must have at least 1 letter to make a valid definition.

Your prefix can have one of each of these:

    i - Ignore Case (String Literal Only)
    m - One or many, this token will repeat as long as it can, Possessive, Greedy.
    o - Optional, this token does not trigger a fail if it does not match. Greedy.
    a - Any, this token does not trigger a fail if it does not match, and can consume as many as it can. Greedy, Possessive.

If 'm' and 'o' (or 'a') are both specified, it acts as the '*' operator, meaning zero or many, but it stays Greedy.
Defining 'm' alone or 'im' makes it Possessive, meaning it will not give any matches back, to attempt to find a more suitable match.
It will simply fail the match entirely, like an atomic group.

Must have only one of these:

    x - Ignore Token
    n - Token is 'Name' in object, property, or label
    y - Token is 'Type' in object, typedvalue, or array
    v - Token is 'Value' in array, property, or typedvalue
    p - Token is 'Property' in object
    f - Token is 'Name' in flag and AddFlag is true.
    r - Token is 'Name' in flag and AddFlag is false.

### Example

    v:typename

    * v : it is stored in the value field

    cvi:null

    * t : null is the string literal
    * v : it is stored in the value field
    * i : it is not case sensitive
    
    tpm:Property

    * t : Property is the token type
    * p : it is stored in the property field
    * m : it will consume as many properties as it can find, giving nothing back

    bn:Keyword{\bscript\b}

    * b : Keyword is the token type, and \bscript\b is the regex string to match
    * n : it is stored in the name field

### Constructable Tokens

- `<b><font name="cascadia code" size=3 color=#5588FF>TokenObject\<T\></font></b>`
  - Name - Optional
  - Type - Optional
  - PropertyList - Zero or more items
  - FlagList - Zero or more items
* `<b><font name="cascadia code" size=3 color=#5588FF>TokenFlag\<T\></font></b>`
  * Name - Required
  * BooleanValue - Required
* `<b><font name="cascadia code" size=3 color=#5588FF>TokenTypedValue\<T\></font></b>`
  * Type - Optional
  * Value - Optional
* `<b><font name="cascadia code" size=3 color=#5588FF>TokenLabel\<T\></font></b>`
  - Name - Required
* `<b><font name="cascadia code" size=3 color=#5588FF>TokenArray\<T\></font></b>`
  * Type - Optional
  * ValueList - Zero or more items
* `<b><font name="cascadia code" size=3 color=#5588FF>TokenProperty\<T\></font></b>`
  - Name - Required
  - Type - Optional
  - Value - Optional
* `<b><font name="cascadia code" size=3 color=#5588FF>TokenStatement\<T\></font></b>`
  - Name - Required
  - Type - Optional
  - Value - Optional
  - ParameterList - Zero or more items
* <b><font name="cascadia code" size=3 color=#5588FF>TokenExpression\<T\></font></b>
  - Left - Optional
  - Type - Optional
  - Right - Optional

#### Guidelines

In a state list, shown below.

    StateLabel:          //TokenLabel
      FRAME A            //TokenStatement OR TokenObject
      FRAME B            //TokenStatement OR TokenObject
      Goto StateLabel2   //TokenStatement

In a script, shown below.

    