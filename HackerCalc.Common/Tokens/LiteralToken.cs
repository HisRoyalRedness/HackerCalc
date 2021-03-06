﻿using System;
using System.ComponentModel;

/*
    Base type for literal tokens

        These are the tokens as parsed from the input stream.
        They'll be passed on to the calculation engine, which
        will probably transform them into calculation tokens
        of some sort.

    Keith Fletcher
    Oct 2018

    This file is Unlicensed.
    See the foot of the file, or refer to <http://unlicense.org>
*/

namespace HisRoyalRedness.com
{
    public enum LiteralTokenType
    {
        [Description("Limited Integer")]
        LimitedInteger,
        [Description("Unlimited Integer")]
        UnlimitedInteger,
        [Description("Float")]
        Float,
        [Description("Date")]
        Date,
        [Description("Time")]
        Time,
        [Description("Timespan")]
        Timespan,
    }

    public interface ILiteralToken : IToken
    {
        object ObjectValue { get; }
        LiteralTokenType LiteralType { get; }
    }

    public interface ILiteralToken<TBaseType, TTypedToken> : ILiteralToken, IEquatable<TTypedToken>, IComparable, IComparable<TTypedToken>
        where TTypedToken : class, ILiteralToken, ILiteralToken<TBaseType, TTypedToken>
    {
        TBaseType TypedValue { get; }
    }

    public abstract class LiteralToken<TBaseType, TTypedToken> : TokenBase<LiteralToken<TBaseType, TTypedToken>>, ILiteralToken<TBaseType, TTypedToken>
        where TTypedToken : class, ILiteralToken, ILiteralToken<TBaseType, TTypedToken>
    {
        protected LiteralToken(LiteralTokenType literalTokenType, TBaseType typedValue, string rawToken, SourcePosition position)
            : base(rawToken, position)
        {
            LiteralType = literalTokenType;
            TypedValue = typedValue;
        }

        public LiteralTokenType LiteralType { get; private set; }
        public bool IsFloat => LiteralType == LiteralTokenType.Float;
        public bool IsLimitedInteger => LiteralType == LiteralTokenType.LimitedInteger;
        public bool IsUnlimitedInteger => LiteralType == LiteralTokenType.UnlimitedInteger;
        public bool IsDate => LiteralType == LiteralTokenType.Date;
        public bool IsTime => LiteralType == LiteralTokenType.Time;
        public bool IsTimespan => LiteralType == LiteralTokenType.Timespan;
        public TBaseType TypedValue { get; protected set; }
        public object ObjectValue => TypedValue;
        public override TokenCategory Category => TokenCategory.LiteralToken;

        #region Equality
        public abstract bool Equals(TTypedToken other);
        public override int GetHashCode() => TypedValue.GetHashCode();
        #endregion Equality

        #region Comparison
        public abstract int CompareTo(TTypedToken other);
        int IComparable.CompareTo(object obj) => CompareTo(obj as TTypedToken);
        #endregion Comparison

        #region ToString
        public override string ToString() => $"{TypedValue}";
        #endregion ToString
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
