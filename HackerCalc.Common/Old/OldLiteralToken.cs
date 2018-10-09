//using System;
//using System.Collections.Generic;
//using System.ComponentModel;
//using System.Text;

//namespace HisRoyalRedness.com
//{
//    public enum TokenDataType
//    {
//        [Description("Float")]
//        Float,
//        [Description("Unlimited Integer")]
//        UnlimitedInteger,
//        [Description("Limited Integer")]
//        LimitedInteger,
//        [Description("Rational Number")]
//        [IgnoreEnum]
//        RationalNumber,
//        [Description("Irrational Number")]
//        [IgnoreEnum]
//        IrrationalNumber, // Replace float?
//        [Description("Digital Integer")]
//        [IgnoreEnum]
//        DigitalInteger,
//        [Description("Timespan")]
//        Timespan,
//        [Description("Time")]
//        Time,
//        [Description("Date")]
//        Date
//    }

//    public enum IntegerBase
//    {
//        Binary,
//        Octal,
//        Decimal,
//        Hexadecimal
//    }

//    public struct TypePair
//    {
//        public TypePair(TokenDataType left, TokenDataType right)
//        {
//            Left = left;
//            Right = right;
//        }

//        public TokenDataType Left { get; private set; }
//        public TokenDataType Right { get; private set; }
//    }

//    public interface IOldLiteralToken : IToken
//    {
//        object ObjectValue { get; }
//        TokenDataType DataType { get; }
//        TToken CastTo<TToken>()
//            where TToken : class, IOldLiteralToken;

//        IOldLiteralToken NumericNegate();
//        IOldLiteralToken BitwiseNegate();
//        IOldLiteralToken CastTo(TokenDataType dataType);

//        string ToHex();
//        string ToDec();
//        string ToOct();
//        string ToBin();
//    }

//    public interface IOldLiteralToken<TBaseType, TTypedToken> : IOldLiteralToken, IEquatable<TTypedToken>, IComparable, IComparable<TTypedToken>
//        where TTypedToken : class, IOldLiteralToken, IOldLiteralToken<TBaseType, TTypedToken>
//    {
//        TBaseType TypedValue { get; }
//    }

//    public abstract class OldLiteralToken<TBaseType, TTypedToken> : TokenBase<OldLiteralToken<TBaseType, TTypedToken>>, IOldLiteralToken<TBaseType, TTypedToken>, ILiteralTokenEval
//        where TTypedToken : class, IOldLiteralToken, IOldLiteralToken<TBaseType, TTypedToken>
//    {
//        public OldLiteralToken(TokenDataType dataType, TBaseType typedValue)
//            : base()
//        {
//            DataType = dataType;
//            TypedValue = typedValue;
//        }

//        public TokenDataType DataType { get; private set; }
//        public bool IsFloat => DataType == TokenDataType.Float;
//        public bool IsLimitedInteger => DataType == TokenDataType.LimitedInteger;
//        public bool IsUnlimitedInteger => DataType == TokenDataType.UnlimitedInteger;
//        public TBaseType TypedValue { get; protected set; }
//        public object ObjectValue => TypedValue;

//        public virtual IOldLiteralToken NumericNegate() { throw new InvalidOperationException($"Numeric negation is not supported by {typeof(TTypedToken).Name}."); }
//        public virtual IOldLiteralToken BitwiseNegate() { throw new InvalidOperationException($"Bitwise negation is not supported by {typeof(TTypedToken).Name}."); }

//        public abstract string ToHex();
//        public abstract string ToDec();
//        public abstract string ToOct();
//        public abstract string ToBin();

//        #region Casting
//        protected virtual TToken InternalCastTo<TToken>()
//            where TToken : class, IOldLiteralToken
//        {
//            return null;
//        }

//        public TToken CastTo<TToken>()
//            where TToken : class, IOldLiteralToken
//        {
//            var token = InternalCastTo<TToken>();
//            if (token == null)
//                throw new InvalidCastException($"Could not cast from a {GetType().Name} to {typeof(TToken).Name}.");

//            return token as TToken;
//        }

//        public IOldLiteralToken CastTo(TokenDataType dataType)
//        {
//            switch (dataType)
//            {
//                case TokenDataType.Date: return CastTo<OldDateToken>();
//                case TokenDataType.Float: return CastTo<OldFloatToken>();
//                case TokenDataType.Time: return CastTo<OldTimeToken>();
//                case TokenDataType.Timespan: return CastTo<OldTimespanToken>();
//                case TokenDataType.UnlimitedInteger: return CastTo<OldUnlimitedIntegerToken>();
//                case TokenDataType.LimitedInteger: return CastTo<OldLimitedIntegerToken>();
//                case TokenDataType.RationalNumber: return CastTo<OldUnlimitedIntegerToken>();
//                case TokenDataType.IrrationalNumber: return CastTo<OldUnlimitedIntegerToken>();
//                default: throw new InvalidCastException($"Could not cast from a {GetType().Name} to {dataType}.");
//            }
//        }
//        #endregion Casting

//        #region Equality
//        public abstract bool Equals(TTypedToken other);
//        public override int GetHashCode() => TypedValue.GetHashCode();
//        #endregion Equality

//        #region Comparison
//        public abstract int CompareTo(TTypedToken other);
//        int IComparable.CompareTo(object obj) => CompareTo(obj as TTypedToken);
//        #endregion Comparison

//        #region ILiteralTokenEval implementation
//        bool ILiteralTokenEval.IsTermToken { get; set; } = false;
//        #endregion ILiteralTokenEval implementation

//        #region ToString
//        public override string ToString() => $"{TypedValue}";
//        #endregion ToString

//        static Dictionary<TokenDataType, IOldLiteralToken> _typeMapping = new Dictionary<TokenDataType, IOldLiteralToken>();
//    }

//    // Mark enum types that should be ignored when enumerating
//    public class IgnoreEnumAttribute : Attribute
//    { }

//}
