HackerCalc
==========

A basic calculator application that supports both 'numeric' and 'programmer' calculation modes.
Numeric handles the basic math operations that you'd expect from most calculators.
Programmer mode works numbers is different bases (binary, octal and hexadecimal), and supports
math based on a particular bit width.

The application supports the mixing of these two modes to some degree. Expressions can be entered
without having to explicitly set the calculater mode beforehand.

Basic date and time calculations are also supported.

Literals
-------------

 * **Integers**  
   * Numeric base
     * **``b1011010``**       Binary
     * **``o147``**           Octal
     * **``12345``**          Decimal
     * **``0x3c``**           Hexadecimal
   * Unlimited integer. Can be positive or negative. No bitwidth limit
     * e.g. ``12345`` or ``-12345``
   * Limited integer. Defined as signed or unsigned. Limited to a particular bitwidth
     * **i | u**          Signed or unsigned
     * Supported bitwidths are 4, 8, 16, 32, 64, 128
     * e.g. ``-12i32``, ``34u8``, ``0x56i64``
 * **Floats**
   * Floating point type
     * e.g. ``1.2``, or ``3f``
 * **Dates**
   * Date only, or date and time
     * Support date formats 'yyyy-MM-dd', 'yy-MM-dd' or 'dd-MM-yyyy'
     * Can use either '-' or '/' as a separator for date elements
     * Support time formats 'HH:mm' or 'HH:mm:ss'
     * e.g. ``2018-02-05``, or ``2018/02/05 11:48``
 * **Times**
   * Time portion only
     * Support time formats 'HH:mm' or 'HH:mm:ss'
     * e.g. ``11:48``, or ``11:48:56``
 * **Timespans**
   * A period of time
     * Supports these time units: days, hours, minutes, seconds
     * e.g. ``1 day 2 hours``, or ``16 hours 2 mins 1 sec``

Operators 
---------
Listed in order of precedence

 * **``(``, ``)``** Brackets
 * **``(<type>)<value>``** Type casting, e.g. convert an integer to a float: ``(float)2``
 * **``!``, ``~``, ``-``** Logical not, bitwise negate, numeric negate
 * **<code>&ast;&ast;</code>, ``//``** Power and root
 * **``*``, ``/``, ``%``** Multiply, divide and modulo
 * **``+``, ``-``** Add and subtract
 * **``<<``, ``>>``** Left shift and right shift
 * **``&``, ``|``, ``^``** AND, OR and XOR

 * [Usual arithmetic conversions](http://c0x.coding-guidelines.com/6.3.1.8.html)
 * [Supported operations](http://en.cppreference.com/w/cpp/language/operator_arithmetic)
 * [Order of operations](http://en.cppreference.com/w/cpp/language/operator_precedence)

Change log
----------

| Date       | Description                                                                                                  |
|------------|--------------------------------------------------------------------------------------------------------------|
| 2018-10-08 | Refactor again! Decided to make a split between tokens that are parsed (i.e. the basic literals types and    |
| ..         | operators), and the objects that are used for calculation, type casting etc.                                 |
| ..         | This way we can develop and test the parsing and calculation operators mostly independantly.                 |
| 2018-10-10 | All literal and operator parsing completed. Added parsing support for functions.                             |
| 2018-10-11 | Made a start on the calculation engine.                                                                      |
| 2018-10-12 | Tweak the visitor pattern interface a little, and refactor TokenPrinter. Add printer tests                   |

Todo
----

* Parsing
  * ~~Get basic literal types parsed~~
    * ~~LimitedInteger~~
    * ~~UnlimitedInteger~~
    * ~~Float~~
    * ~~Date~~
    * ~~Time~~
    * ~~Timespan~~
  * ~~Handle basic operators~~
  * ~~Support for functions~~
  * ~~Make sure all failed parsing raises a ParseException (rather than some other exception)~~
  * Perhaps support timezone offsets on date/times??
* Calculation
  * Get the calculation engine design sorted
  * Implement all data types
   * ~~LimitedIntegerType~~
     * ~~Sign and bitwidth~~
   * ~~UnlimitedIntegerType~~
   * ~~DateType~~
   * ~~FloatType~~
   * ~~TimeType~~
   * ~~TimespanType~~
  * Implement data mappers from literal tokens to data types
   * Make sure limited integers map signs and bitwidths
   * Add unit tests
  * Implement operations on all data types
  * Implement casting to/from data types
   * Full implementation of cast from UnlimitedIntegerType to LimitedIntegerType
* Remove old code
* Put licence headers and footers on all source code


License
-------

This is free and unencumbered software released into the public domain.

Anyone is free to copy, modify, publish, use, compile, sell, or
distribute this software, either in source code form or as a compiled
binary, for any purpose, commercial or non-commercial, and by any
means.

In jurisdictions that recognize copyright laws, the author or authors
of this software dedicate any and all copyright interest in the
software to the public domain. We make this dedication for the benefit
of the public at large and to the detriment of our heirs and
successors. We intend this dedication to be an overt act of
relinquishment in perpetuity of all present and future rights to this
software under copyright law.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
IN NO EVENT SHALL THE AUTHORS BE LIABLE FOR ANY CLAIM, DAMAGES OR
OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE,
ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
OTHER DEALINGS IN THE SOFTWARE.

For more information, please refer to <http://unlicense.org>
