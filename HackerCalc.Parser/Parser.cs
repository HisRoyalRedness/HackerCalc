using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HisRoyalRedness.com
{
    public partial class Parser
    {
        #region AddToken
        #region Literals
        internal void AddInteger(string tokenValue, string intLength = null) => AddToken(new LiteralToken(TokenType.BigInteger, tokenValue.TrimEnd('I')));
        internal void AddFloat(string tokenValue) => AddToken(new LiteralToken(TokenType.Float, tokenValue.TrimEnd('F')));
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
        BigInteger
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

    public class LiteralToken : TokenBase, IGroupingToken
    {
        public LiteralToken(TokenType dataType, string value)
            : base()
        {
            DataType = dataType;
            Value = value;
        }

        public TokenType DataType { get; private set; }
        public string Value { get; private set; }

        public override string ToString() => $"{Value}{(DataType == TokenType.BigInteger ? "I" : "F")}";
    }
    #endregion Literal tokens
}
