HackerCalc Notes
================

Change log
----------

| Date          | Description                                                                                               |
|---------------|-----------------------------------------------------------------------------------------------------------|
| 2018-03-06    | Refactor Coco.atg to make inteter type and bit width seperate tokens                                      |
| 2018-03-08    | Integer working correctly with string value, numeric value, sign and bitwidth.                            |
|               | Float working correctly                                                                                   |
|               | Remove anything thats nt dotnet core                                                                      |
|               | Add unit tests                                                                                            |

Todo
----

* Add integer productions with attributes for bitwidth and type
* Do type checking in scanner definition so we don't allow whitespace between a numeric and it's type
* Add validation to ensure timespan digits are within range (e.g. 1 - 24, 0 - 59 etc).
* Implement full set of unit tests