# Custom Language Definition Extension

A VS Code extension that adds syntax highlighting and language support for a custom language using TextMate grammar.

## Features

- Syntax highlighting for Specification files (.spec files)
- Comment support (line and block comments)
- String and number recognition
- Keyword and identifier highlighting
- Smart indentation rules
- Auto-closing pairs

## Project Structure

- **syntaxes/customlang.tmLanguage.json** - TextMate grammar definition for syntax highlighting
- **language-configuration.json** - Language-specific configuration (comments, brackets, indentation)
- **package.json** - Extension manifest with language contributions
- **src/extension.ts** - Extension activation code
- **.vscode/** - VS Code configuration for debugging and building

## Development

### Prerequisites

- [Node.js](https://nodejs.org/) (v14 or higher)
- [npm](https://www.npmjs.com/) or [yarn](https://yarnpkg.com/)

### Setup

1. Install dependencies:

   ```js
   npm install
   ```

2. Compile TypeScript:

   ```js
   npm run compile
   ```

3. Watch for changes (for development):

   ```js
   npm run watch
   ```

### Testing the Extension

1. Press `F5` to open the Extension Development Host
2. Create or open a `.clang` file to see syntax highlighting in action
3. Edit the grammar file (`syntaxes/customlang.tmLanguage.json`) to customize highlighting rules

## Customization

### Adding Keywords

Edit `syntaxes/customlang.tmLanguage.json` and modify the keyword patterns:

```json
"keyword": {
  "patterns": [
    {
      "name": "keyword.control.customlang",
      "match": "\\b(yourKeyword1|yourKeyword2)\\b"
    }
  ]
}
```

### Modifying File Extensions

In `package.json`, update the language contribution:

```json
"extensions": [".yourext"]
```

### Customizing Comments and Brackets

Edit `language-configuration.json` to define comment styles and bracket pairs.

## Resources

- [VS Code Language Extensions Guide](https://code.visualstudio.com/api/language-extensions/overview)
- [TextMate Grammar Reference](https://macromates.com/manual/en/language_grammars)
- [VS Code Extension API Docs](https://code.visualstudio.com/api)

## License

MIT
