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
        Xor
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
}
