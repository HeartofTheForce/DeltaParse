# What is DeltaParse?
`DeltaParse` is a simple template based data extraction library.

# How it works
Given a template string with tokens matching `{{\\w*?}}` data is extracted using Google's [diff-match-patch](https://github.com/google/diff-match-patch)
```
string text = "The quick brown fox jumps over the lazy dog";
string template = "The quick {{colour}} fox jumps over the lazy {{animal}}";

var parser = new Parser(template, new StandardProcessor());
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

# StandardProcessor vs MungeProcessor
Standard processor uses regex to find and prepare template tokens for parsing and should work fine in the majority of situations.

Munge processor guarantees that only template tokens in the template text will be considered when extracting data, it does however require more memory/processing time.

Using MungeProcessor as follows
```
string  text  =  "The quick {{adjective}} brown fox {{verb}} over the lazy dog";
string  template  =  "The quick {{adjective}} brown fox {{verb}} over the lazy dog";

var  parser  =  new  Parser(template, new  MungeProcessor());
var  results  =  parser.Parse(text);
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
