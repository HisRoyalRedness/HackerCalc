using System;
using System.Collections.Generic;
using System.Text;

namespace HisRoyalRedness.com
{
    public enum TokenDataType
    {
        Float,
        Integer,
        Timespan,
        Time,
        Date
    }

    public interface ILiteralToken : IToken
    {
        object ObjectValue { get; }
        TokenDataType DataType { get; }
        TToken CastTo<TToken>()
            where TToken : class, ILiteralToken;

        ILiteralToken CastTo(TokenDataType dataType);
    }

    public interface ILiteralToken<TBaseType, TTypedToken> : ILiteralToken, IEquatable<TTypedToken>, IComparable, IComparable<TTypedToken>
        where TTypedToken : class, ILiteralToken, ILiteralToken<TBaseType, TTypedToken>
    {
        TBaseType TypedValue { get; }
    }

    public abstract class LiteralToken<TBaseType, TTypedToken> : TokenBase<LiteralToken<TBaseType, TTypedToken>>, ILiteralToken<TBaseType, TTypedToken>, ILiteralTokenEval
        where TTypedToken : class, ILiteralToken, ILiteralToken<TBaseType, TTypedToken>
    {
        public LiteralToken(TokenDataType dataType, string value, TBaseType typedValue)
            : base()
        {
            DataType = dataType;
            Value = value;
            TypedValue = typedValue;
        }

        public TokenDataType DataType { get; private set; }
        public string Value { get; private set; }
        public bool IsFloat => DataType == TokenDataType.Float;
        public bool IsInteger => DataType == TokenDataType.Integer;
        public TBaseType TypedValue { get; protected set; }
        public object ObjectValue => TypedValue;

        #region Casting
        protected virtual TToken InternalCastTo<TToken>()
            where TToken : class, ILiteralToken
        {
            return null;
        }

        public TToken CastTo<TToken>()
            where TToken : class, ILiteralToken
        {
            var token = InternalCastTo<TToken>();
            if (token == null)
                throw new InvalidCastException($"Could not cast from a {GetType().Name} to {typeof(TToken).Name}.");

            return token as TToken;
        }

        public ILiteralToken CastTo(TokenDataType dataType)
        {
            switch(dataType)
            {
                case TokenDataType.Date: return CastTo<DateToken>();
                case TokenDataType.Float: return CastTo<FloatToken>();
                case TokenDataType.Integer: return CastTo<IntegerToken>();
                case TokenDataType.Time: return CastTo<TimeToken>();
                case TokenDataType.Timespan: return CastTo<TimespanToken>();
                default: throw new InvalidCastException($"Could not cast from a {GetType().Name} to {dataType}.");
            }
        }
        #endregion Casting

        #region Equality
        public abstract bool Equals(TTypedToken other);
        public override int GetHashCode() => TypedValue.GetHashCode();
        #endregion Equality

        #region Comparison
        public abstract int CompareTo(TTypedToken other);
        int IComparable.CompareTo(object obj) => CompareTo(obj as TTypedToken);
        #endregion Comparison

        #region ILiteralTokenEval implementation
        bool ILiteralTokenEval.IsTermToken { get; set; } = false;
        #endregion ILiteralTokenEval implementation

        #region ToString
        public override string ToString() => $"{Value}";
        #endregion ToString
    }
}
