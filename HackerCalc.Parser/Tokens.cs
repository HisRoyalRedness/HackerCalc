﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Numerics;
using System.Text;
using System.Linq;

namespace HisRoyalRedness.com
{
    public interface IToken
    {
        void Accept(ITokenVisitor visitor);
        TAggregate Aggregate<TAggregate>(ITokenVisitor<TAggregate> visitor);
    }

    public enum TokenType
    {
        // Operators
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

        // Literals
        Float,
        Integer,
        Timespan,
        Time
    }

    #region Token base
    public abstract class TokenBase<TToken> : IToken
        where TToken : class, IToken
    {
        protected TokenBase(string rawToken = null)
        {
            RawToken = rawToken;
        }

        public virtual void Accept(ITokenVisitor visitor)
            => visitor.Visit<TToken>(this as TToken);

        public TAggregate Aggregate<TAggregate>(ITokenVisitor<TAggregate> visitor)
            => visitor.VisitAndAggregate<TToken>(this as TToken);

        public string RawToken { get; private set; }

        public override string ToString() => $"{RawToken}";
    }
    #endregion Token base

    #region Operator token
    public interface IOperatorToken : IToken
    {
        bool IsUnary { get; }
        IToken Left { get; }
        IToken Right { get; }
    }

    public class OperatorToken : TokenBase<OperatorToken>, IOperatorToken
    {
        public OperatorToken(TokenType op, bool isUnary = false)
            : base()
        {
            Operator = op;
            IsUnary = isUnary;
        }

        public TokenType Operator { get; private set; }

        public bool IsUnary { get; private set; }

        public IToken Left { get; set; }
        public IToken Right { get; set; }

        public static OperatorToken Parse(string value)
        {
            switch (value)
            {
                case "!": return new OperatorToken(TokenType.Not, true);
                case "~": return new OperatorToken(TokenType.Negate, true);
                case "*": return new OperatorToken(TokenType.Multiply);
                case "/": return new OperatorToken(TokenType.Divide);
                case "\\": return new OperatorToken(TokenType.Divide);
                case "%": return new OperatorToken(TokenType.Modulo);
                case "+": return new OperatorToken(TokenType.Add);
                case "-": return new OperatorToken(TokenType.Subtract);
                case "<<": return new OperatorToken(TokenType.LeftShift);
                case ">>": return new OperatorToken(TokenType.RightShift);
                case "&": return new OperatorToken(TokenType.And);
                case "|": return new OperatorToken(TokenType.Or);
                case "^": return new OperatorToken(TokenType.Xor);
                default: throw new ParseException($"Unrecognised operator {value}.");
            }
        }

        public override string ToString() => $"{Operator.GetEnumDescription()}";
    }
    #endregion Operator token

    #region Literal tokens
    public interface ILiteralToken : IToken
    {
        object ObjectValue { get; }
    }

    public interface ILiteralToken<T> : ILiteralToken
    {
        T TypedValue { get; }
    }

    #region LiteralToken
    public class LiteralToken<T> : TokenBase<LiteralToken<T>>, ILiteralToken<T>
    {
        public LiteralToken(TokenType dataType, string value, T typedValue)
            : base()
        {
            DataType = dataType;
            Value = value;
            TypedValue = typedValue;
        }

        public TokenType DataType { get; private set; }
        public string Value { get; private set; }
        public bool IsFloat => DataType == TokenType.Float;
        public bool IsInteger => DataType == TokenType.Integer;
        public T TypedValue { get; private set; }
        public object ObjectValue => TypedValue;

        public override string ToString() => $"{Value}";
    }
    #endregion LiteralToken

    #region IntegerToken
    public class IntegerToken : LiteralToken<BigInteger>
    {
        public enum IntegerBitWidth
        {
            [Description("4")]
            _4 = 4,
            [Description("8")]
            _8 = 8,
            [Description("16")]
            _16 = 16,
            [Description("32")]
            _32 = 32,
            [Description("64")]
            _64 = 64,
            [Description("128")]
            _128 = 128
        }

