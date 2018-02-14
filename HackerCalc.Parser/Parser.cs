using System;
using System.Collections.Generic;
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
        internal void AddInteger(string tokenValue, string intLength = null)
        {
            AddToken(string.IsNullOrEmpty(intLength)
                ? new CalcToken(TokenType.BigInteger, tokenValue)
                : new CalcToken(TokenType.LimitedInteger, tokenValue, intLength));
        }

        internal void AddFloat(string tokenValue)
        {
            AddToken(new CalcToken(TokenType.Float, tokenValue));
        }

        internal void AddNotOperator(string tokenValue)
        {
            AddToken(new CalcToken(TokenType.NotOperator, tokenValue));
        }

        internal void AddAddOperator(string tokenValue)
        {
            AddToken(new CalcToken(TokenType.AddOperator, tokenValue));
        }

        internal void AddMultOperator(string tokenValue)
        {
            AddToken(new CalcToken(TokenType.MultOperator, tokenValue));
        }

        internal void AddShiftOperator(string tokenValue)
        {
            AddToken(new CalcToken(TokenType.ShiftOperator, tokenValue));
        }

        internal void AddBitOperator(string tokenValue)
        {
            AddToken(new CalcToken(TokenType.BitOperator, tokenValue));
        }

        internal void AddLeftBracket()
        {
            AddToken(new CalcToken(TokenType.Bracket, "("));
        }

        internal void AddRightBracket()
        {
            AddToken(new CalcToken(TokenType.Bracket, ")"));
        }

        void AddToken(CalcToken token)
        {
            _tokens.Add(token);
        }
        #endregion AddToken

        public static IEnumerable<CalcToken> ParseExpression(string expression)
        {
            List<CalcToken> tokens;
            using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(expression)))
            {
                var scanner = new Scanner(ms);
                var parser = new Parser(scanner);
                var result = parser.Parse();
                tokens = parser.Tokens;
            }
            return tokens;
        }

        public List<CalcToken> Tokens => _tokens;
        readonly List<CalcToken> _tokens = new List<CalcToken>();
    }

    public enum TokenType
    {
        BigInteger,
        LimitedInteger,
        Float,
        AddOperator,
        MultOperator,
        BitOperator,
        NotOperator,
        ShiftOperator,
        Bracket,
    }

    public class CalcToken
    {
        public CalcToken(TokenType tokenType, string value, string intLength = null)
        {
            Type = tokenType;
            Value = value;
            IntegerLength = string.IsNullOrEmpty(intLength)
                ? 0
                : int.Parse(intLength);
        }

        public TokenType Type { get; private set; }
        public string Value { get; private set; }
        public int IntegerLength { get; private set; }

        public override string ToString() => $"{Value}: {Type}{(IntegerLength > 0 ? IntegerLength.ToString() : "")}";
    }
}
