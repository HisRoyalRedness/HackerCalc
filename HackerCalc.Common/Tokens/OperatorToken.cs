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
        [Description("!~")]
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
        [Description("!!")]
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

        public bool IsSigned { get; private set; }
        public IntegerToken.IntegerBitWidth BitWidth { get; private set; }

        public static CastOperatorToken IntegerCast(bool isSigned = true, IntegerToken.IntegerBitWidth bitWidth = IntegerToken.IntegerBitWidth.Unbound) 
            => new CastOperatorToken(TokenDataType.Integer) { IsSigned = isSigned, BitWidth = bitWidth };
        public static CastOperatorToken FloatCast() => new CastOperatorToken(TokenDataType.Float);
        public static CastOperatorToken TimespanCast() => new CastOperatorToken(TokenDataType.Timespan);
        public static CastOperatorToken TimeCast() => new CastOperatorToken(TokenDataType.Time);
        public static CastOperatorToken DateCast() => new CastOperatorToken(TokenDataType.Date);

        public override string ToString() => $"{Operator.GetEnumDescription()}";
    }
}
