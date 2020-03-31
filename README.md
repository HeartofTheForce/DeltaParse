# What is DeltaParse?
`DeltaParse` is a simple template based data extraction tool, it is designed to be a part of a tool chain.
# How it works
Given a template string with tokens matching `{{\\w*?}}` data is extracted using Google's [diff-match-patch](https://github.com/google/diff-match-patch)
```
string text = "The quick brown fox jumps over the lazy dog";
string template = "The quick {{colour}} fox jumps over the lazy {{animal}}";

var parser = new Parser<SimpleResult>(template, new StandardProcessor(), new SimpleOutputBuilder());
var results = parser.Parse(text);
```
Results in
```
{
  "ParsedValues": {
    "colour": [
      "brown"
    ],
    "animal": [
      "dog"
    ]
  },
  "Difference": 0
}
```

`Difference` is a rough measurement on a scale of 0 -> 1 of how large a difference there is between the template and the text, excluding any matched data.

# What is DeltaParse not?
`DeltaParse` is not a full data extraction library, while it can handle a wide range of inputs it can only parse them naively, it does not support more advanced features like parsing inputs with variable amount of tokens to extract

e.g. parsing a datatable with `n` rows, where `n` cannot be determined when creating the templates is unfeasible, instead chaining `DeltaParse` together with other text processig tools such as Regex or HTML Selectors will give better results.

# StandardProcessor vs MungeProcessor
Standard processor uses regex to find and prepare template tokens for parsing and should work fine in the majority of situations.

Munge processor guarantees that only template tokens in the template text will be considered when extracting data, it does however require more memory/processing time.

Using MungeProcessor as follows
```
string text = "The quick {{adjective}} brown fox {{verb}} over the lazy dog";
string template = "The quick {{adjective}} brown fox {{verb}} over the lazy dog";

var parser = new Parser<SimpleResult>(template, new MungeProcessor(), new SimpleOutputBuilder());
var results = parser.Parse(text);
```
Results in
```
{
  "ParsedValues": {
    "adjective": [
      "{{adjective}}"
    ],
    "verb": [
      "{{verb}}"
    ]
  },
  "Difference": 0
}
```
