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

Todo
----

* Differentiate between timestamp values and time (local and universal) values
* Add validation to ensure timespan digits are within range (e.g. 1 - 24, 0 - 59 etc).
