using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HisRoyalRedness.com
{
    public static class DataTypeMapper
    {
        public static IDataType Map<TToken>(TToken token)
            where TToken : IToken
        {
            switch (token.Category)
            {
                case TokenCategory.LiteralToken:
                    return null;
                    break;

                default:
                    throw new UnrecognisedTokenException($"Unrecognised token category {token.Category}");
            }
        }
    }
}
