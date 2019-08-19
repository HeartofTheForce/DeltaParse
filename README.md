# What is DeltaParse?
`DeltaParse` is a simple template based data extraction library.

# How it works
Given a template string with tokens matching `{{\\w*?}}` data is extracted using Google's [diff-match-patch](https://github.com/google/diff-match-patch)

`The quick {{colour}} fox jumps over the lazy {{animal}}`

applied to

`The quick brown fox jumps over the lazy dog` will return
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
