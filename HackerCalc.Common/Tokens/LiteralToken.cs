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

    public interface ILiteralToken<T> : ILiteralToken
    {
        T TypedValue { get; }
    }

    public abstract class LiteralToken<T> : TokenBase<LiteralToken<T>>, ILiteralToken<T>
    {
        public LiteralToken(TokenDataType dataType, string value, T typedValue)
            : base()
        {
            DataType = dataType;
            Value = value;
            TypedValue = typedValue;
        }

        public LiteralToken(TokenDataType dataType, T typedValue)
            : base()
        {
            DataType = dataType;
            Value = typedValue.ToString();
            TypedValue = typedValue;
        }

        public TokenDataType DataType { get; private set; }
        public string Value { get; private set; }
        public bool IsFloat => DataType == TokenDataType.Float;
        public bool IsInteger => DataType == TokenDataType.Integer;
        public T TypedValue { get; private set; }
        public object ObjectValue => TypedValue;

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

        public override string ToString() => $"{Value}";
    }
}
