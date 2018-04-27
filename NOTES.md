HackerCalc Notes
================

Change log
----------

| Date          | Description                                                                                                  |
|---------------|--------------------------------------------------------------------------------------------------------------|
| 2018-03-06    | Refactor Coco.atg to make integer type and bit width seperate tokens                                         |
| 2018-03-08    | Integer working correctly with string value, numeric value, sign and bitwidth.                               |
|               | Float working correctly                                                                                      |
|               | Remove anything thats not dotnet core                                                                        |
|               | Add unit tests                                                                                               |
| 2018-03-09    | Integer productions with attributes for bitwidth and type                                                    |
|               | Sign flag and bitwidth checking in token context (to disallow space between number, sign flag and bit width) |
|               | Added resolver for true_float to differentiate seconds and floats                                            |
|               | Compound time portions working correctly                                                                     |
|               | Unit testing for all types currently implemented                                                             |
| 2018-03-12    | Rename timespan tokens and productions to differentiate then from time values                                |
| 2018-03-13    | Handle dates, times and date times                                                                           |
|               | Better validation of date and time ranges                                                                    |
| 2018-03-15    | Visitor pattern for tokens                                                                                   |
|               | Reimplemented to pass an AST of ITokens from a root token, rather than an enumeration of tokens              |
| 2018-03-16    | DateTime unit test                                                                                           |
|               | Fix integers being interpreted as bitwidth literals                                                          |
| 2018-03-19    | More DateTime test                                                                                           |
|               | Start of precedence and grouping tests                                                                       |
| 2018-03-26    | Refactor: Seperate token classes and parsing                                                                 |
|               | Tests and implementation for casting between token types                                                     |
|               | Add, subtract, multiply and divide implemented, for all data types                                           |
| 2018-03-27    | Added type cast to parser                                                                                    |
| 2018-03-29    | Added equality                                                                                               |
| 2018-04-19    | Add support of incomplete equations (defined through compiler directive INCOMPLETE_EQ)                       |
| 2018-04-23    | Skeleton WPF front-end                                                                                       |
|               | Changed Parser and Common to .Net Standard                                                                   |
|               | Implement operator overrides to supported operations on literal tokens                                       |
|               | Implement comparison                                                                                         |
|               | Integer token normalisation and tests                                                                        |
| 2018-04-24    | Started IntegerToken normalisation                                                                           |

Todo
----

* Implement all the integer types
* Repair unit tests


* Throw errors on overflow if specified in EvaluatorSettings
* Complete IntegerToken casting for sign and bitwidth
* Base all thrown exceptions of ApplicationException
* Handle the negative sign (e.g. -1)
* Get equality and comparison working for IntegerTokens, to account for sign and bitwidth
* Implement comparison tests
* Implement cast operator
* Implement other operators
* Casting for bound integers (and bitwidths)
* Add operation unit tests
* Add tests for TokenPrinter
* Better error handling of tokens that don't parse correctly (e.g. dates and times)
