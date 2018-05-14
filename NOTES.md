HackerCalc Notes
================

Number syntax
-------------

 * **Integers**  
   * Numeric base
     * **``12345``**          Decimal
     * **``0x3c``**           Hexadecimal
     * **``b1011010``**       Binary
   * Unlimited integer. Can be positive or negative. No bitwidth limit
     * e.g. ``12345``
   * Limited integer. Defined as signed or unsigned. Limited to a particular bitwidth
     * **i | u**          Signed or unsigned
     * Supported bitwidths are 4, 8, 16, 32, 64, 128
     * e.g. ``12i32``, ``34u8``
 * ** Floats **
   * Floating point type
     * e.g. ``1.2``, or ``3f``
 * ** Dates **
   * Date only, or date and time
     * Support date formats 'yyyy-MM-dd', 'yy-MM-dd' or 'dd-MM-yyyy'
     * Support time formats 'HH:mm' or 'HH:mm:ss'
     * e.g. ``2018-02-05``, or ``2018-02-05 11:48``
 * ** Times **
   * Time portion only
     * Support time formats 'HH:mm' or 'HH:mm:ss'
     * e.g. ``11:48``, or ``11:48:56``
 * ** Timespans **
   * A period of time
     * Supports these time units: days, hours, minutes, seconds
     * e.g. ``1 day 2 hours``, or ``16 hours 2 mins 1 sec``

Operators 
---------
Listed in order of precedence

 * ** ``(``, ``)`` ** Brackets
 * ** ``(<type>)<value>`` ** Type casting, e.g. convert an integer to a float: ``(float)2``
 * ** ``!``, ``~``, ``-`` ** Logical not, bitwise negate, numeric negate
 * ** <code>&ast;&ast;</code>, ``//`` ** Power and root
 * ** ``*``, ``/``, ``%`` ** Multiply, divide and modulo
 * ** ``+``, ``-`` ** Add and subtract
 * ** ``<<``, ``>>`` ** Left shift and right shift
 * ** ``&``, ``|``, ``^`` ** AND, OR and XOR

 * [Usual arithmetic conversions](http://c0x.coding-guidelines.com/6.3.1.8.html)
 * [Supported opertions](http://en.cppreference.com/w/cpp/language/operator_arithmetic)
 * [Order of operations](http://en.cppreference.com/w/cpp/language/operator_precedence)

Change log
----------

| Date       | Description                                                                                                  |
|------------|--------------------------------------------------------------------------------------------------------------|
| 2018-03-06 | Refactor Coco.atg to make integer type and bit width seperate tokens                                         |
| 2018-03-08 | Integer working correctly with string value, numeric value, sign and bitwidth.                               |
|            | Float working correctly                                                                                      |
|            | Remove anything thats not dotnet core                                                                        |
|            | Add unit tests                                                                                               |
| 2018-03-09 | Integer productions with attributes for bitwidth and type                                                    |
|            | Sign flag and bitwidth checking in token context (to disallow space between number, sign flag and bit width) |
|            | Added resolver for true_float to differentiate seconds and floats                                            |
|            | Compound time portions working correctly                                                                     |
|            | Unit testing for all types currently implemented                                                             |
| 2018-03-12 | Rename timespan tokens and productions to differentiate then from time values                                |
| 2018-03-13 | Handle dates, times and date times                                                                           |
|            | Better validation of date and time ranges                                                                    |
| 2018-03-15 | Visitor pattern for tokens                                                                                   |
|            | Reimplemented to pass an AST of ITokens from a root token, rather than an enumeration of tokens              |
| 2018-03-16 | DateTime unit test                                                                                           |
|            | Fix integers being interpreted as bitwidth literals                                                          |
| 2018-03-19 | More DateTime test                                                                                           |
|            | Start of precedence and grouping tests                                                                       |
| 2018-03-26 | Refactor: Seperate token classes and parsing                                                                 |
|            | Tests and implementation for casting between token types                                                     |
|            | Add, subtract, multiply and divide implemented, for all data types                                           |
| 2018-03-27 | Added type cast to parser                                                                                    |
| 2018-03-29 | Added equality                                                                                               |
| 2018-04-19 | Add support of incomplete equations (defined through compiler directive INCOMPLETE_EQ)                       |
| 2018-04-23 | Skeleton WPF front-end                                                                                       |
|            | Changed Parser and Common to .Net Standard                                                                   |
|            | Implement operator overrides to supported operations on literal tokens                                       |
|            | Implement comparison                                                                                         |
|            | Integer token normalisation and tests                                                                        |
| 2018-04-24 | Started IntegerToken normalisation                                                                           |
| 2018-05-04 | Added new integer token types.                                                                               |
|            | Refactored token evaluation                                                                                  |
|            | Added power and root operators (parsing)                                                                     |
|            | Partial support for parsing functions                                                                        |
| 2018-05-07 | Begin switching over to LimitedIntegerToken and removing IntegerToken                                        |
| 2018-05-14 | Implemented casting from Unlimited to Limited                                                                |

Todo
----

* Test timezones in date parsing

* In the process of adding the operation type result maps
* Complete the implementation of functions
* Implement all the integer types
* Repair unit tests


* Throw errors on overflow if specified in EvaluatorSettings
* Complete IntegerToken casting for sign and bitwidth
* Base all thrown exceptions of ApplicationException
* Get equality and comparison working for IntegerTokens, to account for sign and bitwidth
* Implement comparison tests
* Casting for bound integers (and bitwidths)
* Add operation unit tests
* Add tests for TokenPrinter
* Better error handling of tokens that don't parse correctly (e.g. dates and times)
