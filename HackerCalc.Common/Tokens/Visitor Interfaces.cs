using System;
using System.ComponentModel;

namespace HisRoyalRedness.com
{
    public interface ITokenVisitor<TRetType>
    {
        TRetType Visit<TToken>(TToken token)
            where TToken : IToken;
    }
}
