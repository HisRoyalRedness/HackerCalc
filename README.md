<img src="HackerCalcIcon.ico" alt="HackerCalc Icon" width="128" height="128">

HackerCalc
==========

A basic calculator application that supports both 'numeric' and 'programmer' calculation modes.
Numeric handles the basic math operations that you'd expect from most calculators.
Programmer mode works with numbers in different bases (binary, octal and hexadecimal), and supports
math based on a particular bit width.

The application supports the mixing of these modes to some degree. Expressions can be entered
without having to explicitly set the calculator mode beforehand.

Basic date and time calculations are also supported.

Literals
-------------
 * **Numbers**
   * Numeric base
     * **``b1011010``**       Binary
     * **``o147``**           Octal
     * **``12345``**          Decimal
     * **``0x3c``**           Hexadecimal
   * Rational numbers. Can be positive or negative. No lower or upper limit
     * e.g. ``12345``, ``-123.456``, ``0xfe56``
   * Integers. Defined as signed or unsigned.
     * **i | u**          Signed or unsigned
     * Supported bitwidths are 4, 8, 16, 32, 64, 128, or unlimited (use bitwidth 0)
     * e.g. ``-12i32``, ``34u8``, ``0x56i64``, ``12345i0``
	 * Unlimited integers are always signed, i.e. there is no ``12345u0``
 * **Dates**
   * Date only, or date and time
     * Support date formats 'yyyy-MM-dd', 'yy-MM-dd' or 'dd-MM-yyyy'
     * Can use either '-' or '/' as a separator for date elements
     * Support time formats 'HH:mm' or 'HH:mm:ss'
     * e.g. ``2018-02-05``, or ``2018/02/05 11:48``
     * **``NOW``** returns the current date and time
 * **Times**
   * Time portion only
     * Support time formats 'HH:mm', 'HH:mm:ss' or 'HH:mm:ss.fff'
     * e.g. ``11:48``, or ``11:48:56``, or ``11:48:56.8``
     * **``TIME``** returns the current time
     * Time types can be configured to allow days, in the format 'd.HH:mm', 'd.HH:mm:ss' or 'd.HH:mm:ss.fff'
     * <mark>Negative times?</mark>
 * **Timespans**
   * A period of time
     * Supports these time units: days, hours, minutes, seconds and milliseconds
     * e.g. ``1 day 2 hours``, or ``16 hours 2 mins 1 sec 20ms``
     * The time portions need not be whole numbers, e.g. ``16.1 hours 2.6 mins 1.03 sec``

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

Functions
---------
Still to design and implement. These are ideas that could be supported

 * Trig (sin, cos, tan)
 * Logs (Support different bases?)
 * Factorial
 * Constants (pi, e, etc.)
 * Stats ()

Todo
----

* ~~Parsing~~
  * <mark>Perhaps support timezone offsets on date/times??</mark>
* Calculation
  * Set state of calculations (over or under flow)
  * Implement CalcEngine operations
   * **Add** ~~implemented and tested~~
   * **Subtract** ~~implemented and tested~~
   * **Multiply** ~~implemented and tested~~
   * **Divide** ~~implemented and tested~~
   * **Power**
   * **Root**
   * **Modulo**
   * **LeftShift**
   * **RightShift**
   * **And**
   * **Or**
   * **Xor**
   * **BitwiseNegate**
   * **NumericNegate**
   * **Not**
  * Implement IComparable and IEquatable on data types, with testing
   * Limited Integers. <mark>Should it consider bitwidth and sign?</mark>
  * ~~Implement casting to/from data types~~
   * ~~Full implementation of cast from UnlimitedIntegerType to LimitedIntegerType~~
     * Casting to a specific bitwidth and sign?
* Find a more accurate way to do power and root
* Remember left shift and right shift on integers is limited by number of bits. Negative shifts aren't intuitive. Maybe block negative shifts? Shifts on BigIntegers are limited to 16 bits
* Make sure left and right shifts make sense for the bitwidth of the LimitedIntegerTypes
* <mark>Add tests for IConfiguration and ICalcState</mark>
* <mark>Add calc for colour (convert RGB to LAB etc., add/remove luminance)</mark>
* <mark>Add tests for Irrational</mark>

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
