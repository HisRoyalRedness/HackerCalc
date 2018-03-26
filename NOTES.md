HackerCalc Notes
================

Change log
----------

| Date          | Description                                                                                               |
|---------------|-----------------------------------------------------------------------------------------------------------|
| 2018-03-06    | Refactor Coco.atg to make inteter type and bit width seperate tokens                                      |
| 2018-03-08    | Integer working correctly with string value, numeric value, sign and bitwidth.                            |
|               | Float working correctly                                                                                   |
|               | Remove anything thats not dotnet core                                                                     |
|               | Add unit tests                                                                                            |
| 2018-03-09    | Integer productions with attributes for bitwidth and type                                                 |
|               | Sign flag and bitwidth checking in token context (to disallow space between number, sign flag and bit width) |
|               | Added resolver for true_float to differentiate seconds and floats                                         |
|               | Compound time portions working correctly                                                                  |
|               | Unit testing for all types currently implemented                                                          |
| 2018-03-12    | Rename timespan tokens and productions to differentiate then from time values                             |
| 2018-03-13    | Handle dates, times and date times                                                                        |
|               | Better validation of date and time ranges                                                                 |
| 2018-03-15    | Visitor pattern for tokens                                                                                |
|               | Reimplemented to pass an AST of ITokens from a root token, rather than an enumeration of tokens           |
| 2018-03-16    | DateTime unit test                                                                                        |
|               | Fix integers being interpreted as bitwidth literals                                                       |
| 2018-03-19    | More DateTime test                                                                                        |
|               | Start of precedence and grouping tests                                                                    |
| 2018-03-26    | Refactor: Seperate token classes and parsing                                                              |
|               | Tests and implementation for casting between token types                                                  |
|               | Add, subtract, multiply and divide implemented, for all data types                                        |

Todo
----

* Add operation unit tests
* Add tests for TokenPrinter
* Better error handling of tokens that don't parse correctly (e.g. dates and times)
