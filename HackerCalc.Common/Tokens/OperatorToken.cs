using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

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
        [Description("%")]
        Modulo,
        [Description("~")]
        Negate,
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

        Cast
    }

    public interface IOperatorToken : IToken
    {
        bool IsUnary { get; }
        IToken Left { get; }
        IToken Right { get; }
    }

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

        public static OperatorToken Parse(string value)
        {
            switch (value)
            {
                case "!": return new OperatorToken(OperatorType.Not, true);
                case "~": return new OperatorToken(OperatorType.Negate, true);
                case "*": return new OperatorToken(OperatorType.Multiply);
                case "/": return new OperatorToken(OperatorType.Divide);
                case "\\": return new OperatorToken(OperatorType.Divide);
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

    public class CastOperatorToken : OperatorToken
    {
        public CastOperatorToken(TokenDataType castToType)
            : base(OperatorType.Cast, true)
        {
            CastToType = castToType;
        }

        public TokenDataType CastToType { get; private set; }

        public static new CastOperatorToken Parse(string value)
        {
            switch (value.ToLower())
            {
                case "(i4)":
                case "(i8)":
                case "(i16)":
                case "(i32)":
                case "(i64)":
                case "(i128)":
                case "(u4)":
                case "(u8)":
                case "(u16)":
                case "(u32)":
                case "(u64)":
                case "(u128)":
                    return new CastOperatorToken(TokenDataType.Integer);

                case "(f)":
                case "(fl)":
                case "(flt)":
                case "(float)":
                    return new CastOperatorToken(TokenDataType.Float);

                case "(ts)":
                case "(timespan)":
                    return new CastOperatorToken(TokenDataType.Timespan);

                case "(t)":
                case "(ti)":
                case "(time)":
                    return new CastOperatorToken(TokenDataType.Time);

                case "(d)":
                case "(dt)":
                case "(date)":
                    return new CastOperatorToken(TokenDataType.Date);

                default: throw new ParseException($"Unrecognised cast operator type {value}.");
            }
        }

        public override string ToString() => $"{Operator.GetEnumDescription()}";
    }
}
