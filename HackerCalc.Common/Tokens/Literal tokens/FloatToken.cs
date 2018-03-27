using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace HisRoyalRedness.com
{
    public class FloatToken : LiteralToken<double>
    {
        public FloatToken(string value, double typedValue)
            : base(TokenDataType.Float, value, typedValue)
        { }

        public FloatToken(double typedValue)
            : this(typedValue.ToString(), typedValue)
        { }

        #region Parsing
        public static FloatToken Parse(string value)
            => new FloatToken(value, double.Parse(value));
        #endregion Parsing

        #region Casting
        protected override TToken InternalCastTo<TToken>()
        {
            if (typeof(TToken).Name == GetType().Name)
                return this as TToken;

            switch (typeof(TToken).Name)
            {
                case nameof(IntegerToken):
                    return new IntegerToken(new BigInteger(TypedValue)) as TToken;

                case nameof(TimespanToken):
                    return new TimespanToken(TimeSpan.FromSeconds((double)TypedValue)) as TToken;

                case nameof(TimeToken):
                    return new TimeToken(TimeSpan.FromSeconds((double)TypedValue)) as TToken;

                case nameof(DateToken):
                    return new DateToken(DateTime.Now.Date + TimeSpan.FromSeconds(TypedValue)) as TToken;

                default:
                    return null;
            }
        }
        #endregion Casting

        public override string ToString() => $"{Value}F  -  {TypedValue:0.000}";
    }
}
