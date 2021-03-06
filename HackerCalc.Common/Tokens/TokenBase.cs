﻿using System;

/*
    Base class for all token types

        Built-in support for the Visitor Pattern

    Keith Fletcher
    Oct 2018

    This file is Unlicensed.
    See the foot of the file, or refer to <http://unlicense.org>
*/

namespace HisRoyalRedness.com
{
    public interface IToken
    {
        // Visitor pattern
        TVisitorReturnType Accept<TVisitorReturnType>(ITokenVisitor<TVisitorReturnType> visitor);

        string RawToken { get; }
        SourcePosition Position { get; }

        TokenCategory Category { get; }
    }

    public enum TokenCategory
    {
        LiteralToken,
        FunctionToken,
        OperatorToken
    }

    public interface ITokenVisitor<TVisistorReturnType>
    {
        TVisistorReturnType Visit<TToken>(TToken token)
            where TToken : IToken;
    }

    public abstract class TokenBase<TToken> : IToken
        where TToken : class, IToken
    {
        protected TokenBase(string rawToken, SourcePosition position)
        {
            RawToken = rawToken;
            Position = position ?? SourcePosition.None;
        }

        public virtual TVisistorReturnType Accept<TVisistorReturnType>(ITokenVisitor<TVisistorReturnType> visitor)
            => visitor.Visit<TToken>(this as TToken);

        public SourcePosition Position { get; private set; }
        public string RawToken { get; private set; }
        public abstract TokenCategory Category { get; }
        public override string ToString() => $"{RawToken}";
    }

    public class SourcePosition
    {
        public SourcePosition(int line, int column)
        {
            Line = line;
            Column = column;
        }

        public static SourcePosition None => new SourcePosition(0, 0);

        public int Line { get; private set; }
        public int Column { get; private set; }
    }
}

/*
This is free and unencumbered software released into the public domain.

Anyone is free to copy, modify, publish, use, compile, sell, or
distribute this software, either in source code form or as a compiled
binary, for any purpose, commercial or non-commercial, and by any
means.

In jurisdictions that recognize copyright laws, the author or authors
of this software dedicate any and all copyright interest in the
software to the public domain. We make this dedication for the benefit
of the public at large and to the detriment of our heirs and
successors. We intend this dedication to be an overt act of
relinquishment in perpetuity of all present and future rights to this
software under copyright law.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
IN NO EVENT SHALL THE AUTHORS BE LIABLE FOR ANY CLAIM, DAMAGES OR
OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE,
ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
OTHER DEALINGS IN THE SOFTWARE.

For more information, please refer to <http://unlicense.org>
*/
