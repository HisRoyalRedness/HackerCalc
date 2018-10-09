using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

/*
    Base type for all operators

        Some special operator types included, e.g. GroupingToken

    Keith Fletcher
    Oct 2018

    This file is Unlicensed.
    See the foot of the file, or refer to <http://unlicense.org>
*/

namespace HisRoyalRedness.com
{
    public enum OperatorType
    {
        [Description("+")]
        Add,
        [Description("-")]
        Subtract,
        [Description("*")]
        Multiply,
        [Description("/")]
        Divide,
        [Description("**")]
        Power,
        [Description("//")]
        Root,
        [Description("%")]
        Modulo,
        [Description("~")]
        BitwiseNegate,          // i.e. 2's complement
        [Description("!-")]
        NumericNegate,          // i.e. value * -1
        [Description("<<")]
        LeftShift,
        [Description(">>")]
        RightShift,
        [Description("&")]
        And,
        [Description("|")]
        Or,
        [Description("!")]
        Not,
        [Description("^")]
        Xor,

        Cast,
        Grouping
    }

    public interface IOperatorToken : IToken
    {
        bool IsUnary { get; }
        IToken Left { get; }
        IToken Right { get; }
        OperatorType Operator { get; }
    }

    #region OperatorToken
    public class OperatorToken : TokenBase<OperatorToken>, IOperatorToken
    {
        public OperatorToken(OperatorType op, bool isUnary = false)
            : base()
        {
            Operator = op;
            IsUnary = isUnary;
        }

        public OperatorType Operator { get; private set; }

        public bool IsUnary { get; private set; }

        public IToken Left { get; set; }
        public IToken Right { get; set; }

        public static OperatorToken ParseNegate(string value)
        {
            switch (value)
            {
                case "!": return new OperatorToken(OperatorType.Not, true);
                case "~": return new OperatorToken(OperatorType.BitwiseNegate, true);
                case "-": return new OperatorToken(OperatorType.NumericNegate, true);
                default: throw new ParseException($"Unrecognised operator {value}.");
            }
        }

        public static OperatorToken Parse(string value)
        {
            switch (value)
            {
                case "!": return new OperatorToken(OperatorType.Not, true);
                case "~": return new OperatorToken(OperatorType.BitwiseNegate, true);
                case "*": return new OperatorToken(OperatorType.Multiply);
                case "/": return new OperatorToken(OperatorType.Divide);
                case "\\": return new OperatorToken(OperatorType.Divide);
                case "**": return new OperatorToken(OperatorType.Power);
                case "//": return new OperatorToken(OperatorType.Root);
                case "\\\\": return new OperatorToken(OperatorType.Root);
                case "%": return new OperatorToken(OperatorType.Modulo);
                case "+": return new OperatorToken(OperatorType.Add);
                case "-": return new OperatorToken(OperatorType.Subtract);
                case "<<": return new OperatorToken(OperatorType.LeftShift);
                case ">>": return new OperatorToken(OperatorType.RightShift);
                case "&": return new OperatorToken(OperatorType.And);
                case "|": return new OperatorToken(OperatorType.Or);
                case "^": return new OperatorToken(OperatorType.Xor);
                default: throw new ParseException($"Unrecognised operator {value}.");
            }
        }

        public override string ToString() => $"{Operator.GetEnumDescription()}";
    }
    #endregion OperatorToken

    #region GroupingToken
    public class GroupingToken : TokenBase<GroupingToken>, IOperatorToken
    {
        public GroupingToken(IToken childToken)
            : base()
        {
            Left = childToken;
        }

        public bool IsUnary => true;
        public IToken Left { get; private set; }
        public IToken Right => null;
        public OperatorType Operator => OperatorType.Grouping;
    }
    #endregion GroupingToken

    //public class CastOperatorToken : OperatorToken
    //{
    //    public CastOperatorToken(TokenDataType castToType)
    //        : base(OperatorType.Cast, true)
    //    {
    //        CastToType = castToType;
    //    }

    //    public TokenDataType CastToType { get; private set; }

    //    public bool IsSigned { get; private set; }
    //    public OldLimitedIntegerToken.IntegerBitWidth BitWidth { get; private set; }

    //    public static CastOperatorToken UnlimitedIntegerCast() => new CastOperatorToken(TokenDataType.UnlimitedInteger);
    //    public static CastOperatorToken LimitedIntegerCast(OldLimitedIntegerToken.IntegerBitWidth bitWidth, bool isSigned = true) 
    //        => new CastOperatorToken(TokenDataType.LimitedInteger) { BitWidth = bitWidth, IsSigned = isSigned };
    //    public static CastOperatorToken FloatCast() => new CastOperatorToken(TokenDataType.Float);
    //    public static CastOperatorToken TimespanCast() => new CastOperatorToken(TokenDataType.Timespan);
    //    public static CastOperatorToken TimeCast() => new CastOperatorToken(TokenDataType.Time);
    //    public static CastOperatorToken DateCast() => new CastOperatorToken(TokenDataType.Date);

    //    public override string ToString() => $"{Operator.GetEnumDescription()}";
    //}
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
