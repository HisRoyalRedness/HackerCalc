HackerCalc Notes
================

Change log
----------

| Date          | Description                                                                                               |
|---------------|-----------------------------------------------------------------------------------------------------------|
| 2018-03-06    | Refactor Coco.atg to make inteter type and bit width seperate tokens                                      |
| 2018-03-08    | Integer working correctly with string value, numeric value, sign and bitwidth.                            |
|               | Float working correctly                                                                                   |

Todo
----

* Do type checking in scanner definition so we don't allow whitespace between a numeric and it's type
* Add validation to ensure timespan digits are within range (e.g. 1 - 24, 0 - 59 etc).
* Get dotnet test working properly