using System;
using System.Collections.Generic;
using System.Text;

namespace HisRoyalRedness.com
{
    public interface ITokenVisitor
    {
        void Visit<TToken>(TToken token)
            where TToken : IToken;
    }

    public interface ITokenVisitor<TAggregate> : ITokenVisitor
    {
        TAggregate VisitAndAggregate<TToken>(TToken token)
            where TToken : IToken;
    }
}
