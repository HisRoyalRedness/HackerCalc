using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace HisRoyalRedness.com
{
    using DTP = DataTypePair<DataType>;
    using DTVP = DataTypeValuePair<DataType>;

    public static class DataMapper
    {
        static DataMapper()
        {
            BuildOperandTypeCastMap();
        }

        public static DTP GetOperandDataTypes(OperatorType opType, DTP operandTypePair)
            => _operandTypeCastMap[opType][operandTypePair];
        public static DTP GetOperandDataTypes(OperatorType opType, DTVP valuePair)
            => _operandTypeCastMap[opType][new DataTypePair<DataType>(valuePair.Left.DataType, valuePair.Right.DataType)];

        #region Map - Convert literal token to data type
        public static IDataType<DataType> Map(ILiteralToken token)
        {
            switch (token?.LiteralType)
            {
                case null:
                    return null;
                case LiteralTokenType.Date:
                    return ConvertToTypedDataType<DateTime, DateToken, DateType>(token, typedToken => new DateType(typedToken.TypedValue));
                case LiteralTokenType.Float:
                    return ConvertToTypedDataType<double, FloatToken, FloatType>(token, typedToken => new FloatType(typedToken.TypedValue));
                case LiteralTokenType.LimitedInteger:
                    return ConvertToTypedDataType<BigInteger, LimitedIntegerToken, LimitedIntegerType>(token, typedToken => new LimitedIntegerType(typedToken.TypedValue));
                case LiteralTokenType.Time:
                    return ConvertToTypedDataType<TimeSpan, TimeToken, TimeType>(token, typedToken => new TimeType(typedToken.TypedValue));
                case LiteralTokenType.Timespan:
                    return ConvertToTypedDataType<TimeSpan, TimespanToken, TimespanType>(token, typedToken => new TimespanType(typedToken.TypedValue));
                case LiteralTokenType.UnlimitedInteger:
                    return ConvertToTypedDataType<BigInteger, UnlimitedIntegerToken, UnlimitedIntegerType>(token, typedToken => new UnlimitedIntegerType(typedToken.TypedValue));
                default:
                    throw new TypeConversionException($"Unhandled literal token type '{token.LiteralType}' while converting to a data type.");
            }
        }

        static IDataType<DataType> ConvertToTypedDataType<TBaseType, TTypedToken, TDataType>(ILiteralToken token, Func<TTypedToken, TDataType> createDataTypeFunc)
            where TTypedToken : class, ILiteralToken, ILiteralToken<TBaseType, TTypedToken>
            where TDataType : class, IDataType<DataType>
            => (token is TTypedToken typedToken)
                ? createDataTypeFunc(typedToken)
                : null;
        #endregion Map - Convert literal token to data type

        static void BuildOperandTypeCastMap()
        {
            _operandTypeCastMap.Add(OperatorType.Add, new Dictionary<DTP, DTP>()
            {
                { new DTP(DataType.Date,                DataType.Date),                  DTP.Unsupported },
                { new DTP(DataType.Date,                DataType.Float),                 new DTP(DataType.Date,                DataType.Timespan) },
                { new DTP(DataType.Date,                DataType.LimitedInteger),        new DTP(DataType.Date,                DataType.Timespan) },
                { new DTP(DataType.Date,                DataType.Time),                  new DTP(DataType.Date,                DataType.Timespan) },
                { new DTP(DataType.Date,                DataType.Timespan),              new DTP(DataType.Date,                DataType.Timespan) },
                { new DTP(DataType.Date,                DataType.UnlimitedInteger),      new DTP(DataType.Date,                DataType.Timespan) },

                { new DTP(DataType.Float,               DataType.Date),                  new DTP(DataType.Timespan,            DataType.Date) },
                { new DTP(DataType.Float,               DataType.Float),                 new DTP(DataType.Float,               DataType.Float) },
                { new DTP(DataType.Float,               DataType.LimitedInteger),        new DTP(DataType.Float,               DataType.Float) },
                { new DTP(DataType.Float,               DataType.Time),                  new DTP(DataType.Timespan,            DataType.Time) },
                { new DTP(DataType.Float,               DataType.Timespan),              new DTP(DataType.Timespan,            DataType.Timespan) },
                { new DTP(DataType.Float,               DataType.UnlimitedInteger),      new DTP(DataType.Float,               DataType.Float) },

                { new DTP(DataType.LimitedInteger,      DataType.Date),                  new DTP(DataType.Timespan,            DataType.Date) },
                { new DTP(DataType.LimitedInteger,      DataType.Float),                 new DTP(DataType.Float,               DataType.Float) },
                { new DTP(DataType.LimitedInteger,      DataType.LimitedInteger),        new DTP(DataType.LimitedInteger,      DataType.LimitedInteger) },
                { new DTP(DataType.LimitedInteger,      DataType.Time),                  new DTP(DataType.Timespan,            DataType.Time) },
                { new DTP(DataType.LimitedInteger,      DataType.Timespan),              new DTP(DataType.Timespan,            DataType.Timespan) },
                { new DTP(DataType.LimitedInteger,      DataType.UnlimitedInteger),      new DTP(DataType.LimitedInteger,      DataType.LimitedInteger) },

                { new DTP(DataType.Time,                DataType.Date),                  new DTP(DataType.Timespan,            DataType.Date) },
                { new DTP(DataType.Time,                DataType.Float),                 new DTP(DataType.Time,                DataType.Timespan) },
                { new DTP(DataType.Time,                DataType.LimitedInteger),        new DTP(DataType.Time,                DataType.Timespan) },
                { new DTP(DataType.Time,                DataType.Time),                  new DTP(DataType.Time,                DataType.Timespan) },
                { new DTP(DataType.Time,                DataType.Timespan),              new DTP(DataType.Time,                DataType.Timespan) },
                { new DTP(DataType.Time,                DataType.UnlimitedInteger),      new DTP(DataType.Time,                DataType.Timespan) },

                { new DTP(DataType.Timespan,            DataType.Date),                  new DTP(DataType.Timespan,            DataType.Date) },
                { new DTP(DataType.Timespan,            DataType.Float),                 new DTP(DataType.Timespan,            DataType.Timespan) },
                { new DTP(DataType.Timespan,            DataType.LimitedInteger),        new DTP(DataType.Timespan,            DataType.Timespan) },
                { new DTP(DataType.Timespan,            DataType.Time),                  new DTP(DataType.Timespan,            DataType.Time) },
                { new DTP(DataType.Timespan,            DataType.Timespan),              new DTP(DataType.Timespan,            DataType.Timespan) },
                { new DTP(DataType.Timespan,            DataType.UnlimitedInteger),      new DTP(DataType.Timespan,            DataType.Timespan) },

                { new DTP(DataType.UnlimitedInteger,    DataType.Date),                  new DTP(DataType.Timespan,            DataType.Date) },
                { new DTP(DataType.UnlimitedInteger,    DataType.Float),                 new DTP(DataType.Float,               DataType.Float) },
                { new DTP(DataType.UnlimitedInteger,    DataType.LimitedInteger),        new DTP(DataType.LimitedInteger,      DataType.LimitedInteger) },
                { new DTP(DataType.UnlimitedInteger,    DataType.Time),                  new DTP(DataType.Timespan,            DataType.Time) },
                { new DTP(DataType.UnlimitedInteger,    DataType.Timespan),              new DTP(DataType.Timespan,            DataType.Timespan) },
                { new DTP(DataType.UnlimitedInteger,    DataType.UnlimitedInteger),      new DTP(DataType.UnlimitedInteger,    DataType.UnlimitedInteger) },

            });


            //        static readonly BinaryOperandTypeMap _addMapping = new BinaryOperandTypeMap()
            //        {
            //            { new OperandTypePair(TokenDataType.Date, TokenDataType.Date),                              OperandTypePair.Unsupported },
            //            { new OperandTypePair(TokenDataType.Date, TokenDataType.Float),                             new OperandTypePair(TokenDataType.Date, TokenDataType.Timespan) },
            //            { new OperandTypePair(TokenDataType.Date, TokenDataType.LimitedInteger),                    new OperandTypePair(TokenDataType.Date, TokenDataType.Timespan) },
            //            { new OperandTypePair(TokenDataType.Date, TokenDataType.Time),                              new OperandTypePair(TokenDataType.Date, TokenDataType.Timespan) },
            //            { new OperandTypePair(TokenDataType.Date, TokenDataType.Timespan),                          new OperandTypePair(TokenDataType.Date, TokenDataType.Timespan) },
            //            { new OperandTypePair(TokenDataType.Date, TokenDataType.UnlimitedInteger),                  new OperandTypePair(TokenDataType.Date, TokenDataType.Timespan) },

            //            { new OperandTypePair(TokenDataType.Float, TokenDataType.Date),                             new OperandTypePair(TokenDataType.Timespan, TokenDataType.Date) },
            //            { new OperandTypePair(TokenDataType.Float, TokenDataType.Float),                            new OperandTypePair(TokenDataType.Float, TokenDataType.Float) },
            //            { new OperandTypePair(TokenDataType.Float, TokenDataType.LimitedInteger),                   new OperandTypePair(TokenDataType.Float, TokenDataType.Float) },
            //            { new OperandTypePair(TokenDataType.Float, TokenDataType.Time),                             new OperandTypePair(TokenDataType.Timespan, TokenDataType.Time) },
            //            { new OperandTypePair(TokenDataType.Float, TokenDataType.Timespan),                         new OperandTypePair(TokenDataType.Timespan, TokenDataType.Timespan) },
            //            { new OperandTypePair(TokenDataType.Float, TokenDataType.UnlimitedInteger),                 new OperandTypePair(TokenDataType.Float, TokenDataType.Float) },

            //            { new OperandTypePair(TokenDataType.LimitedInteger, TokenDataType.Date),                    new OperandTypePair(TokenDataType.Timespan, TokenDataType.Date) },
            //            { new OperandTypePair(TokenDataType.LimitedInteger, TokenDataType.Float),                   new OperandTypePair(TokenDataType.Float, TokenDataType.Float) },
            //            { new OperandTypePair(TokenDataType.LimitedInteger, TokenDataType.LimitedInteger),          new OperandTypePair(TokenDataType.LimitedInteger, TokenDataType.LimitedInteger) },
            //            { new OperandTypePair(TokenDataType.LimitedInteger, TokenDataType.Time),                    new OperandTypePair(TokenDataType.Timespan, TokenDataType.Time) },
            //            { new OperandTypePair(TokenDataType.LimitedInteger, TokenDataType.Timespan),                new OperandTypePair(TokenDataType.Timespan, TokenDataType.Timespan) },
            //            { new OperandTypePair(TokenDataType.LimitedInteger, TokenDataType.UnlimitedInteger),        new OperandTypePair(TokenDataType.LimitedInteger, TokenDataType.LimitedInteger) },

            //            { new OperandTypePair(TokenDataType.Time, TokenDataType.Date),                              new OperandTypePair(TokenDataType.Timespan, TokenDataType.Date) },
            //            { new OperandTypePair(TokenDataType.Time, TokenDataType.Float),                             new OperandTypePair(TokenDataType.Time, TokenDataType.Timespan) },
            //            { new OperandTypePair(TokenDataType.Time, TokenDataType.LimitedInteger),                    new OperandTypePair(TokenDataType.Time, TokenDataType.Timespan) },
            //            { new OperandTypePair(TokenDataType.Time, TokenDataType.Time),                              new OperandTypePair(TokenDataType.Time, TokenDataType.Timespan) },
            //            { new OperandTypePair(TokenDataType.Time, TokenDataType.Timespan),                          new OperandTypePair(TokenDataType.Time, TokenDataType.Timespan) },
            //            { new OperandTypePair(TokenDataType.Time, TokenDataType.UnlimitedInteger),                  new OperandTypePair(TokenDataType.Time, TokenDataType.Timespan) },

            //            { new OperandTypePair(TokenDataType.Timespan, TokenDataType.Date),                          new OperandTypePair(TokenDataType.Timespan, TokenDataType.Date) },
            //            { new OperandTypePair(TokenDataType.Timespan, TokenDataType.Float),                         new OperandTypePair(TokenDataType.Timespan, TokenDataType.Timespan) },
            //            { new OperandTypePair(TokenDataType.Timespan, TokenDataType.LimitedInteger),                new OperandTypePair(TokenDataType.Timespan, TokenDataType.Timespan) },
            //            { new OperandTypePair(TokenDataType.Timespan, TokenDataType.Time),                          new OperandTypePair(TokenDataType.Timespan, TokenDataType.Time) },
            //            { new OperandTypePair(TokenDataType.Timespan, TokenDataType.Timespan),                      new OperandTypePair(TokenDataType.Timespan, TokenDataType.Timespan) },
            //            { new OperandTypePair(TokenDataType.Timespan, TokenDataType.UnlimitedInteger),              new OperandTypePair(TokenDataType.Timespan, TokenDataType.Timespan) },

            //            { new OperandTypePair(TokenDataType.UnlimitedInteger, TokenDataType.Date),                  new OperandTypePair(TokenDataType.Timespan, TokenDataType.Date) },
            //            { new OperandTypePair(TokenDataType.UnlimitedInteger, TokenDataType.Float),                 new OperandTypePair(TokenDataType.Float, TokenDataType.Float) },
            //            { new OperandTypePair(TokenDataType.UnlimitedInteger, TokenDataType.LimitedInteger),        new OperandTypePair(TokenDataType.LimitedInteger, TokenDataType.LimitedInteger) },
            //            { new OperandTypePair(TokenDataType.UnlimitedInteger, TokenDataType.Time),                  new OperandTypePair(TokenDataType.Timespan, TokenDataType.Time) },
            //            { new OperandTypePair(TokenDataType.UnlimitedInteger, TokenDataType.Timespan),              new OperandTypePair(TokenDataType.Timespan, TokenDataType.Timespan) },
            //            { new OperandTypePair(TokenDataType.UnlimitedInteger, TokenDataType.UnlimitedInteger),      new OperandTypePair(TokenDataType.UnlimitedInteger, TokenDataType.UnlimitedInteger) },
            //        };



            //var bw in EnumExtensions.GetEnumCollection<OldLimitedIntegerToken.IntegerBitWidth>()
            //foreach (var tp in EnumExtensions.GetEnumCollection<OperatorType>(e => e.IsBinaryOperator()))
            //{

            //}
        }

        readonly static Dictionary<OperatorType, Dictionary<DTP, DTP>> _operandTypeCastMap = new Dictionary<OperatorType, Dictionary<DTP, DTP>>();
    }
}
