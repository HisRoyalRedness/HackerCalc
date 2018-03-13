using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;

namespace HisRoyalRedness.com
{
    public partial class Parser
    {
        #region AddToken

        #region Operators
        internal IToken AddNotOperator(string tokenValue)
        {
            switch (tokenValue)
            {
                case "!": return AddToken(new OperatorToken(TokenType.Not));
                case "~": return AddToken(new OperatorToken(TokenType.Negate));
            }
            throw new ApplicationException("Invalid token type");
        }

        internal IToken AddAddOperator(string tokenValue)
        {
            switch(tokenValue)
            {
                case "+":   return AddToken(new OperatorToken(TokenType.Add));
                case "-":   return AddToken(new OperatorToken(TokenType.Subtract));
            }
            throw new ApplicationException("Invalid token type");
        }

        internal IToken AddMultOperator(string tokenValue)
        {
            switch (tokenValue)
            {
                case "*": return AddToken(new OperatorToken(TokenType.Multiply));
                case "/": return AddToken(new OperatorToken(TokenType.Divide));
                case "\\": return AddToken(new OperatorToken(TokenType.Divide));
                case "%": return AddToken(new OperatorToken(TokenType.Modulo));
            }
            throw new ApplicationException("Invalid token type");
        }

        internal IToken AddShiftOperator(string tokenValue)
        {
            switch (tokenValue)
            {
                case "<<": return AddToken(new OperatorToken(TokenType.LeftShift));
                case ">>": return AddToken(new OperatorToken(TokenType.RightShift));
            }
            throw new ApplicationException("Invalid token type");
        }

        internal IToken AddBitOperator(string tokenValue)
        {
            switch (tokenValue)
            {
                case "&": return AddToken(new OperatorToken(TokenType.And));
                case "|": return AddToken(new OperatorToken(TokenType.Or));
                case "^": return AddToken(new OperatorToken(TokenType.Xor));
            }
            throw new ApplicationException("Invalid token type");
        }
        #endregion Operators

        #region Grouping
        internal IToken AddLeftBracket() => AddToken(new OperatorToken(TokenType.LeftBracket));
        internal IToken AddRightBracket() => AddToken(new OperatorToken(TokenType.RightBracket));
        #endregion Grouping

        IToken AddToken(IToken token)
        {
            _tokens.Add(token);
            return token;
        }
        #endregion AddToken

