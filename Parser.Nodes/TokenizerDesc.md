# Match rule, Split to tokens

1. Rules are sequences of regex groups `('int_tokentype'\d+) (?'op'[+-*/|&]) ('int_tokentype'\d+)`
1. They are split into 1 token for each capture, into a token that stores: position, value, and type
1. Each token, when the type is encountered has a command that can assemble, modify, or save data in nodes.

# Match rule, Split to Groups

1. Rules are sequences of regex groups `('int_tokentype'\d+) (?'op'[+-*/|&]) ('int_tokentype'\d+)`
1. They are split into 1 token for each group, into a token that stores: IEnumerable[position], IEnumerable[value], and type
1. Each token, when the type is encountered has a command that can assemble, modify, or save data in nodes.

# Match rule, Groups are properties

1. Rules are regex strings.
1. Each match is a token, with each group as a property. If there are multiple captures, the property is an ImmutableCollection[string]
1. Each token is interpreted into a node, with minimal processing commands.

# Match rule, Groups are properties (With Preprocessing and special Groups)

1. Rules are regex strings, with additional properties, like ignore case, or discard, or do not consume
1. Each match is a token, with each capure as a property. If there are multiple captures, a "_1" or "_2" is appended to the group, with special handling so that certain groups mean extra things, like "_t_" prefixing the group assigns the type of the token, and _x_ means discard this group, _d_ means descend hierarchy, _a_ means ascend, etc.
1. Input text has operations that can recurrsively replace certain things with others, (#define preprocessor, removing commments, reducing whitespace, etc.)
1. Each token contains the information that is needed to produce a node, node processing can be defined in the token rules.