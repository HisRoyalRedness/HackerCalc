using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Text.RegularExpressions;

namespace HisRoyalRedness.com
{
    public static class TestCommon
    {
        public static IEnumerable<ILiteralToken> MakeTokens(this IEnumerable<string> tokenStrings)
            => tokenStrings.Select(ts => MakeToken(ts));

        public static TToken MakeToken<TToken>(this string tokenString)
            where TToken : class, ILiteralToken
            => MakeToken(tokenString) as TToken;

        public static ILiteralToken MakeToken(this string tokenString)
        {
            if (string.IsNullOrWhiteSpace(tokenString))
                return null;
                
            var firstSpace = tokenString.IndexOf(' ');
            var tokenType = firstSpace <= 0
                ? tokenString
                : tokenString.Substring(0, firstSpace).Trim();
            var tokenArg = firstSpace <= 0
                ? ""
                : tokenString.Substring(firstSpace).Trim();

            switch (tokenType.ToLower())
            {
                case "date":
                case "datetoken":
                    return DateToken.Parse(tokenArg);

                case "float":
                case "floattoken":
                    return FloatToken.Parse(tokenArg);

                case "integer":
                case "integertoken":
                    var portions = _integerRegex.Match(tokenArg);
                    var isNeg = portions.Groups[1].Value == "-";
                    var isHex = !string.IsNullOrEmpty(portions.Groups[2].Value);
                    var num = portions.Groups[3].Value;
                    var isSigned = portions.Groups[4].Value.ToLower() != "u";
                    var bitWidth = string.IsNullOrEmpty(portions.Groups[5].Value)
                        ? IntegerToken.IntegerBitWidth.Unbound
                        : IntegerToken.ParseBitWidth(portions.Groups[5].Value);
                    return IntegerToken.Parse((isNeg ? $"-{num}" : num), isHex, isSigned, bitWidth);

                case "timespan":
                case "timespantoken":
                    return TimeToken.Parse(tokenArg).CastTo(TokenDataType.Timespan) as TimespanToken;

                case "time":
                case "timetoken":
                    return TimeToken.Parse(tokenArg);

                default:
                    throw new NotSupportedException($"Unrecognised token type {tokenType}");
            }            
        }

        static Regex _integerRegex = new Regex(@"(-)?(0x)?([0-9a-f]+)([iu])?(\d+)?", RegexOptions.Compiled | RegexOptions.IgnoreCase);
    }
}