        public static IEnumerable<IToken> ParseExpression(string expression)
        {
            List<IToken> tokens;
            var result = false;
            using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(expression)))
            {
                var scanner = new Scanner(ms);
                var parser = new Parser(scanner);
                result = parser.Parse();
                tokens = parser.Tokens;
            }
            return result ? tokens : Enumerable.Empty<IToken>();
        }

        public static IEnumerable<Token> ScanExpression(string expression)
        {
            using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(expression)))
            {
                var scanner = new Scanner(ms);
                while (true)
                {
                    var token = scanner.Scan();
                    if (token.kind == 0)
                        yield break;
                    yield return token;
                }
            }
        }

        public bool IsTimespanSeconds()
        {
            switch(la.kind)
            {
                // Definately seconds
                case _typed_ts_seconds:
                    return true;

                // Maybe seconds. Need to check further
                case _dec_integer:
                case _true_float:
                    // true_float could be a time portion or just a numeric value.
                    // Return true if it's a time portion. Assume anything else is a numeric
                    var next = scanner.Peek().kind;
                    scanner.ResetPeek();
                    switch (next)
                    {
                        case _ts_seconds_type:
                            return true;
                        default:
                            return false;
                    }
                // Anything else is a syntax error
                default:
                    SynErr(la.kind);
                    return false;
            }
        }

        public bool IsTimespanMinutes()
        {
            switch (la.kind)
            {
                // Maybe minutes. Need to check further
                case _dec_integer:
                case _true_float:
                    // true_float could be a time portion or just a numeric value.
                    // Return true if it's a time portion. Assume anything else is a numeric
                    var next = scanner.Peek().kind;
                    scanner.ResetPeek();
                    switch (next)
                    {
                        case _ts_minutes_type:
                            return true;
                        default:
                            return false;
                    }
                default:
                    return false;
            }
        }

        public bool IsTimespanHours()
        {
            switch (la.kind)
            {
                // Maybe minutes. Need to check further
                case _dec_integer:
                case _true_float:
                    // true_float could be a time portion or just a numeric value.
                    // Return true if it's a time portion. Assume anything else is a numeric
                    var next = scanner.Peek().kind;
                    scanner.ResetPeek();
                    switch (next)
                    {
                        case _ts_hours_type:
                            return true;
                        default:
                            return false;
                    }
                default:
                    return false;
            }
        }

        public bool IsTimespanDays()
        {
            switch (la.kind)
            {
                // Maybe minutes. Need to check further
                case _dec_integer:
                case _true_float:
                    // true_float could be a time portion or just a numeric value.
                    // Return true if it's a time portion. Assume anything else is a numeric
                    var next = scanner.Peek().kind;
                    scanner.ResetPeek();
                    switch (next)
                    {
                        case _ts_days_type:
                            return true;
                        default:
                            return false;
                    }
                default:
                    return false;
            }
        }

        public bool IsTimespanNumber()
        {
            switch(la.kind)
            {
                // Typed timespan
                case _typed_ts_seconds:
                    return true;

                // Stuff that could be either an integer, float or timespan
                case _true_float:
                case _dec_integer:
                    // true_float could be a time portion or just a numeric value.
                    // Return true if it's a time portion. Assume anything else is a numeric
                    var next = scanner.Peek().kind;
                    scanner.ResetPeek();
                    switch (next)
                    {
                        case _ts_seconds_type:
                        case _ts_minutes_type:
                        case _ts_hours_type:
                        case _ts_days_type:
                            return true;
                        default:
                            return false;
                    }

                // Assume anything else is not a TimeSpan
                default:
                    return false;
            }
        }

        public bool IsDateTime()
        {
            if (la.kind == _date)
            {
                var next = scanner.Peek().kind;
                scanner.ResetPeek();
                return next == _time;
            }
            else
                return false;
        }

        public List<IToken> Tokens => _tokens;
        readonly List<IToken> _tokens = new List<IToken>();
    }

    #region Token base
    public interface IToken
    { }

    public enum TokenType
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
        [Description("(")]
        LeftBracket,
        [Description(")")]
        RightBracket,
        Float,
        Integer,
        Timespan,
        Time
    }

    public abstract class TokenBase : IToken
    {
        protected TokenBase(string rawToken = null)
        {
            RawToken = rawToken;
        }

        public string RawToken { get; private set; }

        public override string ToString() => $"{RawToken}";
    }
    #endregion Token base

    #region Operator tokens
    public interface IOperatorToken : IToken
    { }

    public class OperatorToken: TokenBase, IOperatorToken
    {
        public OperatorToken(TokenType op)
            : base()
        {
            Operator = op;
        }

        public TokenType Operator { get; private set; }



        public override string ToString() => $"{Operator.GetEnumDescription()}";
    }
    #endregion Operator tokens

    #region Grouping tokens
    public interface IGroupingToken : IToken
    { }

    public class GroupingToken : TokenBase, IGroupingToken
    {
        public GroupingToken(TokenType op)
            : base()
        {
            Operator = op;
        }

        public TokenType Operator { get; private set; }


        public override string ToString() => $"{Operator.GetEnumDescription()}";
    }
    #endregion Grouping tokens

    #region Literal tokens
    public interface ILiteralToken : IToken
    { }

    #region LiteralToken
    public class LiteralToken<T> : TokenBase, ILiteralToken
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
            switch(bitWidth)
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
            var dateTime = DateTime.Parse(dmy ? string.Join("-", value.Split('-').Reverse())  : value);
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

    #region Errors
    public partial class Errors
    {
        public virtual void SemErr(int line, int col, string s)
        {
            WriteLine(errMsgFormat, line, col, s);
            count++;
        }

        public virtual void SemErr(string s)
        {
            WriteLine(s);
            count++;
        }

        public virtual void Warning(int line, int col, string s)
        {
            WriteLine(errMsgFormat, line, col, s);
        }

        public virtual void Warning(string s)
        {
            WriteLine(s);
        }

        protected virtual void WriteLine(string format, params object[] args)
        {
            var currentForeColour = Console.ForegroundColor;
            var currentBackColour = Console.BackgroundColor;
            Console.ForegroundColor = ConsoleColor.White;
            Console.BackgroundColor = ConsoleColor.Red;
            Console.WriteLine(format, args);
            Console.ForegroundColor = currentForeColour;
            Console.BackgroundColor = currentBackColour;
        }
    }
    #endregion Errors
}
