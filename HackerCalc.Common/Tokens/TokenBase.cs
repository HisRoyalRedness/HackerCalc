using System;
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
        // Visitor pattern
        TVisitRet Accept<TVisitRet>(ITokenVisitor<TVisitRet> visitor);
    }

    public abstract class TokenBase<TToken> : IToken
        where TToken : class, IToken
    {
        protected TokenBase(string rawToken = null)
        {
            RawToken = rawToken;
        }

        public virtual TVisitRet Accept<TVisitRet>(ITokenVisitor<TVisitRet> visitor)
            => visitor.Visit<TToken>(this as TToken);

        public string RawToken { get; private set; }

        public override string ToString() => $"{RawToken}";
    }
}
