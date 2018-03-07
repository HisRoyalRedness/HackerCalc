using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HisRoyalRedness.com
{
    public partial class Parser
    {
        #region AddToken
        #region Literals
        internal void AddDecInteger(string tokenValue) => AddToken(IntegerToken.Parse(tokenValue, false));
        internal void AddHexInteger(string tokenValue) => AddToken(IntegerToken.Parse(tokenValue, true));
        internal void AddFloat(string tokenValue) => AddToken(FloatToken.Parse(tokenValue));
        internal void AddTimespan(string tokenValue) => AddToken(TimespanToken.Parse(tokenValue));
        #endregion Literals

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
            using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(expression)))
            {
                var scanner = new Scanner(ms);
                var parser = new Parser(scanner);
                var result = parser.Parse();
                tokens = parser.Tokens;
            }
            return tokens;
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
        Timespan
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
            _4,
            [Description("8")]
            _8,
            [Description("16")]
            _16,
            [Description("32")]
            _32,
            [Description("64")]
            _64,
            [Description("128")]
            _128
        }

        public IntegerToken(string value, BigInteger typedValue, bool isSigned = true, IntegerBitWidth bitWidth = IntegerBitWidth._32)
            : base(TokenType.Integer, value, typedValue)
        {
            IsSigned = isSigned;
            BitWidth = bitWidth;
        }

        public static IntegerToken Parse(string value, bool isHex)
            => isHex
                ? new IntegerToken(value, BigInteger.Parse(value.Replace("0x", "00", StringComparison.CurrentCultureIgnoreCase), NumberStyles.HexNumber))
                : new IntegerToken(value, BigInteger.Parse(value, NumberStyles.Integer));

        public bool IsSigned { get; private set; }
        public IntegerBitWidth BitWidth { get; private set; }

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

        public static TimespanToken Parse(string value)
        {
            var trimValue = value.TrimEnd(new[] { 't', 'T' });
            var portions = new Stack<double>(trimValue.Split(':').Select(s => double.Parse(s)));
            var ts = TimeSpan.FromSeconds(portions.Pop());
            if (portions.Count > 0)
                ts += TimeSpan.FromMinutes(portions.Pop());
            if (portions.Count > 0)
                ts += TimeSpan.FromHours(portions.Pop());
            if (portions.Count > 0)
                ts += TimeSpan.FromDays(portions.Pop());
            if (portions.Count > 0)
                throw new ArgumentException($"Too many timespan portions for {value}.");
            return new TimespanToken(trimValue, ts);
        }

        public override string ToString() => $"{Value}T  -  {TypedValue}";
    }
    #endregion TimespanToken
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