        public IntegerToken(string value, BigInteger typedValue, bool isSigned = true, IntegerBitWidth bitWidth = IntegerBitWidth._32)
            : base(TokenType.Integer, value, typedValue)
        {
            IsSigned = isSigned;
            BitWidth = bitWidth;
        }

        public static IntegerToken Parse(string value, bool isHex, bool isSigned = true, IntegerBitWidth bitWidth = IntegerBitWidth._32)
            => isHex
                ? new IntegerToken(value, BigInteger.Parse(value.Replace("0x", "00").Replace("0X", "00"), NumberStyles.HexNumber), isSigned, bitWidth)
                : new IntegerToken(value, BigInteger.Parse(value, NumberStyles.Integer), isSigned, bitWidth);

        public bool IsSigned { get; private set; }
        public IntegerBitWidth BitWidth { get; private set; }
        public static IntegerBitWidth ParseBitWidth(string bitWidth)
        {
            switch (bitWidth)
            {
                case "4": return IntegerBitWidth._4;
                case "8": return IntegerBitWidth._8;
                case "16": return IntegerBitWidth._16;
                case "":
                case null: // the default
                case "32": return IntegerBitWidth._32;
                case "64": return IntegerBitWidth._64;
                case "128": return IntegerBitWidth._128;
                default: throw new ArgumentOutOfRangeException("Invalid bit width");
            }
        }

        public override string ToString() => $"{Value}_{(IsSigned ? "I" : "U")}{BitWidth.GetEnumDescription()}  -  {TypedValue}";
    }
    #endregion IntegerToken

    #region FloatToken
    public class FloatToken : LiteralToken<double>
    {
        public FloatToken(string value, double typedValue)
            : base(TokenType.Float, value, typedValue)
        { }

        public static FloatToken Parse(string value)
            => new FloatToken(value, double.Parse(value));

        public override string ToString() => $"{Value}F  -  {TypedValue:0.000}";
    }
    #endregion FloatToken

    #region TimespanToken
    public class TimespanToken : LiteralToken<TimeSpan>
    {
        public TimespanToken(string value, TimeSpan typedValue)
            : base(TokenType.Timespan, value, typedValue)
        { }

        public TimespanToken()
            : base(TokenType.Timespan, "", TimeSpan.Zero)
        { }

        public static TimespanToken Parse(string value, TimeSpan timespan)
            => new TimespanToken(value, timespan);

        public static TimespanToken operator +(TimespanToken a, TimespanToken b)
            => new TimespanToken(string.Join(" ", a.Value, b.Value), a.TypedValue + b.TypedValue);

        public override string ToString() => $"{Value}  -  {TypedValue}";
    }
    #endregion TimespanToken

    #region TimeToken
    public class TimeToken : LiteralToken<TimeSpan>
    {
        public TimeToken(string value, TimeSpan typedValue)
            : base(TokenType.Time, value, typedValue)
        { }

        public TimeToken()
            : base(TokenType.Time, "", TimeSpan.Zero)
        { }

        public static TimeToken Parse(string value)
        {
            var time = TimeSpan.Parse(value);
            return new TimeToken(value, time);
        }

        public override string ToString() => $"{Value}  -  {TypedValue}";
    }
    #endregion TimeToken

    #region DateToken
    public class DateToken : LiteralToken<DateTime>
    {
        public DateToken(string value, DateTime typedValue)
            : base(TokenType.Time, value, typedValue)
        { }

        public DateToken()
            : base(TokenType.Time, "now", DateTime.Now)
        { }

        public static DateToken Parse(string value, bool dmy = false)
        {
            var dateTime = DateTime.Parse(dmy ? string.Join("-", value.Split('-').Reverse()) : value);
            return new DateToken(value, DateTime.SpecifyKind(dateTime, DateTimeKind.Local));
        }

        public static DateToken operator +(DateToken a, TimeToken b)
            => new DateToken(string.Join(" ", a.Value, b.Value), a.TypedValue + b.TypedValue);

        public static DateToken operator +(TimeToken b, DateToken a)
            => new DateToken(string.Join(" ", a.Value, b.Value), a.TypedValue + b.TypedValue);

        public override string ToString() => $"{Value}  -  {TypedValue}";
    }
    #endregion DateToken

    #endregion Literal tokens
}
