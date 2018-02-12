using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HisRoyalRedness.com
{
    public partial class Parser
    {
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

        internal void AddOperator(TokenType tokenType, string tokenValue)
        {
            AddToken(new CalcToken(tokenType, tokenValue));
        }

        internal void AddToken(TokenType tokenType, string tokenValue, string intLength = null)
        {
            AddToken(new CalcToken(tokenType, tokenValue, intLength));
        }

        void AddToken(CalcToken token)
        {
            _tokens.Add(token);
            Console.WriteLine(token);
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
        NotOperator
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
