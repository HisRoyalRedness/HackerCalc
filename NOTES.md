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
| 2018-03-15    | Visitor pattern for tokens                                                                               |

Todo
----

* Literal bitwidths  interpretted as decimals
* Reimplement ATG to pass IToken to the root token
* Unit tests for date time
* Better error handling of tokens that don't parse correctly (e.g. dates and times)
