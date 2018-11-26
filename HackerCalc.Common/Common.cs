using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/*
    Common enums used throughout the application

    Keith Fletcher
    Oct 2018

    This file is Unlicensed.
    See the foot of the file, or refer to <http://unlicense.org>
*/

namespace HisRoyalRedness.com
{
    public enum IntegerBase
    {
        Binary,
        Octal,
        Decimal,
        Hexadecimal
    }

    public enum OperatorType
    {
        // Binary operators
        [BinaryOperator]
        [Description("+")]
        Add,
        [BinaryOperator]
        [Description("-")]
        Subtract,
        [BinaryOperator]
        [Description("*")]
        Multiply,
        [BinaryOperator]
        [Description("/")]
        Divide,
        [BinaryOperator]
        [Description("**")]
        Power,
        [BinaryOperator]
        [Description("//")]
        Root,
        [BinaryOperator]
        [Description("%")]
        Modulo,
        [BinaryOperator]
        [Description("<<")]
        LeftShift,
        [BinaryOperator]
        [Description(">>")]
        RightShift,
        [BinaryOperator]
        [Description("&")]
        And,
        [BinaryOperator]
        [Description("|")]
        Or,
        [BinaryOperator]
        [Description("^")]
        Xor,

        // Unary operators
        [UnaryOperator]
        [Description("~")]
        BitwiseNegate,          // i.e. 1's complement
        [UnaryOperator]
        [Description("!-")]
        NumericNegate,          // i.e. value * -1
#if SUPPORT_NOT
        [UnaryOperator]
        [Description("!")]
        Not,
#endif

        // Operators that aren't really operators
        [DontEnumerate]
        Cast,
        [DontEnumerate]
        [UnaryOperator]
        Grouping
    }
}

/*
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
*/
